using Demo.ApplicationServices.Interface.Orchestration;

namespace Demo.ApplicationServices.Interface.Command;

public interface ICommand : IRequest
{
}

public interface ICommand<TResult> : IRequest<TResult>
{
}
