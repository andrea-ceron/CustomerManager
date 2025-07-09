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
	public async Task<ActionResult<ReadAndUpdateProductDto>> ReadProduct(int productId)
	{
		var result = await _business.GetProductAsync(productId);
		if (result is null)
			return NotFound();
		return Ok(result);
	}

	[HttpPost(Name = "BuildProduct")]
	public async Task<ActionResult<string?>> BuildProduct(IEnumerable<BuildEndProductDto> ProductsToCreate )
	{
		await _business.BuildProduct(ProductsToCreate);
		return Ok("Prodotti Creati con successo");
	}
}
