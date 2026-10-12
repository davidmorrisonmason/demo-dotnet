using Demo.Infrastructure.Tests.Database;

namespace Demo.ApplicationServices.IntegrationTests.Command
{
    public class CommandTest : DatabaseTest
    {
        public CommandTest(ApplicationServicesIntegrationTestDatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }
    }
}
