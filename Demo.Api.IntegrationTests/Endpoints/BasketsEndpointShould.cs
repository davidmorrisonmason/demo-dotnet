using Demo.Api.Dto;
using Demo.Model.Domain.Checkout;
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

        var now = DateTime.UtcNow;

        var expected = ((BasketBuilder)BuilderFactory.NewBasketBuilder()
            .WithNextId())
            .WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder()
                   .WithNextId()
                   .With(x => x.ProductId, category.Products[0].Id)
                   .With(x => x.Quantity, 2)
                   .Build()
            ])
            .With(x => x.BasketExpirationTime, now.AddHours(1))
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
                BuilderFactory.NewBasketItemBuilder()
                   .WithNextId()
                   .With(x => x.ProductId, category.Products[0].Id)
                   .With(x => x.Quantity, 1)
                   .Build()
            ])
            .BuildAndPersist();

        var expected = ((BasketBuilder)BuilderFactory.NewBasketBuilder().BuildFrom(basket))
            .WithBasketItems(
            [
                BuilderFactory.NewBasketItemBuilder()
                   .BuildFrom(basket.BasketItems[0])
                   .With(x => x.Quantity, 3)
                   .Build(),
                BuilderFactory.NewBasketItemBuilder()
                   .WithNextId()
                   .With(x => x.ProductId, category.Products[1].Id)
                   .With(x => x.Quantity, 3)
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
    public async Task CompleteBasket_WhenPostCompleteCalled()
    {
        // Arrange
        var basket = BuilderFactory.NewBasketBuilder().BuildAndPersist();
        var payload = new CheckoutCompleteDto
        {
            Recipient = "Recipient",
            AddressLine1 = "Address line 1",
            AddressLine2 = "Address line 2",
            AddressLine3 = "Address line 3",
            AddressLine4 = "Address line 4",
            City = "City",
            PostCode = "Post code"
        };

        var expectedBasket = BuilderFactory.NewBasketBuilder()
            .BuildFrom(basket)
            .With(x => x.Status, BasketStatus.Complete)
            .Build();

        var expected = BuilderFactory.NewCheckoutCompletionBuilder()
            .WithBasket(expectedBasket)
            .WithNextId()
            .With(x => x.Recipient, payload.Recipient)
            .With(x => x.AddressLine1, payload.AddressLine1)
            .With(x => x.AddressLine2, payload.AddressLine2)
            .With(x => x.AddressLine3, payload.AddressLine3)
            .With(x => x.AddressLine4, payload.AddressLine4)
            .With(x => x.City, payload.City)
            .With(x => x.PostCode, payload.PostCode)
            .Build();

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/Baskets/{basket.Id}/Complete", payload);

        // Assert
        response.ShouldBeNoContentResponse();
        expected.ShouldBeInDatabase(query => query.CheckoutCompletions.Include(completion => completion.Basket));
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
                BuilderFactory.NewBasketItemBuilder()
                   .WithNextId()
                   .With(x => x.ProductId, category.Products[0].Id)
                   .With(x => x.Quantity, 2)
                   .Build()
            ])
            .BuildAndPersist();


        var expected = new BasketDto
        {
            Id = basket.Id,
            BasketExpirationTime = basket.BasketExpirationTime,
            TotalPrice = 20,
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
