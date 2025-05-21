using CustomerManager.Repository.Enumeration;
using CustomerManager.Repository.Abstraction;
using CustomerManager.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CustomerManager.Repository
{
	public class Repository(ClientsDbContext dbContext) : IRepository
	{
		public async Task<Address> CreateAddressAsync(string street, string city, string postalCode, string country, int customerId, CancellationToken ct = default)
		{
			Address address = new()
			{
				Street = street,
				City = city,
				PostalCode = postalCode,
				Country = country,
				CustomerId = customerId
			};
			await dbContext.Addresses.AddAsync(address, ct);
			return address;
		}

		public async  Task<Customer> CreateCustomerAsync(string companyName, string VATNumber, string taxCode, string certifiedEmail, string email, string phone, CancellationToken ct = default)
		{
			Customer Customer = new()
			{
				CompanyName = companyName,
				VATNumber = VATNumber,
				Email = email,
				TaxCode = taxCode,
				CertifiedEmail = certifiedEmail,
				Phone = phone,
				Status = IdentityStatus.Active, // Assegna lo Status
			};
			await dbContext.Customers.AddAsync(Customer, ct);

			return Customer;

		}

		public async Task<Invoice> CreateInvoiceAsync(DateTime date, int customerId, int addressId, CancellationToken ct = default)
		{
			Invoice invoice = new()
			{
				Date = date,
				CustomerId = customerId,
				Status = InvoiceStatus.Issued,
				AddressId = addressId
			};
			await dbContext.Invoices.AddAsync(invoice, ct);
			return invoice;
		}

		public async Task DeleteCustomerAsync(int customerId, CancellationToken ct = default)
		{
			Customer? customer = await GetCustomerById(customerId, ct);
			if (customer == null) return;
			dbContext.Customers.Remove(customer);
		}

		public async Task DeleteAddressAsync(int addressId, CancellationToken ct = default)
		{
			Address? address = await  GetAddressById(addressId, ct);
			if (address == null) return;
			dbContext.Addresses.Remove(address);
		}

		public async Task<Address?> GetAddressById(int? AddressId, CancellationToken ct = default)
		{
			 return await dbContext.Addresses.Where(a => a.Id == AddressId).SingleOrDefaultAsync(ct);
		}

		public async Task<List<Invoice>> GetAllInvoiceByCustomerId(int? CustomerId, CancellationToken ct = default)
		{
			Customer? customer = await GetCustomerById(CustomerId, ct);
			Customer? newCustomer = null;
			if (!CustomerId.HasValue) return null;
			if (customer == null)
			{
				newCustomer = await GetCustomerById(CustomerId, ct);
			}

			if (newCustomer == null) return null;

			var invoices = await dbContext.Invoices.Where(i => i.CustomerId == CustomerId).ToListAsync(ct);
			return invoices;


		}

		public async Task<List<Address>> GetAllAddressesByCustomerId(int? CustomerId, CancellationToken ct = default)
		{
			Customer? newCustomer = null;
			if (!CustomerId.HasValue) return null;
			var invoices = await dbContext.Addresses.Where(p => p.CustomerId == CustomerId).ToListAsync(ct);
			return invoices;
		}

		public async Task<Customer?> GetCustomerById(int? customerId, CancellationToken ct = default)
		{
			return await dbContext.Customers.Where(c => c.Id == customerId).SingleOrDefaultAsync(ct);
		}

		public async Task<Invoice?> GetInvoiceById(int? InvoiceId, CancellationToken ct = default)
		{
			return await dbContext.Invoices.Where(c => c.Id == InvoiceId).SingleOrDefaultAsync(ct);
		}

		public async  Task<Customer> UpdateCustomerAsync(int? customerId, string companyName, string VATNumber, string taxCode, string certifiedEmail, string email, string phone,  CancellationToken ct = default)
		{
			Customer? newCustomer =await GetCustomerById(customerId, ct);
			if (newCustomer == null) return null;

			newCustomer.CompanyName = companyName;
			newCustomer.VATNumber = VATNumber;
			newCustomer.Email = email;
			newCustomer.TaxCode = taxCode;
			newCustomer.CertifiedEmail = certifiedEmail;
			newCustomer.Phone = phone;

			
			return newCustomer;
		}

		public async Task<Invoice?> UpdateInvoiceStatusAsync(Invoice? invoice, InvoiceStatus status, CancellationToken ct = default)
		{
			if (invoice == null) return null;
			invoice.Status = status;
			await dbContext.Invoices.AddAsync(invoice, ct);
			return invoice;
		}

		public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task<Product> CreateProductAsync( int pieces, decimal price, int VAT,int InvoiceId, CancellationToken ct = default)
		{
			Product product = new()
			{
				Pieces = pieces,
				Price = price,
				VAT = VAT,
				InvoiceId = InvoiceId
			};
			await dbContext.Products.AddAsync(product, ct);
			return product;
		}

		public async  Task<List<Product>> GetAllProductByInvoiceId(int InvoiceId, CancellationToken ct = default)
		{
			return await dbContext.Products.Where(n => n.InvoiceId == InvoiceId).ToListAsync(ct);
		}

		public async Task DeleteProductsByInvoiceIdAsync(int InvoiceId, CancellationToken ct = default)
		{
			var res = await GetAllProductByInvoiceId(InvoiceId, ct);
			dbContext.Products.RemoveRange(res);

		}

		public async Task DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default)
		{
			var res = await GetInvoiceById(InvoiceId, ct);
			dbContext.Invoices.Remove(res);
		}
	}
}
