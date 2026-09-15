using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Time;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Checkout;
using Demo.Model.UnitTests.Builders.Domain;
using NSubstitute;

namespace Demo.Model.UnitTests.Repository;

[Collection(ModelTestsDatabaseTestCollection.Name)]
public class BasketRepositoryShould : DatabaseTest
{
    private readonly DateTime _now = new(2026, 9, 2, 12, 0, 0, DateTimeKind.Utc);
    private readonly IBasketRepository _repository;

    public BasketRepositoryShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        var timeService = Substitute.For<ITimeService>();
        timeService.UtcNow.Returns(_now);

        _repository = new BasketRepository(
            new ApplicationDbContext(DbContextOptions),
            TestRequestContext,
            timeService);
    }


    [Fact]
    public async Task ReturnBasketWithBasketItems_WhenBasketIsOpenAndNotExpired()
    {
        // Arrange
        var category = BuilderFactory.NewCategoryBuilder()
            .WithProducts([BuilderFactory.NewProductBuilder(1).Build()])
            .BuildAndPersist();

        var product = category.Products[0];

        var basket = BuilderFactory.NewBasketBuilder()
            .WithBasketItems([
                BuilderFactory.NewBasketItemBuilder()
                .With(x => x.ProductId, product.Id)
                .With(x => x.Quantity, 2)
                .Build()
            ])
            .With(x => x.BasketExpirationTime, _now.AddMinutes(1))
            .BuildAndPersist();

        var expected = ((BasketBuilder)BuilderFactory.NewBasketBuilder().BuildFrom(basket))
            .Build();

        // Act
        var actual = await _repository.Get(basket.Id);

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task ReturnNull_WhenBasketDoesNotExist()
    {
        // Arrange
        var repository = _repository;

        // Act
        var actual = await repository.Get(999);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenBasketIsDeleted()
    {
        // Arrange
        var basket = BuilderFactory.NewBasketBuilder()
            .With(x => x.IsDeleted, true)
            .BuildAndPersist();

        // Act
        var actual = await _repository.Get(basket.Id);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenBasketIsExpired()
    {
        // Arrange
        var basket = BuilderFactory.NewBasketBuilder()
            .With(x => x.BasketExpirationTime, _now.AddSeconds(-1))
            .BuildAndPersist();

        // Act
        var actual = await _repository.Get(basket.Id);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenBasketIsComplete()
    {
        // Arrange
        var basket = BuilderFactory.NewBasketBuilder()
            .With(x => x.Status, BasketStatus.Complete)
            .BuildAndPersist();

        // Act
        var actual = await _repository.Get(basket.Id);

        // Assert
        actual.ShouldBeNull();
    }
}
