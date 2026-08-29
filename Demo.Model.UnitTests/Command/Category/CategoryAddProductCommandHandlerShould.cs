using Demo.DomainServices.Command.Category;
using Demo.DomainServices.Creation;
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
    public class CategoryAddProductCommandHandlerShould : CommandTest
    {
        private CategoryAddProductCommandHandler _commandHandler;

        public CategoryAddProductCommandHandlerShould(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            var dbContext = new ApplicationDbContext(DbContextOptions);
            var categoryRepository = new CategoryRepository(dbContext, Substitute.For<ILogger<ICategoryRepository>>());
            var aggregateRootFactory = new AggregateRootFactory();
            var unitOfWork = new UnitOfWork(dbContext);
            _commandHandler = new CategoryAddProductCommandHandler(
                Substitute.For<ILogger<CategoryAddProductCommandHandler>>(),
                categoryRepository,
                new CategoryAddProductCommandValidator(categoryRepository),
                unitOfWork);
        }

        [Fact]
        public async Task LoadCategoryAndAddProduct_WhenExecuteCalled_WithValidCommand()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder()
                .WithProducts(
                [
                    BuilderFactory.NewProductBuilder(1).Build(),
                    BuilderFactory.NewProductBuilder(2).Build(),
                    BuilderFactory.NewProductBuilder(3).Build()
                ])
                .BuildAndPersist();

            var expectedNewProduct = BuilderFactory.NewProductBuilder(4)
                .With(x => x.Name, "New Product")
                .With(x => x.Price, 252.4m)
                .WithNextId()
                .Build();

            var expected = ((CategoryBuilder)(BuilderFactory.NewCategoryBuilder().BuildFrom(category)))
                .WithProducts([category.Products[0], category.Products[1], category.Products[2], expectedNewProduct])
                .Build();

            var command = new CategoryAddProductCommand(category.Id, "New Product", 252.4m);

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            var actual = TestContext.DbContext.Categories
                .Include(c => c.Products)
                .SingleOrDefault(c => c.Id == category.Id);

            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public async Task ThrowValidationException_WhenExecuteCalled_WithInvalidCommand_NullName()
        {
            // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var command = new CategoryAddProductCommand(1, null, 23.2m);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

            // Assert
            exception.ErrorMessages.ShouldBeEquivalentTo(CategoryCommandErrorType.Product_Name_Required.BuildErrorMessages());
        }

        [Fact]
        public async Task ThrowValidationException_WhenExecuteCalled_WithInvalidCommand_WhitespaceName()
        {
            // Arrange
            var command = new CategoryAddProductCommand(1, "     ", 23.2m);

            // Act
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _commandHandler.Handle(command, CancellationToken.None));

            // Assert
            exception.ErrorMessages.ShouldBeEquivalentTo(CategoryCommandErrorType.Product_Name_Required.BuildErrorMessages());
        }

        [Fact]
        public async Task ThrowEntityNotFoundException_WhenExecuteCalled_WithNonExistentCategory()
        {
            // Arrange
            var category = BuilderFactory.NewCategoryBuilder()
                .With(x => x.Name, "Category 1")
                .BuildAndPersist();

            var command = new CategoryAddProductCommand(category.Id + 1, "New Name", 23.3m);

            // Act / Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
        }
    }
}
