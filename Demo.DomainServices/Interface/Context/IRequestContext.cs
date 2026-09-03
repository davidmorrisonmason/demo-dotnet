using Demo.Model.Domain;

namespace Demo.DomainServices.Interface.Context;

public interface IRequestContext
{
    int ClientId { get; }
    void SetClient(Client? client);
}
