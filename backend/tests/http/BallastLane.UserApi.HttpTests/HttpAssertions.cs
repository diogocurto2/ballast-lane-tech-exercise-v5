using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace BallastLane.UserApi.HttpTests;

internal static class HttpAssertions
{
    internal static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        return document.RootElement.Clone();
    }

    internal static Task<HttpResponseMessage> PostJsonAsync<T>(HttpClient client, string path, T payload) =>
        client.PostAsJsonAsync(path, payload);

    internal static async Task<HttpResponseMessage> RegisterAsync(
        HttpClient client,
        string email,
        string password) =>
        await client.PostAsJsonAsync("/api/users/register", new { email, password });

    internal static void AssertNoCredentialMaterial(JsonElement body)
    {
        var text = body.GetRawText();
        Assert.DoesNotContain("password", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("hash", text, StringComparison.OrdinalIgnoreCase);
    }

    internal static void AssertNoInternalDetail(string body)
    {
        Assert.DoesNotContain("   at ", body, StringComparison.Ordinal);
        Assert.DoesNotContain("Npgsql", body, StringComparison.Ordinal);
        Assert.DoesNotContain("PostgresException", body, StringComparison.Ordinal);
        Assert.DoesNotContain("SELECT", body, StringComparison.Ordinal);
    }

    internal static async Task<string> AssertProblemAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        AssertNoInternalDetail(body);
        return body;
    }

    /// <summary>
    /// Describes a problem body by the fields that carry meaning for a caller, leaving out the
    /// per-request correlation identifier.
    /// </summary>
    internal static string DescribeProblem(string problemBody)
    {
        using var document = JsonDocument.Parse(problemBody);
        var root = document.RootElement;

        var description = new System.Text.StringBuilder();
        foreach (var property in new[] { "type", "title", "status", "detail" })
        {
            description.Append(property)
                .Append('=')
                .Append(root.TryGetProperty(property, out var value) ? value.ToString() : "<absent>")
                .Append(';');
        }

        description.Append("errors=")
            .Append(root.TryGetProperty("errors", out var errors) ? errors.ToString() : "<absent>");

        return description.ToString();
    }
}
