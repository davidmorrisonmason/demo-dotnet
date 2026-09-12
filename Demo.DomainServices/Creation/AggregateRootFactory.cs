using Demo.Model.Domain;
using Demo.Model.Domain.Checkout;

namespace Demo.DomainServices.Creation;

public class AggregateRootFactory : IAggregateRootFactory
{
    public Category NewCategory(string name, int clientId)
    {
        return DoCreate(new Category(name, clientId));
    }

    public Client NewClient(string name, string apiKey)
    {
        return DoCreate(new Client(name, apiKey));
    }

    public CheckoutCompletion NewCheckoutCompletion(
        int basketId,
        string recipient,
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string? addressLine4,
        string city,
        string postCode)
    {
        return DoCreate(new Demo.Model.Domain.Checkout.CheckoutCompletion(
            basketId,
            recipient,
            addressLine1,
            addressLine2,
            addressLine3,
            addressLine4,
            city,
            postCode));
    }

    private T DoCreate<T>(T aggregateRoot) where T : IAggregateRoot
    {
        aggregateRoot.OnCreated();
        return aggregateRoot;
    }
}
