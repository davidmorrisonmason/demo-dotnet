using Demo.DomainServices.Interface.Orchestration;

namespace Demo.DomainServices.Interface.Command;

public interface ICommand : IRequest
{
}

public interface ICommand<TResult> : IRequest<TResult>
{
}
