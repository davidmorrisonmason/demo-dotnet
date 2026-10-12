using Demo.ApplicationServices.Interface.Orchestration;

namespace Demo.ApplicationServices.Interface.Query;

public interface IQuery<TResult> : IRequest<TResult>
{
}
