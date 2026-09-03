using Demo.DomainServices.Interface.Orchestration;
using Demo.DomainServices.Interface.Query;
using Demo.Infrastructure.Data;
using Demo.Model.Domain;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

using Demo.DomainServices.Context;
using Demo.DomainServices.Interface.Context;

namespace Demo.Infrastructure.Query;

/// <summary>
/// A query that returns a single entity.
/// </summary>
/// <typeparam name="TQuery">The Mediator query payload type</typeparam>
/// <typeparam name="TEntity">The return type</typeparam>
public abstract class SingleQueryHandler<TQuery, TQueryValidator, TEntity> : BaseQueryHandler<TQuery, TQueryValidator, TEntity?>, IRequestHandler<TQuery, TEntity?>
    where TQuery : IQuery<TEntity>
    where TQueryValidator : IValidator<TQuery>
    where TEntity : DomainObject
{

    public SingleQueryHandler(
        ApplicationDbContext dbContext,
        TQueryValidator queryValidator,
        ILogger logger,
        IRequestContext requestContext) : base(dbContext, queryValidator, logger, requestContext)
    {
    }

    public async Task<TEntity?> Handle(TQuery query, CancellationToken cancellationToken)
    {
        Logger.LogDebug("Executing query: {0}", typeof(TQuery).Name);
        var logPayload = JObject.FromObject(ToLogObject(query));
        LogQueryPayload(logPayload);

        await QueryPrep(query);
        var result = await DoQuery(query);

        Logger.LogDebug("Query execution{0} completed successfully", typeof(TQuery).Name);

        return result;
    }
}
