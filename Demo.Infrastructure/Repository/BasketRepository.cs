using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Time;
using Demo.Infrastructure.Data;
using Demo.Model.Domain.Checkout;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Repository;

public class BasketRepository : Repository<Basket>, IBasketRepository
{
    private readonly ITimeService _timeService;

    public BasketRepository(ApplicationDbContext dbContext, IRequestContext requestContext, ITimeService timeService) : base(dbContext, requestContext)
    {
        _timeService = timeService;
    }

    public override Task<Basket?> Get(int id)
    {
        var now = _timeService.UtcNow;

        return NonDeletedEntities
            .Include(b => b.BasketItems)
            .Where(b => b.Id == id && b.BasketExpirationTime >= now && b.Status == BasketStatus.Open)
            .FirstOrDefaultAsync();
    }
}
