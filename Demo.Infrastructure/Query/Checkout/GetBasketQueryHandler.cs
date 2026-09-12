using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Query.Checkout;
using Demo.DomainServices.Interface.Time;
using Demo.Infrastructure.Data;
using Demo.Model.Domain.Checkout;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Query.Checkout;

public class GetBasketQueryHandler : SingleQueryHandler<
    GetBasketQuery,
    GetBasketQueryValidator,
    Model.Domain.Checkout.Basket>
{
    private readonly ITimeService _timeService;

    public GetBasketQueryHandler(
        ApplicationDbContext dbContext,
        GetBasketQueryValidator queryValidator,
        ILogger<GetBasketQueryHandler> logger,
        IRequestContext requestContext,
        ITimeService timeService) : base(dbContext, queryValidator, logger, requestContext)
    {
        _timeService = timeService;
    }

    protected override async Task<Model.Domain.Checkout.Basket?> DoQuery(GetBasketQuery query)
    {
        var now = _timeService.UtcNow;

        return await QueryNonDeleted<Model.Domain.Checkout.Basket>()
            .Include(basket => basket.BasketItems)
                .ThenInclude(item => item.Product)
            .Where(basket => basket.Id == query.Id && basket.BasketExpirationTime >= now && basket.Status == BasketStatus.Open)
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
