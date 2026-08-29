using Demo.DomainServices.Command.Validation;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demo.DomainServices.Command.Category;

public class CategoryUpdateProductCommandHandler : CommandHandler<CategoryUpdateProductCommand, CategoryUpdateProductCommandValidator>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryUpdateProductCommandHandler(
        ILogger<CategoryUpdateProductCommandHandler> logger,
        ICategoryRepository categoryRepository,
        CategoryUpdateProductCommandValidator validator,
        IUnitOfWork unitOfWork) : base(logger, validator, unitOfWork)
    {
        _categoryRepository = categoryRepository;
    }

    protected override async Task<Model.Domain.Category> Execute(CategoryUpdateProductCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(command.Id);

        if (category is null)
        {
            throw new EntityNotFoundException($"Category with ID {command.Id} does not exist");
        }

        category.UpdateProduct(command.ProductId, command.ProductName, command.ProductPrice);

        return category;
    }
}

public class CategoryUpdateProductCommandValidator : AbstractValidator<CategoryUpdateProductCommand>
{
    public CategoryUpdateProductCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
                .WithError(CategoryCommandErrorType.Product_Name_Required);

        RuleFor(x => x.ProductPrice)
            .NotEmpty()
                .WithError(CategoryCommandErrorType.Product_Price_Required);
    }
}
