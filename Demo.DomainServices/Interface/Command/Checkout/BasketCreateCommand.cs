using Demo.Model.Domain.Checkout;

namespace Demo.DomainServices.Interface.Command.Checkout;

public record BasketCreateCommand(List<BasketItemCommand> BasketItems) : Command<Basket>
{
}

public record BasketItemCommand(int CategoryId, int ProductId, int Quantity)
{
}
