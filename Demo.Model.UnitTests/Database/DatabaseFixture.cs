using Demo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Model.UnitTests.Database;

public abstract class BaseDatabaseFixture
{
    private static readonly object LockObject = new();
    private static readonly string DatabaseDirectory = "../../../../Databases";
    protected abstract string DatabaseFile { get; }
    public DbContextOptions<ApplicationDbContext> DbContextOptions { get; }

    public BaseDatabaseFixture()
    {
        DbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite($"Data Source={Path.Combine(DatabaseDirectory, DatabaseFile)}")
            .Options;
    }

    public void ResetDatabase()
    {
        lock (LockObject)
        {
            if (!File.Exists(Path.Combine(DatabaseDirectory, DatabaseFile)))
            {
                Directory.CreateDirectory(DatabaseDirectory);
                File.WriteAllText(Path.Combine(DatabaseDirectory, DatabaseFile), null);
            }

            using var context = new ApplicationDbContext(DbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}

public class DatabaseFixture : BaseDatabaseFixture
{
    protected override string DatabaseFile => "ModelTestDatabase.db";
}
