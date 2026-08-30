using Demo.Api.Dto;
using Demo.Model.UnitTests;
using Demo.Model.UnitTests.Builders.Domain;
using Demo.Model.UnitTests.Database;
using Demo.Model.UnitTests.Validation;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Demo.Api.IntegrationTest.Endpoints;

[Collection(DatabaseTestCollection.Name)]
public class BasketsEndpointShould : DemoApiIntegrationTest
{
    public BasketsEndpointShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task CreateBasket_WhenPostCalled()
    {
        // Arrange
        var category = BuilderFactory.NewCategoryBuilder()
            .WithProducts(
            [
                BuilderFactory.NewProductBuilder().Build()
            ])
            .BuildAndPersist();

        var payload = new BasketCreateDto
        {
            BasketItems =
            [
                new BasketItemCreateDto
                {
                    CategoryId = category.Id,
                    ProductId = category.Products[0].Id,
                    Quantity = 2
                }
            ]
        };

        var basketBuilder
            = (BasketBuilder)(BuilderFactory.NewBasketBuilder().WithNextId());
        var basketId = basketBuilder.Build().Id;

        var expected = basketBuilder
            .WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder(basketId, category.Products[0].Id, 2)
                   .WithNextId()
                   .Build()
            ])
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/Baskets", payload);

        // Assert

        expected.ShouldBeInDatabase(query => query.Baskets.Include(basket => basket.BasketItems));
    }

    [Fact]
    public async Task AddBasketItems_WhenPutCalled()
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
                BuilderFactory.NewBasketItemBuilder(basket.Id, category.Products[0].Id, 3, 1)
                    .With(x => x.Id, basket.BasketItems[0].Id)
                    .Build(),
                BuilderFactory.NewBasketItemBuilder(basket.Id, category.Products[1].Id, 3, 2)
                    .WithNextId()
                    .Build()
            ])
            .Build();

        var payload = new BasketCreateDto
        {
            BasketItems =
            [
                new BasketItemCreateDto
                {
                    CategoryId = category.Id,
                    ProductId = category.Products[0].Id,
                    Quantity = 2
                },
                new BasketItemCreateDto
                {
                    CategoryId = category.Id,
                    ProductId = category.Products[1].Id,
                    Quantity = 3
                }
            ]
        };

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/Baskets/{basket.Id}", payload);

        // Assert
        response.ShouldBeNoContentResponse();
        expected.ShouldBeInDatabase(query => query.Baskets.Include(item => item.BasketItems));
    }

    [Fact]
    public async Task ReturnCorrectBasket_WhenGetByIdCalled()
    {
        // Arrange
        var category = BuilderFactory.NewCategoryBuilder()
            .WithProducts(
            [
                BuilderFactory.NewProductBuilder(1).Build()
            ])
            .BuildAndPersist();
        var basket = BuilderFactory.NewBasketBuilder()
            .WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder(0, category.Products[0].Id, 2, 1).Build()
            ])
            .BuildAndPersist();

        var expected = new BasketDto
        {
            Id = basket.Id,
            BasketItems =
            [
                new BasketItemDto
                {
                    Id = basket.BasketItems[0].Id,
                    ProductId = category.Products[0].Id,
                    Quantity = 2,
                    Product = new ProductDto
                    {
                        Id = category.Products[0].Id,
                        Name = category.Products[0].Name,
                        Price = category.Products[0].Price
                    }
                }
            ]
        };

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/Baskets/{basket.Id}");

        // Assert
        response.ShouldBeOkResponse(expected);
    }

    [Fact]
    public async Task ReturnNotFound_WhenGetByIdCalledForNonExistentBasket()
    {
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/Baskets/999");

        // Assert
        response.ShouldBeNotFoundErrorResponse();
    }

    [Fact]
    public async Task ReturnValidationError_WhenPostCalledWithEmptyBasketItems()
    {
        // Act
        var response = await Client.PostAsJsonAsync(
            $"{BaseUrl}/Baskets",
            new BasketCreateDto());

        // Assert
        response.ShouldBeModelValidationErrorResponse(
            Demo.DomainServices.Command.Checkout.BasketCommandErrorType.Basket_Items_Required.BuildErrorMessage());
    }
}
