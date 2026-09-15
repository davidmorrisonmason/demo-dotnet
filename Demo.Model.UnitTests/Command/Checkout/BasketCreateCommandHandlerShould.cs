using Demo.DomainServices.Command.Checkout;
using Demo.DomainServices.Configuration;
using Demo.DomainServices.Interface.Command.Checkout;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Time;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Builders.Domain;
using Demo.Model.UnitTests.Validation;
using Demo.Model.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Checkout;

[Collection(ModelTestsDatabaseTestCollection.Name)]
public class BasketCreateCommandHandlerShould : CommandTest
{
    private readonly BasketCreateCommandHandler _commandHandler;
    private readonly DateTime _now = new(2026, 8, 31, 10, 0, 0, DateTimeKind.Utc);

    public BasketCreateCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        var dbContext = new ApplicationDbContext(DbContextOptions);
        var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>(), TestRequestContext);
        var basketRepository = new BasketRepository(dbContext, TestRequestContext, Substitute.For<ITimeService>());
        var unitOfWork = new UnitOfWork(dbContext);
        var timeService = Substitute.For<ITimeService>();
        timeService.UtcNow.Returns(_now);

        _commandHandler = new BasketCreateCommandHandler(
            Substitute.For<ILogger<BasketCreateCommandHandler>>(),
            new BasketCreateCommandValidator(),
            categoryRepository,
            basketRepository,
            unitOfWork,
            timeService,
            Options.Create(new BasketSettings { BasketExpirationMinutes = 60 }),
            TestRequestContext);
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

        var expected = ((BasketBuilder)BuilderFactory.NewBasketBuilder()
            .WithNextId())
            .WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder()
                    .WithNextId()
                    .With(x => x.ProductId, category.Products[0].Id)
                    .With(x => x.Quantity, 2)
                    .Build(),
                BuilderFactory.NewBasketItemBuilder()
                    .WithNextId(2)
                    .With(x => x.ProductId, category.Products[1].Id)
                    .With(x => x.Quantity, 1)
                    .Build(),
            ])
            .With(x => x.BasketExpirationTime, _now.AddMinutes(60))
            .Build();


        // Act
        var actual = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert

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
