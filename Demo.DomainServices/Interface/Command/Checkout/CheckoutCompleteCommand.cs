namespace Demo.DomainServices.Interface.Command.Checkout;

public record CheckoutCompleteCommand(
    int BasketId,
    string Recipient,
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string? AddressLine4,
    string City,
    string PostCode) : Command
{
}
