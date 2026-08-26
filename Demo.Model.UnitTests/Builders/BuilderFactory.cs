using Demo.Infrastructure.Data;
using Demo.Model.UnitTests.Builders.Domain;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.UnitTests.Builders
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
            return (CategoryBuilder)new CategoryBuilder(this, databaseSeed, propertySeed).WithDbContextOptions(_dbContextOptions);
        }
        public ProductBuilder NewProductBuilder(int propertySeed = 1, int databaseSeed = 0)
        {
            return (ProductBuilder)new ProductBuilder(this, 0, databaseSeed, propertySeed).WithDbContextOptions(_dbContextOptions);
        }
    }
}
