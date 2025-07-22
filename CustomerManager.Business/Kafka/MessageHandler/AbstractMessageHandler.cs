using AutoMapper;
using Utility.Kafka.ExceptionManager;
using Utility.Kafka.MessageHandlers;
namespace CustomerManager.Business.Kafka.MessageHandler;

public abstract class AbstractMessageHandler<TMessageDto, TDomainDto>
	(ErrorManagerMiddleware errorManager, IMapper map)
	: OperationMessageHandlerBase<TMessageDto>(errorManager)
	where TMessageDto : class
	where TDomainDto : class
{
	protected async  override Task DeleteAsync(TMessageDto messageDto, CancellationToken cancellationToken = default)
	{
		await DeleteDto(map.Map<TDomainDto>(messageDto), messageDto, cancellationToken);
	}

	protected async override Task InsertAsync(TMessageDto messageDto, CancellationToken cancellationToken = default)
	{
		await InsertDto(map.Map<TDomainDto>(messageDto), cancellationToken);
	}

	protected async  override Task UpdateAsync(TMessageDto messageDto, CancellationToken cancellationToken = default)
	{
		await UpdateDto(map.Map<TDomainDto>(messageDto), cancellationToken);
	}

	protected override async Task CompensationDeleteAsync(TMessageDto messageDto, CancellationToken cancellationToken = default)
	{
		await CompensationDeleteDto(map.Map<TDomainDto>(messageDto), messageDto, cancellationToken);
	}

	protected override async Task CompensationInsertAsync(TMessageDto messageDto, CancellationToken cancellationToken = default)
	{
		await CompensationInsertDto(map.Map<TDomainDto>(messageDto), cancellationToken);
	}

	protected override async Task CompensationUpdateAsync(TMessageDto messageDto, CancellationToken cancellationToken = default)
	{
		await CompensationUpdateDto(map.Map<TDomainDto>(messageDto), cancellationToken);
	}

	protected abstract Task InsertDto(TDomainDto? messageDto, CancellationToken ct = default);
	protected abstract Task UpdateDto(TDomainDto? messageDto, CancellationToken ct = default);
	protected abstract Task DeleteDto(TDomainDto? messageDto, TMessageDto rawPayload, CancellationToken ct = default);
	protected abstract Task CompensationInsertDto(TDomainDto? messageDto, CancellationToken ct = default);
	protected abstract Task CompensationUpdateDto(TDomainDto? messageDto, CancellationToken ct = default);
	protected abstract Task CompensationDeleteDto(TDomainDto? messageDto, TMessageDto rawPayload, CancellationToken ct = default);
}
