using CustomerManager.Shared;
using CustomerManager.Shared.DTO;

namespace CustomerManager.Business.Abstraction
{
    public interface IBusiness
    {
		public Task CreateInvoiceAsync(CreateSellingInvoiceDto invoice, CancellationToken ct = default);
		public Task DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default);
		public Task<ReadSellingInvoiceDto> GetInvoiceAsync(int InvoiceId, CancellationToken ct = default);



		public Task CreateCustomerAsync(CreateCustomerDto Customer, CancellationToken ct = default);
		public Task DeleteCustomerAsync(int CustomerId, CancellationToken ct = default);
		public Task<ReadCustomerDto?> GetCustomerAsync(int CustomerId, CancellationToken ct = default);
		public Task UpdateCustomerAsync(UpdateCustomerDto Customer,  CancellationToken ct = default);


		public Task<ReadAndUpdateProductDto> GetProductAsync(int productId, CancellationToken ct = default);
		public Task BuildProduct(IEnumerable<BuildEndProductDto> listOfEndproductsToBuild, CancellationToken ct = default);

		// ogni volta che faccio una get sia per invoice che per Customer modifico eventualmente lo stato dei due elementi verificando determinate variabili
	}
}
