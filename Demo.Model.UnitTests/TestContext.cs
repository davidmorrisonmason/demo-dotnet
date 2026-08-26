using Demo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Demo.Model.UnitTests;

public class TestContext
{
    private static DbContextOptions<ApplicationDbContext>? _dbContextOptions;

    public static void SetupDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public static ApplicationDbContext DbContext => new ApplicationDbContext(_dbContextOptions);
}
