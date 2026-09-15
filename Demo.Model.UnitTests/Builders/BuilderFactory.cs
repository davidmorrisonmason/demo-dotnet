using Demo.Infrastructure.Data;
using Demo.Model.Domain;
using Demo.Model.Domain.Checkout;
using Demo.Model.UnitTests.Builders.Domain;
using Microsoft.EntityFrameworkCore;

namespace Demo.Model.UnitTests.Builders
{
    public class BuilderFactory
    {
        private DbContextOptions<ApplicationDbContext>? _dbContextOptions;

        #region Constructors / Initialisation

        public BuilderFactory()
        {
        }

        public void SetupDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        #endregion

        public CategoryBuilder NewCategoryBuilder(int propertySeed = 1, int databaseSeed = 0)
        {
            return BuildWithDbContextOptions<CategoryBuilder, Category>(() => new CategoryBuilder(this, databaseSeed, propertySeed));
        }

        public ProductBuilder NewProductBuilder(int propertySeed = 1, int databaseSeed = 0)
        {
            return BuildWithDbContextOptions<ProductBuilder, Product>(() => new ProductBuilder(this, 0, databaseSeed, propertySeed));
        }

        public BasketBuilder NewBasketBuilder(int propertySeed = 1, int databaseSeed = 0)
        {
            return BuildWithDbContextOptions<BasketBuilder, Basket>(() => new BasketBuilder(this, databaseSeed, propertySeed));
        }

        public BasketItemBuilder NewBasketItemBuilder(int propertySeed = 1, int databaseSeed = 0)
        {
            return BuildWithDbContextOptions<BasketItemBuilder, BasketItem>(() => new BasketItemBuilder(this, databaseSeed, propertySeed));
        }

        public CheckoutCompletionBuilder NewCheckoutCompletionBuilder(int propertySeed = 1, int databaseSeed = 0)
        {
            return BuildWithDbContextOptions<CheckoutCompletionBuilder, CheckoutCompletion>(() => new CheckoutCompletionBuilder(this, databaseSeed, propertySeed));
        }

        public ClientBuilder NewClientBuilder(int propertySeed = 1, int databaseSeed = 0)
        {
            return BuildWithDbContextOptions<ClientBuilder, Client>(() => new ClientBuilder(this, databaseSeed, propertySeed));
        }

        private TBuilder BuildWithDbContextOptions<TBuilder, TObject>(Func<TBuilder> buildAction) where TBuilder : DomainObjectBuilder<TObject> where TObject : DomainObject
        {
            var builder = buildAction.Invoke();

            if (_dbContextOptions is not null)
            {
                builder.WithDbContextOptions(_dbContextOptions);
            }

            return builder;
        }
    }
}
