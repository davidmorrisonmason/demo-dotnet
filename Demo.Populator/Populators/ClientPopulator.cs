using Demo.DomainServices.Interface.Command.Client;
using Demo.DomainServices.Interface.Encryption;
using Demo.DomainServices.Interface.Orchestration;
using Demo.Model.Domain;
using Demo.Populator.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demo.Populator.Populators;

public class ClientPopulator : Populator, IClientPopulator
{
    private readonly IEncryptionService _encryptionService;
    public Dictionary<string, Client> ClientsByPlainTextApiKey { get; } = [];

    public ClientPopulator(ILogger<ClientPopulator> logger, IMediator mediator, IEncryptionService encryptionService) : base(logger, mediator)
    {
        _encryptionService = encryptionService;
    }

    public override async Task Populate()
    {
        var client1 = await Mediator.Send(new ClientCreateCommand("Client 1", _encryptionService.OneWayHash("test-api-key")));
        var client2 = await Mediator.Send(new ClientCreateCommand("Client 2", _encryptionService.OneWayHash("test-api-key-2")));

        ClientsByPlainTextApiKey["test-api-key"] = client1;
        ClientsByPlainTextApiKey["test-api-key-2"] = client2;
    }
}
