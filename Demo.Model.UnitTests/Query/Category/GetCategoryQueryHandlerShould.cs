using Demo.DomainServices.Interface.Query.Category;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Query.Category;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Query.Category
{
    [Collection(ModelTestsDatabaseTestCollection.Name)]
    public class GetCategoryQueryHandlerShould : QueryTest
    {
        public GetCategoryQueryHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        private GetCategoryQueryHandler NewQueryHandler()
        {
            return new GetCategoryQueryHandler(
                new ApplicationDbContext(DbContextOptions),
                new GetCategoryQueryValidator(),
                Substitute.For<ILogger<GetCategoryQueryHandler>>(),
                TestRequestContext);
        }

        [Fact]
        public async Task ExcludeDeletedChildren_WhenLoadingCategoryGraph_WithDeletedEntitiesAlreadyTracked()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products, [
                    BuilderFactory.NewProductBuilder(1).Build(),
                    BuilderFactory.NewProductBuilder(2).WithDeletedStatus().Build()
                ])
                .With(x => x.SubCategories, [
                    BuilderFactory.NewCategoryBuilder(2).Build(),
                    BuilderFactory.NewCategoryBuilder(3).WithDeletedStatus().Build()
                ])
                .BuildAndPersist();
            var expected = BuilderFactory.NewCategoryBuilder().BuildFrom(category)
                .With(x => x.Products, [BuilderFactory.NewProductBuilder().BuildFrom(category.Products[0]).Build()])
                .With(x => x.SubCategories, [BuilderFactory.NewCategoryBuilder().BuildFrom(category.SubCategories[0]).Build()])
                .Build();
            using var dbContext = new ApplicationDbContext(DbContextOptions);
            dbContext.Products.ToList();
            dbContext.Categories.ToList();
            using var queryHandler = new GetCategoryQueryHandler(dbContext, new GetCategoryQueryValidator(),
                Substitute.For<ILogger<GetCategoryQueryHandler>>(), TestRequestContext);

            // Act
            var actual = await queryHandler.Handle(new GetCategoryQuery(category.Id), Xunit.TestContext.Current.CancellationToken);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public async Task ReturnCategory_When_Exists()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products, [
                    BuilderFactory.NewProductBuilder(1).Build(),
                    BuilderFactory.NewProductBuilder(2).WithDeletedStatus().Build()])
                .With(x => x.SubCategories, [
                    BuilderFactory.NewCategoryBuilder(2).Build(),
                    BuilderFactory.NewCategoryBuilder(3).WithDeletedStatus().Build()
                ])
            .BuildAndPersist();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(original)
                .With(x => x.Products, [
                    BuilderFactory.NewProductBuilder().BuildFrom(original.Products[0]).Build() ])
                .With(x => x.SubCategories, [
                    BuilderFactory.NewCategoryBuilder().BuildFrom(original.SubCategories[0]).Build() ])
                .Build();

            // Act
            using var queryHandler = NewQueryHandler();
            var actual = await queryHandler.Handle(new GetCategoryQuery(original.Id), CancellationToken.None);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public async Task ReturnNull_When_DoesNotExist()
        {
            // Arrange

            // Act
            using var queryHandler = NewQueryHandler();
            var actual = await queryHandler.Handle(new GetCategoryQuery(1), CancellationToken.None);

            // Assert
            actual.ShouldBeNull();
        }

        [Fact]
        public async Task ReturnNull_When_Deleted()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .With(x => x.IsDeleted, true)
                .BuildAndPersist();

            // Act
            using var queryHandler = NewQueryHandler();
            var actual = await queryHandler.Handle(new GetCategoryQuery(original.Id), CancellationToken.None);

            // Assert
            actual.ShouldBeNull();
        }
    }
}
