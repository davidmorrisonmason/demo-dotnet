using Demo.Infrastructure.Data;
using Demo.Infrastructure.UnitTests.Builders;
using Demo.Model.Domain;

namespace Demo.Model.UnitTests.Builders.Domain
{
    public class ProductBuilder : DomainObjectBuilder<Product>
    {
        public ProductBuilder(BuilderFactory builderFactory, int categoryId, int databaseSeed, int propertySeed) : base(builderFactory, new Product(
            databaseSeed,
            categoryId,
            $"Product {propertySeed}",
            propertySeed * 10))
        {
        }

        protected override void Persist(ApplicationDbContext applicationDbContext)
        {
            applicationDbContext.Products.Add(Target);
        }
    }
}
