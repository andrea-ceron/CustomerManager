using CustomerManager.Repository.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Repository.Model
{
	public class Invoice {
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public InvoiceStatus Status { get; set; }
		public int CustomerId { get; set; }
		public Customer Customer { get; set; }
		public List<Product> Products { get; set; }
		public Address Address{get; set;}
		public int? AddressId { get; set; }

	}
}
