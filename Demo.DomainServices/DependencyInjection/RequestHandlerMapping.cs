namespace Demo.DomainServices.DependencyInjection;

public record RequestHandlerMapping(Type RequestType, Type HandlerType, HandlerResponseType HandlerResponseType);

