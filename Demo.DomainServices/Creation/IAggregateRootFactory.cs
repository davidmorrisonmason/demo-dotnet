using Demo.Model.Domain;
using Demo.Model.Domain.Checkout;

namespace Demo.DomainServices.Creation;

public interface IAggregateRootFactory
{
    Category NewCategory(string name, int clientId);
    Client NewClient(string name, string apiKey);
    CheckoutCompletion NewCheckoutCompletion(
        int basketId,
        string recipient,
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string? addressLine4,
        string city,
        string postCode);
}

