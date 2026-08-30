using Newtonsoft.Json;

namespace Demo.Model.Domain.Checkout;

public class Basket : DomainObject, IAggregateRoot
{
    #region Properties

    public List<BasketItem> BasketItems { get; set; } = [];

    #endregion

    #region Constructors

    public Basket() : this(UnsavedID)
    {
    }

    public Basket(int id) : base(id)
    {
    }
    public Basket(IEnumerable<BasketItem> basketItems) : this(UnsavedID, basketItems)
    {
    }

    [JsonConstructor]
    public Basket(int id, IEnumerable<BasketItem> basketItems) : base(id)
    {
        BasketItems.AddRange(basketItems);
    }

    #endregion

    #region Business Logic

    public void AddItems(IEnumerable<BasketItem> basketItems)
    {
        foreach (var basketItem in basketItems)
        {
            var existingItem = BasketItems.FirstOrDefault(i => i.ProductId == basketItem.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += basketItem.Quantity;
            }
            else
            {
                BasketItems.Add(basketItem);
            }
        }
    }

    #endregion
}
