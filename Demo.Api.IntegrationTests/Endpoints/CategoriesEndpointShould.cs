using Demo.Api.Dto;
using Demo.Api.IntegrationTests.Builders;
using Demo.Model.Domain;
using Demo.Model.UnitTests.Database;
namespace Demo.Api.IntegrationTest.Endpoints;

using Demo.DomainServices.Command.Category;
using Demo.Model.UnitTests;
using Demo.Model.UnitTests.Validation;
using System.Net.Http.Json;

[Collection(DatabaseTestCollection.Name)]
public class CategoriesEndpointShould : DemoApiIntegrationTest
{
    public CategoriesEndpointShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    #region List

    [Fact]
    public async Task ReturnCorrectCategories_WhenGetCalled()
    {
        // Arrange
        var categories = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder(1).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(2).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(3).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(4).With(x => x.IsDeleted, true).BuildAndPersist()
        };

        var expected = new List<CategoryDto>()
        {
            CategoryDtoBuilder.BuildFromCategory(categories[0]).Build(),
            CategoryDtoBuilder.BuildFromCategory(categories[1]).Build(),
            CategoryDtoBuilder.BuildFromCategory(categories[2]).Build(),
        };

        // Act
        var actual = await Client.GetAsync($"{BaseUrl}/Categories");

        // Assert
        actual.ShouldBeOkListResponse(expected);
    }

    #endregion

    #region Get

    [Fact]
    public async Task ReturnCorrectCategory_WhenGetByIdCalled()
    {
        // Arrange
        var categories = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder(1).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(2).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(3).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(4).With(x => x.IsDeleted, true).BuildAndPersist()
        };

        var expected = CategoryDtoBuilder.BuildFromCategory(categories[1]).Build();

        // Act
        var actual = await Client.GetAsync($"{BaseUrl}/Categories/{categories[1].Id}");

        // Assert
        actual.ShouldBeOkResponse(expected);
    }

    [Fact]
    public async Task ReturnNotFound_WhenGetByIdCalledForNonExistentCategory()
    {
        // Arrange
        var categories = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder(1).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(2).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(3).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(4).With(x => x.IsDeleted, true).BuildAndPersist()
        };

        // Act
        var actual = await Client.GetAsync($"{BaseUrl}/Categories/99");

        // Assert
        actual.ShouldBeNotFoundErrorResponse();
    }

    [Fact]
    public async Task ReturnNotFound_WhenGetByIdCalledForDeletedCategory()
    {
        // Arrange
        var categories = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder(1).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(2).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(3).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(4).With(x => x.IsDeleted, true).BuildAndPersist()
        };

        // Act
        var actual = await Client.GetAsync($"{BaseUrl}/Categories/{categories[3].Id}");

        // Assert
        actual.ShouldBeNotFoundErrorResponse();
    }

    #endregion

    #region Post

    [Fact]
    public async Task CreateCategory_WhenPostCalled()
    {
        // Arrange
        var categories = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder(1).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(2).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(3).BuildAndPersist()
        };

        var newCategoryBuilder = BuilderFactory.NewCategoryBuilder(4)
            .WithNextId();

        var expected = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[0]).Build(),
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[1]).Build(),
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[2]).Build(),
        };

        // Act
        var response = await Client.PostAsJsonAsync($"{BaseUrl}/Categories/",
            CategoryCreateDtoBuilder.BuildFromCategory(newCategoryBuilder.Build())
                .Build());

        // Assert
        AssertCreatedResponse(response, expected, newCategoryBuilder);
    }

    [Fact]
    public async Task ReturnValidationError_WhenPostCalledWithInvalidPayload()
    {
        // Arrange
        var categories = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder(1).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(2).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(3).BuildAndPersist()
        };

        var newCategoryBuilder = BuilderFactory.NewCategoryBuilder(4)
            .With(x => x.Name, categories[0].Name);

        var expected = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[0]).Build(),
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[1]).Build(),
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[2]).Build(),
        };

        // Act
        var actual = await Client.PostAsJsonAsync($"{BaseUrl}/Categories/",
            CategoryCreateDtoBuilder.BuildFromCategory(newCategoryBuilder.Build())
                .Build());

        // Assert
        actual.ShouldBeModelValidationErrorResponse(CategoryCommandErrorType.Category_Name_Must_Be_Unique.BuildErrorMessage());
    }

    #endregion

    #region Put

    [Fact]
    public async Task UpdateCategory_WhenPutCalled()
    {
        // Arrange
        var categories = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder(1).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(2).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(3).BuildAndPersist()
        };

        var updatedCategory = BuilderFactory.NewCategoryBuilder()
            .BuildFrom(categories[1])
            .With(x => x.Name, "New Category")
            .Build();

        var expected = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[0]).Build(),
            updatedCategory,
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[2]).Build(),
        };

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/Categories/{updatedCategory.Id}",
            CategoryDtoBuilder.BuildFromCategory(updatedCategory).Build());

        // Assert
        AssertUpdatedResponse(response, expected);
    }


    [Fact]
    public async Task ReturnValidationError_WhenPutCalledWithInvalidPayload()
    {
        // Arrange
        var categories = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder(1).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(2).BuildAndPersist(),
            BuilderFactory.NewCategoryBuilder(3).BuildAndPersist()
        };

        var updatedCategory = BuilderFactory.NewCategoryBuilder()
            .BuildFrom(categories[1])
            .With(x => x.Name, categories[2].Name)
            .Build();

        var expected = new List<Category>
        {
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[0]).Build(),
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[1]).Build(),
            BuilderFactory.NewCategoryBuilder().BuildFrom(categories[2]).Build(),
        };

        // Act
        var actual = await Client.PutAsJsonAsync($"{BaseUrl}/Categories/{updatedCategory.Id}",
            CategoryDtoBuilder.BuildFromCategory(updatedCategory).Build());

        // Assert
        actual.ShouldBeModelValidationErrorResponse(CategoryCommandErrorType.Category_Name_Must_Be_Unique.BuildErrorMessage());
        AssertCollection(expected);
    }

    #endregion
}
