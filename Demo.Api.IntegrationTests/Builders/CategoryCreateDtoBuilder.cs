using Demo.Api.Dto;
using Demo.Model.Domain;
using Demo.Model.UnitTests;

namespace Demo.Api.IntegrationTests.Builders
{
    public class CategoryCreateDtoBuilder : Builder<CategoryCreateDto>
    {
        private CategoryCreateDtoBuilder(Category category) : base(new CategoryCreateDto
        {
            Name = category.Name
        })
        {
        }

        public static CategoryCreateDtoBuilder BuildFromCategory(Category category)
        {
            return new CategoryCreateDtoBuilder(category);
        }
    }
}
