using Demo.Infrastructure.Data;
using Demo.Model.Domain.Checkout;

namespace Demo.Model.UnitTests.Builders.Domain;

public class CheckoutCompletionBuilder : DomainObjectBuilder<CheckoutCompletion>
{
    public CheckoutCompletionBuilder(
        BuilderFactory builderFactory,
        int databaseSeed,
        int propertySeed) : base(builderFactory, new CheckoutCompletion(
            databaseSeed,
            propertySeed,
            $"Recipient {propertySeed}",
            $"Address line 1 {propertySeed}",
            $"Address line 2 {propertySeed}",
            $"Address line 3 {propertySeed}",
            $"Address line 4 {propertySeed}",
            $"City {propertySeed}",
            $"Post code {propertySeed}"))
    {
    }

    public CheckoutCompletionBuilder WithBasket(Basket basket)
    {
        With(x => x.Basket, basket);
        With(x => x.BasketId, basket.Id);
        return this;
    }

    protected override void Persist(ApplicationDbContext applicationDbContext)
    {
        applicationDbContext.CheckoutCompletions.Add(Build());
    }
}
