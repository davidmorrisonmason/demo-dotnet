namespace Demo.Model.UnitTests.Database;

[CollectionDefinition(Name, DisableParallelization = true)]
public class DatabaseTestCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "DatabaseTest";
}
