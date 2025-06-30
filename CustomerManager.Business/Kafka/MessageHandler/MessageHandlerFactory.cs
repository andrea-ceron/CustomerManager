using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Kafka.Abstraction.MessageHandlers;
using Utility.Kafka.Services;

namespace CustomerManager.Business.Kafka.MessageHandler;

public class MessageHandlerFactory(ILogger<ConsumerService<KafkaTopicInput>> logger, IOptions<KafkaTopicInput> optionsTopics) : IMessageHandlerFactory<string, string>
{
	private KafkaTopicInput _optionsTopics = optionsTopics.Value;

	public IMessageHandler<string, string> Create(string topic, IServiceProvider serviceProvider)
	{
		if (topic == _optionsTopics.EndProduct)
		{
			return ActivatorUtilities.CreateInstance<EndProductsKafkaMessageHandler>(serviceProvider);
		}
		throw new ArgumentException($"No message handler found for topic: {topic}");
	}
}
