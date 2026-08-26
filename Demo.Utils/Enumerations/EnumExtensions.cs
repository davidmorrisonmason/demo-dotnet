namespace Demo.Utils.Enumerations;

public static class EnumExtensions
{
    public static bool IsValidEnumValue<T>(this string enumString) where T : System.Enum
    {
        if (string.IsNullOrWhiteSpace(enumString))
        {
            return false;
        }

        return System.Enum.TryParse(typeof(T), enumString, out object _);
    }

    public static T EnumValueFromString<T>(this string enumString) where T : System.Enum
    {
        if (enumString != null)
        {
            if (System.Enum.TryParse(typeof(T), enumString, out object testValue))
            {
                return (T)testValue;
            }
        }

        string enumErrorString = string.IsNullOrWhiteSpace(enumString) ? "<no value supplied>" : enumString;

        throw new ArgumentException($"{enumErrorString} is not a valid {typeof(T).Name} enum string");
    }

    public static T EnumValueFromString<T>(this string enumString, T defaultValue) where T : System.Enum
    {
        if (enumString != null)
        {
            if (System.Enum.TryParse(typeof(T), enumString, out object testValue))
            {
                return (T)testValue;
            }
        }

        return defaultValue;
    }

    public static string Description<T>(this T enumValue) where T : System.Enum
    {
        var description = GetEnumDescriptionAttribute(enumValue);
        return description == null ? enumValue.ToString() : description.Description;
    }

    public static T FromDescription<T>(this string description) where T : System.Enum
    {
        foreach (T enumValue in System.Enum.GetValues(typeof(T)))
        {
            var testDescription = GetEnumDescriptionAttribute(enumValue);
            if (testDescription != null && testDescription.Description == description)
            {
                return enumValue;
            }
        }

        throw new ArgumentException($"{description} is not a valid enum value description");
    }

    private static EnumDescriptionAttribute GetEnumDescriptionAttribute<T>(T errorCode) where T : System.Enum
    {
        try
        {
            var member = typeof(T).GetMember(errorCode.ToString());

            if (member[0].GetCustomAttributes(typeof(EnumDescriptionAttribute), false).FirstOrDefault() is EnumDescriptionAttribute errorDescription)
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
