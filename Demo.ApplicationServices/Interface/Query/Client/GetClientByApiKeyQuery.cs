namespace Demo.ApplicationServices.Interface.Query.Client;

public record GetClientByApiKeyQuery(string ApiKey) : IQuery<Model.Domain.Client?>;
