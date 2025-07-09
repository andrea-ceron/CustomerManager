using CustomerManager.Business.Abstraction;
using CustomerManager.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CustomerManager.Shared.DTO;

namespace CustomerManager.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SellingInvoiceController(IBusiness business, ILogger<SellingInvoiceController> logger) : Controller
{
	private readonly IBusiness _business = business;
	private readonly ILogger<SellingInvoiceController> _logger = logger;

	[HttpPost(Name = "CreateInvoice")]
	public async Task<ActionResult<String?>> CreateInvoice(CreateSellingInvoiceDto invoice)
	{

		await _business.CreateInvoiceAsync(invoice);
		return Ok("Invoice creata con successo");
		

	}

	[HttpGet(Name = "ReadInvoice")]
	public async Task<ActionResult> ReadInvoice(int invoiceId)
	{
		var result = await _business.GetInvoiceAsync(invoiceId);
		return Ok(result);
	}

	[HttpDelete(Name = "DeleteInvoice")]
	public async Task<ActionResult<string?>> DeleteInvoice(int invoiceId)
	{
		await _business.DeleteInvoiceAsync(invoiceId);
		return Ok("Invoice eliminata con successo");
	}
}
