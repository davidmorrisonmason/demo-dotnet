namespace Demo.DomainServices.Interface.Command.Category;

public record CategoryUpdateProductCommand(int Id, int ProductId, string ProductName, decimal ProductPrice) : Command()
{
}
