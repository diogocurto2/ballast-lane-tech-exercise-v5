using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace BallastLane.UserApi.HttpTests;

[Collection(UserApiCollection.Name)]
public sealed class LoginTests : IAsyncLifetime
{
    private const string Email = "login.user@example.test";
    private const string Password = "ValidPassw0rd!";

    private readonly UserApiFactory _factory;

    public LoginTests(UserApiFactory factory) => _factory = factory;

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
    public async Task Valid_credentials_return_a_token_payload()
    {
        var response = await Client.PostAsJsonAsync("/api/users/login", new { email = Email, password = Password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("accessToken").GetString()));
        Assert.Equal("Bearer", body.GetProperty("tokenType").GetString());
        Assert.True(DateTimeOffset.TryParse(body.GetProperty("expiresAtUtc").GetString(), out var expiresAt));
        Assert.True(expiresAt > DateTimeOffset.UtcNow);
        HttpAssertions.AssertNoCredentialMaterial(body);
    }

    [Fact]
    public async Task A_wrong_password_and_an_unknown_email_return_an_identical_401_body()
    {
        var wrongPassword = await Client.PostAsJsonAsync("/api/users/login", new { email = Email, password = "WrongPassw0rd!" });
        var unknownEmail = await Client.PostAsJsonAsync(
            "/api/users/login",
            new { email = "nobody@example.test", password = Password });

        var wrongPasswordBody = await HttpAssertions.AssertProblemAsync(wrongPassword, HttpStatusCode.Unauthorized);
        var unknownEmailBody = await HttpAssertions.AssertProblemAsync(unknownEmail, HttpStatusCode.Unauthorized);

        // The correlation identifier is expected to differ per request, so the comparison covers the
        // parts a caller could use to tell the two causes apart.
        Assert.Equal(
            HttpAssertions.DescribeProblem(wrongPasswordBody),
            HttpAssertions.DescribeProblem(unknownEmailBody));
        Assert.DoesNotContain("email exists", wrongPasswordBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("not found", wrongPasswordBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("errors", wrongPasswordBody, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("", "ValidPassw0rd!")]
    [InlineData(Email, "")]
    public async Task A_missing_field_returns_400(string email, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/users/login", new { email, password });

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task The_current_user_endpoint_returns_the_callers_identity()
    {
        var token = await LoginAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal(Email, body.GetProperty("email").GetString());
        HttpAssertions.AssertNoCredentialMaterial(body);
    }

    [Fact]
    public async Task The_current_user_endpoint_returns_401_without_a_token()
    {
        var response = await Client.GetAsync("/api/users/me");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
    }

    private async Task<string> LoginAsync()
    {
        var response = await Client.PostAsJsonAsync("/api/users/login", new { email = Email, password = Password });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        return body.GetProperty("accessToken").GetString()!;
    }
}
