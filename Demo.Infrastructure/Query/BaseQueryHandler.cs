using Demo.Infrastructure.Data;
using Demo.Model.Domain;
using Demo.Model.Domain.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

using Demo.DomainServices.Context;
using Demo.DomainServices.Interface.Context;

namespace Demo.Infrastructure.Query;

public abstract class BaseQueryHandler<TQuery, TQueryValidator, TResult> : IDisposable
    where TQueryValidator : IValidator<TQuery>
{
    private readonly ILogger _logger;
    private readonly TQueryValidator _queryValidator;
    private ApplicationDbContext _dbContext;

    protected ApplicationDbContext DbContext => _dbContext;
    protected ILogger Logger => _logger;
    protected IRequestContext RequestContext { get; }

    protected BaseQueryHandler(
        ApplicationDbContext dbContext,
        TQueryValidator queryValidator,
        ILogger logger,
        IRequestContext requestContext)
    {
        _dbContext = dbContext;
        _queryValidator = queryValidator;
        _logger = logger;
        RequestContext = requestContext;
    }

    protected IQueryable<T> QueryNonDeleted<T>() where T : DomainObject
    {
        return _dbContext.Set<T>()
            .Where(x => x.IsDeleted == false);
    }

    /// <summary>
    /// Hook for specific authorisation in a query subclass
    /// </summary>
    protected virtual Task DoSpecificQueryAuthorisation(TQuery query)
    {
        return Task.CompletedTask;
    }

    protected void LogQueryPayload(JObject queryPayload)
    {
        if (queryPayload == null)
        {
            _logger.LogDebug("No query payload supplied for logging");
        }
        else
        {
            _logger.LogDebug(queryPayload.ToString());
        }
    }

    protected async Task QueryPrep(TQuery query)
    {
        await Validate(query);
        await Authorise(query);
    }

    protected async Task Validate(TQuery query)
    {
        var validationResults = await _queryValidator.ValidateAsync(query);

        if (validationResults.Errors.Any())
        {
            var errors = validationResults.Errors
                .Select(x => new ErrorMessage(x.ErrorCode, x.ErrorMessage))
                .ToList();

            throw new Model.Domain.Validation.ValidationException(errors);
        }
    }

    protected async Task Authorise(TQuery query)
    {
        await DoSpecificQueryAuthorisation(query);
    }

    protected abstract Task<TResult> DoQuery(TQuery query);
    protected abstract dynamic ToLogObject(TQuery query);

    public void Dispose()
    {
        if (_dbContext != null)
        {
            _dbContext.Dispose();
        }
    }
}
