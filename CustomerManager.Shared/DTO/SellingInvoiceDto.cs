using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Shared.DTO
{
    public class ReadSellingInvoiceDto
    {
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public int CustomerId { get; set; }
		public ReadCustomerForSellingInvoiceControllerDto Customer { get; set; }
		public IEnumerable<ReadInvoiceProductsDto> ProductList { get; set; }
		public ReadAndUpdateAddressDto Address { get; set; }
	}

	public class CreateSellingInvoiceDto
	{
		public DateTime Date { get; set; }
		public int CustomerId { get; set; }
		public IEnumerable<CreateInvoiceProductsDto> ProductList { get; set; }
		public int AddressId { get; set; }
	}
}
