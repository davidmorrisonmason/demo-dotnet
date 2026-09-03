using Demo.Model.Validation;

namespace Demo.DomainServices.Command.Client;

public enum ClientCommandErrorType
{
    [ErrorDescription(ErrorCode = "CLIENT_NAME_REQUIRED", ErrorMessage = "Client name is required")]
    Client_Name_Required,

    [ErrorDescription(ErrorCode = "CLIENT_NAME_MUST_BE_UNIQUE", ErrorMessage = "Client name must be unique")]
    Client_Name_Must_Be_Unique,

    [ErrorDescription(ErrorCode = "CLIENT_API_KEY_REQUIRED", ErrorMessage = "Client API key is required")]
    Client_ApiKey_Required,

    [ErrorDescription(ErrorCode = "CLIENT_API_KEY_MUST_BE_UNIQUE", ErrorMessage = "Client API key must be unique")]
    Client_ApiKey_Must_Be_Unique,
}
