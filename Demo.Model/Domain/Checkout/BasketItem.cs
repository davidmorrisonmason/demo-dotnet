using Newtonsoft.Json;

namespace Demo.Model.Domain.Checkout;

public class BasketItem : DomainObject
{
    #region Properties

    public int BasketId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }

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
}
