using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CustomerManager.Repository.Enumeration;

namespace CustomerManager.Repository.Model
{
    public class Customer
    {
		public int Id { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public List<Address> Address { get; set; }
		public List<Invoice> Invoices { get; set; }
		public IdentityStatus Status { get; set; }
		public string CompanyName { get; set; }
		public string VATNumber { get; set; }
		public string TaxCode { get; set; }
		public string CertifiedEmail { get; set; }
	}
}
