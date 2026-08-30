using Demo.DomainServices.Interface.Command.Checkout;
using Demo.DomainServices.Interface.Repository;
using Demo.Model.Validation;

namespace Demo.DomainServices.Command.Checkout;

internal static class BasketCommandHandlerUtils
{
    internal static async Task<Dictionary<int, Model.Domain.Category>> GetCategoriesAndValidateProducts(
        ICategoryRepository categoryRepository,
        IEnumerable<BasketItemCommand> basketItems)
    {
        var categories = new Dictionary<int, Model.Domain.Category>();

        foreach (var categoryId in basketItems.Select(i => i.CategoryId).Distinct())
        {
            var category = await categoryRepository.Get(categoryId);

            if (category is null)
            {
                throw new EntityNotFoundException($"Category with ID {categoryId} does not exist");
            }

            categories.Add(categoryId, category);
        }

        foreach (var item in basketItems)
        {
            if (!categories[item.CategoryId].Products.Any(product => product.Id == item.ProductId))
            {
                throw new EntityNotFoundException(
                    $"Product with ID {item.ProductId} does not exist within category with ID {item.CategoryId}");
            }
        }

        return categories;
    }
}
