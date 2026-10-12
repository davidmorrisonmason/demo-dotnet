namespace Demo.Api.IntegrationTests;

// xUnit discovers collection definitions by reflection within each test assembly.
[CollectionDefinition(Name, DisableParallelization = true)]
public class ApiDatabaseTestCollectionDefinition : ICollectionFixture<ApiIntegrationTestDatabaseFixture>
{
    public const string Name = "ApiIntegrationTestDatabaseTest";
}
