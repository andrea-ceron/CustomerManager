using CustomerManager.Repository.Enumeration;
using CustomerManager.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Repository.Abstraction
{
    public interface IRepository
    {
		public Task<Customer> CreateCustomerAsync(string companyName, string VATNumber, string taxCode, string certifiedEmail, string email, string phone, CancellationToken ct = default);
		public Task<Invoice> CreateInvoiceAsync(DateTime date, int customerId, int addressId, CancellationToken ct = default);
		public Task<Address> CreateAddressAsync(string street, string city, string postalCode, string country, int customerId, CancellationToken ct = default);
		public Task<Product> CreateProductAsync(int pieces, decimal price, int VAT, int InvoiceId, CancellationToken ct = default);
		public Task<Customer> UpdateCustomerAsync(int? customerId, string companyName, string VATNumber, string taxCode, string certifiedEmail, string email, string phone, CancellationToken ct = default);
		public Task<Invoice?> UpdateInvoiceStatusAsync(Invoice invoice, InvoiceStatus status, CancellationToken ct = default);
		public Task<Customer?> GetCustomerById(int? customerId, CancellationToken ct = default);
		public Task<Invoice?> GetInvoiceById(int? InvoiceId, CancellationToken ct = default);
		public Task<Address?> GetAddressById(int? AddressId, CancellationToken ct = default);
		public Task<List<Invoice>> GetAllInvoiceByCustomerId(int? CustomerId, CancellationToken ct = default);
		public Task<List<Product>> GetAllProductByInvoiceId(int InvoiceId, CancellationToken ct = default);
		public Task<List<Address>> GetAllAddressesByCustomerId(int? CustomerId, CancellationToken ct = default);
		public Task DeleteCustomerAsync(int customerId, CancellationToken ct = default);
		public Task DeleteAddressAsync(int addressId, CancellationToken ct = default);
		public Task DeleteProductsByInvoiceIdAsync(int InvoiceId, CancellationToken ct = default);
		public Task DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default);

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

	}
}
