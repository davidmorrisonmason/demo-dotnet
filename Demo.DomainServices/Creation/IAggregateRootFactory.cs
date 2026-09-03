using Demo.Model.Domain;

namespace Demo.DomainServices.Creation;

public interface IAggregateRootFactory
{
    Category NewCategory(string name, int clientId);
    Client NewClient(string name, string apiKey);
}

