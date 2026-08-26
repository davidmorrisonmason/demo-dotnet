using Demo.DomainServices.Interface.Orchestration;
using Demo.DomainServices.Interface.Query;
using Demo.Infrastructure.Data;
using Demo.Model.Domain;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Demo.Infrastructure.Query;

/// <summary>
/// A query that returns a list of entities. 
/// </summary>
/// <typeparam name="TQuery">The Mediator query payload type</typeparam>
/// <typeparam name="TEntity">The return type</typeparam>
public abstract class ListQueryHandler<TQuery, TQueryValidator, TEntity> : BaseQueryHandler<TQuery, TQueryValidator, IEnumerable<TEntity>>, IRequestHandler<TQuery, IEnumerable<TEntity>>
    where TQuery : IQuery<IEnumerable<TEntity>>
    where TQueryValidator : IValidator<TQuery>
    where TEntity : DomainObject
{

    public ListQueryHandler(
        ApplicationDbContext dbContext,
        TQueryValidator queryValidator,
        ILogger logger) : base(dbContext, queryValidator, logger)
    {
    }

    public async Task<IEnumerable<TEntity>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        Logger.LogDebug("Executing query: {0}", typeof(TQuery).Name);
        var logPayload = JObject.FromObject(ToLogObject(query));
        LogQueryPayload(logPayload);

        await QueryPrep(query);
        var results = await DoQuery(query);

        Logger.LogDebug("Query execution{0} completed successfully", typeof(TQuery).Name);

        return results;
    }
}
