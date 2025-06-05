using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Repository;

    public class ExceptionHandlerRepository : Exception
    {
	public int StatusCode { get; }
	public Object? InvolvedElement { get; }

	public ExceptionHandlerRepository(string message, int statusCode = 400)
		: base(message)
	{
		StatusCode = statusCode;
	}

	public ExceptionHandlerRepository(string message, Object elem, int statusCode = 400)
		:base(message)
	{
		StatusCode = statusCode;
		InvolvedElement = elem;

	}
}
