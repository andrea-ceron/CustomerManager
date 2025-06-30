using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Kafka.DependencyInjection;

namespace CustomerManager.Business.Kafka;

public class KafkaTopicInput : AbstractInputKafkaTopics
{
	[Required]
	[ConfigurationKeyName("EndProduct")]
	public string EndProduct { get; set; }

	public override IEnumerable<string> GetTopics() => [EndProduct];
}
