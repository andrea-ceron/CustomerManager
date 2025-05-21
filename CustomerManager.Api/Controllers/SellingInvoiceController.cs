using CustomerManager.Business.Abstraction;
using CustomerManager.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomerManager.Shared;

namespace CustomerManager.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SellingInvoiceController(IBusiness business, ILogger<SellingInvoiceController> logger) : Controller
{
	private readonly IBusiness _business = business;
	private readonly ILogger<SellingInvoiceController> _logger = logger;

	[HttpPost(Name = "CreateInvoice")]
	public async Task<ActionResult> CreateInvoice(SellingInvoiceDto invoice)
	{

		await _business.CreateInvoiceAsync(invoice);
		return Ok();
		

	}

	[HttpGet(Name = "GetInvoice")]
	public async Task<ActionResult> ReadInvoice(int invoiceId)
	{
		await _business.GetInvoiceAsync(invoiceId);
		return Ok();
	}

	[HttpDelete(Name = "DeleteInvoice")]
	public async Task<ActionResult> DeleteInvoice(int invoiceId)
	{
		await _business.DeleteInvoiceAsync(invoiceId);
		return Ok();
	}
}
