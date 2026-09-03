using Demo.Model.Domain;

namespace Demo.Populator.Interfaces;

public interface IClientPopulator : IPopulator
{
    Dictionary<string, Client> ClientsByPlainTextApiKey { get; }
}
