using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Encryption;
using Demo.DomainServices.Interface.Query.Client;
using Demo.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo.Infrastructure.Query.Client;

public class GetClientByApiKeyQueryHandler : SingleQueryHandler<
    GetClientByApiKeyQuery,
    GetClientByApiKeyQueryValidator,
    Model.Domain.Client>
{
    private readonly IEncryptionService _encryptionService;

    public GetClientByApiKeyQueryHandler(
        ApplicationDbContext dbContext,
        GetClientByApiKeyQueryValidator queryValidator,
        ILogger<GetClientByApiKeyQueryHandler> logger,
        IRequestContext requestContext,
        IEncryptionService encryptionService) : base(dbContext, queryValidator, logger, requestContext)
    {
        _encryptionService = encryptionService;
    }

    protected async override Task<Model.Domain.Client?> DoQuery(GetClientByApiKeyQuery query)
    {
        var allClients = await QueryNonDeleted<Model.Domain.Client>().ToListAsync();
        return allClients.FirstOrDefault(c => _encryptionService.Verify(query.ApiKey, c.ApiKey));
    }

    protected override dynamic ToLogObject(GetClientByApiKeyQuery query) => new { query.ApiKey };
}

public class GetClientByApiKeyQueryValidator : AbstractValidator<GetClientByApiKeyQuery>
{
}
