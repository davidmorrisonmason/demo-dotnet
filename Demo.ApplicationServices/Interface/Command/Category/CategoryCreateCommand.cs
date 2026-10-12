namespace Demo.ApplicationServices.Interface.Command.Category;

public record CategoryCreateCommand(string Name) : Command<Model.Domain.Category>()
{
}
