using Demo.Api.Integration.Tests.Database;
using Demo.Api.IntegrationTests;

namespace Demo.Api.IntegrationTest;

// xUnit discovers collection definitions by reflection within each test assembly.
[CollectionDefinition(ApiIntegrationTestDatabaseTestCollection.Name, DisableParallelization = true)]
public class ApiDatabaseTestCollectionDefinition : ICollectionFixture<ApiIntegrationTestDatabaseFixture>
{
}
