namespace Demo.DomainServices.Interface.Orchestration;

public interface IMediator
{
    /// <summary>
    /// Sends a request to the mediator, for handlers that don't return a response
    /// </summary>
    Task Send(IRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to the mediator, for handlers that return a response
    /// </summary>
    Task<TResult> Send<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
}
