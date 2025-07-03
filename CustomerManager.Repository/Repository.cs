using CustomerManager.Repository.Enumeration;
using CustomerManager.Repository.Abstraction;
using CustomerManager.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomerManager.Repository
{
	public class Repository(ClientsDbContext dbContext) : IRepository
	{

		#region Invoice 
		public async Task<Invoice> CreateInvoiceAsync(Invoice model,CancellationToken ct = default)
		{
			await dbContext.Invoices.AddAsync(model, ct);
			return model;
		}
		public async Task<List<Invoice>> GetAllInvoiceByCustomerId(int? CustomerId, CancellationToken ct = default)
		{
			Customer? customer = await GetCustomerById(CustomerId, ct);
			Customer? newCustomer = null;
			if (!CustomerId.HasValue) throw new ExceptionHandlerRepository($"Nessun customer corrisponde all'ID {CustomerId}", 400);

			var invoices = await dbContext.Invoices.Where(i => i.CustomerId == CustomerId).ToListAsync(ct);
			return invoices;


		}
		public async Task<Invoice?> GetInvoiceById(int? InvoiceId, CancellationToken ct = default)
		{
			return await dbContext.Invoices.Where(c => c.Id == InvoiceId).SingleOrDefaultAsync(ct);
		}
		public async Task<Invoice?> UpdateInvoiceStatusAsync(Invoice? invoice, InvoiceStatus status, CancellationToken ct = default)
		{
			if (invoice == null) throw new ExceptionHandlerRepository("Invoice risulta essere nulla", 400);
			await dbContext.Invoices.AddAsync(invoice, ct);
			return invoice;
		}
		public async Task DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default)
		{
			var res = await GetInvoiceById(InvoiceId, ct);
			if (res == null) throw new ExceptionHandlerRepository($"nessuna invoice corrisponde all'ID {InvoiceId}", 404);
			dbContext.Invoices.Remove(res);
		}
		public async Task<Invoice?> GetInvoiceByIdAsync(int InvoiceId, CancellationToken ct = default)
		{
			return await dbContext.Invoices
				.Where(i => i.Id == InvoiceId)
				.Include(i => i.Customer)
				.Include(i => i.Address)
				.Include(i => i.ProductList)
				.SingleOrDefaultAsync(ct);
		}

		#endregion



		#region Customer
		public async  Task<Customer> CreateCustomerAsync(Customer model,CancellationToken ct = default)
		{
			await dbContext.Customers.AddAsync(model, ct);
			return model;
		}
		public async Task DeleteCustomerAsync(int customerId, CancellationToken ct = default)
		{
			Customer? customer = await GetCustomerById(customerId, ct);
			if (customer == null) throw new ExceptionHandlerRepository($"nessun Customer è stato trovato con ID {customerId}", 404);
			dbContext.Customers.Remove(customer);
		}
		public async Task<Customer?> GetCustomerById(int? customerId, CancellationToken ct = default)
		{
			return await dbContext.Customers.Where(c => c.Id == customerId).SingleOrDefaultAsync(ct);
		}
		public async  Task<Customer> UpdateCustomerAsync(Customer model,  CancellationToken ct = default)
		{
			dbContext.Customers.Update(model);
			return model;
		}
		#endregion



		#region Address
		public async Task<Address> CreateAddressAsync(Address model, CancellationToken ct = default)
		{

			await dbContext.Addresses.AddAsync(model, ct);
			return model;
		}
		public async Task DeleteAddressAsync(int addressId, CancellationToken ct = default)
		{
			Address? address = await  GetAddressById(addressId, ct);
			if (address == null) throw new ExceptionHandlerRepository($"Nessun Address trovato con id {addressId}", 404);
			dbContext.Addresses.Remove(address);
		}
		public async Task<Address?> GetAddressById(int? AddressId, CancellationToken ct = default)
		{
			 return await dbContext.Addresses.Where(a => a.Id == AddressId).SingleOrDefaultAsync(ct);
		}
		public async Task<List<Address>> GetAllAddressesByCustomerId(int? CustomerId, CancellationToken ct = default)
		{
			Customer? newCustomer = null;
			if (!CustomerId.HasValue) return null;
			var invoices = await dbContext.Addresses.Where(p => p.CustomerId == CustomerId).ToListAsync(ct);
			return invoices;
		}
		public async Task DeleteAddressListAsync(IEnumerable<int> AddressToDelete, int CustomerId, CancellationToken ct = default)
		{
			List<Address> addresses = new();
			if (AddressToDelete.Count() == 0) throw new ExceptionHandlerRepository("Lista di Address da eliminare risulta essere vuota", 404);
			foreach (var addressId in AddressToDelete)
			{
				var address = await GetAddressById(addressId, ct);
				if(address == null || address.CustomerId != CustomerId)
				{
					throw new ExceptionHandlerRepository($"Non si puo eliminare l indirizzo con id {addressId} perche associato ad un cliente diverso", 404);
				}
				
				addresses.Add(address);
			}
			dbContext.Addresses.RemoveRange(addresses);

		}
		public async Task CreateListOfAddressesAsync(IEnumerable<Address> addressList, CancellationToken ct = default)
		{
			List<Address> addresses = addressList.ToList();
			await dbContext.Addresses.AddRangeAsync(addresses, ct);
		}

		#endregion



		#region Product 
		public async Task<Product> CreateProductAsync(Product model, CancellationToken ct = default)
		{
			await dbContext.Products.AddAsync(model, ct);
			return model;
		}

		public async Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default)
		{
			return await dbContext.Products.Where(p => p.Id == productId)
				.SingleOrDefaultAsync(ct);
		}
		public async Task<Product> UpdateProductAsync(Product model, CancellationToken ct = default)
		{
			dbContext.Products.Update(model);
			return model;
		}

		public async Task DeleteProductAsync(int productId, CancellationToken ct = default)
		{
			var elem = await  GetProductByIdAsync(productId, ct);
			if(elem == null) throw new ExceptionHandlerRepository($"Product with Id {productId} not found", 404);
			dbContext.Products.Remove(elem);
		}


		#endregion



		#region InvoiceProducts
		public async  Task<InvoiceProducts> CreateInvoiceProductAsync(InvoiceProducts model, CancellationToken ct = default)
		{
			await dbContext.InvoiceProducts.AddAsync(model, ct);
			return model;
		}
		public  async Task DeleteInvoiceProductsByInvoiceIdAsync(int InvoiceId, CancellationToken ct = default)
		{
			var res = await GetAllInvoiceProductsByInvoiceId(InvoiceId, ct);
			if (res == null) throw new ExceptionHandlerRepository($"InvoiceProduct non trovate con Id {InvoiceId}", 404);
			dbContext.InvoiceProducts.RemoveRange(res);
		}
		public async Task<IEnumerable<InvoiceProducts>> GetAllInvoiceProductsByInvoiceId(int InvoiceId, CancellationToken ct = default)
		{
			return await dbContext.InvoiceProducts
				.Where(n => n.InvoiceId == InvoiceId)
				.Include(n => n.Product)
				.ToListAsync(ct);
		}
		public async Task<bool> CheckForInvoiceProductFromProductId(int productId, CancellationToken ct = default)
		{
			return await dbContext.InvoiceProducts
				.AnyAsync(ip => ip.ProductId == productId, ct);
		}
		#endregion




		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{

			return await dbContext.SaveChangesAsync(cancellationToken);


		}
		public async Task CreateTransaction(Func<Task> action)
		{
			if (dbContext.Database.CurrentTransaction != null)
			{
				await action();
			}
			else
			{
				using var transaction = await dbContext.Database.BeginTransactionAsync();
				try
				{
					await action();
					await transaction.CommitAsync();
				}
				catch
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
		}


	}
}
