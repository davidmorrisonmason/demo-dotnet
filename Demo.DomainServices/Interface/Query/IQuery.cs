using Demo.DomainServices.Interface.Orchestration;

namespace Demo.DomainServices.Interface.Query;

public interface IQuery<TResult> : IRequest<TResult>
{
}
