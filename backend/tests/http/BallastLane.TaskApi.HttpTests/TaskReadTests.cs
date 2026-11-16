using System.Net;
using System.Net.Http.Headers;
using BallastLane.TaskApi.HttpTests;

namespace BallastLane.TaskApi.HttpTests;

[Collection(TaskApiCollection.Name)]
public sealed class TaskReadTests : IAsyncLifetime
{
    private readonly TaskApiFactory _factory;

    public TaskReadTests(TaskApiFactory factory) => _factory = factory;

    private HttpClient Client { get; set; } = null!;

    private Guid OwnerId { get; set; }

    private Guid OtherId { get; set; }

    public async Task InitializeAsync()
    {
        await TestDatabase.ResetAsync();
        Client = _factory.CreateClient();
        OwnerId = await TestDatabase.CreateUserAsync("read.owner@example.test", "unused-hash");
        OtherId = await TestDatabase.CreateUserAsync("read.other@example.test", "unused-hash");
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Listing_returns_only_the_callers_tasks()
    {
        await InsertTaskAsync(OwnerId, "Mine", "2026-11-20");
        await InsertTaskAsync(OtherId, "Not mine", "2026-11-19");
        Authenticate(OwnerId, "read.owner@example.test");

        var response = await Client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        var titles = body.EnumerateArray()
            .Select(item => item.GetProperty("title").GetString() ?? string.Empty)
            .ToArray();
        Assert.Equal(["Mine"], titles);
    }

    [Fact]
    public async Task Listing_a_fresh_account_returns_200_with_an_empty_array()
    {
        var emptyId = await TestDatabase.CreateUserAsync("read.empty@example.test", "unused-hash");
        Authenticate(emptyId, "read.empty@example.test");

        var response = await Client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal(0, body.GetArrayLength());
    }

    [Fact]
    public async Task Listing_without_a_token_returns_401()
    {
        var response = await Client.GetAsync("/api/tasks");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Getting_an_owned_task_returns_its_full_representation()
    {
        var taskId = await InsertTaskAsync(OwnerId, "Mine", "2026-11-20", description: "Cover traceability", status: "InProgress");
        Authenticate(OwnerId, "read.owner@example.test");

        var response = await Client.GetAsync($"/api/tasks/{taskId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal(taskId.ToString(), body.GetProperty("id").GetString());
        Assert.Equal("Mine", body.GetProperty("title").GetString());
        Assert.Equal("Cover traceability", body.GetProperty("description").GetString());
        Assert.Equal("InProgress", body.GetProperty("status").GetString());
        Assert.Equal("2026-11-20", body.GetProperty("dueDate").GetString());
        Assert.False(body.TryGetProperty("ownerId", out _));
    }

    [Fact]
    public async Task Getting_another_users_task_returns_404_and_no_data()
    {
        var taskId = await InsertTaskAsync(OtherId, "Not mine", "2026-11-20");
        Authenticate(OwnerId, "read.owner@example.test");

        var response = await Client.GetAsync($"/api/tasks/{taskId}");

        var body = await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.NotFound);
        Assert.DoesNotContain("Not mine", body, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("3f1e0000-0000-4000-8000-000000000000")]
    [InlineData("not-a-uuid")]
    public async Task Getting_an_unknown_or_non_uuid_identifier_returns_404(string identifier)
    {
        Authenticate(OwnerId, "read.owner@example.test");

        var response = await Client.GetAsync($"/api/tasks/{identifier}");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Getting_a_task_without_a_token_returns_401()
    {
        var taskId = await InsertTaskAsync(OwnerId, "Mine", "2026-11-20");

        var response = await Client.GetAsync($"/api/tasks/{taskId}");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
    }

    private void Authenticate(Guid userId, string email) =>
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpAssertions.CreateTokenFor(userId, email));

    private static async Task<Guid> InsertTaskAsync(
        Guid ownerId,
        string title,
        string dueDate,
        string? description = null,
        string status = "Pending")
    {
        var id = Guid.NewGuid();
        var descriptionValue = description is null ? "NULL" : $"'{description}'";
        await TestDatabase.ExecuteAsync(
            $"""
             INSERT INTO tasks (id, owner_id, title, description, status, due_date)
             VALUES ('{id}', '{ownerId}', '{title}', {descriptionValue}, '{status}', DATE '{dueDate}')
             """);
        return id;
    }
}
