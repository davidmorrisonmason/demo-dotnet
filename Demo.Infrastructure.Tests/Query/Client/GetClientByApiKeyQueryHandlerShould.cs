using Demo.ApplicationServices.Interface.Query.Client;
using Demo.DomainServices.Interface.Encryption;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Query.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Infrastructure.Tests.Query.Client;

[Collection(InfrastructureDatabaseTestCollection.Name)]
public class GetClientByApiKeyQueryHandlerShould : QueryTest
{
    private readonly IEncryptionService _encryptionService;
    private readonly IMemoryCache _cache;

    public GetClientByApiKeyQueryHandlerShould(InfrastructureTestDatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _encryptionService = Substitute.For<IEncryptionService>();
        _cache = Substitute.For<IMemoryCache>();
    }

    private GetClientByApiKeyQueryHandler NewQueryHandler() => new(
        new ApplicationDbContext(DbContextOptions),
        new GetClientByApiKeyQueryValidator(),
        Substitute.For<ILogger<GetClientByApiKeyQueryHandler>>(),
        TestRequestContext,
        _encryptionService,
        _cache);

    [Fact]
    public async Task ReturnClient_WhenClientDoesNotExistInCache()
    {
        // Arrange
        var other = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed")
            .BuildAndPersist();
        var client = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed2")
            .BuildAndPersist();

        _encryptionService.Fingerprint("plainText").Returns("fingerprint");
        _cache.TryGetValue("fingerprint", out int? _).Returns(false);
        _encryptionService.Verify("plainText", "hashed").Returns(false);
        _encryptionService.Verify("plainText", "hashed2").Returns(true);

        // Act
        using var queryHandler = NewQueryHandler();
        var actual = await queryHandler.Handle(new GetClientByApiKeyQuery("plainText"), CancellationToken.None);

        // Assert
        actual.ShouldBeEquivalentTo(client);
    }

    [Fact]
    public async Task ReturnClientWithoutVerification_WhenClientDoesExistInCache()
    {
        // Arrange
        var other = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed")
            .BuildAndPersist();
        var client = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed2")
            .BuildAndPersist();

        _encryptionService.Fingerprint("plainText").Returns("fingerprint");
        _cache.TryGetValue($"{nameof(GetClientByApiKeyQueryHandler)}:fingerprint", out int? clientId).Returns(x =>
        {
            x[1] = client.Id;
            return true;
        });

        // Act
        using var queryHandler = NewQueryHandler();
        var actual = await queryHandler.Handle(new GetClientByApiKeyQuery("plainText"), CancellationToken.None);

        // Assert
        actual.ShouldBeEquivalentTo(client);
        _encryptionService.DidNotReceive().Verify("plainText", "hashed");
        _encryptionService.DidNotReceive().Verify("plainText", "hashed2");
    }


    [Fact]
    public async Task ReturnNull_WhenApiKeyDoesNotExist()
    {
        // Arrange
        using var queryHandler = NewQueryHandler();
        var client1 = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed1")
            .BuildAndPersist();
        var client2 = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed2")
            .BuildAndPersist();

        _encryptionService.Verify("plainText", "hashed1").Returns(false);
        _encryptionService.Verify("plainText", "hashed2").Returns(false);

        // Act
        var actual = await queryHandler.Handle(new GetClientByApiKeyQuery("plainText"), CancellationToken.None);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenClientIsDeleted()
    {
        // Arrange
        var other = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed1")
            .BuildAndPersist();
        var client = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed2")
            .WithDeletedStatus()
            .BuildAndPersist();

        _encryptionService.Verify("plainText", "hashed1").Returns(false);

        // Act
        using var queryHandler = NewQueryHandler();
        var actual = await queryHandler.Handle(new GetClientByApiKeyQuery("hashed"), CancellationToken.None);

        // Assert
        actual.ShouldBeNull();
    }
}
