using Demo.DomainServices.Interface.Query.Category;
using Demo.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Query.Category;

public class GetCategoriesQueryHandler : ListQueryHandler<GetCategoriesQuery, GetCategoriesQueryValidator, Model.Domain.Category>
{
    public GetCategoriesQueryHandler(
        ApplicationDbContext dbContext,
        GetCategoriesQueryValidator queryValidator,
        ILogger<GetCategoriesQueryHandler> logger) : base(dbContext, queryValidator, logger)
    {
    }

    protected override async Task<IEnumerable<Model.Domain.Category>> DoQuery(GetCategoriesQuery query)
    {
        var categories = await QueryNonDeleted<Model.Domain.Category>()
            .ToListAsync();

        return categories;
    }

    protected override dynamic ToLogObject(GetCategoriesQuery query)
    {
        return new
        {
        };
    }
}

public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
    }
}

