﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Kafka.ExceptionManager;
using Utility.Kafka.MessageHandlers;

namespace CustomerManager.Business.Kafka.MessageHandler;

public abstract class AbstractMessageHandler<TMessageDto, TDomainDto>
	(ErrorManagerMiddleware errorManager, IMapper map)
	: OperationMessageHandlerBase<TMessageDto, TDomainDto>(errorManager)
	where TMessageDto : class
	where TDomainDto : class
{
	protected async  override Task DeleteAsync(TMessageDto messageDto, CancellationToken cancellationToken = default)
	{
		await DeleteDto(map.Map<TDomainDto>(messageDto), cancellationToken);
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
		await DeleteDto(map.Map<TDomainDto>(messageDto), cancellationToken);
	}

	protected override async Task CompensationInsertAsync(TMessageDto messageDto, CancellationToken cancellationToken = default)
	{
		await InsertDto(map.Map<TDomainDto>(messageDto), cancellationToken);
	}

	protected override async Task CompensationUpdateAsync(TMessageDto messageDto, CancellationToken cancellationToken = default)
	{
		await UpdateDto(map.Map<TDomainDto>(messageDto), cancellationToken);
	}
	protected abstract Task InsertDto(TDomainDto? domainDto, CancellationToken ct = default);
	protected abstract Task UpdateDto(TDomainDto? messageDto, CancellationToken ct = default);
	protected abstract Task DeleteDto(TDomainDto? messageDto, CancellationToken ct = default);
	protected abstract Task CompensationInsertDto(TDomainDto? domainDto, CancellationToken ct = default);
	protected abstract Task CompensationUpdateDto(TDomainDto? messageDto, CancellationToken ct = default);
	protected abstract Task CompensationDeleteDeleteDto(TDomainDto? messageDto, CancellationToken ct = default);
}
