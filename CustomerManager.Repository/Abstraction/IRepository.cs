using CustomerManager.Repository.Enumeration;
using CustomerManager.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Repository.Abstraction
{
    public interface IRepository
    {
		public Task<Customer> CreateCustomerAsync(Customer model, CancellationToken ct = default);
		public Task<Invoice> CreateInvoiceAsync(Invoice model, CancellationToken ct = default);
		public Task<Address> CreateAddressAsync(Address model, CancellationToken ct = default);
		public Task<InvoiceProducts> CreateInvoiceProductAsync(InvoiceProducts model, CancellationToken ct = default);

		public Task<Customer> UpdateCustomerAsync(Customer model, CancellationToken ct = default);
		public Task<Invoice?> UpdateInvoiceStatusAsync(Invoice invoice, InvoiceStatus status, CancellationToken ct = default);
		public Task<Customer?> GetCustomerById(int? customerId, CancellationToken ct = default);
		public Task<Invoice?> GetInvoiceById(int? InvoiceId, CancellationToken ct = default);
		public Task<Address?> GetAddressById(int? AddressId, CancellationToken ct = default);
		public Task<List<Invoice>> GetAllInvoiceByCustomerId(int? CustomerId, CancellationToken ct = default);
		public Task<List<Address>> GetAllAddressesByCustomerId(int? CustomerId, CancellationToken ct = default);
		public Task DeleteCustomerAsync(int customerId, CancellationToken ct = default);
		public Task DeleteAddressAsync(int addressId, CancellationToken ct = default);
		public Task DeleteInvoiceProductsByInvoiceIdAsync(int InvoiceId, CancellationToken ct = default);
		public Task DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default);
		public Task<IEnumerable<InvoiceProducts>> GetAllInvoiceProductsByInvoiceId(int InvoiceId, CancellationToken ct = default);
		public Task DeleteAddressListAsync(IEnumerable<int> AddressToDelete, int CustomerId, CancellationToken ct = default);
		public Task CreateListOfAddressesAsync(IEnumerable<Address> addressList, CancellationToken ct = default);
		public Task<Product> CreateProductAsync(Product model, CancellationToken ct = default);
		public Task<Product> GetProductByIdAsync(int productId, CancellationToken ct = default);
		public Task<Product> UpdateProductAsync(Product model, CancellationToken ct = default);
		public Task DeleteProductAsync(int productId, CancellationToken ct = default);
		public Task<Invoice?> GetInvoiceByIdAsync(int InvoiceId, CancellationToken ct = default);
		public Task<bool> CheckForInvoiceProductFromProductId(int productId, CancellationToken cancellationToken = default);

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		public Task CreateTransaction(Func<Task> action);


	}
}
