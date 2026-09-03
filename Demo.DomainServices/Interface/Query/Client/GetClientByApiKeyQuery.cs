namespace Demo.DomainServices.Interface.Query.Client;

public record GetClientByApiKeyQuery(string ApiKey) : IQuery<Model.Domain.Client>;
