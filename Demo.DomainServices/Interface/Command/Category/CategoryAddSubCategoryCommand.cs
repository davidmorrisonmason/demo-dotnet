namespace Demo.DomainServices.Interface.Command.Category;

public record CategoryAddSubCategoryCommand(int Id, string Name) : Command<Model.Domain.Category>()
{
}
