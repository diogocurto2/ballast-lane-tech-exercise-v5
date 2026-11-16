using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace BallastLane.TaskApi.HttpTests;

[Collection(TaskApiCollection.Name)]
public sealed class TaskUpdateTests : IAsyncLifetime
{
    private readonly TaskApiFactory _factory;

    public TaskUpdateTests(TaskApiFactory factory) => _factory = factory;

    private HttpClient Client { get; set; } = null!;

    private Guid OwnerId { get; set; }

    public async Task InitializeAsync()
    {
        await TestDatabase.ResetAsync();
        Client = _factory.CreateClient();
        OwnerId = await TestDatabase.CreateUserAsync("update.owner@example.test", "unused-hash");
        Authenticate(OwnerId, "update.owner@example.test");
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    public static TheoryData<string, string> StatusTransitions => new()
    {
        { "Pending", "InProgress" },
        { "InProgress", "Pending" },
        { "InProgress", "Completed" },
        { "Completed", "InProgress" },
        { "Pending", "Completed" },
        { "Completed", "Pending" },
    };

    [Theory]
    [MemberData(nameof(StatusTransitions))]
    public async Task Every_status_transition_returns_200_with_the_updated_representation(string from, string to)
    {
        var id = await InsertTaskAsync(OwnerId, "Write the specification", from, "2026-11-20");

        var response = await PutAsync(id, new { title = "Write the specification", status = to, dueDate = "2026-11-20" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(response);
        Assert.Equal(to, body.GetProperty("status").GetString());
        Assert.Equal(id.ToString(), body.GetProperty("id").GetString());
    }

    [Theory]
    [InlineData("", "status")]
    [InlineData("   ", "status")]
    [InlineData("Title", null)]
    public async Task A_blank_title_or_missing_status_returns_400_and_leaves_the_task_unchanged(
        string title,
        string? status)
    {
        var id = await InsertTaskAsync(OwnerId, "Original title", "Pending", "2026-11-20");

        var response = await PutAsync(id, new { title, status, dueDate = "2026-11-20" });

        var body = await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.BadRequest);
        if (status is null)
        {
            using var document = JsonDocument.Parse(body);
            Assert.True(document.RootElement.GetProperty("errors").TryGetProperty("status", out _));
        }

        Assert.Equal("Original title", await TaskTitleAsync(id));
        Assert.Equal("Pending", await TaskStatusAsync(id));
    }

    [Theory]
    [InlineData("Done")]
    [InlineData("completed")]
    public async Task An_unknown_status_returns_400(string status)
    {
        var id = await InsertTaskAsync(OwnerId, "Original title", "Pending", "2026-11-20");

        var response = await PutAsync(id, new { title = "Original title", status, dueDate = "2026-11-20" });

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.BadRequest);
        Assert.Equal("Pending", await TaskStatusAsync(id));
    }

    [Fact]
    public async Task A_due_date_with_a_time_component_returns_400_and_nothing_changes()
    {
        var id = await InsertTaskAsync(OwnerId, "Original title", "Pending", "2026-11-20");

        var response = await PutAsync(id, new { title = "Changed", status = "Pending", dueDate = "2026-11-20T10:00:00Z" });

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.BadRequest);
        Assert.Equal("Original title", await TaskTitleAsync(id));
    }

    [Fact]
    public async Task Another_users_task_returns_404_and_is_unchanged()
    {
        var otherId = await TestDatabase.CreateUserAsync("update.other@example.test", "unused-hash");
        var otherTask = await InsertTaskAsync(otherId, "Not mine", "Pending", "2026-11-20");

        var response = await PutAsync(otherTask, new { title = "Hijacked", status = "Completed", dueDate = "2027-01-01" });

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.NotFound);
        Assert.Equal("Not mine", await TaskTitleAsync(otherTask));
        Assert.Equal("Pending", await TaskStatusAsync(otherTask));
    }

    [Fact]
    public async Task An_unknown_identifier_returns_404()
    {
        var response = await PutAsync(Guid.NewGuid(), new { title = "Title", status = "Pending", dueDate = "2026-11-20" });

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Updating_without_a_token_returns_401_and_changes_nothing()
    {
        var id = await InsertTaskAsync(OwnerId, "Original title", "Pending", "2026-11-20");
        Client.DefaultRequestHeaders.Authorization = null;

        var response = await PutAsync(id, new { title = "Changed", status = "Completed", dueDate = "2026-11-20" });

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
        Assert.Equal("Original title", await TaskTitleAsync(id));
    }

    [Fact]
    public async Task An_update_never_changes_the_owner()
    {
        var otherId = await TestDatabase.CreateUserAsync("update.other2@example.test", "unused-hash");
        var id = await InsertTaskAsync(OwnerId, "Original title", "Pending", "2026-11-20");

        var response = await Client.PutAsJsonAsync(
            $"/api/tasks/{id}",
            new { title = "Changed", status = "Pending", dueDate = "2026-11-20", ownerId = otherId });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(OwnerId.ToString(), await TestDatabase.ScalarAsync($"SELECT owner_id FROM tasks WHERE id = '{id}'"));
    }

    private void Authenticate(Guid userId, string email) =>
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpAssertions.CreateTokenFor(userId, email));

    private Task<HttpResponseMessage> PutAsync(Guid id, object payload) =>
        Client.PutAsJsonAsync($"/api/tasks/{id}", payload);

    private static async Task<Guid> InsertTaskAsync(Guid ownerId, string title, string status, string dueDate)
    {
        var id = Guid.NewGuid();
        await TestDatabase.ExecuteAsync(
            $"""
             INSERT INTO tasks (id, owner_id, title, status, due_date)
             VALUES ('{id}', '{ownerId}', '{title}', '{status}', DATE '{dueDate}')
             """);
        return id;
    }

    private static Task<string> TaskTitleAsync(Guid id) =>
        TestDatabase.ScalarAsync($"SELECT title FROM tasks WHERE id = '{id}'");

    private static Task<string> TaskStatusAsync(Guid id) =>
        TestDatabase.ScalarAsync($"SELECT status FROM tasks WHERE id = '{id}'");
}
