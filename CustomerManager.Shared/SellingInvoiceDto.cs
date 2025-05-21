using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Shared
{
    public class SellingInvoiceDto
    {
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public int CustomerId { get; set; }
		public List<ProductDto> ProductList { get; set; }
		public AddressDto Address { get; set; }

		public int AddressId { get; set; }
	}
}
