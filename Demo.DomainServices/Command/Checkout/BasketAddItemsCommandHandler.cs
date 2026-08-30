using Demo.DomainServices.Command.Validation;
using Demo.DomainServices.Interface.Command.Checkout;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Domain.Checkout;
using Demo.Model.Validation;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Demo.DomainServices.Command.Checkout;

public class BasketAddItemsCommandHandler : CommandHandler<BasketAddItemsCommand, BasketAddItemsCommandValidator>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBasketRepository _basketRepository;

    public BasketAddItemsCommandHandler(
        ILogger<BasketAddItemsCommandHandler> logger,
        BasketAddItemsCommandValidator validator,
        ICategoryRepository categoryRepository,
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork) : base(logger, validator, unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _basketRepository = basketRepository;
    }

    protected override async Task Execute(BasketAddItemsCommand command, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.Get(command.BasketId);

        if (basket == null)
        {
            throw new EntityNotFoundException($"Basket with ID {command.BasketId} does not exist");
        }

        var categories = await BasketCommandHandlerUtils.GetCategoriesAndValidateProducts(
            _categoryRepository,
            command.BasketItems);

        basket.AddItems(command.BasketItems.Adapt<List<BasketItem>>());
    }
}

public class BasketAddItemsCommandValidator : CommandValidator<BasketAddItemsCommand>
{
    public BasketAddItemsCommandValidator()
    {
        RuleFor(x => x.BasketItems)
            .NotEmpty()
                .WithError(BasketCommandErrorType.Basket_Items_Required);

        RuleForEach(x => x.BasketItems)
            .ChildRules(item => item.RuleFor(x => x.Quantity)
                .GreaterThan(0)
                    .WithError(BasketCommandErrorType.Basket_Item_Quantity_Must_Be_Greater_Than_Zero));
    }
}
