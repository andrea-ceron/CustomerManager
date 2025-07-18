﻿using CustomerManager.Shared.DTO;

namespace CustomerManager.ClientHttp.Abstractions;

public interface ICustomerManagerClientHttp
{
	#region Customer
	Task<string?> CreateCustomerAsync(CreateCustomerDto? soggetto, CancellationToken cancellationToken = default);
	Task<string?> DeleteCustomerAsync(int CustomerId, CancellationToken ct = default);
	Task<ReadCustomerDto?> GetCustomerAsync(int CustomerId, CancellationToken ct = default);
	Task<string?> UpdateCustomerAsync(UpdateCustomerDto Customer, CancellationToken ct = default);
	#endregion

	#region Invoice
	Task<string?> CreateInvoiceAsync(CreateSellingInvoiceDto invoice, CancellationToken ct = default);
	Task<string?> DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default);
	Task<ReadSellingInvoiceDto?> GetInvoiceAsync(int InvoiceId, CancellationToken ct = default);
	#endregion

	#region Product
	public Task<ReadAndUpdateProductDto?> GetProductAsync(int productId, CancellationToken ct = default);
	public Task<string?> BuildProduct(IEnumerable<BuildEndProductDto> ProductsToCreate, CancellationToken ct = default);

	#endregion

}
