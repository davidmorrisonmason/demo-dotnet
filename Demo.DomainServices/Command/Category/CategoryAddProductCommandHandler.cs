using Demo.DomainServices.Command.Validation;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Domain;
using Demo.Model.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;

using Demo.DomainServices.Context;
using Demo.DomainServices.Interface.Context;

namespace Demo.DomainServices.Command.Category;

public class CategoryAddProductCommandHandler : ResultCommandHandler<CategoryAddProductCommand, CategoryAddProductCommandValidator, Product>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryAddProductCommandHandler(
        ILogger<CategoryAddProductCommandHandler> logger,
        ICategoryRepository categoryRepository,
        CategoryAddProductCommandValidator validator,
        IUnitOfWork unitOfWork,
        IRequestContext requestContext) : base(logger, validator, unitOfWork, requestContext)
    {
        _categoryRepository = categoryRepository;
    }

    protected override async Task<Product> Execute(CategoryAddProductCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(command.Id);

        if (category is null)
        {
            throw new EntityNotFoundException($"Category with ID {command.Id} does not exist");
        }

        var product = category.AddProduct(command.ProductName, command.ProductPrice);

        return product;
    }
}

public class CategoryAddProductCommandValidator : AbstractValidator<CategoryAddProductCommand>
{
    public CategoryAddProductCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
                .WithError(CategoryCommandErrorType.Product_Name_Required);

        RuleFor(x => x.ProductPrice)
            .NotEmpty()
                .WithError(CategoryCommandErrorType.Product_Price_Required);
    }
}
