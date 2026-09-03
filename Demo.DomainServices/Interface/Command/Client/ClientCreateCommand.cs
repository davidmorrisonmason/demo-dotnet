namespace Demo.DomainServices.Interface.Command.Client;

public record ClientCreateCommand(string Name, string ApiKey) : Command<Model.Domain.Client>()
{
}
