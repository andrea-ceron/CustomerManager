using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Shared
{
    public class CustomerDto
    {
		public int Id { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string CompanyName { get; set; }
		public string VATNumber { get; set; }
		public string TaxCode { get; set; }
		public string CertifiedEmail { get; set; }
		public List<AddressDto> Address { get; set; }
		public List<int> RemovingAddress { get; set; }
		public List<AddressDto> AddingAddress { get; set; }

	}
}
