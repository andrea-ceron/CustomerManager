using CustomerManager.Business.Abstraction;
using System.Reactive.Subjects;
namespace CustomerManager.Business;

public class CustomerManagerSubject : ISubject
{
	private Subject<int> _endProductSubject = new();

	public IObservable<int> AddEndProductCustomerToStock => _endProductSubject;

	IObserver<int> ICustomerManagerObserver.AddEndProductCustomerToStock => _endProductSubject;
}
