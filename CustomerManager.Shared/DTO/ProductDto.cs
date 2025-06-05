using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Shared.DTO;
public class ReadAndUpdateProductDto
{
	public int Id { get; set; }
	public int AvailablePieces { get; set; }
	public decimal Price { get; set; }
	public int VAT { get; set; }
}

public class CreateProductDto
{
	public int AvailablePieces { get; set; }
	public decimal Price { get; set; }
	public int VAT { get; set; }
}
