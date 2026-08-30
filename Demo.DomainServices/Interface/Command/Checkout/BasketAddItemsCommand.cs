namespace Demo.DomainServices.Interface.Command.Checkout;

public record BasketAddItemsCommand(int BasketId, List<BasketItemCommand> BasketItems) : Command
{
}
