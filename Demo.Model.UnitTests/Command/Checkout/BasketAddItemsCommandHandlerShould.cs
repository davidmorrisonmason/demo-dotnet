using Demo.DomainServices.Command.Checkout;
using Demo.DomainServices.Interface.Command.Checkout;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Builders.Domain;
using Demo.Model.UnitTests.Validation;
using Demo.Model.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Checkout;

[Collection(DatabaseTestCollection.Name)]
public class BasketAddItemsCommandHandlerShould : CommandTest
{
    private readonly BasketAddItemsCommandHandler _commandHandler;

    public BasketAddItemsCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        var dbContext = new ApplicationDbContext(DbContextOptions);
        var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>());
        var basketRepository = new BasketRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        _commandHandler = new BasketAddItemsCommandHandler(
            Substitute.For<ILogger<BasketAddItemsCommandHandler>>(),
            new BasketAddItemsCommandValidator(),
            categoryRepository,
            basketRepository,
            unitOfWork);
    }

    [Fact]
    public async Task AddBasketItem_WhenExecuteCalled_WithValidCommand()
    {
        // Arrange
        var category = BuilderFactory.NewCategoryBuilder()
            .WithProducts(
            [
                BuilderFactory.NewProductBuilder(1).Build(),
                BuilderFactory.NewProductBuilder(2).Build()
            ])
            .BuildAndPersist();

        var basket = BuilderFactory.NewBasketBuilder()
            .WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder(0, category.Products[0].Id, 1, 1).Build()
            ])
            .BuildAndPersist();

        var expected = ((BasketBuilder)BuilderFactory.NewBasketBuilder().BuildFrom(basket))
            .WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder(basket.Id, category.Products[0].Id, 1, 1)
                    .With(x => x.Id, basket.BasketItems[0].Id)
                    .Build(),
                BuilderFactory.NewBasketItemBuilder(basket.Id, category.Products[1].Id, 2, 2)
                    .WithNextId()
                    .Build()
            ])
            .Build();

        var command = new BasketAddItemsCommand(
            basket.Id,
            [new BasketItemCommand(category.Id, category.Products[1].Id, 2)]);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        expected.ShouldBeInDatabase(dbContext => dbContext.Baskets.Include(x => x.BasketItems));
    }

    [Fact]
    public async Task IncreaseQuantity_WhenExecuteCalled_WithExistingProduct()
    {
        // Arrange
        var category = BuilderFactory.NewCategoryBuilder()
            .WithProducts([BuilderFactory.NewProductBuilder(1).Build()])
            .BuildAndPersist();

        var basket = BuilderFactory.NewBasketBuilder()
            .WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder(0, category.Products[0].Id, 1).Build()
            ])
            .BuildAndPersist();

        var expected = ((BasketBuilder)BuilderFactory.NewBasketBuilder().BuildFrom(basket))
            .WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder(basket.Id, category.Products[0].Id, 3)
                    .With(x => x.Id, basket.BasketItems[0].Id)
                    .Build()
             ])
            .Build();

        var command = new BasketAddItemsCommand(
            basket.Id,
            [new BasketItemCommand(category.Id, category.Products[0].Id, 2)]);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        expected.ShouldBeInDatabase(dbContext => dbContext.Baskets.Include(x => x.BasketItems));
    }

    [Fact]
    public async Task ThrowValidationException_WhenExecuteCalled_WithNonPositiveQuantity()
    {
        // Arrange
        var command = new BasketAddItemsCommand(
            1,
            [new BasketItemCommand(1, 1, 0)]);

        // Act
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _commandHandler.Handle(command, CancellationToken.None));

        // Assert
        exception.ErrorMessages.ShouldBeEquivalentTo(
            BasketCommandErrorType.Basket_Item_Quantity_Must_Be_Greater_Than_Zero.BuildErrorMessages());
    }

    [Fact]
    public async Task ThrowEntityNotFoundException_WhenExecuteCalled_WithNonExistentBasket()
    {
        // Arrange
        var command = new BasketAddItemsCommand(
            999,
            [new BasketItemCommand(1, 1, 1)]);

        // Act / Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task ThrowEntityNotFoundException_WhenExecuteCalled_WithNonExistentCategory()
    {
        // Arrange
        var basket = BuilderFactory.NewBasketBuilder().BuildAndPersist();
        var command = new BasketAddItemsCommand(
            basket.Id,
            [new BasketItemCommand(999, 1, 1)]);

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
        var basket = BuilderFactory.NewBasketBuilder().BuildAndPersist();
        var command = new BasketAddItemsCommand(
            basket.Id,
            [new BasketItemCommand(category.Id, category.Products[0].Id + 1, 1)]);

        // Act / Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _commandHandler.Handle(command, CancellationToken.None));
    }
}
