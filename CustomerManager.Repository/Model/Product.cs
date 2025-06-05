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
		public int AvailablePieces { get; set; }
		public decimal Price { get; set; }
		public int VAT { get; set; }
		public IEnumerable<InvoiceProducts> InvoiceProduct { get; set; } 
	}
}
