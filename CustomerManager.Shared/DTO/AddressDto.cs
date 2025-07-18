﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Shared.DTO;

public class CreateAddressDto
{
	public string Street { get; set; }
	public string City { get; set; }
	public string PostalCode { get; set; }
	public string Country { get; set; }
}


public class ReadAndUpdateAddressDto
{
	public int Id { get; set; }
	public string Street { get; set; }
	public string City { get; set; }
	public string PostalCode { get; set; }
	public string Country { get; set; }
}