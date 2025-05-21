using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Shared
{
    public class ProductDto
    {
		public int Id { get; set; }
		public int Pieces { get; set; }
		public decimal Price { get; set; }
		public int VAT { get; set; }
	}
}
