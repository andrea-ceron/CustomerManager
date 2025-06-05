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



	[HttpPost(Name = "CreateProduct")]
	public async Task<ActionResult> CreateProduct(CreateProductDto product)
	{
		await _business.CreateProductAsync(product);
		return Ok();
	}

	[HttpGet(Name = "ReadProduct")]
	public async Task<ActionResult<ReadCustomerDto>> GetProduct(int productId)
	{
		var result = await _business.GetProductAsync(productId);
		return Ok(result);
	}

	[HttpPut(Name = "UpdateProduct")]
	public async Task<ActionResult> UpdateProduct(ReadAndUpdateProductDto product)
	{
		await _business.UpdateProductAsync(product);
		return Ok();
	}

	[HttpDelete(Name = "DeleteProduct")]
	public async Task<ActionResult> DeleteProduct(int productId)
	{
		await _business.DeleteProductAsync(productId);
		return Ok();
	}

}
