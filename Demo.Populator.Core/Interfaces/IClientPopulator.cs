using Demo.Model.Domain;

namespace Demo.Populator.Core.Interfaces;

public interface IClientPopulator : IPopulator
{
    Dictionary<string, Client> ClientsByPlainTextApiKey { get; }
}
