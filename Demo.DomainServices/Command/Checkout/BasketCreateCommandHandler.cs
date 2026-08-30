using Demo.DomainServices.Command.Validation;
using Demo.DomainServices.Interface.Command.Checkout;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Domain.Checkout;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Demo.DomainServices.Command.Checkout;

public class BasketCreateCommandHandler : ResultCommandHandler<BasketCreateCommand, BasketCreateCommandValidator, Basket>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBasketRepository _basketRepository;

    public BasketCreateCommandHandler(
        ILogger<BasketCreateCommandHandler> logger,
        BasketCreateCommandValidator validator,
        ICategoryRepository categoryRepository,
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork) : base(logger, validator, unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _basketRepository = basketRepository;
    }

    protected override async Task<Basket> Execute(BasketCreateCommand command, CancellationToken cancellationToken)
    {
        var categories = await BasketCommandHandlerUtils.GetCategoriesAndValidateProducts(
            _categoryRepository,
            command.BasketItems);

        var basket = new Basket(command.BasketItems.Adapt<List<BasketItem>>());
        basket.OnCreated();

        await _basketRepository.Add(basket);

        return basket;
    }
}

public class BasketCreateCommandValidator : CommandValidator<BasketCreateCommand>
{
    public BasketCreateCommandValidator()
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
