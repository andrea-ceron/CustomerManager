using AutoMapper;
using CustomerManager.Business.Abstraction;
using CustomerManager.Business.Factory;
using CustomerManager.Repository.Abstraction;
using CustomerManager.Repository.Model;
using Microsoft.Extensions.Logging;
using StockManager.Shared.DTO;
using Utility.Kafka.ExceptionManager;

namespace CustomerManager.Business.Kafka.MessageHandler;

public class EndProductsKafkaMessageHandler(
	ILogger<EndProductsKafkaMessageHandler> logger,
	IMapper map,
	IRepository repository,
	ErrorManagerMiddleware errorManager,
	ICustomerManagerObserver observer)
	: AbstractMessageHandler<EndProductDtoForKafka, Product>(errorManager, map)
{
	static private List<InvoiceProducts>? ListOfInvoiceProducts = new();

	protected async override Task DeleteDto(Product? message, EndProductDtoForKafka payload, CancellationToken ct = default)
	{
		 ListOfInvoiceProducts = (List<InvoiceProducts>?)await repository.GetAllInvoiceProductsByProductId(message.Id, ct);
		if (message == null)
			throw new Exception("Il messaggio non può essere null.");

		if (ListOfInvoiceProducts != null && !ListOfInvoiceProducts.Any())
		{
			await repository.DeleteProductAsync(message.Id, ct);
			await repository.SaveChangesAsync(ct);
		}
		else
		{
			payload.Id = 0;
			logger.LogInformation("Produzione operazione di compensazione eseguita");

			var recordKafka = map.Map<EndProductDtoForKafka>(payload);
			var record = TransactionalOutboxFactory.CreateCompensationInsertRawMaterial(recordKafka);
			await repository.InsertTransactionalOutboxAsync(record, ct);

			logger.LogWarning("Materia prima eliminata");
			await repository.DeleteProductAsync(message.Id, ct);

			await repository.SaveChangesAsync(ct);
			observer.AddEndProductCustomerToStock.OnNext(1);
		}
	}

	protected async override Task InsertDto(Product? messageDto, CancellationToken ct = default)
	{
		if (messageDto == null)
		{
			logger.LogWarning("Il messaggio del prodotto è null. Operazione di creazione annullata.");
			return;
		}

		logger.LogInformation("Creazione prodotto con ID: {ProductId}", messageDto.Id);
		var result = await repository.CreateProductAsync(messageDto, ct);
		await repository.SaveChangesAsync(ct);

		if (ListOfInvoiceProducts.Count > 0)
		{
			logger.LogInformation("Eseguo la creazione della lista di RawMaterialForProduction");
			ListOfInvoiceProducts.ForEach(async x =>
			{
				x.ProductId = result.Id;
				x.Id = 0;
				await repository.CreateInvoiceProductAsync(x, ct);
			});

			await repository.SaveChangesAsync(ct);
			ListOfInvoiceProducts.Clear();
		}
	}

	protected async override Task UpdateDto(Product? messageDto, CancellationToken ct = default)
	{
		if (messageDto == null)
		{
			logger.LogWarning("Il messaggio del prodotto è null. Operazione di aggiornamento annullata.");
			return;
		}

		logger.LogInformation("Aggiornamento prodotto con ID: {ProductId}", messageDto.Id);
		await repository.UpdateProductAsync(messageDto, ct);
		await repository.SaveChangesAsync(ct);
	}
}
