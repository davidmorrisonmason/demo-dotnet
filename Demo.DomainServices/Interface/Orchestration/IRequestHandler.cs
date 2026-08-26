namespace Demo.DomainServices.Interface.Orchestration;

/// <summary>
/// Handler interface for request handlers that don't return a result
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IRequestHandler<TRequest> where TRequest : IRequest
{
    Task Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Handler interface for request handlers that return a result
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
{
    Task<TResult> Handle(TRequest request, CancellationToken cancellationToken);
}
