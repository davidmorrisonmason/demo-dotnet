namespace Demo.DomainServices.Interface.Transaction;

public interface IUnitOfWork
{
    Task Execute(Func<Task> action, bool transactional = true);
    Task Execute(Action action, bool transactional = true);
}
