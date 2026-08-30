using Demo.Infrastructure.UnitTests.Builders;
using Demo.Model.Domain.Checkout;

namespace Demo.Model.UnitTests.Builders.Domain;

public class BasketItemBuilder : DomainObjectBuilder<BasketItem>
{
    public BasketItemBuilder(
        BuilderFactory builderFactory,
        int basketId,
        int productId,
        int quantity,
        int databaseSeed) : base(builderFactory, new BasketItem(databaseSeed, basketId, productId, quantity))
    {
    }
}
