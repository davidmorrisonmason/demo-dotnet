namespace Demo.DomainServices.Interface.Command.Category;

public record CategoryRemoveSubCategoryCommand(int Id, int SubCategoryId) : Command()
{
}
