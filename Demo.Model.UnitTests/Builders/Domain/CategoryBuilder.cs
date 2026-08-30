using Demo.Infrastructure.Data;
using Demo.Infrastructure.UnitTests.Builders;
using Demo.Model.Domain;

namespace Demo.Model.UnitTests.Builders.Domain
{
    public class CategoryBuilder : DomainObjectBuilder<Category>
    {
        public CategoryBuilder(BuilderFactory builderFactory, int databaseSeed, int propertySeed) : base(builderFactory, new Category(
            databaseSeed,
            $"Category {propertySeed}",
            null))
        {
        }

        public CategoryBuilder WithProducts(IEnumerable<Product> products)
        {
            Target.Products.Clear();

            foreach (Product product in products)
            {
                product.CategoryId = Target.Id;
                Target.Products.Add(product);
            }

            return this;
        }

        public CategoryBuilder WithSubCategories(IEnumerable<Category> subCategories)
        {
            Target.SubCategories.Clear();

            foreach (Category category in subCategories)
            {
                category.ParentCategoryId = Target.Id;
                Target.SubCategories.Add(category);
            }

            return this;
        }


        protected override void Persist(ApplicationDbContext applicationDbContext)
        {
            applicationDbContext.Categories.Add(Target);
        }
    }
}
