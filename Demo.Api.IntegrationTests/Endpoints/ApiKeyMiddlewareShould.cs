using Demo.Model.UnitTests.Database;
using System.Net;

namespace Demo.Api.IntegrationTest.Endpoints;

[Collection(DatabaseTestCollection.Name)]
public class ApiKeyMiddlewareShould : DemoApiIntegrationTest
{
    public ApiKeyMiddlewareShould(DatabaseFixture databaseFixture) : base(databaseFixture)
    {
    }

    [Fact]
    public async Task ReturnUnauthorized_WhenApiKeyHeaderIsMissing()
    {
        // Arrange
        Client.DefaultRequestHeaders.Remove("x-api-key");

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/Categories");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ReturnUnauthorized_WhenApiKeyDoesNotIdentifyAClient()
    {
        // Arrange
        Client.DefaultRequestHeaders.Remove("x-api-key");
        Client.DefaultRequestHeaders.Add("x-api-key", "unknown-api-key");

        // Act
        var response = await Client.GetAsync($"{BaseUrl}/Categories");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
