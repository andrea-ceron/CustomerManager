namespace Microsoft.Extensions.DependencyInjection;

public class CustomerManagerClientOption
{
	public const string SectionName = "CustomerManagerClientHttp";
	public string BaseAddress { get; set; } = "";

}
