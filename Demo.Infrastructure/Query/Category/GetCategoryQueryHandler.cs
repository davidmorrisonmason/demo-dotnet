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
        ILogger<GetCategoryQueryHandler> logger) : base(dbContext, queryValidator, logger)
    {
    }

    protected override async Task<Model.Domain.Category?> DoQuery(GetCategoryQuery query)
    {
        var category = await QueryNonDeleted<Model.Domain.Category>()
            .Where(x => x.Id == query.Id)
            .FirstOrDefaultAsync();

        return category;
    }

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
