using Demo.DomainServices.Interface.Query.Category;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Query.Category;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Query.Category
{
    [Collection(DatabaseTestCollection.Name)]
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

            List<Domain.Category> expected = new()
            {
                BuilderFactory.NewCategoryBuilder()
                    .BuildFrom(item1)
                    .Build(),
                BuilderFactory.NewCategoryBuilder()
                    .BuildFrom(item3)
                    .Build()
            };

            // Act
            using var queryHandler = NewQueryHandler();
            var actual = await queryHandler.Handle(new GetCategoriesQuery(), CancellationToken.None);

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

            List<Domain.Category> expected = new();

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
            List<Domain.Category> expected = new();

            // Act
            using var queryHandler = NewQueryHandler();
            var actual = await queryHandler.Handle(new GetCategoriesQuery(), CancellationToken.None);

            // Assert
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
