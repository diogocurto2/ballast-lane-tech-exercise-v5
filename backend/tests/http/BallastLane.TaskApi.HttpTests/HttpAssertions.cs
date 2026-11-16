using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace BallastLane.TaskApi.HttpTests;

internal static class HttpAssertions
{
    internal static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        return document.RootElement.Clone();
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
    /// Mints a token with the same issuer, audience, and signing key the hosts validate, so a test can
    /// act as a specific account without going through the User API.
    /// </summary>
    internal static string CreateTokenFor(Guid userId, string email, DateTimeOffset? expiresAt = null)
    {
        var expires = expiresAt ?? DateTimeOffset.UtcNow.AddMinutes(30);

        var token = new JwtSecurityToken(
            issuer: "ballastlane-user-api",
            audience: "ballastlane-clients",
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
            ],
            notBefore: expires.UtcDateTime.AddHours(-1),
            expires: expires.UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TaskApiFactory.SigningKey)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    internal static HttpClient WithToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
