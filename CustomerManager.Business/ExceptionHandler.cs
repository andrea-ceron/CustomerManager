using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Business
{
    public class ExceptionHandler : Exception
    {
		public int StatusCode { get; }
		public Object? InvolvedElement { get; }

		public ExceptionHandler(string message, int statusCode = 400)
			: base(message)
		{
			StatusCode = statusCode;
		}

		public ExceptionHandler(string message, Object elem, int statusCode = 400)
			:base(message)
		{
			StatusCode = statusCode;
			InvolvedElement = elem;

		}
	}
}
