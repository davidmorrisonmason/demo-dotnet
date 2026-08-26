using Demo.Infrastructure.Data;
using Demo.Model.UnitTests.Model;
using Microsoft.EntityFrameworkCore;

namespace Demo.Model.UnitTests.Database
{
    public class DatabaseTest : ModelTest
    {
        protected DbContextOptions<ApplicationDbContext> DbContextOptions { get; private set; }

        public DatabaseTest(DatabaseFixture databaseFixture)
        {
            databaseFixture.ResetDatabase();

            DbContextOptions = databaseFixture.DbContextOptions;
            SetUpDatabaseContext(DbContextOptions);
        }
    }
}
