using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using BallastLane.UserApi.HttpTests;
using Microsoft.IdentityModel.Tokens;

namespace BallastLane.UserApi.HttpTests;

[Collection(UserApiCollection.Name)]
public sealed class AuthenticationBoundaryTests : IAsyncLifetime
{
    private const string Email = "boundary.user@example.test";
    private const string Password = "ValidPassw0rd!";

    private readonly UserApiFactory _factory;

    public AuthenticationBoundaryTests(UserApiFactory factory) => _factory = factory;

    private HttpClient Client { get; set; } = null!;

    public async Task InitializeAsync()
    {
        await TestDatabase.ResetAsync();
        Client = _factory.CreateClient();
        var registered = await HttpAssertions.RegisterAsync(Client, Email, Password);
        Assert.Equal(HttpStatusCode.Created, registered.StatusCode);
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task The_public_info_endpoint_answers_anonymously()
    {
        var response = await Client.GetAsync("/api/public/info");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal("ballastlane-user-api", body.GetProperty("service").GetString());
    }

    [Fact]
    public async Task Registration_and_login_answer_without_credentials()
    {
        var register = await HttpAssertions.RegisterAsync(Client, "another.user@example.test", Password);
        var login = await Client.PostAsJsonAsync("/api/users/login", new { email = Email, password = Password });

        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    public static TheoryData<string> InvalidCredentialCases => ["missing", "malformed", "bad-signature", "expired"];

    [Theory]
    [MemberData(nameof(InvalidCredentialCases))]
    public async Task The_current_user_endpoint_rejects_every_invalid_credential_with_401(string credentialCase)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");

        if (credentialCase != "missing")
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(credentialCase));
        }

        var response = await Client.SendAsync(request);

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task The_current_user_endpoint_returns_the_callers_own_identity_for_a_valid_token()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await LoginAsync());

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal(Email, body.GetProperty("email").GetString());
    }

    [Fact]
    public async Task A_token_for_another_account_cannot_change_whose_identity_is_returned()
    {
        await HttpAssertions.RegisterAsync(Client, "second.user@example.test", Password);
        var otherToken = await LoginAsync("second.user@example.test");

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/users/me?email={Email}&sub={Guid.NewGuid()}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", otherToken);

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal("second.user@example.test", body.GetProperty("email").GetString());
    }

    [Fact]
    public async Task A_route_without_an_explicit_authorization_attribute_is_denied_by_the_fallback_policy()
    {
        var response = await Client.GetAsync("/api/probe/undecorated");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
    }

    private async Task<string> LoginAsync(string? email = null)
    {
        var response = await Client.PostAsJsonAsync(
            "/api/users/login",
            new { email = email ?? Email, password = Password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        return body.GetProperty("accessToken").GetString()!;
    }

    private static string CreateToken(string credentialCase)
    {
        var signingKey = credentialCase == "bad-signature"
            ? "a-completely-different-signing-key-0123456789"
            : UserApiFactory.SigningKey;

        if (credentialCase == "malformed")
        {
            return "this-is-not-a-jwt";
        }

        var expires = credentialCase == "expired"
            ? DateTimeOffset.UtcNow.AddMinutes(-10)
            : DateTimeOffset.UtcNow.AddMinutes(30);

        var token = new JwtSecurityToken(
            issuer: "ballastlane-user-api",
            audience: "ballastlane-clients",
            claims: [new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString())],
            notBefore: expires.UtcDateTime.AddHours(-1),
            expires: expires.UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
