using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Shared.DTO
{
    public class ReadInvoiceProductsDto
    {
		public int Id { get; set; }
		public int Pieces { get; set; }
		public decimal Price { get; set; }
		public decimal VAT { get; set; }
		public int ProductId { get; set; } 
	}

	public class CreateInvoiceProductsDto
	{
		public int ProductId { get; set; }
		public int Pieces { get; set; } = 0;

	}
}
