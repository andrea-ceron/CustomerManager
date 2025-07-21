namespace CustomerManager.Business.Abstraction;
public interface ICustomerManagerObserver
{
	IObserver<int> AddEndProductCustomerToStock { get; }

}
