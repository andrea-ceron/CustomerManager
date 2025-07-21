using System.ComponentModel.DataAnnotations;
using Utility.Kafka.DependencyInjection;
namespace CustomerManager.Business.Kafka;

public class KafkaTopicInput : AbstractInputKafkaTopics
{
	[Required]
	public string EndProductStockToCustomer { get; set; }

	public override IEnumerable<string> GetTopics() => [EndProductStockToCustomer];
}

public class KafkaTopicsOutput : AbstractOutputKafkaTopics
{
	[Required]
	public string EndProductCustomerToStock { get; set; }
	public override IEnumerable<string> GetTopics() => [EndProductCustomerToStock];
}