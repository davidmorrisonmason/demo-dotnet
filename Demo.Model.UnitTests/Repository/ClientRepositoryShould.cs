using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain;
using Demo.Model.UnitTests.Builders.Domain;

namespace Demo.Model.UnitTests.Repository;

[Collection(DatabaseTestCollection.Name)]
public class ClientRepositoryShould : DatabaseTest
{
    private IClientRepository _clientRepository;
    protected override bool AddRequestContextTestClient => false;

    public ClientRepositoryShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _clientRepository = new ClientRepository(
        new ApplicationDbContext(DbContextOptions),
        TestRequestContext);
    }

    [Fact]
    public async Task ReturnClient_WhenGetCalled_AndClientExists()
    {
        // Arrange
        var client = BuilderFactory.NewClientBuilder(1, 2)
            .BuildAndPersist();

        var expected = BuilderFactory.NewClientBuilder().BuildFrom(client).Build();

        // Act
        var actual = await _clientRepository.Get(client.Id);

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task ReturnNull_WhenGetCalled_AndClientDoesNotExist()
    {
        // Arrange

        // Act
        var actual = await _clientRepository.Get(999);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenGetCalled_AndClientIsDeleted()
    {
        // Arrange
        var client = BuilderFactory.NewClientBuilder()
            .With(x => x.IsDeleted, true)
            .BuildAndPersist();

        // Act
        var actual = await _clientRepository.Get(client.Id);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnClientsWithMatchingName_WhenGetAllByNameCalled()
    {
        // Arrange
        var matchingClient = BuilderFactory.NewClientBuilder(1)
            .BuildAndPersist();
        var otherClient = BuilderFactory.NewClientBuilder(2)
            .BuildAndPersist();
        var deletedMatchingClient = BuilderFactory.NewClientBuilder(3)
            .With(x => x.Name, matchingClient.Name)
            .WithDeletedStatus()
            .BuildAndPersist();

        var expected = new List<Client>()
        {
            BuilderFactory.NewClientBuilder().BuildFrom(matchingClient).Build()
        };

        // Act
        var actual = await _clientRepository.GetAllByName(matchingClient.Name);

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task ReturnClientsWithMatchingApiKey_WhenGetAllByApiKeyCalled()
    {
        // Arrange
        var matchingClient = BuilderFactory.NewClientBuilder(1)
            .BuildAndPersist();
        var otherClient = BuilderFactory.NewClientBuilder(2)
            .BuildAndPersist();
        var deletedMatchingClient = BuilderFactory.NewClientBuilder(3)
            .With(x => x.ApiKey, matchingClient.ApiKey)
            .With(x => x.IsDeleted, true)
            .BuildAndPersist();

        var expected = new List<Client>()
        {
            ((ClientBuilder)BuilderFactory.NewClientBuilder().BuildFrom(matchingClient)).Build()
        };

        // Act
        var actual = await _clientRepository.GetAllByApiKey(matchingClient.ApiKey);

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }
}
