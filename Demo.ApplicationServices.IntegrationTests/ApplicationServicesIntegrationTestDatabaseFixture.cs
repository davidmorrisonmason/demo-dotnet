using Demo.Infrastructure.Tests.Database;

namespace Demo.ApplicationServices.IntegrationTests;

public class ApplicationServicesIntegrationTestDatabaseFixture : BaseDatabaseFixture
{
    protected override string DatabaseFile => "ApplicationServicesIntegrationTestDatabase.db";
}
