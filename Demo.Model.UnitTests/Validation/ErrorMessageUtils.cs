using Demo.Model.Domain.Validation;
using Demo.Model.Validation;

namespace Demo.Model.UnitTests.Validation
{
    public static class ErrorMessageUtils
    {
        public static IEnumerable<ErrorMessage> BuildErrorMessages<T>(this T error) where T : Enum
        {
            return BuildErrorMessages(new List<T> { error });
        }

        public static IEnumerable<ErrorMessage> BuildErrorMessages<T>(this IEnumerable<T> errors) where T : Enum
        {
            return errors.Select(x => BuildErrorMessage(x));
        }

        public static ErrorMessage BuildErrorMessage<T>(this T error) where T : Enum
        {
            return new ErrorMessage(error.ErrorCode(), error.ErrorMessage());
        }
    }
}
