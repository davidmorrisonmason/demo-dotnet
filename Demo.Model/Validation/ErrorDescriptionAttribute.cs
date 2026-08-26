namespace Demo.Model.Validation;

public class ErrorDescriptionAttribute : Attribute
{
    public required string ErrorCode { get; set; }
    public required string ErrorMessage { get; set; }
}

public static class ErrorDescriptionAttributeUtils
{
    public static string ErrorCode<T>(this T errorType) where T : Enum
    {
        var description = GetErrorDescriptionAttribute(errorType);
        return description == null ? errorType.ToString() : description.ErrorCode;
    }
    public static string ErrorMessage<T>(this T errorType) where T : Enum
    {
        var description = GetErrorDescriptionAttribute(errorType);
        return description == null ? errorType.ToString() : description.ErrorMessage;
    }

    private static ErrorDescriptionAttribute GetErrorDescriptionAttribute<T>(T errorCode) where T : Enum
    {
        try
        {
            var member = typeof(T).GetMember(errorCode.ToString());
            var errorDescription = member[0].GetCustomAttributes(typeof(ErrorDescriptionAttribute), false).FirstOrDefault() as ErrorDescriptionAttribute;
            if (errorDescription != null)
            {
                return errorDescription;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
