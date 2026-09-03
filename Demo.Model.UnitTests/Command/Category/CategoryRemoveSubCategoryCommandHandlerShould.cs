using Demo.DomainServices.Command.Category;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Category
{
    [Collection(DatabaseTestCollection.Name)]
    public class CategoryRemoveSubCategoryCommandHandlerShould : CommandTest
    {
        private readonly CategoryRemoveSubCategoryCommandHandler _commandHandler;

        public CategoryRemoveSubCategoryCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            var dbContext = new ApplicationDbContext(DbContextOptions);
            var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>(), TestRequestContext);

            _commandHandler = new CategoryRemoveSubCategoryCommandHandler(
                Substitute.For<ILogger<CategoryRemoveSubCategoryCommandHandler>>(),
                categoryRepository,
                new CategoryRemoveSubCategoryCommandValidator(),
                new UnitOfWork(dbContext),
                TestRequestContext);
        }

        [Fact]
        public async Task SoftDeleteSubCategoryAndItsProducts_WhenExecuteCalled_WithValidCommand()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder()
                .With(x => x.SubCategories,
                [
                    BuilderFactory.NewCategoryBuilder(1)
                        .With(x => x.Products,
                        [
                            BuilderFactory.NewProductBuilder(1).Build(),
                            BuilderFactory.NewProductBuilder(2).Build()
                        ])
                        .Build(),
                    BuilderFactory.NewCategoryBuilder(2).Build()
                ])
                .BuildAndPersist();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(category)
                .With(x => x.SubCategories,
                [
                    BuilderFactory.NewCategoryBuilder()
                        .BuildFrom(category.SubCategories[0])
                        .WithDeletedStatus()
                        .With(x => x.Products,
                        [
                            BuilderFactory.NewProductBuilder()
                                .BuildFrom(category.SubCategories[0].Products[0])
                                .WithDeletedStatus()
                                .Build(),
                            BuilderFactory.NewProductBuilder()
                                .BuildFrom(category.SubCategories[0].Products[1])
                                .WithDeletedStatus()
                                .Build()
                        ])
                        .Build(),
                    BuilderFactory.NewCategoryBuilder()
                        .BuildFrom(category.SubCategories[1])
                        .Build()
                ])
                .Build();

            var command = new CategoryRemoveSubCategoryCommand(category.Id, category.SubCategories[0].Id);

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
            var command = new CategoryRemoveSubCategoryCommand(999, 1);

            // Act / Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task ThrowEntityNotFoundException_WhenExecuteCalled_WithNonExistentSubCategory()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder().BuildAndPersist();
            var command = new CategoryRemoveSubCategoryCommand(category.Id, 999);

            // Act / Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
        }
    }
}
