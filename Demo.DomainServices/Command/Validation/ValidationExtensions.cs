using Demo.Model.Validation;
using FluentValidation;

namespace Demo.DomainServices.Command.Validation;

public static class ValidationExtensions
{
    /// <summary>
    /// Utility method for adding validation error codes and messages to Fluent validation call chains
    /// </summary>
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty, TEnum>(this IRuleBuilderOptions<T, TProperty> options, TEnum errorType) where TEnum : Enum
    {
        return options
            .WithErrorCode(errorType.ErrorCode())
            .WithMessage(errorType.ErrorMessage());

    }
}
