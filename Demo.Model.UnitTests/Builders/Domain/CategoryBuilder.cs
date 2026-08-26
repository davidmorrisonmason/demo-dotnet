using Demo.Infrastructure.Data;
using Demo.Infrastructure.UnitTests.Builders;
using Demo.Model.Domain;

namespace Demo.Model.UnitTests.Builders.Domain
{
    public class CategoryBuilder : DomainObjectBuilder<Category>
    {
        public CategoryBuilder(BuilderFactory builderFactory, int databaseSeed, int propertySeed) : base(builderFactory, new Category(
            databaseSeed,
            $"Category {propertySeed}"))
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


        protected override void Persist(ApplicationDbContext applicationDbContext)
        {
            applicationDbContext.Categories.Add(Target);
            //applicationDbContext.SaveChanges();
            //Target.Products.ForEach(p =>
            //{
            //    p.CategoryId = Target.Id;
            //    applicationDbContext.Products.Add(p);
            //});
        }
    }
}
