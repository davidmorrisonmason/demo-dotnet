using Demo.Model.Domain;

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

    private T DoCreate<T>(T aggregateRoot) where T : IAggregateRoot
    {
        aggregateRoot.OnCreated();
        return aggregateRoot;
    }
}
