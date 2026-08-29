using Demo.Api.Dto;
using Demo.DomainServices.Command.Category;
using Demo.Model.Domain;
using Demo.Model.UnitTests;
using Demo.Model.UnitTests.Builders.Domain;
using Demo.Model.UnitTests.Database;
using Demo.Model.UnitTests.Validation;
using System.Net.Http.Json;

namespace Demo.Api.IntegrationTest.Endpoints;

[Collection(DatabaseTestCollection.Name)]
public class ProductsEndpointShould : DemoApiIntegrationTest
{
    public ProductsEndpointShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    #region Post

    [Fact]
    public async Task CreateProduct_WhenPostCalled()
    {
        // Arrange
        var category = CreateCategoryWithProducts();
        var newProductBuilder = BuilderFactory.NewProductBuilder(4)
            .With(x => x.Name, "New Product")
            .With(x => x.Price, 252.4m)
            .With(x => x.CategoryId, category.Id)
            .WithNextId();
        var expected = category.Products
            .Select(product => BuilderFactory.NewProductBuilder().BuildFrom(product).Build())
            .ToList();

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/Categories/{category.Id}/Products",
            new ProductCreateDto { Name = "New Product", Price = 252.4m });

        // Assert
        AssertCreatedResponse(response, expected, newProductBuilder);
    }

    [Fact]
    public async Task ReturnValidationError_WhenPostCalledWithInvalidPayload()
    {
        // Arrange
        var category = CreateCategoryWithProducts();
        var expected = category.Products
            .Select(product => BuilderFactory.NewProductBuilder().BuildFrom(product).Build())
            .ToList();

        // Act
        var actual = await Client.PostAsJsonAsync($"{BaseUrl}/Categories/{category.Id}/Products",
            new ProductCreateDto { Name = "New Product", Price = 0 });

        // Assert
        actual.ShouldBeModelValidationErrorResponse(CategoryCommandErrorType.Product_Price_Required.BuildErrorMessage());
        AssertCollection(expected);
    }

    #endregion

    #region Put

    [Fact]
    public async Task UpdateProduct_WhenPutCalled()
    {
        // Arrange
        var category = CreateCategoryWithProducts();
        var updatedProduct = BuilderFactory.NewProductBuilder()
            .BuildFrom(category.Products[1])
            .With(x => x.Name, "Updated Product")
            .With(x => x.Price, 252.4m)
            .Build();
        var expected = new List<Product>
        {
            BuilderFactory.NewProductBuilder().BuildFrom(category.Products[0]).Build(),
            updatedProduct,
            BuilderFactory.NewProductBuilder().BuildFrom(category.Products[2]).Build(),
        };

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/Categories/{category.Id}/Products/{updatedProduct.Id}",
            new ProductUpdateDto { Name = updatedProduct.Name, Price = updatedProduct.Price });

        // Assert
        AssertUpdatedResponse(response, expected);
    }

    [Fact]
    public async Task ReturnValidationError_WhenPutCalledWithInvalidPayload()
    {
        // Arrange
        var category = CreateCategoryWithProducts();
        var expected = category.Products
            .Select(product => BuilderFactory.NewProductBuilder().BuildFrom(product).Build())
            .ToList();

        // Act
        var actual = await Client.PutAsJsonAsync($"{BaseUrl}/Categories/{category.Id}/Products/{category.Products[1].Id}",
            new ProductUpdateDto { Name = "Updated Product", Price = 0 });

        // Assert
        actual.ShouldBeModelValidationErrorResponse(CategoryCommandErrorType.Product_Price_Required.BuildErrorMessage());
        AssertCollection(expected);
    }

    #endregion

    private Category CreateCategoryWithProducts()
    {
        return BuilderFactory.NewCategoryBuilder()
            .WithProducts(
            [
                BuilderFactory.NewProductBuilder(1).Build(),
                BuilderFactory.NewProductBuilder(2).Build(),
                BuilderFactory.NewProductBuilder(3).Build()
            ])
            .BuildAndPersist();
    }
}
