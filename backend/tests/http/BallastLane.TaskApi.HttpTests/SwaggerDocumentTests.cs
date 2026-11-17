using System.Net;
using System.Text.Json;
using BallastLane.TaskApi.HttpTests;

namespace BallastLane.TaskApi.HttpTests;

/// <summary>
/// Proves the OpenAPI document is actually served by the running pipeline, not only that the
/// decision helper says so.
/// </summary>
[Collection(TaskApiCollection.Name)]
public sealed class SwaggerDocumentTests
{
    private readonly TaskApiFactory _factory;

    public SwaggerDocumentTests(TaskApiFactory factory) => _factory = factory;

    [Fact]
    public async Task The_development_host_serves_the_OpenAPI_document()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(document.RootElement.GetProperty("paths").TryGetProperty("/api/tasks", out _));
    }
}
