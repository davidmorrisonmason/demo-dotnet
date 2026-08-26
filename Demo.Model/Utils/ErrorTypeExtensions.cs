using Demo.Model.Domain.Validation;
using Demo.Model.Validation;

namespace Demo.Model.Utils;

public static class ErrorTypeExtensions
{
    public static ErrorMessage ToErrorMessage<T>(this T errorType) where T : Enum
    {
        return new ErrorMessage(errorType.ErrorCode(), errorType.ErrorMessage());
    }
}

