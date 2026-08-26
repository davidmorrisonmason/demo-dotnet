using Demo.DomainServices.Command.Category;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Validation;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Demo.Model.UnitTests.Command.Category
{
    [Collection(DatabaseTestCollection.Name)]
    public class CategoryDeleteCommandHandlerShould : CommandTest
    {
        private CategoryDeleteCommandHandler _commandHandler;

        public CategoryDeleteCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            var dbContext = new ApplicationDbContext(DbContextOptions);
            var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>());
            var unitOfWork = new UnitOfWork(dbContext);

            _commandHandler = new CategoryDeleteCommandHandler(
                            Substitute.For<ILogger<CategoryDeleteCommandHandler>>(),
                            categoryRepository,
                            new CategoryDeleteCommandValidator(),
                            unitOfWork);
        }

        [Fact]
        public async Task LoadCategoryAndSoftDelete_WhenExecuteCalled_WithValidCommand()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder()
                .BuildAndPersist();

            var expected = BuilderFactory.NewCategoryBuilder()
                .BuildFrom(category)
                .WithDeletedStatus()
                .Build();

            var command = new CategoryDeleteCommand(category.Id);

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            expected.ShouldBeInDatabase();
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
