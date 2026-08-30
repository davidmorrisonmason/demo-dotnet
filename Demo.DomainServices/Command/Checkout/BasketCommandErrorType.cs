using Demo.Model.Validation;

namespace Demo.DomainServices.Command.Checkout;

public enum BasketCommandErrorType
{
    [ErrorDescription(ErrorCode = "BASKET_ITEMS_REQUIRED", ErrorMessage = "Basket items are required")]
    Basket_Items_Required,

    [ErrorDescription(ErrorCode = "BASKET_ITEM_QUANTITY_MUST_BE_GREATER_THAN_ZERO", ErrorMessage = "Basket item quantity must be greater than zero")]
    Basket_Item_Quantity_Must_Be_Greater_Than_Zero,
}
