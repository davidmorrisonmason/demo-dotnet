using Demo.DomainServices.Interface.Query.Checkout;
using Demo.DomainServices.Interface.Time;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Query.Checkout;
using Demo.Model.Domain.Checkout;
using Demo.Model.UnitTests.Builders.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Query.Checkout;

[Collection(ModelTestsDatabaseTestCollection.Name)]
public class GetBasketQueryHandlerShould : QueryTest
{
    private readonly ITimeService _timeService;

    public GetBasketQueryHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _timeService = Substitute.For<ITimeService>();
    }

    private GetBasketQueryHandler NewQueryHandler() => new(
        new ApplicationDbContext(DbContextOptions),
        new GetBasketQueryValidator(),
        Substitute.For<ILogger<GetBasketQueryHandler>>(),
        TestRequestContext,
        _timeService);

    [Fact]
    public async Task ReturnBasketWithItemsAndProducts_WhenBasketIsOpenAndNotExpired()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _timeService.UtcNow.Returns(now);

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
                BuilderFactory.NewBasketItemBuilder(0)
                    .With(x => x.ProductId, category.Products[0].Id)
                    .With(x => x.Quantity, 2)
                    .Build(),
                BuilderFactory.NewBasketItemBuilder(0)
                    .With(x => x.ProductId, category.Products[1].Id)
                    .With(x => x.Quantity, 3)
                    .Build(),
            ])
            .With(x => x.BasketExpirationTime, now.AddMinutes(5))
            .BuildAndPersist();

        var expected = ((BasketBuilder)(BuilderFactory.NewBasketBuilder().BuildFrom(basket)))
            .WithBasketItems(
            [
                ((BasketItemBuilder)BuilderFactory.NewBasketItemBuilder().BuildFrom(basket.BasketItems[0]))
                    .WithProduct(category.Products[0])
                    .Build(),
                ((BasketItemBuilder)BuilderFactory.NewBasketItemBuilder().BuildFrom(basket.BasketItems[1]))
                    .WithProduct(category.Products[1])
                    .Build(),
            ])
            .Build();

        // Act
        using var queryHandler = NewQueryHandler();
        var actual = await queryHandler.Handle(new GetBasketQuery(basket.Id), CancellationToken.None);

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task ReturnNull_WhenBasketDoesNotExist()
    {
        // Arrange
        _timeService.UtcNow.Returns(DateTime.UtcNow);

        // Act
        using var queryHandler = NewQueryHandler();
        var actual = await queryHandler.Handle(new GetBasketQuery(1), CancellationToken.None);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenBasketIsDeleted()
    {
        // Arrange
        _timeService.UtcNow.Returns(DateTime.UtcNow);
        var basket = BuilderFactory.NewBasketBuilder()
            .WithDeletedStatus()
            .BuildAndPersist();

        // Act
        using var queryHandler = NewQueryHandler();
        var actual = await queryHandler.Handle(new GetBasketQuery(basket.Id), CancellationToken.None);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenBasketIsExpired()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _timeService.UtcNow.Returns(now);
        var basket = BuilderFactory.NewBasketBuilder()
            .With(x => x.BasketExpirationTime, now.AddSeconds(-1))
            .BuildAndPersist();

        // Act
        using var queryHandler = NewQueryHandler();
        var actual = await queryHandler.Handle(new GetBasketQuery(basket.Id), CancellationToken.None);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenBasketIsComplete()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _timeService.UtcNow.Returns(now);
        var basket = ((BasketBuilder)BuilderFactory.NewBasketBuilder()
            .With(x => x.BasketExpirationTime, now.AddSeconds(1))
            .With(x => x.Status, BasketStatus.Complete))
            .BuildAndPersist();

        // Act
        using var queryHandler = NewQueryHandler();
        var actual = await queryHandler.Handle(new GetBasketQuery(basket.Id), CancellationToken.None);

        // Assert
        actual.ShouldBeNull();
    }
}
