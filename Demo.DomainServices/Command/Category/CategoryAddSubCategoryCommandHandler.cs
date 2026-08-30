using Demo.DomainServices.Command.Validation;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Domain;
using Demo.Model.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demo.DomainServices.Command.Category;

public class CategoryAddSubCategoryCommandHandler : ResultCommandHandler<CategoryAddSubCategoryCommand, CategoryAddSubCategoryCommandValidator, Model.Domain.Category>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryAddSubCategoryCommandHandler(
        ILogger<CategoryAddSubCategoryCommandHandler> logger,
        ICategoryRepository categoryRepository,
        CategoryAddSubCategoryCommandValidator validator,
        IUnitOfWork unitOfWork) : base(logger, validator, unitOfWork)
    {
        _categoryRepository = categoryRepository;
    }

    protected override async Task<Model.Domain.Category> Execute(CategoryAddSubCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(command.Id);

        if (category is null)
        {
            throw new EntityNotFoundException($"Category with ID {command.Id} does not exist");
        }

        return category.AddSubCategory(command.Name);
    }
}

public class CategoryAddSubCategoryCommandValidator : AbstractValidator<CategoryAddSubCategoryCommand>
{
    public CategoryAddSubCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithError(CategoryCommandErrorType.SubCategory_Name_Required);
    }
}
