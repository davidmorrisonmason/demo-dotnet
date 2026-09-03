using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Model.Domain;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Repository;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(ApplicationDbContext dbContext, IRequestContext requestContext) : base(dbContext, requestContext)
    {
    }

    public async Task<IEnumerable<Client>> GetAllByName(string name)
    {
        var query = BaseQuery.Where(x => x.Name == name);
        return await query.ToListAsync();

    }
    public async Task<IEnumerable<Client>> GetAllByApiKey(string apiKey)
    {
        var query = BaseQuery.Where(x => x.ApiKey == apiKey);
        return await query.ToListAsync();
    }

    private IQueryable<Client> BaseQuery =>
        NonDeletedEntities;
}
