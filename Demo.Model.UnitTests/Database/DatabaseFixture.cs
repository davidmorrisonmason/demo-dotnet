using Demo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Model.UnitTests.Database;

public class DatabaseFixture
{
    private static readonly object LockObject = new();

    public DbContextOptions<ApplicationDbContext> DbContextOptions { get; }

    public DatabaseFixture()
    {
        DbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=..\\..\\..\\..\\Databases\\DemoTestDatabase.db")
            .Options;
    }

    public void ResetDatabase()
    {
        lock (LockObject)
        {
            using var context = new ApplicationDbContext(DbContextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
