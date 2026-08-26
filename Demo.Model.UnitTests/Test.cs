using Demo.Infrastructure.Data;
using Demo.Infrastructure.UnitTests.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demo.Model.UnitTests
{
    public class Test
    {
        protected BuilderFactory BuilderFactory { get; private set; }

        public Test()
        {
            BuilderFactory = new BuilderFactory();
        }

        protected void SetUpDatabaseContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            BuilderFactory.SetupDbContext(dbContextOptions);
            TestContext.SetupDbContext(dbContextOptions);
        }
    }
}
