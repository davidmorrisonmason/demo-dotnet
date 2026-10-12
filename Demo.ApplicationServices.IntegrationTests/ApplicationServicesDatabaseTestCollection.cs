namespace Demo.ApplicationServices.IntegrationTests;

[CollectionDefinition(Name, DisableParallelization = true)]
public class ApplicationServicesDatabaseTestCollection : ICollectionFixture<ApplicationServicesIntegrationTestDatabaseFixture>
{
    public const string Name = "ApplicationServicesIntegrationTestDatabaseTest";
}
