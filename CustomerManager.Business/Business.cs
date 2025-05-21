using CustomerManager.Business.Abstraction;
using CustomerManager.Repository.Abstraction;
using CustomerManager.Repository.Model;
using CustomerManager.Shared;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace CustomerManager.Business
{
	public class Business(IRepository repository, ILogger<Business> Logger) : IBusiness
	{

		public async Task CreateCustomerAsync(CustomerDto Customer, List<AddressDto> AddressList, CancellationToken ct = default)
		{
			if (Customer == null)
				throw new ExceptionHandler("Il CustomerDto non può essere null.", 400);
			if (AddressList == null || AddressList.Count == 0)
				throw new ExceptionHandler("È necessario fornire almeno un address.", 400);
			var resCustomer = await repository.CreateCustomerAsync(Customer.CompanyName, Customer.VATNumber, Customer.TaxCode, Customer.CertifiedEmail, Customer.Email, Customer.Phone, ct);
			if (resCustomer == null)
				throw new ExceptionHandler("Errore durante la creazione del cliente.", 500);
			await repository.SaveChangesAsync(ct);
			foreach(var address in AddressList)
			{
				if (string.IsNullOrEmpty(address.Street) ||string.IsNullOrEmpty(address.City) ||string.IsNullOrEmpty(address.PostalCode))
				{
					throw new ExceptionHandler("I campi Street, City e PostalCode sono obbligatori per ogni indirizzo.", 400);
				}
				var createdAddress =await repository.CreateAddressAsync(address.Street, address.City, address.PostalCode, address.Country, resCustomer.Id, ct);
				if (createdAddress == null)
					throw new ExceptionHandler("Errore durante la creazione di un indirizzo.", 500);
			}
			await repository.SaveChangesAsync(ct);

		}

		public async Task CreateInvoiceAsync(SellingInvoiceDto? invoice, CancellationToken ct = default)
		{
			if (invoice == null) throw new ExceptionHandler("Invoice mancante", 400);
			var res = await repository.GetCustomerById(invoice.CustomerId, ct);
			if (res == null) throw new ExceptionHandler("Customer non esistente", invoice.CustomerId, 404);
			List<ProductDto>? Products = invoice.ProductList;
			if (Products == null || Products.Count == 0) throw new ExceptionHandler("Nessun prodotto da aggiungere in fattura", invoice.CustomerId, 400);
			Address? Address =await repository.GetAddressById(invoice.AddressId, ct);
			if (Address == null) throw new ExceptionHandler("Indirizzo non esistente", invoice.AddressId, 404);
			if(Address.Id != invoice.Id) throw new ExceptionHandler("Indirizzo non appartenente al cliente intestatario della fattura", invoice.AddressId, 400);
			var newInvoice = await repository.CreateInvoiceAsync(invoice.Date, invoice.CustomerId, Address.Id, ct);
			await repository.SaveChangesAsync(ct);
			foreach (var product in Products)
			{
				if (product.Pieces <= 0)
					throw new ExceptionHandler("Numero di pezzi non valido", product, 400);

				if (product.Price < 0)
					throw new ExceptionHandler("Prezzo non valido", product, 400);

				if (product.VAT != 4 && product.VAT != 10 && product.VAT != 22)
					throw new ExceptionHandler("IVA non valida", product, 400);
				var productCreated = await repository.CreateProductAsync(product.Pieces, product.Price, product.VAT, newInvoice.Id, ct);
			}
			await repository.SaveChangesAsync(ct);
		}

		public async Task DeleteCustomerAsync(int CustomerId, CancellationToken ct = default)
		{
			var Invoices = await repository.GetAllInvoiceByCustomerId(CustomerId, ct);
			if (Invoices != null && Invoices.Count != 0)
			{
				var mostRecentInvoice = Invoices.OrderByDescending(n => n.Date).First();
				DateTime CurrentDate = DateTime.Now;
				bool SpentOver10Years = mostRecentInvoice.Date.AddYears(10) >= CurrentDate;
				if (!SpentOver10Years)
					throw new ExceptionHandler("Non è possibile cancellare il cliente: fatture troppo recenti", CustomerId, 400);

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
		}

		public async Task DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default)
		{
			// se la fattura è piu vecchia di 10 anni
			var Invoice = await repository.GetInvoiceById(InvoiceId, ct);
			if (Invoice == null)
				throw new ExceptionHandler($"Fattura con Id {InvoiceId} non trovata", InvoiceId, 404); DateTime CurrentDate = DateTime.Now;
			bool SpentOver10Years = Invoice.Date.AddYears(10) >= CurrentDate;
			if (!SpentOver10Years)
				throw new ExceptionHandler("Non è possibile la fattura: fatture troppo recenti", InvoiceId, 400);
			// procediamo con l'e liminazione
			await repository.DeleteProductsByInvoiceIdAsync(InvoiceId, ct);
			await repository.DeleteInvoiceAsync(InvoiceId, ct);
			await repository.SaveChangesAsync(ct);

		}

		public async Task<CustomerDto?> GetCustomerAsync(int CustomerId, CancellationToken ct = default)
		{
			var Customer = await repository.GetCustomerById(CustomerId, ct);
			if (Customer == null)
				throw new ExceptionHandler($"Customer con Id {CustomerId} non trovato", CustomerId, 404);
			var Addresses = await repository.GetAllAddressesByCustomerId(CustomerId, ct);
			if (Addresses == null)
				throw new ExceptionHandler($"Indirizzi per il Customer {CustomerId} non trovati", CustomerId, 404);
			List<AddressDto> AddressDtoList = new();
			foreach(var address in Addresses)
			{
				if (address == null)
					throw new ExceptionHandler("Indirizzo nullo rilevato nella lista indirizzi", null, 500);
				AddressDto addressDto = new()
				{
					Id = address.Id,
					Street = address.Street,
					City = address.City,
					PostalCode = address.PostalCode,
					Country = address.Country
				};
				AddressDtoList.Add(addressDto);
			}
			CustomerDto customerDto = new()
			{
				Id = Customer.Id,
				Email = Customer.Email,
				Phone = Customer.Phone,
				CompanyName = Customer.CompanyName,
				VATNumber = Customer.VATNumber,
				TaxCode = Customer.TaxCode,
				CertifiedEmail = Customer.CertifiedEmail,
				Address = AddressDtoList
			};
			return customerDto;
		}

		public async Task<SellingInvoiceDto?> GetInvoiceAsync(int InvoiceId, CancellationToken ct = default)
		{
			var res = await repository.GetInvoiceById(InvoiceId, ct);
			if (res == null)
				throw new ExceptionHandler($"fattura con Id {InvoiceId} non trovata", InvoiceId, 404);
			var resAddress = await repository.GetAddressById(res.AddressId, ct);
			if (resAddress == null)
				throw new ExceptionHandler($"Indirizzo con Id {res.AddressId} associato alla fattura non trovato", res.AddressId, 404);
			var products = await repository.GetAllProductByInvoiceId(res.Id, ct);
			if (products == null || !products.Any())
				throw new ExceptionHandler($"Nessun prodotto trovato per la fattura {res.Id}", res.Id, 404);
			AddressDto Address = new()
			{
				Id = resAddress.Id,
				Street = resAddress.Street,
				City = resAddress.PostalCode,
				PostalCode = resAddress.PostalCode,
				Country = resAddress.Country
			};
			List<ProductDto> ProductsDtoList = new();
			foreach(var product in products)
			{
				if (product == null)
					throw new ExceptionHandler("Prodotto nullo nella fattura", null, 500);
				ProductDto productDto = new()
				{
					Pieces = product.Pieces,
					Price = product.Price,
					VAT = product.VAT
				};
				ProductsDtoList.Add(productDto);
			}
			SellingInvoiceDto Invoice = new()
			{
				Date = res.Date,
				CustomerId = res.CustomerId,
				AddressId = Address.Id,
				Address = Address,
				ProductList = ProductsDtoList
			};
			return Invoice;

		}

		public async Task UpdateCustomerAsync(CustomerDto Customer, List<AddressDto> NewAddress, List<int> AddresToRemove, CancellationToken ct = default)
		{
			if (Customer == null)
				throw new ExceptionHandler("Customer non può essere null", 400);
			var existingCustomer = await repository.GetCustomerById(Customer.Id, ct);
			if (existingCustomer == null)
				throw new ExceptionHandler($"Customer con Id {Customer.Id} non trovato", Customer.Id, 404);

			if (AddresToRemove.Count > 0)
			{
				foreach(var removeId in AddresToRemove)
				{
					var addressToRemove = await repository.GetAddressById(removeId, ct);
					if (addressToRemove == null)
						throw new ExceptionHandler($"Indirizzo con Id {removeId} da rimuovere non trovato", removeId, 404);

					await repository.DeleteAddressAsync(removeId, ct);

				}

			}
			if(NewAddress.Count > 0)
			{
				foreach (var add in NewAddress)
				{
					if (string.IsNullOrEmpty(add.Street) || string.IsNullOrEmpty(add.City))
						throw new ExceptionHandler("Indirizzo non valido: Street e City sono obbligatori", add, 400);

					await repository.CreateAddressAsync(add.Street, add.City, add.PostalCode, add.Country, Customer.Id, ct);
				}
			}

			await repository.UpdateCustomerAsync(Customer.Id, Customer.CompanyName, Customer.VATNumber, Customer.TaxCode, Customer.CertifiedEmail, Customer.Email, Customer.Phone, ct);
			await repository.SaveChangesAsync(ct);

		}
	}
}
