using CustomerManager.Shared;

namespace CustomerManager.Business.Abstraction
{
    public interface IBusiness
    {
		public Task CreateInvoiceAsync(SellingInvoiceDto? invoice, CancellationToken ct = default);
		public Task DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default);
		public Task<SellingInvoiceDto?> GetInvoiceAsync(int InvoiceId, CancellationToken ct = default);
		public Task CreateCustomerAsync(CustomerDto Customer, List<AddressDto> AddressList, CancellationToken ct = default);
		public Task DeleteCustomerAsync(int CustomerId, CancellationToken ct = default);
		public Task<CustomerDto?> GetCustomerAsync(int CustomerId, CancellationToken ct = default);
		public Task UpdateCustomerAsync(CustomerDto Customer, List<AddressDto> NewAddress, List<int> AddresToRemove, CancellationToken ct = default);

			 // ogni volta che faccio una get sia per invoice che per Customer modifico eventualmente lo stato dei due elementi verificando determinate variabili
	}
}
