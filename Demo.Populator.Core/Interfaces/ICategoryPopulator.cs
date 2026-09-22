using Demo.Model.Domain;

namespace Demo.Populator.Core.Interfaces;

public interface ICategoryPopulator : IPopulator
{
    void SetClients(Dictionary<string, Client> clientsByPlainTextApiKey);
}
