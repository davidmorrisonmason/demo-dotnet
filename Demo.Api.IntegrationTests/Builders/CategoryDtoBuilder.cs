using Demo.Api.Dto;
using Demo.Model.Domain;
using Demo.Model.UnitTests.Builders;

namespace Demo.Api.IntegrationTests.Builders
{
    public class CategoryDtoBuilder : Builder<CategoryDto>
    {
        private CategoryDtoBuilder(Category category) : base(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Products = [.. category.Products
                .Select(p => new ProductDto()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                })],
            SubCategories = [.. category.SubCategories
                .Select(subCategory => new SubCategoryDto
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name
                })]
        })
        {
        }

        public static CategoryDtoBuilder BuildFromCategory(Category category)
        {
            return new CategoryDtoBuilder(category);
        }
    }
}
