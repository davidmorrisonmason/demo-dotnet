namespace Demo.DomainServices.Interface.Command.Category;

public record CategoryUpdateCommand(int Id, string? Name) : Command()
{
}
