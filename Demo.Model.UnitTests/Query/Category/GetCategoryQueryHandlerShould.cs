using Demo.DomainServices.Interface.Query.Category;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Query.Category;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Query.Category
{
    [Collection(DatabaseTestCollection.Name)]
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
        public async Task ReturnCategory_When_Exists()
        {
            // Arrange
            var original = BuilderFactory.NewCategoryBuilder()
                .BuildAndPersist();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(original)
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
