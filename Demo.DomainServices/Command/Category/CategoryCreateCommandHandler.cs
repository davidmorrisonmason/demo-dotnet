using Demo.DomainServices.Command.Validation;
using Demo.DomainServices.Creation;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demo.DomainServices.Command.Category;

public class CategoryCreateCommandHandler : ResultCommandHandler<CategoryCreateCommand, CategoryCreateCommandValidator, Model.Domain.Category>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAggregateRootFactory _aggregateRootFactory;

    public CategoryCreateCommandHandler(
        ILogger<CategoryCreateCommandHandler> logger,
        CategoryCreateCommandValidator validator,
        ICategoryRepository categoryRepository,
        IAggregateRootFactory aggregateRootFactory,
        IUnitOfWork unitOfWork) : base(logger, validator, unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _aggregateRootFactory = aggregateRootFactory;
    }

    protected override async Task<Model.Domain.Category> Execute(CategoryCreateCommand request, CancellationToken cancellationToken)
    {
        var category = _aggregateRootFactory.NewCategory(request.Name);

        await _categoryRepository.Add(category);

        return category;
    }
}

public class CategoryCreateCommandValidator : CommandValidator<CategoryCreateCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryCreateCommandValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithError(CategoryCommandErrorType.Category_Name_Required)
            .MustAsync(async (name, cancellationToken) => await IsUniqueName(name))
                .WithError(CategoryCommandErrorType.Category_Name_Must_Be_Unique);
    }

    private async Task<bool> IsUniqueName(string name)
    {
        var others = await _categoryRepository.GetAllByName(name);
        return !others.Any();
    }
}
