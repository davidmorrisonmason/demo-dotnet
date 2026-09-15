namespace Demo.Model.UnitTests.Database;

[CollectionDefinition(Name, DisableParallelization = true)]
public class ModelTestsDatabaseTestCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "ModelTestDatabaseTest";
}
