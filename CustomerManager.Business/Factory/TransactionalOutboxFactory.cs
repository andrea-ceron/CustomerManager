using CustomerManager.Repository.Model;
using StockManager.Shared.DTO;
using System.Text.Json;
using Utility.Kafka.MessageHandlers;
namespace CustomerManager.Business.Factory;

public static class TransactionalOutboxFactory
{
	public static TransactionalOutbox CreateCompensationInsertRawMaterial(EndProductDtoForKafka dto) => Create(dto, Operations.CompensationInsert);

	private static TransactionalOutbox Create(EndProductDtoForKafka dto, string operation, EndProductDtoForKafka? previousState = null) => Create(nameof(EndProductDtoForKafka), dto, operation, previousState);

	private static TransactionalOutbox Create<TDTO, TModel>(string table, TDTO dto, string operation, TModel? model) 
		where TDTO : class 
		where TModel: class, new()
	{

		OperationMessage<TDTO, TModel> opMsg = new()
		{
			Dto = dto,
			Operation = operation,
			previousState = model

		};

		return new TransactionalOutbox()
		{
			Table = table,
			Message = JsonSerializer.Serialize(opMsg)
		};
	}

	public static OperationMessage<TDTO, TModel> Deserialize<TDTO, TModel>(string json) where TDTO : class where TModel : class, new()
	{
		return JsonSerializer.Deserialize<OperationMessage<TDTO, TModel>>(json)!;
	}
}
