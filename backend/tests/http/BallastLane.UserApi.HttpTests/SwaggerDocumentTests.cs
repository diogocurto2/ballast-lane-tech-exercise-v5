using System.Net;
using System.Text.Json;
using BallastLane.UserApi.HttpTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BallastLane.UserApi.HttpTests;

/// <summary>
/// Proves the OpenAPI document is actually served by the running pipeline, not only that the
/// decision helper says so.
/// </summary>
[Collection(UserApiCollection.Name)]
public sealed class SwaggerDocumentTests
{
    private const string DocumentPath = "/swagger/v1/swagger.json";

    private readonly UserApiFactory _factory;

    public SwaggerDocumentTests(UserApiFactory factory) => _factory = factory;

    [Fact]
    public async Task The_development_host_serves_the_OpenAPI_document()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(DocumentPath);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await DescribesLoginAsync(response));
    }

    [Fact]
    public async Task A_shipped_host_serves_the_document_when_it_opts_in()
    {
        using var factory = new OptedInProductionFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(DocumentPath);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await DescribesLoginAsync(response));
    }

    private static async Task<bool> DescribesLoginAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        return document.RootElement.GetProperty("paths").TryGetProperty("/api/users/login", out _);
    }

    /// <summary>
    /// The container demonstration stack: a shipped environment that sets <c>Swagger__Enabled</c>
    /// explicitly, so the document is never served by default.
    /// </summary>
    private sealed class OptedInProductionFactory : WebApplicationFactory<UserApiEntryPoint>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(Environments.Production);
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = TestDatabase.ConnectionString,
                    ["Jwt:SigningKey"] = UserApiFactory.SigningKey,
                    ["FrontendOrigin"] = "http://localhost:5173",
                    ["Swagger:Enabled"] = "true",
                }));
        }
    }
}
