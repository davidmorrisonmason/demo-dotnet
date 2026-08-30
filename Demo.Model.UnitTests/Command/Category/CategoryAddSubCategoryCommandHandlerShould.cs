using Demo.DomainServices.Command.Category;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Builders.Domain;
using Demo.Model.UnitTests.Validation;
using Demo.Model.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Category
{
    [Collection(DatabaseTestCollection.Name)]
    public class CategoryAddSubCategoryCommandHandlerShould : CommandTest
    {
        private readonly CategoryAddSubCategoryCommandHandler _commandHandler;

        public CategoryAddSubCategoryCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            var dbContext = new ApplicationDbContext(DbContextOptions);
            var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>());
            _commandHandler = new CategoryAddSubCategoryCommandHandler(
                Substitute.For<ILogger<CategoryAddSubCategoryCommandHandler>>(),
                categoryRepository,
                new CategoryAddSubCategoryCommandValidator(categoryRepository),
                new UnitOfWork(dbContext));
        }

        [Fact]
        public async Task AddSubCategory_WhenExecuteCalled_WithValidCommand()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder().BuildAndPersist();
            var command = new CategoryAddSubCategoryCommand(category.Id, "New Subcategory");

            var expectedNewCategory = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Name, "New Subcategory")
                .With(x => x.ParentCategoryId, category.Id)
                .WithNextId()
                .Build();

            var expected = ((CategoryBuilder)(BuilderFactory.NewCategoryBuilder().BuildFrom(category)))
                .WithSubCategories([expectedNewCategory])
                .Build();

            // Act
            var actual = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            actual.ShouldBeEquivalentTo(expectedNewCategory);
            expected.ShouldBeInDatabase(dbContext => dbContext.Categories.Include(c => c.SubCategories));
        }

        [Fact]
        public async Task ThrowValidationException_WhenExecuteCalled_WithInvalidCommand()
        {
            // Arrange
#pragma warning disable CS8625
            var command = new CategoryAddSubCategoryCommand(1, null);
#pragma warning restore CS8625

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

            // Assert
            exception.ErrorMessages.ShouldBeEquivalentTo(CategoryCommandErrorType.SubCategory_Name_Required.BuildErrorMessages());
        }

        [Fact]
        public async Task ThrowEntityNotFoundException_WhenExecuteCalled_WithNonExistentCategory()
        {
            // Arrange
            var command = new CategoryAddSubCategoryCommand(999, "New Subcategory");

            // Act / Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
        }
    }
}
