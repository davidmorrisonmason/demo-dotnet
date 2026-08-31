using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demo.DomainServices.Command.Category;

public class CategoryRemoveSubCategoryCommandHandler : CommandHandler<CategoryRemoveSubCategoryCommand, CategoryRemoveSubCategoryCommandValidator>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryRemoveSubCategoryCommandHandler(
        ILogger<CategoryRemoveSubCategoryCommandHandler> logger,
        ICategoryRepository categoryRepository,
        CategoryRemoveSubCategoryCommandValidator validator,
        IUnitOfWork unitOfWork) : base(logger, validator, unitOfWork)
    {
        _categoryRepository = categoryRepository;
    }

    protected override async Task Execute(CategoryRemoveSubCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(command.Id);

        if (category is null)
        {
            throw new EntityNotFoundException($"Category with ID {command.Id} does not exist");
        }

        category.RemoveSubCategory(command.SubCategoryId);
    }
}

public class CategoryRemoveSubCategoryCommandValidator : AbstractValidator<CategoryRemoveSubCategoryCommand>
{
}
