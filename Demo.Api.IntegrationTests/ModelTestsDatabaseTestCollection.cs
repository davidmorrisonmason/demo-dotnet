using Demo.Infrastructure.Tests.Database;

namespace Demo.Api.Integration.Tests.Database;

[CollectionDefinition(Name, DisableParallelization = true)]
public class ApiIntegrationTestDatabaseTestCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "ApiIntegrationTestDatabaseTest";
}
