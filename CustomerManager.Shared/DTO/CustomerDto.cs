using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Shared.DTO;

public class ReadCustomerDto
{
	public int Id { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public string CompanyName { get; set; }
	public string VATNumber { get; set; }
	public string TaxCode { get; set; }
	public string CertifiedEmail { get; set; }
	public IEnumerable<ReadAndUpdateAddressDto>? Address { get; set; }
	public IEnumerable<ReadInvoiceProductsDto>? Invoices { get; set; }

}
public class ReadCustomerForSellingInvoiceControllerDto
{
	public int Id { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public string CompanyName { get; set; }
	public string VATNumber { get; set; }
	public string TaxCode { get; set; }
	public string CertifiedEmail { get; set; }

}


public class UpdateCustomerDto
{
	public int Id { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public string CompanyName { get; set; }
	public string VATNumber { get; set; }
	public string TaxCode { get; set; }
	public string CertifiedEmail { get; set; }
	public IEnumerable<int> RemovingAddress { get; set; }
	public IEnumerable<CreateAddressDto> AddingAddress { get; set; }

}

public class CreateCustomerDto
{
	public string Email { get; set; }
	public string Phone { get; set; }
	public string CompanyName { get; set; }
	public string VATNumber { get; set; }
	public string TaxCode { get; set; }
	public string CertifiedEmail { get; set; }
	public IEnumerable<CreateAddressDto> AddingAddress { get; set; }

}