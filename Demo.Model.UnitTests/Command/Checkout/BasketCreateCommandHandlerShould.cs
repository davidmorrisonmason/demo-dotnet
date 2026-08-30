using Demo.DomainServices.Command.Checkout;
using Demo.DomainServices.Interface.Command.Checkout;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Validation;
using Demo.Model.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Checkout;

[Collection(DatabaseTestCollection.Name)]
public class BasketCreateCommandHandlerShould : CommandTest
{
    private readonly BasketCreateCommandHandler _commandHandler;

    public BasketCreateCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        var dbContext = new ApplicationDbContext(DbContextOptions);
        var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>());
        var basketRepository = new BasketRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        _commandHandler = new BasketCreateCommandHandler(
            Substitute.For<ILogger<BasketCreateCommandHandler>>(),
            new BasketCreateCommandValidator(),
            categoryRepository,
            basketRepository,
            unitOfWork);
    }

    [Fact]
    public async Task CreateBasket_WhenExecuteCalled_WithValidCommand()
    {
        // Arrange
        var category = BuilderFactory.NewCategoryBuilder()
            .WithProducts(
            [
                BuilderFactory.NewProductBuilder(1).Build(),
                BuilderFactory.NewProductBuilder(2).Build()
            ])
            .BuildAndPersist();

        var command = new BasketCreateCommand(
        [
            new BasketItemCommand(category.Id, category.Products[0].Id, 2),
            new BasketItemCommand(category.Id, category.Products[1].Id, 1)
        ]);

        // Act
        var actual = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        var expectedBuilder = BuilderFactory.NewBasketBuilder(databaseSeed: actual.Id);
        expectedBuilder.WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder(actual.Id, category.Products[0].Id, 2, actual.BasketItems[0].Id).Build(),
                BuilderFactory.NewBasketItemBuilder(actual.Id, category.Products[1].Id, 1, actual.BasketItems[1].Id).Build()
            ]);

        var expected = expectedBuilder.Build();

        expected.ShouldBeInDatabase(dbContext => dbContext.Baskets.Include(x => x.BasketItems));
    }

    [Fact]
    public async Task ThrowValidationException_WhenExecuteCalled_WithNonPositiveQuantity()
    {
        // Arrange
        var command = new BasketCreateCommand(
        [
            new BasketItemCommand(1, 1, 0)
        ]);

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _commandHandler.Handle(command, CancellationToken.None));

        // Assert
        exception.ErrorMessages.ShouldBeEquivalentTo(
            BasketCommandErrorType.Basket_Item_Quantity_Must_Be_Greater_Than_Zero.BuildErrorMessages());
    }

    [Fact]
    public async Task ThrowEntityNotFoundException_WhenExecuteCalled_WithNonExistentCategory()
    {
        // Arrange
        var command = new BasketCreateCommand(
        [
            new BasketItemCommand(999, 1, 1)
        ]);

        // Act / Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ThrowEntityNotFoundException_WhenExecuteCalled_WithNonExistentProductInCategory()
    {
        // Arrange
        var category = BuilderFactory.NewCategoryBuilder()
            .WithProducts([BuilderFactory.NewProductBuilder(1).Build()])
            .BuildAndPersist();

        var command = new BasketCreateCommand(
        [
            new BasketItemCommand(category.Id, category.Products[0].Id + 1, 1)
        ]);

        // Act / Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _commandHandler.Handle(command, CancellationToken.None));
    }
}
