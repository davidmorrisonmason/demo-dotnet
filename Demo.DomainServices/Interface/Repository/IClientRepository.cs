using Demo.Model.Domain;

namespace Demo.DomainServices.Interface.Repository;

public interface IClientRepository : IRepository<Client>
{
    Task<IEnumerable<Client>> GetAllByName(string name);
    Task<IEnumerable<Client>> GetAllByApiKey(string apiKey);
}
