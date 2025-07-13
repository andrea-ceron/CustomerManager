using CustomerManager.ClientHttp.Abstractions;
using CustomerManager.Shared.DTO;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net.Http.Json;
using System.Threading;

namespace CustomerManager.ClientHttp;

public class CustomerManagerClientHttp(HttpClient httpClient) : ICustomerManagerClientHttp
{
	#region Customer
	public async Task<string?> CreateCustomerAsync(CreateCustomerDto? soggetto, CancellationToken cancellationToken = default)
	{
		var response = await httpClient.PostAsync($"/Customer/CreateCustomer", JsonContent.Create(soggetto), cancellationToken);
		var content = await response.Content.ReadAsStringAsync(cancellationToken);
		if (!response.IsSuccessStatusCode)
		{
			throw new CustomerServiceException((int)response.StatusCode, content);
		}
		return content;
	}
	public async Task<string?> DeleteCustomerAsync(int CustomerId, CancellationToken ct = default)
	{
		var response = await httpClient.DeleteAsync($"/Customer/DeleteCustomer?CustomerId={CustomerId}", ct);
		var content =  await response.Content.ReadAsStringAsync(ct);
		if (!response.IsSuccessStatusCode)
		{
			throw new CustomerServiceException((int)response.StatusCode, content);
		}
		return content;
	}
	public async Task<ReadCustomerDto?> GetCustomerAsync(int CustomerId, CancellationToken ct = default)
	{
		var queryString = QueryString.Create(new Dictionary<string, string?>() {
			{ "customerId", CustomerId.ToString(CultureInfo.InvariantCulture) }
		});
		var response = await httpClient.GetAsync($"/Customer/ReadCustomer{queryString}", ct);
		var content = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ReadCustomerDto?>(cancellationToken: ct);
		return content;
	}
	public async Task<string?> UpdateCustomerAsync(UpdateCustomerDto Customer, CancellationToken ct = default)
	{
		var response = await httpClient.PutAsync($"/Customer/UpdateCustomer", JsonContent.Create(Customer), ct);
		var content = await response.Content.ReadAsStringAsync(ct);
		if (!response.IsSuccessStatusCode)
		{
			throw new CustomerServiceException((int)response.StatusCode, content);
		}
		return content;
	}
	#endregion

	#region Invoice
	public async Task<string?> CreateInvoiceAsync(CreateSellingInvoiceDto invoice, CancellationToken ct = default)
	{

		var response = await httpClient.PostAsync($"/SellingInvoice/CreateInvoice", JsonContent.Create(invoice), ct);
		var content = await response.Content.ReadAsStringAsync(ct);
		if (!response.IsSuccessStatusCode)
		{
			throw new CustomerServiceException((int)response.StatusCode, content);
		}
		return content;
	}
	public async  Task<string?> DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default)
	{
		var response = await httpClient.DeleteAsync($"/SellingInvoice/DeleteInvoice?InvoiceId={InvoiceId}", ct);
		var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync(ct);
		if (!response.IsSuccessStatusCode)
		{
			throw new CustomerServiceException((int)response.StatusCode, content);
		}
		return content;
	}
	public async  Task<ReadSellingInvoiceDto?> GetInvoiceAsync(int InvoiceId, CancellationToken ct = default)
	{
		var queryString = QueryString.Create(new Dictionary<string, string?>() {
			{ "invoiceId", InvoiceId.ToString(CultureInfo.InvariantCulture) }
		});
		var response = await httpClient.GetAsync($"/SellingInvoice/ReadInvoice{queryString}", ct);
		return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ReadSellingInvoiceDto?>(cancellationToken: ct);
	}
	#endregion

	#region Product
	public async Task<ReadAndUpdateProductDto?> GetProductAsync(int productId, CancellationToken ct = default)
	{
		var queryString = QueryString.Create(new Dictionary<string, string?>() {
			{ "productId", productId.ToString(CultureInfo.InvariantCulture) }
		});
		var response = await httpClient.GetAsync($"/Product/ReadProduct{queryString}", ct);
		return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ReadAndUpdateProductDto?>(cancellationToken: ct);
	}

	public async Task<string?> BuildProduct(IEnumerable<BuildEndProductDto> ProductsToCreate, CancellationToken ct = default)
	{
		var response = await httpClient.PostAsync($"/Product/BuildProduct", JsonContent.Create(ProductsToCreate), ct);
		var content = await response.Content.ReadAsStringAsync(ct);
		if (!response.IsSuccessStatusCode)
		{
			throw new CustomerServiceException((int)response.StatusCode, content);
		}
		return content;
	}
	#endregion





}
