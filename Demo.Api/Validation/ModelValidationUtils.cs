using Demo.Model.Domain.Validation;

namespace Demo.Api.Validation;

public static class ModelValidationUtils
{
    /// <summary>
    /// Utility method to turn a Core Three custom validation attribute error message into the same ErrorMessage format used
    /// by the domain layer. Custom attributes store the error message in the format API|Code|Description. We extract the code and
    /// description and use them to populate the error message object.
    /// </summary>
    /// <param name="validationErrorMessage"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static bool TryGetValidationErrorMessage(this string validationErrorMessage, out ErrorMessage errorMessage)
    {
        errorMessage = null;
        bool ok = false;

        if (validationErrorMessage != null && validationErrorMessage.StartsWith("API|"))
        {
            var tokens = validationErrorMessage.Split("|");
            if (tokens.Length == 3)
            {
                errorMessage = new ErrorMessage(tokens[1], tokens[2]);
                ok = true;
            }
        }
        return ok;
    }
}
