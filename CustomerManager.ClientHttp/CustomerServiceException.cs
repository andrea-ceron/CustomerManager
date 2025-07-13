namespace CustomerManager.ClientHttp;

public class CustomerServiceException : Exception
{
	public int StatusCode { get; }
	public string? ResponseContent { get; }

	public CustomerServiceException(int statusCode, string? responseContent)
		: base($"Chiamata a CustomerService fallita con StatusCode {statusCode}")
	{
		StatusCode = statusCode;
		ResponseContent = responseContent;
	}
}
