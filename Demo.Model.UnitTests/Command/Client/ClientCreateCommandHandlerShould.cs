using Demo.DomainServices.Command.Client;
using Demo.DomainServices.Creation;
using Demo.DomainServices.Interface.Command.Client;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Validation;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Client;

[Collection(DatabaseTestCollection.Name)]
public class ClientCreateCommandHandlerShould : CommandTest
{
    private readonly ClientCreateCommandHandler _commandHandler;
    protected override bool AddRequestContextTestClient => false;

    public ClientCreateCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        var dbContext = new ApplicationDbContext(DbContextOptions);
        var repository = new ClientRepository(dbContext, TestRequestContext);
        _commandHandler = new ClientCreateCommandHandler(
            Substitute.For<ILogger<ClientCreateCommandHandler>>(),
            new ClientCreateCommandValidator(repository),
            repository,
            new AggregateRootFactory(),
            new UnitOfWork(dbContext),
            TestRequestContext);
    }

    [Fact]
    public async Task CreateClient_WhenExecuteCalled_WithValidCommand()
    {
        // Arrange
        var client = BuilderFactory.NewClientBuilder().Build();

        var expected = BuilderFactory
            .NewClientBuilder()
            .BuildFrom(client)
            .WithNextId()
            .Build();

        // Act
        await _commandHandler.Handle(new ClientCreateCommand(client.Name, client.ApiKey), CancellationToken.None);

        // Assert
        expected.ShouldBeInDatabase();
    }

    [Theory]
    [InlineData("", "api-key")]
    [InlineData("   ", "api-key")]
    [InlineData(null, "api-key")]
    public async Task ThrowValidationException_WhenNameIsInvalid(string? name, string apiKey)
    {
        // Act
#pragma warning disable CS8604
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _commandHandler.Handle(new ClientCreateCommand(name, apiKey), CancellationToken.None));
#pragma warning restore CS8604

        // Assert
        exception.ErrorMessages.ShouldBeEquivalentTo(ClientCommandErrorType.Client_Name_Required.BuildErrorMessages());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ThrowValidationException_WhenApiKeyIsInvalid(string? apiKey)
    {
        // Act
#pragma warning disable CS8604
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _commandHandler.Handle(new ClientCreateCommand("Client", apiKey), CancellationToken.None));
#pragma warning restore CS8604

        // Assert
        exception.ErrorMessages.ShouldBeEquivalentTo(ClientCommandErrorType.Client_ApiKey_Required.BuildErrorMessages());
    }

    [Fact]
    public async Task ThrowValidationException_WhenNameOrApiKeyIsNotUnique()
    {
        // Arrange
        var existing = BuilderFactory.NewClientBuilder().BuildAndPersist();

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _commandHandler.Handle(new ClientCreateCommand(existing.Name, existing.ApiKey), CancellationToken.None));

        // Assert
        exception.ErrorMessages.ShouldBeEquivalentTo([
            .. ClientCommandErrorType.Client_Name_Must_Be_Unique.BuildErrorMessages(),
            .. ClientCommandErrorType.Client_ApiKey_Must_Be_Unique.BuildErrorMessages()
        ]);
    }
}
