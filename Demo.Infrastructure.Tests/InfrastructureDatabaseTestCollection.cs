namespace Demo.Infrastructure.Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
public class InfrastructureDatabaseTestCollection : ICollectionFixture<InfrastructureTestDatabaseFixture>
{
    public const string Name = "InfrastructureTestDatabaseTest";
}
