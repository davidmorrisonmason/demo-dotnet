using Demo.ApplicationServices.Command.Category;
using Demo.ApplicationServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.ApplicationServices.IntegrationTests.Command.Category
{
    [Collection(ApplicationServicesDatabaseTestCollection.Name)]
    public class CategoryDeleteCommandHandlerShould : CommandTest
    {
        private CategoryDeleteCommandHandler _commandHandler;

        public CategoryDeleteCommandHandlerShould(ApplicationServicesIntegrationTestDatabaseFixture databaseFixture) : base(databaseFixture)
        {
            var dbContext = new ApplicationDbContext(DbContextOptions);
            var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>(), TestRequestContext);
            var unitOfWork = new UnitOfWork(dbContext);

            _commandHandler = new CategoryDeleteCommandHandler(
                            Substitute.For<ILogger<CategoryDeleteCommandHandler>>(),
                            categoryRepository,
                            new CategoryDeleteCommandValidator(),
                            unitOfWork,
                            TestRequestContext);
        }

        [Fact]
        public async Task SoftDeleteCategorySubCategoriesAndTheirProducts_WhenExecuteCalled_WithValidCommand()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Products, [BuilderFactory.NewProductBuilder(1).Build()])
                .With(x => x.SubCategories,
                [
                    BuilderFactory.NewCategoryBuilder(1)
                        .With(x => x.Products, [BuilderFactory.NewProductBuilder(2).Build()])
                        .Build(),
                    BuilderFactory.NewCategoryBuilder(2).Build()
                ])
                .BuildAndPersist();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(category)
                .WithDeletedStatus()
                .Build();
            expected.Products[0].IsDeleted = true;
            expected.SubCategories[0].IsDeleted = true;
            expected.SubCategories[0].Products[0].IsDeleted = true;
            expected.SubCategories[1].IsDeleted = true;

            var command = new CategoryDeleteCommand(category.Id);

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            expected.ShouldBeInDatabase(dbContext => dbContext.Categories
                .Include(x => x.Products)
                .Include(x => x.SubCategories)
                    .ThenInclude(x => x.Products));
        }

        [Fact]
        public async Task ThrowEntityNotFoundException_WhenExecuteCalled_WithNonExistentCategory()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder()
                .BuildAndPersist();

            var command = new CategoryDeleteCommand(category.Id + 1);

            // Act / Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
        }
    }
}
