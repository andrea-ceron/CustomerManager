using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Repository.Model
{
    public class Address
    {
		public int Id { get; set; }
		public Customer Customer { get; set; }
		public int CustomerId { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string PostalCode { get; set; }
		public string Country { get; set; }
		public List<Invoice> Invoices { get; set; }
	}
}
