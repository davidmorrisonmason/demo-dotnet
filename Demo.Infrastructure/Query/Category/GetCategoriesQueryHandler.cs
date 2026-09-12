using Demo.DomainServices.Interface.Context;
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
        ILogger<GetCategoriesQueryHandler> logger,
        IRequestContext requestContext) : base(dbContext, queryValidator, logger, requestContext)
    {
    }

    protected override async Task<IEnumerable<Model.Domain.Category>> DoQuery(GetCategoriesQuery query)
    {
        var categories = await BaseQuery
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.Products)
            .Include(c => c.SubCategories)
            .ToListAsync();

        return categories;
    }

    protected override dynamic ToLogObject(GetCategoriesQuery query)
    {
        return new
        {
        };
    }

    protected IQueryable<Model.Domain.Category> BaseQuery =>
        QueryNonDeleted<Model.Domain.Category>()
        .Where(c => c.ClientId == RequestContext.ClientId);
}

public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
    }
}

