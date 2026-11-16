using System.Net;
using System.Net.Http.Headers;
using BallastLane.TaskApi.HttpTests;

namespace BallastLane.TaskApi.HttpTests;

/// <summary>
/// Proves the access boundary of the Task API: every route is protected by default, and identity is
/// taken only from validated claims.
/// </summary>
[Collection(TaskApiCollection.Name)]
public sealed class AccessBoundaryTests : IAsyncLifetime
{
    private readonly TaskApiFactory _factory;

    public AccessBoundaryTests(TaskApiFactory factory) => _factory = factory;

    private HttpClient Client { get; set; } = null!;

    private Guid UserId { get; set; }

    public async Task InitializeAsync()
    {
        await TestDatabase.ResetAsync();
        Client = _factory.CreateClient();
        UserId = await TestDatabase.CreateUserAsync("boundary.owner@example.test", "unused-hash");
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task A_route_without_an_explicit_authorization_attribute_is_denied_by_the_fallback_policy()
    {
        var response = await Client.GetAsync("/api/probe/undecorated");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task A_protected_route_answers_200_with_a_valid_token()
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpAssertions.CreateTokenFor(UserId, "boundary.owner@example.test"));

        var response = await Client.GetAsync("/api/probe/protected");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public static TheoryData<string> InvalidCredentialCases => ["missing", "malformed", "bad-signature", "expired"];

    [Theory]
    [MemberData(nameof(InvalidCredentialCases))]
    public async Task A_protected_route_rejects_every_invalid_credential_with_401(string credentialCase)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/probe/protected");

        if (credentialCase != "missing")
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateInvalidToken(credentialCase));
        }

        var response = await Client.SendAsync(request);

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Identity_comes_from_the_token_rather_than_a_query_or_body_value()
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpAssertions.CreateTokenFor(UserId, "boundary.owner@example.test"));

        var response = await Client.GetAsync($"/api/probe/whoami?userId={Guid.NewGuid()}&ownerId={Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal(UserId.ToString(), body.GetProperty("userId").GetString());
        Assert.Equal("boundary.owner@example.test", body.GetProperty("email").GetString());
    }

    private static string CreateInvalidToken(string credentialCase)
    {
        if (credentialCase == "malformed")
        {
            return "this-is-not-a-jwt";
        }

        if (credentialCase == "expired")
        {
            return HttpAssertions.CreateTokenFor(Guid.NewGuid(), "boundary.owner@example.test", DateTimeOffset.UtcNow.AddMinutes(-10));
        }

        // A structurally valid token signed with a key the host does not trust.
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: "ballastlane-user-api",
            audience: "ballastlane-clients",
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes("a-completely-different-signing-key-0123456789")),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256));

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }
}
