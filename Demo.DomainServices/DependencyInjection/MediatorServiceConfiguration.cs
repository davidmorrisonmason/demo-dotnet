using Demo.DomainServices.Interface.Orchestration;
using System.Reflection;

namespace Demo.DomainServices.DependencyInjection;

/// <summary>
/// Configuration class for mediator configuration
/// </summary>
public class MediatorServiceConfiguration : IMediatorServiceConfiguration
{
    /// <summary>
    /// Stores a list of handler mappings for all request types
    /// </summary>
    private List<RequestHandlerMapping> _requestHandlerMappings = [];

    /// <summary>
    /// Returns all request / handler mappings
    /// </summary>
    public IEnumerable<RequestHandlerMapping> RequestHandlerMappings => _requestHandlerMappings;

    public void RegisterServicesFromAssembly(Assembly assembly)
    {
        RegisterNonResponseHandlers(assembly);
        RegisterResponseHandlers(assembly);
    }

    /// <summary>
    /// Registers all handlers that do not return a response with their associated request type
    /// </summary>
    private void RegisterNonResponseHandlers(Assembly assembly)
    {
        var handlers = assembly.GetTypes().Where(type =>
            !type.IsAbstract &&
            type.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)));

        foreach (var handlerType in handlers)
        {
            var requestType = handlerType.GetInterfaces().Where(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
                .Select(i => i.GetGenericArguments().First())
                .First();

            _requestHandlerMappings.Add(new RequestHandlerMapping(requestType, handlerType, HandlerResponseType.NoResponse));
        }
    }

    /// <summary>
    /// Registers all handlers that return a response with their associated request type
    /// </summary>
    private void RegisterResponseHandlers(Assembly assembly)
    {
        var handlers = assembly.GetTypes().Where(type =>
            !type.IsAbstract &&
            type.GetInterfaces().Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

        foreach (var handlerType in handlers)
        {
            var requestType = handlerType.GetInterfaces().Where(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .Select(i => i.GetGenericArguments().First())
                .First();

            _requestHandlerMappings.Add(new RequestHandlerMapping(requestType, handlerType, HandlerResponseType.Response));
        }
    }
}
