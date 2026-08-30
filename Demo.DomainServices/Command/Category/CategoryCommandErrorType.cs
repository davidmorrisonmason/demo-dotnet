using Demo.Model.Validation;

namespace Demo.DomainServices.Command.Category;

public enum CategoryCommandErrorType
{
    [ErrorDescription(ErrorCode = "CATEGORY_NAME_REQUIRED", ErrorMessage = "Category name is required")]
    Category_Name_Required,

    [ErrorDescription(ErrorCode = "CATEGORY_NAME_MUST_BE_UNIQUE", ErrorMessage = "Category name must be unique")]
    Category_Name_Must_Be_Unique,

    [ErrorDescription(ErrorCode = "PRODUCT_NAME_REQUIRED", ErrorMessage = "Product name is required")]
    Product_Name_Required,

    [ErrorDescription(ErrorCode = "PRODUCT_PRICE_REQUIRED", ErrorMessage = "Product price is required")]
    Product_Price_Required,

    [ErrorDescription(ErrorCode = "SUBCATEGORY_NAME_REQUIRED", ErrorMessage = "Subcategory name is required")]
    SubCategory_Name_Required,
}
