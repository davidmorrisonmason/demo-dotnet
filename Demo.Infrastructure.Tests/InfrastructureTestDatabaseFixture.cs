using Demo.Infrastructure.Tests.Database;

namespace Demo.Infrastructure.Tests;

public class InfrastructureTestDatabaseFixture : BaseDatabaseFixture
{
    protected override string DatabaseFile => "InfrastructureTestDatabase.db";
}
