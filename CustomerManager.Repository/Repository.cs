using CustomerManager.Repository.Enumeration;
using CustomerManager.Repository.Abstraction;
using CustomerManager.Repository.Model;
using Microsoft.EntityFrameworkCore;
namespace CustomerManager.Repository;

public class Repository(ClientsDbContext dbContext) : IRepository
{

	#region Invoice 
	public async Task<Invoice> CreateInvoiceAsync(Invoice model,CancellationToken ct = default)
	{
		await dbContext.Invoices.AddAsync(model, ct);
		return model;
	}
	public async Task<List<Invoice>> GetAllInvoiceByCustomerId(int CustomerId, CancellationToken ct = default)
	{
		Customer customer = await GetCustomerById(CustomerId, ct);
		var invoices = await dbContext.Invoices.Where(i => i.CustomerId == CustomerId).ToListAsync(ct) 
			 ?? throw new ArgumentException($"Nessun Fattura associata a Customer con ID {CustomerId} trovato");
		return invoices;
	}
	public async Task<Invoice> GetInvoiceById(int InvoiceId, CancellationToken ct = default)
	{
		return await dbContext.Invoices.Where(c => c.Id == InvoiceId).SingleOrDefaultAsync(ct)
			?? throw new ArgumentException($"Nessun Fattura trovata con ID {InvoiceId}");
	}
	public async Task<Invoice> UpdateInvoiceStatusAsync(Invoice invoice, InvoiceStatus status, CancellationToken ct = default)
	{
		await dbContext.Invoices.AddAsync(invoice, ct);
		return invoice;
	}
	public async Task DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default)
	{
		var res = await GetInvoiceById(InvoiceId, ct);
		dbContext.Invoices.Remove(res);
	}
	public async Task<Invoice> GetInvoiceByIdAsync(int InvoiceId, CancellationToken ct = default)
	{
		return await dbContext.Invoices
			.Where(i => i.Id == InvoiceId)
			.Include(i => i.Customer)
			.Include(i => i.Address)
			.Include(i => i.ProductList)
			.SingleOrDefaultAsync(ct)
			?? throw new ArgumentException($"Nessuna fattura trovata con ID {InvoiceId}");
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
		Customer customer = await GetCustomerById(customerId, ct);
		dbContext.Customers.Remove(customer);
	}
	public async Task<Customer> GetCustomerById(int customerId, CancellationToken ct = default)
	{
		return await dbContext.Customers.Where(c => c.Id == customerId).SingleOrDefaultAsync(ct)
			?? throw new ArgumentException($"Nessun Customer trovato con ID {customerId}");
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
		dbContext.Addresses.Remove(address);
	}
	public async Task<Address> GetAddressById(int AddressId, CancellationToken ct = default)
	{
		 return await dbContext.Addresses.Where(a => a.Id == AddressId).SingleOrDefaultAsync(ct)
			?? throw new ArgumentException($"Nessun Address trovato con ID {AddressId}");
	}
	public async Task<List<Address>> GetAllAddressesByCustomerId(int CustomerId, CancellationToken ct = default)
	{
		var invoices = await dbContext.Addresses.Where(p => p.CustomerId == CustomerId).ToListAsync(ct)
			?? throw new ArgumentException($"Nessun Address associato a Customer con ID {CustomerId} trovato");
		return invoices;
	}
	public async Task DeleteAddressListAsync(IEnumerable<int> AddressToDelete, int CustomerId, CancellationToken ct = default)
	{
		List<Address> addresses = new();
		foreach (var addressId in AddressToDelete)
		{
			var address = await GetAddressById(addressId, ct);			
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
	public async Task<Product> GetProductByIdAsync(int productId, CancellationToken ct = default)
	{
		return await dbContext.Products.Where(p => p.Id == productId)
			.SingleOrDefaultAsync(ct)
			?? throw new ArgumentException($"Nessun Product trovato con ID {productId}");
	}
	public async Task<Product> UpdateProductAsync(Product model, CancellationToken ct = default)
	{
		dbContext.Products.Update(model);
		return model;
	}
	public async Task DeleteProductAsync(int productId, CancellationToken ct = default)
	{
		var elem = await  GetProductByIdAsync(productId, ct);
		dbContext.Products.Remove(elem);
	}
	public async Task<bool> CheckIfEndProductIsUsed(int productId, CancellationToken ct = default)
	{
		return await dbContext.InvoiceProducts
			.AnyAsync(ip => ip.ProductId == productId, ct);
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
		dbContext.InvoiceProducts.RemoveRange(res);
	}
	public async Task<IEnumerable<InvoiceProducts>> GetAllInvoiceProductsByProductId(int productId, CancellationToken ct = default)
	{
		return await dbContext.InvoiceProducts
			.Where(n => n.ProductId == productId)
			.ToListAsync(ct);
	}
	public async Task<IEnumerable<InvoiceProducts>> GetAllInvoiceProductsByInvoiceId(int InvoiceId, CancellationToken ct = default)
	{
		return await dbContext.InvoiceProducts
			.Where(n => n.InvoiceId == InvoiceId)
			.Include(n => n.Product)
			.ToListAsync(ct)
			?? throw new ArgumentException($"Nessun InvoiceProducts trovato con ID {InvoiceId}");
	}
	public async Task<bool> CheckForInvoiceProductFromProductId(int productId, CancellationToken ct = default)
	{
		return await dbContext.InvoiceProducts
			.AnyAsync(ip => ip.ProductId == productId, ct);

	}
	#endregion

	#region TransactionalOutbox
	public async Task<IEnumerable<TransactionalOutbox>> GetAllTransactionalOutbox(CancellationToken ct = default)
	{
		return await dbContext.TransactionalOutboxes
			.ToListAsync(ct);
	}
	public async Task DeleteTransactionalOutboxAsync(long id, CancellationToken cancellationToken = default)
	{
		dbContext.TransactionalOutboxes.Remove(await GetTransactionalOutboxByKeyAsync(id, cancellationToken)
			?? throw new ArgumentException($"TransactionalOutbox con id {id} non trovato", nameof(id)));
	}

	public async Task<TransactionalOutbox?> GetTransactionalOutboxByKeyAsync(long id, CancellationToken cancellationToken = default)
	{
		return await dbContext.TransactionalOutboxes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public async Task InsertTransactionalOutboxAsync(TransactionalOutbox transactionalOutbox, CancellationToken cancellationToken = default)
	{
		await dbContext.TransactionalOutboxes.AddAsync(transactionalOutbox, cancellationToken);
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
