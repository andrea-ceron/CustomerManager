using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Business.DTOHelper;

public class CreateInvoiceProductHelper
{
	public int ProductId { get; set; }
	public int Pieces { get; set; } = 0;
	public decimal? Price { get; set; } = 0;
	public decimal? VAT { get; set; } = 0;
}
