namespace Demo.DomainServices.Interface.Command.Category;

public record CategoryUpdateSubCategoryCommand(int Id, int SubCategoryId, string Name) : Command()
{
}
