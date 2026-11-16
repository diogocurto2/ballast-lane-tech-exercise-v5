using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace BallastLane.TaskApi.HttpTests;

[Collection(TaskApiCollection.Name)]
public sealed class TaskCreateTests : IAsyncLifetime
{
    private readonly TaskApiFactory _factory;

    public TaskCreateTests(TaskApiFactory factory) => _factory = factory;

    private HttpClient Client { get; set; } = null!;

    private Guid OwnerId { get; set; }

    public async Task InitializeAsync()
    {
        await TestDatabase.ResetAsync();
        Client = _factory.CreateClient();
        OwnerId = await TestDatabase.CreateUserAsync("create.owner@example.test", "unused-hash");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpAssertions.CreateTokenFor(OwnerId, "create.owner@example.test"));
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Creating_without_a_status_returns_201_with_a_location_header_and_Pending()
    {
        var response = await PostAsync(new { title = "Write the specification", dueDate = "2026-11-20" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal("Pending", body.GetProperty("status").GetString());
        Assert.Equal($"/api/tasks/{body.GetProperty("id").GetString()}", response.Headers.Location?.ToString());
        Assert.Equal(1, await CountTasksAsync());
    }

    [Fact]
    public async Task Creating_with_a_status_honours_it()
    {
        var response = await PostAsync(new { title = "In progress", status = "InProgress", dueDate = "2026-11-20" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal("InProgress", body.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Creating_without_a_description_returns_null_for_it()
    {
        var response = await PostAsync(new { title = "No description", dueDate = "2026-11-20" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal(JsonValueKind.Null, body.GetProperty("description").ValueKind);
    }

    [Fact]
    public async Task Creating_with_a_past_due_date_is_accepted()
    {
        var response = await PostAsync(new { title = "Overdue obligation", dueDate = "2020-01-01" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal("2020-01-01", body.GetProperty("dueDate").GetString());
    }

    [Theory]
    [InlineData("", "title")]
    [InlineData("   ", "title")]
    [InlineData(null, "title")]
    public async Task Creating_with_a_blank_or_missing_title_returns_400(string? title, string expectedField)
    {
        var response = await PostAsync(new { title, dueDate = "2026-11-20" });

        var body = await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.BadRequest);
        using var document = JsonDocument.Parse(body);
        Assert.True(document.RootElement.GetProperty("errors").TryGetProperty(expectedField, out _));
        Assert.Equal(0, await CountTasksAsync());
    }

    [Theory]
    [InlineData("2026-11-20T10:00:00Z")]
    [InlineData("not-a-date")]
    public async Task Creating_with_a_non_calendar_due_date_returns_400(string dueDate)
    {
        var response = await PostAsync(new { title = "Title", dueDate });

        var body = await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.BadRequest);
        using var document = JsonDocument.Parse(body);
        Assert.True(document.RootElement.GetProperty("errors").TryGetProperty("dueDate", out _));
        Assert.Equal(0, await CountTasksAsync());
    }

    [Fact]
    public async Task Creating_with_an_unknown_status_returns_400()
    {
        var response = await PostAsync(new { title = "Title", status = "Done", dueDate = "2026-11-20" });

        var body = await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.BadRequest);
        using var document = JsonDocument.Parse(body);
        Assert.True(document.RootElement.GetProperty("errors").TryGetProperty("status", out _));
    }

    [Fact]
    public async Task A_supplied_owner_for_another_user_is_ignored()
    {
        var otherId = await TestDatabase.CreateUserAsync("create.other@example.test", "unused-hash");

        var response = await PostAsync(new
        {
            title = "Mine, not theirs",
            dueDate = "2026-11-20",
            ownerId = otherId,
            userId = otherId,
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        var createdId = body.GetProperty("id").GetString();

        Assert.Equal(
            OwnerId.ToString(),
            await TestDatabase.ScalarAsync($"SELECT owner_id FROM tasks WHERE id = '{createdId}'"));
    }

    [Fact]
    public async Task Creating_without_a_token_returns_401_and_persists_nothing()
    {
        Client.DefaultRequestHeaders.Authorization = null;

        var response = await PostAsync(new { title = "Unauthorized", dueDate = "2026-11-20" });

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
        Assert.Equal(0, await CountTasksAsync());
    }

    private Task<HttpResponseMessage> PostAsync(object payload) =>
        Client.PostAsJsonAsync("/api/tasks", payload);

    private static async Task<int> CountTasksAsync() =>
        int.Parse(await TestDatabase.ScalarAsync("SELECT count(*) FROM tasks"), System.Globalization.CultureInfo.InvariantCulture);
}
