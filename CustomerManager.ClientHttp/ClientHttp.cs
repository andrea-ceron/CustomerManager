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
		return await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync(cancellationToken);
	}
	public async Task<string?> DeleteCustomerAsync(int CustomerId, CancellationToken ct = default)
	{
		var response = await httpClient.DeleteAsync($"/Customer/DeleteCustomer?CustomerId={CustomerId}", ct);
		return await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync(ct);
	}
	public async Task<ReadCustomerDto?> GetCustomerAsync(int CustomerId, CancellationToken ct = default)
	{
		var queryString = QueryString.Create(new Dictionary<string, string?>() {
			{ "customerId", CustomerId.ToString(CultureInfo.InvariantCulture) }
		});
		var response = await httpClient.GetAsync($"/Customer/ReadCustomer{queryString}", ct);
		return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<ReadCustomerDto?>(cancellationToken: ct);
	}
	public async Task<string?> UpdateCustomerAsync(UpdateCustomerDto Customer, CancellationToken ct = default)
	{
		var response = await httpClient.PutAsync($"/Customer/UpdateCustomer", JsonContent.Create(Customer), ct);
		return await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync(ct);
	}
	#endregion

	#region Invoice
	public async Task<string?> CreateInvoiceAsync(CreateSellingInvoiceDto invoice, CancellationToken ct = default)
	{
		var response = await httpClient.PostAsync($"/SellingInvoice/CreateInvoice", JsonContent.Create(invoice), ct);
		return await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync(ct);
	}
	public async  Task<string?> DeleteInvoiceAsync(int InvoiceId, CancellationToken ct = default)
	{
		var response = await httpClient.DeleteAsync($"/SellingInvoice/DeleteInvoice?InvoiceId={InvoiceId}", ct);
		return await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync(ct);
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
	#endregion





}
