using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Model.Domain.Checkout;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Repository;

public class BasketRepository : Repository<Basket>, IBasketRepository
{
    public BasketRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override Task<Basket?> Get(int id)
    {
        return NonDeletedEntities
            .Include(b => b.BasketItems)
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();
    }
}
