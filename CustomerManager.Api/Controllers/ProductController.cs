using CustomerManager.Business.Abstraction;
using CustomerManager.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManager.Api.Controllers;


[ApiController]
[Route("[controller]/[action]")]
public class ProductController(IBusiness business, ILogger<ProductController> logger) : Controller
{
	private readonly IBusiness _business = business;
	private readonly ILogger<ProductController> _logger = logger;
	
	[HttpGet(Name = "ReadProduct")]
	public async Task<ActionResult<ReadAndUpdateProductDto>> GetProduct(int productId)
	{
		var result = await _business.GetProductAsync(productId);
		if (result is null)
			return NotFound();
		return Ok(result);
	}
}
