using Demo.DomainServices.Command.Validation;
using Demo.DomainServices.Creation;
using Demo.DomainServices.Interface.Command.Checkout;
using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demo.DomainServices.Command.Checkout;

public class CheckoutCompleteCommandHandler
    : CommandHandler<CheckoutCompleteCommand, CheckoutCompleteCommandValidator>
{
    private readonly IBasketRepository _basketRepository;
    private readonly ICheckoutCompletionRepository _checkoutCompletionRepository;
    private readonly IAggregateRootFactory _aggregateRootFactory;

    public CheckoutCompleteCommandHandler(
        ILogger<CheckoutCompleteCommandHandler> logger,
        CheckoutCompleteCommandValidator validator,
        ICheckoutCompletionRepository checkoutCompletionRepository,
        IAggregateRootFactory aggregateRootFactory,
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork,
        IRequestContext requestContext)
        : base(logger, validator, unitOfWork, requestContext)
    {
        _basketRepository = basketRepository;
        _checkoutCompletionRepository = checkoutCompletionRepository;
        _aggregateRootFactory = aggregateRootFactory;
    }

    protected override async Task Execute(
        CheckoutCompleteCommand command,
        CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.Get(command.BasketId);

        var checkoutCompletion = _aggregateRootFactory.NewCheckoutCompletion(
            command.BasketId,
            command.Recipient,
            command.AddressLine1,
            command.AddressLine2,
            command.AddressLine3,
            command.AddressLine4,
            command.City,
            command.PostCode);

        await _checkoutCompletionRepository.Add(checkoutCompletion);

        basket!.Complete();
    }
}

public class CheckoutCompleteCommandValidator : CommandValidator<CheckoutCompleteCommand>
{
    public CheckoutCompleteCommandValidator(IBasketRepository basketRepository)
    {
        RuleFor(x => x.BasketId)
            .MustAsync(async (basketId, _) => await basketRepository.Get(basketId) is not null)
            .WithError(CheckoutCompleteCommandErrorType.Basket_Does_Not_Exist);

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithError(CheckoutCompleteCommandErrorType.Address_Line1_Required);

        RuleFor(x => x.City)
            .NotEmpty()
            .WithError(CheckoutCompleteCommandErrorType.City_Required);

        RuleFor(x => x.PostCode)
            .NotEmpty()
            .WithError(CheckoutCompleteCommandErrorType.Post_Code_Required);
    }
}

public enum CheckoutCompleteCommandErrorType
{
    [ErrorDescription(ErrorCode = "BASKET_DOES_NOT_EXIST", ErrorMessage = "Basket does not exist")]
    Basket_Does_Not_Exist,

    [ErrorDescription(ErrorCode = "ADDRESS_LINE_1_REQUIRED", ErrorMessage = "Address line 1 is required")]
    Address_Line1_Required,

    [ErrorDescription(ErrorCode = "CITY_REQUIRED", ErrorMessage = "City is required")]
    City_Required,

    [ErrorDescription(ErrorCode = "POST_CODE_REQUIRED", ErrorMessage = "Post code is required")]
    Post_Code_Required,
}
