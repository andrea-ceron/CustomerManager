using CustomerManager.Repository.Model;
using StockManager.Shared.DTO;
using System.Text.Json;
using Utility.Kafka.MessageHandlers;
namespace CustomerManager.Business.Factory;

public static class TransactionalOutboxFactory
{
	public static TransactionalOutbox CreateCompensationInsertRawMaterial(EndProductDtoForKafka dto) => Create(dto, Operations.CompensationInsert);

	private static TransactionalOutbox Create(EndProductDtoForKafka dto, string operation) => Create(nameof(EndProductDtoForKafka), dto, operation);

	private static TransactionalOutbox Create<TDTO>(string table, TDTO dto, string operation) 
		where TDTO : class, new()
	{

		OperationMessage<TDTO> opMsg = new()
		{
			Dto = dto,
			Operation = operation,
		};

		return new TransactionalOutbox()
		{
			Table = table,
			Message = JsonSerializer.Serialize(opMsg)
		};
	}

	public static OperationMessage<TDTO> Deserialize<TDTO>(string json) where TDTO : class, new()
	{
		return JsonSerializer.Deserialize<OperationMessage<TDTO>>(json)!;
	}
}
