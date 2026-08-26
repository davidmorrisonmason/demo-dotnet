using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Model.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ILogger<ICategoryRepository> _logger;

    public CategoryRepository(
        ApplicationDbContext dbContext,
        ILogger<ICategoryRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    public override Task<Category?> Get(int id)
    {
        var query = NonDeletedEntities
            .Include(c => c.Products)
            .Where(c => c.Id == id);

        return query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Category>> GetAllByName(string name)
    {
        name ??= "";

        return await NonDeletedEntities
            .Where(x => x.Name.ToLower() == name.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetAllByNameExcludingId(string name, int idToExclude)
    {
        name ??= "";

        return await NonDeletedEntities
            .Where(x => x.Name.ToLower() == name.ToLower() && x.Id != idToExclude)
            .ToListAsync();

    }
}
