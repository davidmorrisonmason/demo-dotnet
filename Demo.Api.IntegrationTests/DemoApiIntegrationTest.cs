using Demo.Infrastructure.Data;
using Demo.Model.Domain;
using Demo.Model.UnitTests;
using Demo.Model.UnitTests.Builders.Domain;
using Demo.Model.UnitTests.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Demo.Api.IntegrationTest;

public class DemoApiIntegrationTest : DatabaseTest, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;
    protected const string BaseUrl = "/api";

    protected HttpClient Client => _httpClient;

    public DemoApiIntegrationTest(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("ApiIntegrationTests");
            });
        _httpClient = _factory.CreateClient();

        var client = BuilderFactory.NewClientBuilder()
            .With(x => x.ApiKey, TestApiKeyHashedText)
            .BuildAndPersist();

        _httpClient.DefaultRequestHeaders.Add("x-api-key", TestApiKeyPlainText);
    }

    protected void AssertCreatedResponse<T>(
        HttpResponseMessage response,
        List<T> expectedExistingEntities,
        DomainObjectBuilder<T> expectedBuilder) where T : DomainObject
    {
        var id = response.ShouldBeCreatedResponse();
        expectedExistingEntities.Add(expectedBuilder
            .With(x => x.Id, id)
            .Build());
        AssertCollection(expectedExistingEntities);
    }

    protected void AssertUpdatedResponse<T>(
        HttpResponseMessage response,
        List<T> expectedEntities) where T : DomainObject
    {
        response.ShouldBeNoContentResponse();
        AssertCollection(expectedEntities);
    }

    protected void AssertCollection<T>(
        List<T> expectedEntities) where T : DomainObject
    {
        using var dbContext = new ApplicationDbContext(DbContextOptions);
        var actual = dbContext.Set<T>().ToList();
        actual.ShouldBeEquivalentTo(expectedEntities);
    }
}
