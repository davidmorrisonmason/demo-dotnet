namespace Demo.Utils.Enumerations;

public class EnumDescriptionAttribute : Attribute
{
    public string Description { get; set; }

    public EnumDescriptionAttribute(string description)
    {
        Description = description;
    }
}