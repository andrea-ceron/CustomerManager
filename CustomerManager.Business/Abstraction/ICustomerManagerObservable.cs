namespace CustomerManager.Business.Abstraction;

public interface ICustomerManagerObservable
{
	IObservable<int> AddEndProductCustomerToStock { get; }
}
