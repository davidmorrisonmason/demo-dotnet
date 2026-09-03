using Demo.DomainServices.Interface.Context;
using Demo.Model.Domain;
using Demo.Model.Domain.Exceptions;
using Demo.Model.Domain.Validation;

namespace Demo.DomainServices.Context;

public class RequestContext : IRequestContext
{

    private Client? _client;

    public int ClientId => _client?.Id ?? throw new AuthorisationException(
        new ErrorMessage("CLIENT_NOT_SET", "A client has not been set for the current request."));

    public void SetClient(Client? client)
    {
        _client = client;
    }
}
