using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Repository.Model
{
    public class InvoiceProducts
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
		public Invoice Invoice { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; }
		public int Pieces { get; set; }
		public decimal Price { get; set; }
		public decimal VAT { get; set; }

	}


}
