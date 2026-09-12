using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;

using Demo.DomainServices.Context;
using Demo.DomainServices.Interface.Context;

namespace Demo.DomainServices.Command.Category;

public class CategoryDeleteCommandHandler : CommandHandler<CategoryDeleteCommand, CategoryDeleteCommandValidator>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryDeleteCommandHandler(
        ILogger<CategoryDeleteCommandHandler> logger,
        ICategoryRepository categoryRepository,
        CategoryDeleteCommandValidator validator,
        IUnitOfWork unitOfWork,
        IRequestContext requestContext) : base(logger, validator, unitOfWork, requestContext)
    {
        _categoryRepository = categoryRepository;
    }

    protected override async Task Execute(CategoryDeleteCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.Get(command.Id);

        if (category is null)
        {
            throw new EntityNotFoundException($"Category with ID {command.Id} does not exist");
        }

        category.OnDeleted();
    }
}

public class CategoryDeleteCommandValidator : AbstractValidator<CategoryDeleteCommand>
{
}
