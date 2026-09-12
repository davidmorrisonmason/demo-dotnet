using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Model.Domain.Checkout;

namespace Demo.Infrastructure.Repository;

public class CheckoutCompletionRepository : Repository<CheckoutCompletion>, ICheckoutCompletionRepository
{
    public CheckoutCompletionRepository(ApplicationDbContext dbContext, IRequestContext requestContext)
        : base(dbContext, requestContext)
    {
    }
}
