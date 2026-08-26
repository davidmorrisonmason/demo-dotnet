using Demo.DomainServices.Command.Category;
using Demo.DomainServices.Creation;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Validation;
using Demo.Model.UnitTests.Validation;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Category
{
    [Collection(DatabaseTestCollection.Name)]
    public class CategoryCreateCommandHandlerShould : CommandTest
    {
        private CategoryCreateCommandHandler _commandHandler;

        public CategoryCreateCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            var dbContext = new ApplicationDbContext(DbContextOptions);
            var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>());
            var aggregateRootFactory = new AggregateRootFactory();
            var unitOfWork = new UnitOfWork(dbContext);
            _commandHandler = new CategoryCreateCommandHandler(
                Substitute.For<ILogger<CategoryCreateCommandHandler>>(),
                new CategoryCreateCommandValidator(categoryRepository),
                categoryRepository,
                aggregateRootFactory,
                unitOfWork);
        }

        [Fact]
        public async Task CreateCategory_WhenExecuteCalled_WithValidCommand()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder()
                .Build();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(category)
                .WithNextId()
                .Build();

            var command = new CategoryCreateCommand(category.Name);

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            expected.ShouldBeInDatabase();
        }

        [Theory]
        [InlineData("    ")]
        [InlineData(null)]
        public async Task ThrowValidationException_WhenExecuteCalled_WithInvalidCommand_NullName(string? name)
        {
            // Arrange
#pragma warning disable CS8604 // Possible null reference argument.
            var command = new CategoryCreateCommand(name);
#pragma warning restore CS8604 // Possible null reference argument.

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

            // Assert
            exception.ErrorMessages.ShouldBeEquivalentTo(CategoryCommandErrorType.Category_Name_Required.BuildErrorMessages());
        }

        [Fact]
        public async Task ThrowValidationException_WhenExecuteCalled_WithNonUniqueName()
        {
            // Arrange
            var categories = new List<Domain.Category>
            {
                BuilderFactory.NewCategoryBuilder(0, 1).BuildAndPersist(),
                BuilderFactory.NewCategoryBuilder(0, 2).BuildAndPersist(),
                BuilderFactory.NewCategoryBuilder(0, 3).BuildAndPersist(),
            };

            var command = new CategoryCreateCommand(categories[1].Name);

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

            // Assert
            exception.ErrorMessages.ShouldBeEquivalentTo(CategoryCommandErrorType.Category_Name_Must_Be_Unique.BuildErrorMessages());
        }
    }
}
