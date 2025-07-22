using CustomerManager.Business.Abstraction;
using CustomerManager.Business.Factory;
using CustomerManager.Repository.Abstraction;
using CustomerManager.Repository.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockManager.Shared.DTO;
using Utility.Kafka.Abstraction.Clients;
using Utility.Kafka.ExceptionManager;
using Utility.Kafka.MessageHandlers;
namespace CustomerManager.Business.Kafka;

public class ProducerServiceWithSubscription(
	IServiceProvider serviceProvider,
	ErrorManagerMiddleware errormanager,
	IOptions<KafkaTopicsOutput> optionTopics
	, IServiceScopeFactory serviceScopeFactory
	, IProducerClient<string, string> producerClient
	, ICustomerManagerObservable observable,
	ILogger<ProducerServiceWithSubscription> logger

	)
	: Utility.Kafka.Services.ProducerServiceWithSubscription(serviceProvider, errormanager)
{
	protected override IEnumerable<string> GetTopics()
	{
		return optionTopics.Value.GetTopics();
	}

	protected async  override Task OperationsAsync(CancellationToken cancellationToken)
	{
		using IServiceScope scope = serviceScopeFactory.CreateScope();
		IRepository repository = scope.ServiceProvider.GetRequiredService<IRepository>();
		IBusiness business = scope.ServiceProvider.GetRequiredService<IBusiness>();

		IEnumerable<TransactionalOutbox> transactionalOutboxes = (await repository.GetAllTransactionalOutbox(cancellationToken)).OrderBy(x => x.Id);
		if (!transactionalOutboxes.Any())
		{
			return;
		}
		foreach (TransactionalOutbox elem in transactionalOutboxes)
		{
			string topic = elem.Table switch
			{
				nameof(EndProductDtoForKafka) => optionTopics.Value.EndProductCustomerToStock,
				_ => throw new ArgumentOutOfRangeException($"La tabella {elem.Table} non è prevista come topic nel Producer")
			};
			try
			{
				await producerClient.ProduceAsync(topic, elem.Id.ToString(), elem.Message, null, cancellationToken);
				await repository.DeleteTransactionalOutboxAsync(elem.Id, cancellationToken);
				await repository.SaveChangesAsync(cancellationToken);
			}
			catch(InvalidOperationException ex)
			{
				var res = (await repository.GetAllTransactionalOutbox(cancellationToken));
				switch (elem.Table)
				{
					case nameof(EndProductDtoForKafka):
						await EndProductCompensationOperation(elem, repository, business, cancellationToken);
						break;

					default:
						logger.LogError("Tabella non supportata per compensazione: {table}", elem.Table);
						break;
				}
				await repository.SaveChangesAsync(cancellationToken);
				continue;
			}
			catch (Exception ex)
			{
				continue;
			}

		}
	}

	private async Task EndProductCompensationOperation(TransactionalOutbox elem, IRepository repository, IBusiness business, CancellationToken cancellationToken = default)
	{
		var opMsg = TransactionalOutboxFactory.Deserialize<EndProductDtoForKafka>(elem.Message);
		switch (opMsg.Operation)
		{
			case Operations.Insert:
				logger.LogWarning("Incontrata Label Insert. Compensazione Insert: eliminato rawMaterial con ID {id}", opMsg.Dto.Id);
				break;

			case Operations.Update:
				logger.LogWarning("Incontrata Label Update. Compensazione Update: eliminato rawMaterial con ID {id}", opMsg.Dto.Id);
				break;

			case Operations.Delete:
				logger.LogWarning("Incontrata Label Delete. Compensazione Delete: eliminato rawMaterial con ID {id}", opMsg.Dto.Id);
				break;

			case Operations.CompensationInsert:
				logger.LogWarning("Incontrata Compensazione Insert: nessuna azione eseguita dal produttore");
				break;

			case Operations.CompensationUpdate:
				logger.LogWarning("Incontrata Compensazione Update: nessuna azione eseguita dal produttore");
				break;

			case Operations.CompensationDelete:
				logger.LogWarning("Incontrata Compensazione Delete: nessuna azione eseguita dal produttore");
				break;

			default:
				logger.LogError("Operazione sconosciuta: {op}", opMsg.Operation);
				break;
		}
	}
	protected override IDisposable Subscribe(TaskCompletionSource<bool> tcs)
	{
		return observable.AddEndProductCustomerToStock.Subscribe((change) => tcs.TrySetResult(true));
	}
}
