using Demo.DomainServices.Command.Checkout;
using Demo.DomainServices.Creation;
using Demo.DomainServices.Interface.Command.Checkout;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Checkout;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Checkout;

[Collection(DatabaseTestCollection.Name)]
public class CheckoutCompleteCommandHandlerShould : CommandTest
{
    private readonly CheckoutCompleteCommandHandler _commandHandler;

    public CheckoutCompleteCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        var dbContext = new ApplicationDbContext(DbContextOptions);
        var basketRepository = new BasketRepository(dbContext, TestRequestContext, Substitute.For<Demo.DomainServices.Interface.Time.ITimeService>());
        var checkoutCompletionRepository = new CheckoutCompletionRepository(dbContext, TestRequestContext);

        _commandHandler = new CheckoutCompleteCommandHandler(
            Substitute.For<ILogger<CheckoutCompleteCommandHandler>>(),
            new CheckoutCompleteCommandValidator(basketRepository),
            checkoutCompletionRepository,
            new AggregateRootFactory(),
            basketRepository,
            new UnitOfWork(dbContext),
            TestRequestContext);
    }

    [Fact]
    public async Task CompleteCheckout_WhenExecuteCalled_WithValidCommand()
    {
        // Arrange
        var basket = BuilderFactory.NewBasketBuilder().BuildAndPersist();
        var command = new CheckoutCompleteCommand(
            basket.Id,
            "Recipient",
            "Address line 1",
            "Address line 2",
            "Address line 3",
            "Address line 4",
            "City",
            "Post code");

        var expectedBasket = BuilderFactory.NewBasketBuilder()
            .BuildFrom(basket)
            .With(x => x.Status, BasketStatus.Complete)
            .Build();

        var expected = BuilderFactory.NewCheckoutCompletionBuilder()
            .WithBasket(expectedBasket)
            .WithNextId()
            .With(x => x.Recipient, command.Recipient)
            .With(x => x.AddressLine1, command.AddressLine1)
            .With(x => x.AddressLine2, command.AddressLine2)
            .With(x => x.AddressLine3, command.AddressLine3)
            .With(x => x.AddressLine4, command.AddressLine4)
            .With(x => x.City, command.City)
            .With(x => x.PostCode, command.PostCode)
            .With(x => x.Basket, expectedBasket)
            .Build();

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        expected.ShouldBeInDatabase(dbContext => dbContext.CheckoutCompletions.Include(x => x.Basket));
    }

    [Fact]
    public async Task ThrowValidationException_WhenExecuteCalled_WithNonExistentBasket()
    {
        // Arrange
        var command = new CheckoutCompleteCommand(999, "Recipient", "Address line 1", null, null, null, "City", "Post code");

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

        // Assert
        exception.ErrorMessages.ShouldBeEquivalentTo(CheckoutCompleteCommandErrorType.Basket_Does_Not_Exist.BuildErrorMessages());
    }

    [Fact]
    public async Task ThrowValidationException_WhenExecuteCalled_WithMissingRequiredAddressDetails()
    {
        // Arrange
        var basket = BuilderFactory.NewBasketBuilder().BuildAndPersist();
        var command = new CheckoutCompleteCommand(basket.Id, "Recipient", "", null, null, null, "", "");

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

        // Assert
        exception.ErrorMessages.ShouldBeEquivalentTo([
            .. CheckoutCompleteCommandErrorType.Address_Line1_Required.BuildErrorMessages(),
            .. CheckoutCompleteCommandErrorType.City_Required.BuildErrorMessages(),
            .. CheckoutCompleteCommandErrorType.Post_Code_Required.BuildErrorMessages()
        ]);
    }
}
