using Demo.DomainServices.Interface.Encryption;
using Demo.DomainServices.Interface.Query.Client;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Query.Client;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Query.Client;

[Collection(DatabaseTestCollection.Name)]
public class GetClientByApiKeyQueryHandlerShould : QueryTest
{
    public GetClientByApiKeyQueryHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _encryptionService = Substitute.For<IEncryptionService>();
    }

    private IEncryptionService _encryptionService;

    private GetClientByApiKeyQueryHandler NewQueryHandler() => new(
        new ApplicationDbContext(DbContextOptions),
        new GetClientByApiKeyQueryValidator(),
        Substitute.For<ILogger<GetClientByApiKeyQueryHandler>>(),
        TestRequestContext,
        _encryptionService);

    [Fact]
    public async Task ReturnClient_WhenApiKeyExists()
    {
        // Arrange
        var other = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed")
            .BuildAndPersist();
        var client = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, "hashed2")
            .BuildAndPersist();

        _encryptionService.Verify("plainText", "hashed").Returns(false);
        _encryptionService.Verify("plainText", "hashed2").Returns(true);

        // Act
        using var queryHandler = NewQueryHandler();
        var actual = await queryHandler.Handle(new GetClientByApiKeyQuery("plainText"), CancellationToken.None);

        // Assert
        actual.ShouldBeEquivalentTo(client);
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
