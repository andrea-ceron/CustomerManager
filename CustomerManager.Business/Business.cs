using AutoMapper;
using CustomerManager.Business.Abstraction;
using CustomerManager.Business.DTOHelper;
using CustomerManager.Repository.Abstraction;
using CustomerManager.Repository.Model;
using CustomerManager.Shared.DTO;
using Microsoft.Extensions.Logging;
using StockManager.Shared.DTO;

namespace CustomerManager.Business
{
	public class Business(StockManager.ClientHttp.Abstractions.IStockManagerClientHttp stockManagerClientHttp, IRepository repository, ILogger<Business> logger, IMapper _mapper) : IBusiness
	{
		#region Customer
		public async Task CreateCustomerAsync(CreateCustomerDto Customer, CancellationToken ct = default)
		{
			var AddressList = Customer.AddingAddress;
			Customer.AddingAddress = new List<CreateAddressDto>();
			Customer customer = _mapper.Map<Customer>(Customer);

			await repository.CreateTransaction(async () =>
			{
				var resCustomer = await repository.CreateCustomerAsync(customer, ct);
				await repository.SaveChangesAsync(ct);
				if (resCustomer == null)
					throw new ExceptionHandlerBuisiness("Errore durante la creazione del cliente.", 500);
				foreach(var address in AddressList)
				{
					if (string.IsNullOrEmpty(address.Street) ||string.IsNullOrEmpty(address.City) ||string.IsNullOrEmpty(address.PostalCode))
					{
						throw new ExceptionHandlerBuisiness("I campi Street, City e PostalCode sono obbligatori per ogni indirizzo.", 400);
					}
					Address addressModel = _mapper.Map<Address>(address);
					addressModel.CustomerId = resCustomer.Id;
					var createdAddress =await repository.CreateAddressAsync(addressModel, ct);
					await repository.SaveChangesAsync(ct);
					if (createdAddress == null)
						throw new ExceptionHandlerBuisiness("Errore durante la creazione di un indirizzo.", 500);
				}
				logger.LogInformation("Cliente creato con Id: {CustomerId}", resCustomer.Id);

			});


		}
		public async Task DeleteCustomerAsync(int CustomerId, CancellationToken ct = default)
		{
			var Invoices = await repository.GetAllInvoiceByCustomerId(CustomerId, ct);
			if (Invoices != null && Invoices.Count != 0)
			{
				var mostRecentInvoice = Invoices.OrderByDescending(n => n.Date).First();
				DateTime CurrentDate = DateTime.Now;
				bool SpentOver10Years = mostRecentInvoice.Date.AddYears(10) <= CurrentDate;
				if (!SpentOver10Years)
					throw new ExceptionHandlerBuisiness("Non è possibile cancellare il cliente: fatture troppo recenti", CustomerId, 400);

				foreach (var invoice in Invoices)
				{
					await DeleteInvoiceAsync(invoice.Id, ct);
				}
			}

			var Addresses = await repository.GetAllAddressesByCustomerId(CustomerId, ct);
			foreach(var elem in Addresses)
			{
				await repository.DeleteAddressAsync(elem.Id, ct);
			}
			await repository.DeleteCustomerAsync(CustomerId, ct);
			await repository.SaveChangesAsync(ct);
			logger.LogInformation("Cliente con Id {CustomerId} eliminato con successo", CustomerId);
		}
		public async Task<ReadCustomerDto?> GetCustomerAsync(int CustomerId, CancellationToken ct = default)
		{
			var customerModel = await repository.GetCustomerById(CustomerId, ct);
			if (customerModel == null)
				throw new ExceptionHandlerBuisiness($"Customer con Id {CustomerId} non trovato", CustomerId, 404);
			var Addresses = await repository.GetAllAddressesByCustomerId(CustomerId, ct);
			if (Addresses == null)
				throw new ExceptionHandlerBuisiness($"Indirizzi per il Customer {CustomerId} non trovati", CustomerId, 404);
			List<ReadAndUpdateAddressDto> Address = _mapper.Map<List<ReadAndUpdateAddressDto>>(Addresses);
			ReadCustomerDto customerDto = _mapper.Map<ReadCustomerDto>(customerModel);
			logger.LogInformation("Customer con Id {CustomerId} recuperato con successo", CustomerId);
			return customerDto;
		}
		public async Task UpdateCustomerAsync(UpdateCustomerDto Customer, CancellationToken ct = default)
		{
			var addingAddress = Customer.AddingAddress;
			Customer.AddingAddress = new List<CreateAddressDto>();
			Customer customer = _mapper.Map<Customer>(Customer);
			IEnumerable<Address> NewAddress = _mapper.Map<IEnumerable<Address>>(addingAddress);
			foreach (var item in NewAddress)
			{
				item.CustomerId = customer.Id;
			}

			await repository.CreateTransaction(async () =>
			{
				await repository.DeleteAddressListAsync(Customer.RemovingAddress, Customer.Id, ct);
				await repository.CreateListOfAddressesAsync(NewAddress, ct);
				await repository.UpdateCustomerAsync(customer, ct);
				await repository.SaveChangesAsync(ct);
			});
			logger.LogInformation("Customer con Id {CustomerId} aggiornato con successo", Customer.Id);
		}

		#endregion
		
		
		#region Invoice
		public async Task CreateInvoiceAsync(CreateSellingInvoiceDto invoiceDto, CancellationToken ct = default)
		{
			var res = await repository.GetCustomerById(invoiceDto.CustomerId, ct);
			IEnumerable<CreateInvoiceProductsDto>? Products = new List<CreateInvoiceProductsDto>();
			Products = invoiceDto.ProductList;
			Address? Address =await repository.GetAddressById(invoiceDto.AddressId, ct);
			if (res == null) 
				throw new ExceptionHandlerBuisiness("Customer non esistente", invoiceDto.CustomerId, 404);
			if ( !Products.Any() ) 
				throw new ExceptionHandlerBuisiness("Nessun prodotto da aggiungere in fattura", invoiceDto.CustomerId, 400);
			if(Address == null || Address.CustomerId != invoiceDto.CustomerId) 
				throw new ExceptionHandlerBuisiness("Indirizzo non appartenente al cliente intestatario della fattura", invoiceDto.AddressId, 400);

			// Creo una spedizione per la fattura chiamando la funzione CreateShipment su StockManager 
			CreateShipmentDto newShipment = new()
			{
				DestinationAddress = string.Join(", ", Address.Street, Address.City, Address.PostalCode, Address.Country), 
				Items = invoiceDto.ProductList.Select(p => new CreateShippingItemsDto
				{
					EndProductId = p.ProductId,
					Quantity = p.Pieces,

				}).ToList() 
			};
			
			await stockManagerClientHttp.CreateShipment(newShipment, ct);


			await repository.CreateTransaction(async () =>
			{
				await CreateInvoiceHelper(Products, invoiceDto, ct);
			});

		}

		public async Task CreateInvoiceHelper(IEnumerable<CreateInvoiceProductsDto>? Products, CreateSellingInvoiceDto invoiceDto, CancellationToken ct = default)
		{
			List<CreateInvoiceProductHelper> InvoiceProductsHelperList = new();
			foreach (var product in Products)
			{
				var productResult = await repository.GetProductByIdAsync(product.ProductId, ct);
				if (productResult == null)
					throw new ExceptionHandlerBuisiness($"Prodotto con Id {product.ProductId} non trovato", product.ProductId, 404);
				CreateInvoiceProductHelper createInvoiceProductHelper = new()
				{
					ProductId = productResult.Id,
					Pieces = product.Pieces,
					Price = productResult.Price * product.Pieces,
					VAT = productResult.VAT
				};
				InvoiceProductsHelperList.Add(createInvoiceProductHelper);
				if (productResult.AvailablePieces - createInvoiceProductHelper.Pieces < 0)
				{
					throw new ExceptionHandlerBuisiness("Non e possibile creare una fattura quando la disponibilita dei prodotti e pari o inferiore a 0", 403);
				}
				productResult.AvailablePieces -= product.Pieces;
				await repository.UpdateProductAsync(productResult, ct);
				await repository.SaveChangesAsync(ct);
			}
			Invoice model = _mapper.Map<Invoice>(invoiceDto);
			model.ProductList = new List<InvoiceProducts>();
			var newInvoice = await repository.CreateInvoiceAsync(model, ct);

			await repository.SaveChangesAsync(ct);
			foreach (var product in InvoiceProductsHelperList)
			{
				if (product.Pieces <= 0)
					throw new ExceptionHandlerBuisiness("Numero di pezzi non valido", product, 400);

				InvoiceProducts productModel = _mapper.Map<InvoiceProducts>(product);
				productModel.InvoiceId = newInvoice.Id;
				// Verifico che il prodotto esista
				var productCreated = await repository.CreateInvoiceProductAsync(productModel, ct);
				// aggiungere transactionalOutbox
			}
			await repository.SaveChangesAsync(ct);
			logger.LogInformation("Fattura creata con Id: {InvoiceId}", newInvoice.Id);
		}
		public async Task DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default)
		{
			// se la fattura è piu vecchia di 10 anni
			var Invoice = await repository.GetInvoiceById(InvoiceId, ct);
			if (Invoice == null)
				throw new ExceptionHandlerBuisiness($"Fattura con Id {InvoiceId} non trovata", InvoiceId, 404); DateTime CurrentDate = DateTime.Now;
			bool SpentOver10Years = Invoice.Date.AddYears(10) <= CurrentDate;
			if (!SpentOver10Years)
				throw new ExceptionHandlerBuisiness("Non è possibile la fattura: fatture troppo recenti", InvoiceId, 400);
			// procediamo con l'e liminazione
			await repository.CreateTransaction(async () =>
			{
				
				await repository.DeleteInvoiceProductsByInvoiceIdAsync(InvoiceId, ct);
				await repository.SaveChangesAsync(ct);
				await repository.DeleteInvoiceAsync(InvoiceId, ct);
				await repository.SaveChangesAsync(ct);

			});
			logger.LogInformation("Fattura con Id {InvoiceId} eliminata con successo", InvoiceId);
		}
		public async Task<ReadSellingInvoiceDto?> GetInvoiceAsync(int InvoiceId, CancellationToken ct = default)
		{
			var res = await repository.GetInvoiceByIdAsync(InvoiceId, ct);
			if (res == null) throw new ExceptionHandlerBuisiness("il valore passato alla funzione non è associato a nessun identificativo di fattura", 404);
			logger.LogInformation("Customer is {State}", res.Customer != null ? "present" : "null");
			logger.LogInformation("Address is {State}", res.Address != null ? "present" : "null");
			logger.LogInformation("ProductList has {Count} items", res.ProductList?.Count() ?? 0);
			var Invoice = _mapper.Map<ReadSellingInvoiceDto>(res);
			logger.LogInformation("Fattura con Id {InvoiceId} recuperata con successo", Invoice.Id);
			return Invoice;
		}

		#endregion

		#region Product
	
		public async Task<ReadAndUpdateProductDto> GetProductAsync(int productId, CancellationToken ct = default)
		{
			var result = await  repository.GetProductByIdAsync(productId, ct);
			if(result == null)
				throw new ExceptionHandlerBuisiness($"Prodotto con Id {productId} non trovato", productId, 404);
			ReadAndUpdateProductDto productDto = _mapper.Map<ReadAndUpdateProductDto>(result);
			logger.LogInformation("Prodotto con Id {ProductId} recuperato con successo", productId);
			return productDto;
		}

		public async Task BuildProduct(IEnumerable<BuildEndProductDto> listOfEndproductsToBuild, CancellationToken ct = default)
		{
			// eseguo la chiamata Clienhttp riguardo alla crezione di diversi processi produttivi in base alla lunghezza della lista
			try
			{   
				List<CreateProductionProcessDto> stockManagerList = _mapper.Map<List<CreateProductionProcessDto>>(listOfEndproductsToBuild);
				foreach(var elem in stockManagerList)
				{
					logger.LogInformation("Invio a StockManager: {EndProductId}, Quantità: {Quantity}",
						elem.EndProductId, elem.Quantity);
					var result = await stockManagerClientHttp.CreateProductionProcess(elem, ct);
					logger.LogDebug(result);
				}
				
			}
			catch(Exception ex)
			{
				throw;
			}

			// se il risultato non restituisce errore allora inserisco all interno del model Product le quantita aggiunte
			await repository.CreateTransaction(async () => 
			{
				foreach(var item in listOfEndproductsToBuild)
				{
					var endProduct = await repository.GetProductByIdAsync(item.Id, ct);
					endProduct.AvailablePieces += item.elementsToBuild;
				}
				await repository.SaveChangesAsync(ct);

			});
		}

		#endregion
	}
}
