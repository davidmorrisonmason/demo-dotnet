using Demo.DomainServices.Command.Validation;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demo.DomainServices.Command.Category;

public class CategoryUpdateCommandHandler : CommandHandler<CategoryUpdateCommand, CategoryUpdateCommandValidator>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryUpdateCommandHandler(
        ILogger<CategoryUpdateCommandHandler> logger,
        ICategoryRepository categoryRepository,
        CategoryUpdateCommandValidator validator,
        IUnitOfWork unitOfWork) : base(logger, validator, unitOfWork)
    {
        _categoryRepository = categoryRepository;
    }

    protected override async Task<Model.Domain.Category> Execute(CategoryUpdateCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(command.Id);

        if (category is null)
        {
            throw new EntityNotFoundException($"Category with ID {command.Id} does not exist");
        }

        category.Update(command.Name);

        return category;
    }
}

public class CategoryUpdateCommandValidator : AbstractValidator<CategoryUpdateCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryUpdateCommandValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithError(CategoryCommandErrorType.Category_Name_Required)
            .MustAsync(async (c, name, cancellationToken) => await IsUniqueName(c.Id, name ?? string.Empty))
                .WithError(CategoryCommandErrorType.Category_Name_Must_Be_Unique);
    }

    private async Task<bool> IsUniqueName(int id, string name)
    {
        var others = await _categoryRepository.GetAllByNameExcludingId(name, id);
        return !others.Any();
    }
}
