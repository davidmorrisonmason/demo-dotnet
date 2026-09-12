using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Query.Category;
using Demo.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Query.Category;

public class GetCategoryQueryHandler : SingleQueryHandler<GetCategoryQuery, GetCategoryQueryValidator, Model.Domain.Category>
{
    public GetCategoryQueryHandler(
        ApplicationDbContext dbContext,
        GetCategoryQueryValidator queryValidator,
        ILogger<GetCategoryQueryHandler> logger,
        IRequestContext requestContext) : base(dbContext, queryValidator, logger, requestContext)
    {
    }

    protected override async Task<Model.Domain.Category?> DoQuery(GetCategoryQuery query)
    {
        var category = await BaseQuery
            .Include(c => c.Products)
            .Include(c => c.SubCategories)
            .Where(x => x.Id == query.Id)
            .FirstOrDefaultAsync();

        return category;
    }

    protected IQueryable<Model.Domain.Category> BaseQuery =>
        QueryNonDeleted<Model.Domain.Category>()
        .Where(c => c.ClientId == RequestContext.ClientId);

    protected override dynamic ToLogObject(GetCategoryQuery query)
    {
        return new
        {
            query.Id
        };
    }
}

public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
{
    public GetCategoryQueryValidator()
    {
    }
}
