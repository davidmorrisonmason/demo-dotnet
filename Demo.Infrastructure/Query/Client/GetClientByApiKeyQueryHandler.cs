using Demo.ApplicationServices.Interface.Query.Client;
using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Encryption;
using Demo.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Query.Client;

public class GetClientByApiKeyQueryHandler : SingleQueryHandler<
    GetClientByApiKeyQuery,
    GetClientByApiKeyQueryValidator,
    Model.Domain.Client>
{
    private readonly IEncryptionService _encryptionService;
    private readonly IMemoryCache _cache;

    public GetClientByApiKeyQueryHandler(
        ApplicationDbContext dbContext,
        GetClientByApiKeyQueryValidator queryValidator,
        ILogger<GetClientByApiKeyQueryHandler> logger,
        IRequestContext requestContext,
        IEncryptionService encryptionService,
        IMemoryCache cache) : base(dbContext, queryValidator, logger, requestContext)
    {
        _encryptionService = encryptionService;
        _cache = cache;
    }

    protected async override Task<Model.Domain.Client?> DoQuery(GetClientByApiKeyQuery query)
    {
        // ArgonId hashing is expensive. We cache client IDs that have been verified using a SHA256 fingerprint of the plain text API key  
        var fingerprint = _encryptionService.Fingerprint(query.ApiKey);
        var cacheKey = $"{nameof(GetClientByApiKeyQueryHandler)}:{fingerprint}";
        if (_cache.TryGetValue(cacheKey, out int? clientId) && clientId is not null)
        {
            // get the client from the DB to ensure we get all properties loaded
            return await QueryNonDeleted<Model.Domain.Client>().Where(c => c.Id == clientId).FirstAsync();
        }

        var allClients = await QueryNonDeleted<Model.Domain.Client>().AsNoTracking().ToListAsync();
        var client = allClients.FirstOrDefault(c => _encryptionService.Verify(query.ApiKey, c.ApiKey));

        if (client is not null)
        {
            _cache.Set(cacheKey, client.Id, TimeSpan.FromSeconds(60));
        }

        return client;
    }

    protected override dynamic ToLogObject(GetClientByApiKeyQuery query) => new { ApiKey = "[REDACTED]" };

    private sealed record CachedClient(int Id, string Name, string ApiKey);
}

public class GetClientByApiKeyQueryValidator : AbstractValidator<GetClientByApiKeyQuery>
{
}
