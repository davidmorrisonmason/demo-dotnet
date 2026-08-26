namespace Demo.DomainServices.Interface.Command.Category;

public record CategoryAddProductCommand(int Id, string ProductName, decimal ProductPrice) : Command<Model.Domain.Product>()
{
}
