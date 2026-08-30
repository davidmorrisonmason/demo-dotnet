using Demo.Infrastructure.Data;
using Demo.Infrastructure.UnitTests.Builders;
using Demo.Model.Domain.Checkout;

namespace Demo.Model.UnitTests.Builders.Domain;

public class BasketBuilder : DomainObjectBuilder<Basket>
{
    public BasketBuilder(BuilderFactory builderFactory, int databaseSeed, int propertySeed) : base(builderFactory, new Basket(databaseSeed))
    {
    }

    public BasketBuilder WithBasketItems(IEnumerable<BasketItem> basketItems)
    {
        Target.BasketItems.Clear();

        foreach (var basketItem in basketItems)
        {
            basketItem.BasketId = Target.Id;
            Target.BasketItems.Add(basketItem);
        }

        return this;
    }

    protected override void Persist(ApplicationDbContext applicationDbContext)
    {
        applicationDbContext.Baskets.Add(Target);
    }
}
