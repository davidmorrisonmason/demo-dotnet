using Demo.Model.UnitTests.Database;

namespace Demo.Api.IntegrationTests;

public class ApiIntegrationTestDatabaseFixture : BaseDatabaseFixture
{
    protected override string DatabaseFile => "ApiIntegrationTestDatabase.db";
}
