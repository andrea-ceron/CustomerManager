using AutoMapper;
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
	ErrorManagerMiddleware errorManager	)
	: AbstractMessageHandler<EndProductDtoForKafka, Product>(errorManager, map)
{
	protected override Task CompensationDeleteDeleteDto(Product? messageDto, CancellationToken ct = default)
	{
		throw new NotImplementedException();
	}

	protected override Task CompensationInsertDto(Product? domainDto, CancellationToken ct = default)
	{
		throw new NotImplementedException();
	}

	protected override Task CompensationUpdateDto(Product? messageDto, CancellationToken ct = default)
	{
		throw new NotImplementedException();
	}

	protected override async Task DeleteDto(Product? messageDto, CancellationToken ct = default)
	{
		await repository.DeleteProductAsync(messageDto?.Id ?? 0, ct);
		await repository.SaveChangesAsync(ct);

	}

	protected async override Task InsertDto(Product? messageDto, CancellationToken ct = default)
	{
		await repository.CreateProductAsync(messageDto, ct);
		await repository.SaveChangesAsync(ct);
	}

	protected async  override Task UpdateDto(Product? messageDto, CancellationToken ct = default)
	{
		await repository.UpdateProductAsync(messageDto, ct);
		await repository.SaveChangesAsync(ct);
	}
}
