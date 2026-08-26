using Demo.DomainServices.Command.Category;
using Demo.DomainServices.Creation;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Validation;
using Demo.Model.Validation;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Category
{
    [Collection(DatabaseTestCollection.Name)]
    public class CategoryUpdateCommandHandlerShould : CommandTest
    {
        private CategoryUpdateCommandHandler _commandHandler;

        public CategoryUpdateCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            var dbContext = new ApplicationDbContext(DbContextOptions);
            var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>());
            var aggregateRootFactory = new AggregateRootFactory();
            var unitOfWork = new UnitOfWork(dbContext);
            _commandHandler = new CategoryUpdateCommandHandler(
                Substitute.For<ILogger<CategoryUpdateCommandHandler>>(),
                categoryRepository,
                new CategoryUpdateCommandValidator(categoryRepository),
                unitOfWork);
        }

        [Fact]
        public async Task LoadCategoryAndUpdate_WhenExecuteCalled_WithValidCommand()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder().BuildAndPersist();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(category)
                .With(x => x.Name, "New Name")
                .Build();

            var command = new CategoryUpdateCommand(category.Id, "New Name");

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            expected.ShouldBeInDatabase();
        }

        [Fact]
        public async Task ThrowValidationException_WhenExecuteCalled_WithInvalidCommand_NullName()
        {
            // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var command = new CategoryUpdateCommand(1, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

            // Assert
            exception.ErrorMessages.ShouldBeEquivalentTo(CategoryCommandErrorType.Category_Name_Required.BuildErrorMessages());
        }

        [Fact]
        public async Task ThrowValidationException_WhenExecuteCalled_WithInvalidCommand_WhitespaceName()
        {
            // Arrange
            var command = new CategoryUpdateCommand(1, "     ");

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

            // Assert
            exception.ErrorMessages.ShouldBeEquivalentTo(CategoryCommandErrorType.Category_Name_Required.BuildErrorMessages());
        }

        [Fact]
        public async Task ThrowValidationException_WhenExecuteCalled_WithNonUniqueName()
        {
            // Arrange
            var category1 = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Name, "Category 1")
                .BuildAndPersist();
            var category2 = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Name, "Category 2")
                .BuildAndPersist();

            var command = new CategoryUpdateCommand(category1.Id, category2.Name);

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

            // Assert
            exception.ErrorMessages.ShouldBeEquivalentTo(CategoryCommandErrorType.Category_Name_Must_Be_Unique.BuildErrorMessages());
        }

        [Fact]
        public async Task ThrowEntityNotFoundException_WhenExecuteCalled_WithNonExistentCategory()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Name, "Category 1")
                .BuildAndPersist();

            var command = new CategoryUpdateCommand(category.Id + 1, "New Name");

            // Act / Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
        }
    }
}
