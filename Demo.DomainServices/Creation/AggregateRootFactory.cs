using Demo.Model.Domain;

namespace Demo.DomainServices.Creation;

public class AggregateRootFactory : IAggregateRootFactory
{
    public Category NewCategory(string name)
    {
        return DoCreate(new Category(name));
    }

    private T DoCreate<T>(T aggregateRoot) where T : IAggregateRoot
    {
        aggregateRoot.OnCreated();
        return aggregateRoot;
    }
}
