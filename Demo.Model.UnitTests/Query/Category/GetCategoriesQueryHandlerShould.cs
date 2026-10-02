using Demo.DomainServices.Interface.Query.Category;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Query.Category;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Query.Category
{
    [Collection(ModelTestsDatabaseTestCollection.Name)]
    public class GetCategoriesQueryHandlerShould : QueryTest
    {
        private GetCategoriesQueryHandler NewQueryHandler()
        {
            return new GetCategoriesQueryHandler(
                new ApplicationDbContext(DbContextOptions),
                new GetCategoriesQueryValidator(),
                Substitute.For<ILogger<GetCategoriesQueryHandler>>(),
                TestRequestContext);
        }

        public GetCategoriesQueryHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task ReturnCategories_WhenSomeNotDeleted()
        {
            // Arrange
            var item1 = BuilderFactory.NewCategoryBuilder(1)
                .WithSubCategories(
                [
                    BuilderFactory.NewCategoryBuilder(2).Build()
                ])
                .BuildAndPersist();

            var item2 = BuilderFactory.NewCategoryBuilder(3)
                .With(x => x.IsDeleted, true)
                .BuildAndPersist();

            var item3 = BuilderFactory.NewCategoryBuilder(4)
                .BuildAndPersist();

            List<Domain.Category> expected =
            [
                BuilderFactory.NewCategoryBuilder()
                    .BuildFrom(item1)
                    .Build(),
                BuilderFactory.NewCategoryBuilder()
                    .BuildFrom(item3)
                    .Build()
            ];

            // Act
            using var queryHandler = NewQueryHandler();
            var actual = await queryHandler.Handle(new GetCategoriesQuery(), CancellationToken.None);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public async Task ExcludeDeletedChildren_WhenLoadingCategoryGraphs_WithDeletedEntitiesAlreadyTracked()
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
            var expectedCategory = BuilderFactory.NewCategoryBuilder().BuildFrom(category)
                .With(x => x.Products, [BuilderFactory.NewProductBuilder().BuildFrom(category.Products[0]).Build()])
                .With(x => x.SubCategories, [BuilderFactory.NewCategoryBuilder().BuildFrom(category.SubCategories[0]).Build()])
                .Build();
            var expected = new[] { expectedCategory };
            using var dbContext = new ApplicationDbContext(DbContextOptions);
            dbContext.Products.ToList();
            dbContext.Categories.ToList();
            using var queryHandler = new GetCategoriesQueryHandler(dbContext, new GetCategoriesQueryValidator(),
                Substitute.For<ILogger<GetCategoriesQueryHandler>>(), TestRequestContext);

            // Act
            var actual = await queryHandler.Handle(new GetCategoriesQuery(), Xunit.TestContext.Current.CancellationToken);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public async Task ReturnEmptyList_WhenAllDeleted()
        {
            // Arrange
            var item1 = BuilderFactory.NewCategoryBuilder(1)
                .With(x => x.IsDeleted, true)
                .BuildAndPersist();

            var item2 = BuilderFactory.NewCategoryBuilder(2)
                .With(x => x.IsDeleted, true)
                .BuildAndPersist();

            var item3 = BuilderFactory.NewCategoryBuilder(3)
                .With(x => x.IsDeleted, true)
                .BuildAndPersist();

            List<Domain.Category> expected = [];

            // Act
            using var queryHandler = NewQueryHandler();
            var actual = await queryHandler.Handle(new GetCategoriesQuery(), CancellationToken.None);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }


        [Fact]
        public async Task ReturnEmptyList_WhenNoneInDatabase()
        {
            // Arrange
            List<Domain.Category> expected = [];

            // Act
            using var queryHandler = NewQueryHandler();
            var actual = await queryHandler.Handle(new GetCategoriesQuery(), CancellationToken.None);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
