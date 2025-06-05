using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomerManager.Business.Factory
{
    public static class TransactionalOutboxFactory
    {
		//private static TransactionalOutbox Create<TDTO>(string table, TDTO dto, string operation) where TDTO : class, new()
		//{

		//	OperationMessage<TDTO> opMsg = new OperationMessage<TDTO>()
		//	{
		//		Dto = dto,
		//		Operation = operation
		//	};
		//	opMsg.CheckMessage();

		//	return new TransactionalOutbox()
		//	{
		//		Tabella = table,
		//		Messaggio = JsonSerializer.Serialize(opMsg)
		//	};
		//}
	}
}
