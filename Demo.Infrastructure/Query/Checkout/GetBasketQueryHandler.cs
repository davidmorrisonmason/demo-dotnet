using Demo.DomainServices.Interface.Query.Checkout;
using Demo.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Query.Checkout;

public class GetBasketQueryHandler : SingleQueryHandler<
    GetBasketQuery,
    GetBasketQueryValidator,
    Model.Domain.Checkout.Basket>
{
    public GetBasketQueryHandler(
        ApplicationDbContext dbContext,
        GetBasketQueryValidator queryValidator,
        ILogger<GetBasketQueryHandler> logger) : base(dbContext, queryValidator, logger)
    {
    }

    protected override async Task<Model.Domain.Checkout.Basket?> DoQuery(GetBasketQuery query)
    {
        return await QueryNonDeleted<Model.Domain.Checkout.Basket>()
            .Include(basket => basket.BasketItems)
                .ThenInclude(item => item.Product)
            .Where(basket => basket.Id == query.Id)
            .FirstOrDefaultAsync();
    }

    protected override dynamic ToLogObject(GetBasketQuery query)
    {
        return new
        {
            query.Id
        };
    }
}

public class GetBasketQueryValidator : AbstractValidator<GetBasketQuery>
{
    public GetBasketQueryValidator()
    {
    }
}
