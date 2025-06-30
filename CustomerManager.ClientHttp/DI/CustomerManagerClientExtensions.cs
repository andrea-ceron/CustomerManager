using CustomerManager.ClientHttp.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerManager.ClientHttp.DI;

public static  class CustomerManagerClientExtensions
{
	public static IServiceCollection  AddCustomerManagerClientHttp(this IServiceCollection services, IConfiguration config)
	{
		IConfigurationSection section = config.GetSection(CustomerManagerClientOption.SectionName);
		CustomerManagerClientOption options = section.Get<CustomerManagerClientOption>() ?? new();
		services.AddHttpClient<IClientHttp, ClientHttp>(o => {
			o.BaseAddress = new Uri(options.BaseAddress);
		}).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
		{
			ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
		});
		return services;
	}

}
