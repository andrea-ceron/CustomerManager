using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Utility.Kafka.Abstraction.MessageHandlers;
namespace CustomerManager.Business.Kafka.MessageHandler;

public class MessageHandlerFactory(IOptions<KafkaTopicInput> optionsTopics) : IMessageHandlerFactory<string, string>
{
	private KafkaTopicInput _optionsTopics = optionsTopics.Value;

	public IMessageHandler<string, string> Create(string topic, IServiceProvider serviceProvider)
	{
		if (topic == _optionsTopics.EndProductStockToCustomer)
		{
			return ActivatorUtilities.CreateInstance<EndProductsKafkaMessageHandler>(serviceProvider);
		}
		throw new ArgumentException($"No message handler found for topic: {topic}");
	}
}
