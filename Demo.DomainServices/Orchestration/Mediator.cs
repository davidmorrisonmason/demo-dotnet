using Demo.DomainServices.Interface.Orchestration;

namespace Demo.DomainServices.Orchestration;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var handler = _serviceProvider.GetService(handlerType);
        var handlerMethod = handlerType.GetMethod("Handle");
        var task = (Task)handlerMethod!.Invoke(handler, new object[] { request, cancellationToken })!;

        await task;
    }

    public async Task<TResult> Send<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResult));
        var handler = _serviceProvider.GetService(handlerType);
        var handlerMethod = handlerType.GetMethod("Handle");
        var task = (Task<TResult>)handlerMethod!.Invoke(handler, new object[] { request, cancellationToken })!;

        return await task;
    }
}
