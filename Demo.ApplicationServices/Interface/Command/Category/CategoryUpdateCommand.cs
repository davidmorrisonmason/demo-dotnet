namespace Demo.ApplicationServices.Interface.Command.Category;

public record CategoryUpdateCommand(int Id, string Name) : Command()
{
}
