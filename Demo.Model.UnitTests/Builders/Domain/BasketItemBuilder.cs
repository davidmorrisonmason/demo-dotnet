using Demo.Model.Domain;
using Demo.Model.Domain.Checkout;

namespace Demo.Model.UnitTests.Builders.Domain;

public class BasketItemBuilder : DomainObjectBuilder<BasketItem>
{
    public BasketItemBuilder(
        BuilderFactory builderFactory,
        int databaseSeed = 0,
        int propertySeed = 1) : base(builderFactory, new BasketItem(databaseSeed, propertySeed, propertySeed, propertySeed))
    {
    }

    public BasketItemBuilder WithProduct(Product product)
    {
        With(x => x.Product, product);
        With(x => x.ProductId, product.Id);
        return this;
    }
}
