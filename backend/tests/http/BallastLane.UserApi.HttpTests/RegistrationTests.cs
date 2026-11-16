using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BallastLane.UserApi.HttpTests;

namespace BallastLane.UserApi.HttpTests;

[Collection(UserApiCollection.Name)]
public sealed class RegistrationTests : IAsyncLifetime
{
    private readonly UserApiFactory _factory;

    public RegistrationTests(UserApiFactory factory) => _factory = factory;

    private HttpClient Client { get; set; } = null!;

    public async Task InitializeAsync()
    {
        await TestDatabase.ResetAsync();
        Client = _factory.CreateClient();
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Registering_a_new_account_returns_201_without_any_password_or_hash()
    {
        var response = await HttpAssertions.RegisterAsync(Client, "new.user@example.test", "ValidPassw0rd!");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal("new.user@example.test", body.GetProperty("email").GetString());
        Assert.NotEqual(Guid.Empty, body.GetProperty("id").GetGuid());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("createdAt").GetString()));
        HttpAssertions.AssertNoCredentialMaterial(body);
    }

    [Fact]
    public async Task Registering_a_duplicate_email_differing_only_in_case_returns_409()
    {
        var first = await HttpAssertions.RegisterAsync(Client, "Case.User@Example.Test", "ValidPassw0rd!");
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await HttpAssertions.RegisterAsync(Client, "case.user@example.test", "AnotherPassw0rd!");

        await HttpAssertions.AssertProblemAsync(second, HttpStatusCode.Conflict);
        var remaining = await TestDatabase.ScalarAsync("SELECT count(*) FROM users");
        Assert.Equal("1", remaining);
    }

    [Theory]
    [InlineData("not-an-email", "ValidPassw0rd!", "email")]
    [InlineData("", "ValidPassw0rd!", "email")]
    [InlineData("missing.password@example.test", "", "password")]
    [InlineData("short.password@example.test", "short12", "password")]
    public async Task Registering_invalid_input_returns_400_with_a_field_error(
        string email,
        string password,
        string expectedField)
    {
        var response = await HttpAssertions.RegisterAsync(Client, email, password);

        var body = await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.BadRequest);
        using var document = JsonDocument.Parse(body);
        var errors = document.RootElement.GetProperty("errors");
        Assert.True(errors.TryGetProperty(expectedField, out _), $"expected an error for '{expectedField}'");
        Assert.Equal("0", await TestDatabase.ScalarAsync("SELECT count(*) FROM users"));
    }

    [Fact]
    public async Task Registration_ignores_an_owner_or_role_field_supplied_by_the_client()
    {
        var response = await Client.PostAsJsonAsync(
            "/api/users/register",
            new { email = "tamper@example.test", password = "ValidPassw0rd!", role = "admin" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.False(body.TryGetProperty("role", out _));
    }
}
