using Demo.Model.Domain;

namespace Demo.DomainServices.Creation;

public interface IAggregateRootFactory
{
    Category NewCategory(string name);
}

