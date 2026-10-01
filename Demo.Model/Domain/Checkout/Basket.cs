using Newtonsoft.Json;

namespace Demo.Model.Domain.Checkout;

public class Basket : DomainObject, IAggregateRoot
{
    #region Properties

    public List<BasketItem> BasketItems { get; private set; } = [];
    public DateTime BasketExpirationTime { get; private set; }
    public BasketStatus Status { get; private set; } = BasketStatus.Open;
    public int ClientId { get; private set; }

    public decimal TotalPrice => BasketItems.Sum(i => i.Price);

    #endregion

    #region Constructors

    public Basket() : this(UnsavedID, DateTime.MinValue, UnsavedID)
    {
    }

    public Basket(int id, DateTime basketExpirationTime, int clientId) : base(id)
    {
        BasketExpirationTime = basketExpirationTime;
        ClientId = clientId;
    }

    public Basket(DateTime basketExpirationTime, IEnumerable<BasketItem> basketItems, int clientId) : this(UnsavedID, basketExpirationTime, basketItems, clientId)
    {
    }

    [JsonConstructor]
    public Basket(int id, DateTime basketExpirationTime, IEnumerable<BasketItem> basketItems, int clientId) : this(id, basketExpirationTime, clientId)
    {
        BasketItems.AddRange(basketItems);

        foreach (var basketItem in basketItems)
        {
            basketItem.OnCreated();
        }
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
                existingItem.AddQuantity(basketItem.Quantity);
            }
            else
            {
                basketItem.OnCreated();
                BasketItems.Add(basketItem);
            }
        }
    }

    public void Complete()
    {
        Status = BasketStatus.Complete;
    }

    #endregion
}
