using Demo.Model.UnitTests.Database;

namespace Demo.Api.IntegrationTest;

// xUnit discovers collection definitions by reflection within each test assembly.
[CollectionDefinition(DatabaseTestCollection.Name, DisableParallelization = true)]
public class ApiDatabaseTestCollectionDefinition : ICollectionFixture<DatabaseFixture>
{
}
