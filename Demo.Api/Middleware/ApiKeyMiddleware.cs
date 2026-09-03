using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Encryption;
using Demo.DomainServices.Interface.Orchestration;
using Demo.DomainServices.Interface.Query.Client;
using Demo.Model.Domain.Exceptions;
using Demo.Model.Utils;
using Demo.Model.Validation;

namespace Demo.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IMediator mediator, IRequestContext requestContext, IEncryptionService encryptionService)
    {
        if (!context.Request.Headers.TryGetValue("x-api-key", out var apiKey) ||
            string.IsNullOrWhiteSpace(apiKey.ToString()))
        {
            throw new AuthorisationException(AuthorisationErrorType.Api_Key_Required.ToErrorMessage());
        }

        var plainText = apiKey.ToString();
        var client = await mediator.Send(new GetClientByApiKeyQuery(plainText), context.RequestAborted);

        if (client is null)
        {
            throw new AuthorisationException(AuthorisationErrorType.Api_Key_Invalid.ToErrorMessage());
        }

        requestContext.SetClient(client);
        await _next(context);
    }
}

public enum AuthorisationErrorType
{
    [ErrorDescription(ErrorCode = "API_KEY_REQUIRED", ErrorMessage = "API key is required")]
    Api_Key_Required,

    [ErrorDescription(ErrorCode = "API_KEY_INVALID", ErrorMessage = "API key is invalid")]
    Api_Key_Invalid,
}

