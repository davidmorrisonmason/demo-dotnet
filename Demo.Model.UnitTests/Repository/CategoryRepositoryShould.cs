using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.UnitTests.Builders.Domain;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Repository;

[Collection(DatabaseTestCollection.Name)]
public class CategoryRepositoryShould : DatabaseTest
{
    private readonly ICategoryRepository _repository;

    public CategoryRepositoryShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _repository = new CategoryRepository(
            new ApplicationDbContext(DbContextOptions),
            Substitute.For<ILogger<Demo.DomainServices.Interface.Repository.ICategoryRepository>>(),
            TestRequestContext);
    }

    [Fact]
    public async Task ReturnCategoryWithProductsAndSubCategoryProducts_WhenGetCalled_AndCategoryExists()
    {
        // Arrange
        var category = BuilderFactory.NewCategoryBuilder()
            .WithProducts([BuilderFactory.NewProductBuilder(1).Build()])
            .WithSubCategories([
                BuilderFactory.NewCategoryBuilder(2)
                    .WithProducts([BuilderFactory.NewProductBuilder(3).Build()])
                    .Build()
            ])
            .BuildAndPersist();

        var expected = ((CategoryBuilder)BuilderFactory.NewCategoryBuilder().BuildFrom(category)).Build();

        // Act
        var actual = await _repository.Get(category.Id);

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task ReturnNull_WhenGetCalled_AndCategoryDoesNotExist()
    {
        // Arrange

        // Act
        var actual = await _repository.Get(999);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenGetCalled_AndCategoryIsDeleted()
    {
        // Arrange
        var category = BuilderFactory.NewCategoryBuilder()
            .With(x => x.IsDeleted, true)
            .BuildAndPersist();

        // Act
        var actual = await _repository.Get(category.Id);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnNull_WhenGetCalled_AndCategoryBelongsToAnotherClient()
    {
        // Arrange
        var otherClient = BuilderFactory.NewClientBuilder()
            .BuildAndPersist();

        var category = BuilderFactory.NewCategoryBuilder()
            .With(x => x.ClientId, otherClient.Id)
            .BuildAndPersist();

        // Act
        var actual = await _repository.Get(category.Id);

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnCategoriesWithMatchingName_WhenGetAllByNameCalled()
    {
        // Arrange
        var matchingCategory = BuilderFactory.NewCategoryBuilder(1)
            .WithProducts([BuilderFactory.NewProductBuilder(1).Build()])
            .BuildAndPersist();
        var otherCategory = BuilderFactory.NewCategoryBuilder(2)
            .BuildAndPersist();
        var otherClient = BuilderFactory.NewClientBuilder()
            .BuildAndPersist();
        var differentClientCategory = BuilderFactory.NewCategoryBuilder(3)
            .With(x => x.ClientId, otherClient.Id)
            .With(x => x.Name, matchingCategory.Name)
            .BuildAndPersist();

        var expected = new[]
        {
            ((CategoryBuilder)BuilderFactory.NewCategoryBuilder().BuildFrom(matchingCategory)).Build()
        };

        // Act
        var actual = await _repository.GetAllByName(matchingCategory.Name);

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task ReturnCategories_WhenGetAllByNameExcludingIdCalled()
    {
        // Arrange
        var excludedCategory = BuilderFactory.NewCategoryBuilder(1, 1)
            .BuildAndPersist();
        var matchingCategory = BuilderFactory.NewCategoryBuilder(1, 2)
            .WithProducts([BuilderFactory.NewProductBuilder(1).Build()])
            .BuildAndPersist();
        var otherClient = BuilderFactory.NewClientBuilder()
            .BuildAndPersist();
        var differentClientCategory = BuilderFactory.NewCategoryBuilder(3)
            .With(x => x.ClientId, otherClient.Id)
            .With(x => x.Name, matchingCategory.Name)
            .BuildAndPersist();

        var expected = new[]
        {
            ((CategoryBuilder)BuilderFactory.NewCategoryBuilder().BuildFrom(matchingCategory)).Build()
        };

        // Act
        var actual = (await _repository.GetAllByNameExcludingId(
            excludedCategory.Name,
            excludedCategory.Id)).ToArray();

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }
}
