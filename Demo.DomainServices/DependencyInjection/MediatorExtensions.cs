using Demo.DomainServices.Interface.Orchestration;
using Demo.DomainServices.Orchestration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.DomainServices.DependencyInjection;

public static class MediatorExtensions
{
    public static void AddMediator(this IServiceCollection services, Action<MediatorServiceConfiguration> configurationAction)
    {
        var configuration = new MediatorServiceConfiguration();
        configurationAction.Invoke(configuration);
        ApplyConfiguration(services, configuration);
    }

    private static void ApplyConfiguration(IServiceCollection services, MediatorServiceConfiguration configuration)
    {
        foreach (var handlerMapping in configuration.RequestHandlerMappings)
        {
            if (handlerMapping.HandlerResponseType == HandlerResponseType.NoResponse)
            {
                var mediatorHandlerType = typeof(IRequestHandler<>).MakeGenericType(handlerMapping.RequestType);
                services.AddTransient(mediatorHandlerType, handlerMapping.HandlerType);
            }
            else
            {
                var responseType = handlerMapping.HandlerType.GetInterfaces().Where(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    .Select(t => t.GetGenericArguments()[1])
                    .First();
                var mediatorHandlerType = typeof(IRequestHandler<,>).MakeGenericType(handlerMapping.RequestType, responseType);
                services.AddTransient(mediatorHandlerType, handlerMapping.HandlerType);
            }
        }

        services.AddTransient<IMediator, Mediator>();
    }
}
