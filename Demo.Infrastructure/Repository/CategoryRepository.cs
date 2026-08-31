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

    public async override Task<Category?> Get(int id)
    {
        return await BaseQuery
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Category>> GetAllByName(string name)
    {
        name ??= "";

        return await BaseQuery
            .Where(x => x.Name.ToLower() == name.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetAllByNameExcludingId(string name, int idToExclude)
    {
        name ??= "";

        return await BaseQuery
            .Where(x => x.Name.ToLower() == name.ToLower() && x.Id != idToExclude)
            .ToListAsync();

    }

    private IQueryable<Category> BaseQuery =>
        NonDeletedEntities
            .Include(c => c.Products)
            .Include(c => c.SubCategories)
                .ThenInclude(c => c.Products);
}
