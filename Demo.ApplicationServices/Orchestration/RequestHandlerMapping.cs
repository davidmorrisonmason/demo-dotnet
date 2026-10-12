namespace Demo.ApplicationServices.Orchestration;

public record RequestHandlerMapping(Type RequestType, Type HandlerType, HandlerResponseType HandlerResponseType);

