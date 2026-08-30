using Demo.Model.Logging;
using Demo.Model.Validation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.Model.Domain;

public class Category : DomainObject, IAggregateRoot
{
    #region Fields

    private static readonly ILogger<Category> _logger = DomainContext.Instance.CreateLogger<Category>();

    #endregion

    #region Constructors

    public Category(
        string name) : this(UnsavedID, name, null)
    {
    }
    public Category(
        string name,
        int parentCategoryId) : this(UnsavedID, name, parentCategoryId)
    {
    }

    [JsonConstructor]
    public Category(
        int id,
        string name,
        int? parentCategoryId) : base(id)
    {
        Name = name;
        ParentCategoryId = parentCategoryId;
    }

    #endregion

    #region Properties

    public string Name { get; set; }
    public int? ParentCategoryId { get; set; } = null;

    public List<Product> Products { get; set; } = [];
    public List<Category> SubCategories { get; set; } = [];

    #endregion

    #region Business Logic 

    public void Update(string name)
    {
        Name = name;
    }

    public Category AddSubCategory(string name)
    {
        ValidateSubCategoryNameUnique(name);

        var subCategory = new Category(name, Id);
        subCategory.OnCreated();
        SubCategories.Add(subCategory);

        return subCategory;
    }

    public Product AddProduct(string name, decimal price)
    {
        ValidateProductNameUnique(name);

        var product = new Product(this.Id, name, price);
        product.OnCreated();
        Products.Add(product);

        return product;
    }

    public void UpdateProduct(int productId, string newName, decimal newPrice)
    {
        ValidateProductExists(productId);
        ValidateProductNameUnique(newName, productId);

        var product = Products.Single(x => x.Id == productId);
        product.Update(newName, newPrice);
    }

    public void ValidateProductExists(int id)
    {
        if (!Products.Any(x => x.Id == id))
        {
            LogMessageAndThrowEntityNotFoundException(_logger, $"Product with supplied ID '{id}' does not exist");
        }
    }

    private void ValidateProductNameUnique(string name, int? excludeId = null)
    {
        if (Products.Any(x => x.Name == name && x.Id != excludeId))
        {
            LogMessageAndThrowValidationException(_logger, CategoryErrorType.Product_Name_Must_Be_Unique);
        }
    }

    private void ValidateSubCategoryNameUnique(string name, int? excludeId = null)
    {
        if (SubCategories.Any(x => x.Name == name && x.Id != excludeId))
        {
            LogMessageAndThrowValidationException(_logger, CategoryErrorType.SubCategory_Name_Must_Be_Unique);
        }
    }

    #endregion

    #region Error Types

    public enum CategoryErrorType
    {
        [ErrorDescription(ErrorCode = "PRODUCT_DOES_NOT_EXIST", ErrorMessage = "Product does not exist")]
        Product_Does_Not_Exist,

        [ErrorDescription(ErrorCode = "PRODUCT_NAME_MUST_BE_UNIQUE", ErrorMessage = "Product name must be unique")]
        Product_Name_Must_Be_Unique,

        [ErrorDescription(ErrorCode = "SUBCATEGORY_NAME_MUST_BE_UNIQUE", ErrorMessage = "Subcategory name must be unique")]
        SubCategory_Name_Must_Be_Unique,
    }

    #endregion
}
