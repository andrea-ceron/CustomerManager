using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Repository.Model
{
    public class Product
    {
		public int Id { get; set; }
		public int Pieces { get; set; }
		public decimal Price { get; set; }
		public int VAT { get; set; }
		public Invoice Invoice { get; set; } 
		public int InvoiceId { get; set; }
	}
}
