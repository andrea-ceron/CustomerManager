using CustomerManager.Business.Abstraction;
using CustomerManager.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManager.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class CustomerController(IBusiness business, ILogger<CustomerController> logger) : Controller
{
		private readonly IBusiness _business = business;
		private readonly ILogger<CustomerController> _logger = logger;



	[HttpPost(Name = "CreateCustomer")]
	public async Task<ActionResult> CreateCustomer(CustomerDto customer)
    {

		await _business.CreateCustomerAsync(customer, customer.AddingAddress);
		return Ok();


    }

	[HttpGet(Name = "ReadCustomer")]
	public async Task<ActionResult> GetCustomer(int customerId)
    {
        CustomerDto? customer = await _business.GetCustomerAsync(customerId);
		if (customer == null) return NotFound("Customer non trovato");
        return Ok(customer);
    }

	[HttpPut(Name = "UpdateCustomer")]
	public async Task<ActionResult> UpdateCustomer(CustomerDto customer)
	{
		await _business.UpdateCustomerAsync(customer, customer.AddingAddress, customer.RemovingAddress);
		return Ok();
	}

	[HttpDelete(Name = "DeleteCustomer")]
	public async Task<ActionResult> DeleteCustomer(int customerId)
	{
		await _business.DeleteCustomerAsync(customerId);
		return Ok();
	}

}
