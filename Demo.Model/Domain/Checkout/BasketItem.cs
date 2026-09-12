using Demo.Model.Logging;
using Demo.Model.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.Model.Domain.Checkout;

public class BasketItem : DomainObject
{
    #region Fields

    private static readonly ILogger<BasketItem> _logger = DomainContext.Instance.CreateLogger<BasketItem>();

    #endregion

    #region Properties

    public int BasketId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price => Quantity * Product?.Price ?? 0;

    #endregion

    #region Constructors

    public BasketItem(
        int basketId,
        int productId,
        int quantity) : this(UnsavedID, basketId, productId, quantity)
    {
    }

    [JsonConstructor]
    public BasketItem(
        int id,
        int basketId,
        int productId,
        int quantity) : base(id)
    {
        BasketId = basketId;
        ProductId = productId;
        Quantity = quantity;
    }

    #endregion

    #region Business Logic

    public override void OnCreated()
    {
        ValidateQuantityGreaterThanZero();
    }

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
        ValidateQuantityGreaterThanZero();
    }

    private void ValidateQuantityGreaterThanZero()
    {
        if (Quantity <= 0)
        {
            LogMessageAndThrowValidationException(_logger, BasketItemErrorType.Quantity_Must_Be_Greater_Than_Zero);
        }
    }

    #endregion

    #region Error Types

    public enum BasketItemErrorType
    {
        [ErrorDescription(ErrorCode = "BASKET_ITEM_QUANTITY_MUST_BE_GREATER_THAN_ZERO", ErrorMessage = "Basket item quantity must be greater than zero")]
        Quantity_Must_Be_Greater_Than_Zero,
    }

    #endregion
}
