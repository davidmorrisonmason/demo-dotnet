using Demo.Api.Dto;
using Demo.Model.Domain;
using Demo.Model.UnitTests;

namespace Demo.Api.IntegrationTests.Builders
{
    public class CategoryDtoBuilder : Builder<CategoryDto>
    {
        private CategoryDtoBuilder(Category category) : base(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            SubCategories = category.SubCategories
                .Select(subCategory => new SubCategoryDto
                {
                    Id = subCategory.Id,
                    Name = subCategory.Name
                })
                .ToList()
        })
        {
        }

        public static CategoryDtoBuilder BuildFromCategory(Category category)
        {
            return new CategoryDtoBuilder(category);
        }
    }
}
