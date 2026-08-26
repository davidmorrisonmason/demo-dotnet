using Newtonsoft.Json;

namespace Demo.Model.Domain;

public class Product : DomainObject
{
    #region Properties

    public int CategoryId { get; set; }
    public string Name { get; set; }

    public decimal Price { get; set; }

    #endregion

    #region Constructors

    public Product(
        int categoryId,
        string name,
        decimal price) : this(UnsavedID, categoryId, name, price)
    {
    }

    [JsonConstructor]
    public Product(
        int id,
        int categoryId,
        string name,
        decimal price) : base(id)
    {
        CategoryId = categoryId;
        Name = name;
        Price = price;
    }

    #endregion

    #region Business Logic

    public void Update(
        string? name,
        decimal price)
    {
        Name = name;
        Price = price;
    }

    #endregion
}
