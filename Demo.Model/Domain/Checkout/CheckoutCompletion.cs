using Newtonsoft.Json;

namespace Demo.Model.Domain.Checkout;

public class CheckoutCompletion : DomainObject, IAggregateRoot
{
    #region Properties

    public int BasketId { get; set; }
    public Basket Basket { get; set; } = null!;
    public string Recipient { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? AddressLine3 { get; set; }
    public string? AddressLine4 { get; set; }
    public string City { get; set; }
    public string PostCode { get; set; }

    #endregion

    #region Constructors

    public CheckoutCompletion(
        int basketId,
        string recipient,
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string? addressLine4,
        string city,
        string postCode) : this(UnsavedID, basketId, recipient, addressLine1, addressLine2, addressLine3, addressLine4, city, postCode)
    {
    }

    [JsonConstructor]
    public CheckoutCompletion(
        int id,
        int basketId,
        string recipient,
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string? addressLine4,
        string addressLine5,
        string postCode) : base(id)
    {
        BasketId = basketId;
        Recipient = recipient;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        AddressLine3 = addressLine3;
        AddressLine4 = addressLine4;
        City = addressLine5;
        PostCode = postCode;
    }

    #endregion
}
