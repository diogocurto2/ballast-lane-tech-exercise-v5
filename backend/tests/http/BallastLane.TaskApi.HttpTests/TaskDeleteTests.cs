using System.Net;
using System.Net.Http.Headers;

namespace BallastLane.TaskApi.HttpTests;

[Collection(TaskApiCollection.Name)]
public sealed class TaskDeleteTests : IAsyncLifetime
{
    private readonly TaskApiFactory _factory;

    public TaskDeleteTests(TaskApiFactory factory) => _factory = factory;

    private HttpClient Client { get; set; } = null!;

    private Guid OwnerId { get; set; }

    public async Task InitializeAsync()
    {
        await TestDatabase.ResetAsync();
        Client = _factory.CreateClient();
        OwnerId = await TestDatabase.CreateUserAsync("delete.owner@example.test", "unused-hash");
        Authenticate(OwnerId, "delete.owner@example.test");
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Deleting_an_owned_task_returns_204_and_a_later_get_returns_404()
    {
        var id = await InsertTaskAsync(OwnerId, "Delete me");

        var response = await Client.DeleteAsync($"/api/tasks/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
        Assert.Equal("0", await TaskCountAsync(id));
        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync($"/api/tasks/{id}")).StatusCode);
    }

    [Fact]
    public async Task Deleting_the_same_task_twice_returns_404_the_second_time()
    {
        var id = await InsertTaskAsync(OwnerId, "Delete me once");

        Assert.Equal(HttpStatusCode.NoContent, (await Client.DeleteAsync($"/api/tasks/{id}")).StatusCode);
        await HttpAssertions.AssertProblemAsync(
            await Client.DeleteAsync($"/api/tasks/{id}"),
            HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Deleting_one_task_leaves_the_others_untouched()
    {
        var first = await InsertTaskAsync(OwnerId, "Keep me");
        var second = await InsertTaskAsync(OwnerId, "Delete me");

        await Client.DeleteAsync($"/api/tasks/{second}");

        Assert.Equal("1", await TaskCountAsync(first));
        Assert.Equal("Keep me", await TestDatabase.ScalarAsync($"SELECT title FROM tasks WHERE id = '{first}'"));
    }

    [Fact]
    public async Task Deleting_another_users_task_returns_404_and_the_task_still_exists()
    {
        var otherId = await TestDatabase.CreateUserAsync("delete.other@example.test", "unused-hash");
        var otherTask = await InsertTaskAsync(otherId, "Not mine");

        var response = await Client.DeleteAsync($"/api/tasks/{otherTask}");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.NotFound);
        Assert.Equal("1", await TaskCountAsync(otherTask));
    }

    [Theory]
    [InlineData("3f1e0000-0000-4000-8000-000000000000")]
    [InlineData("not-a-uuid")]
    public async Task Deleting_an_unknown_or_non_uuid_identifier_returns_404(string identifier)
    {
        var response = await Client.DeleteAsync($"/api/tasks/{identifier}");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Deleting_without_a_token_returns_401_and_deletes_nothing()
    {
        var id = await InsertTaskAsync(OwnerId, "Still here");
        Client.DefaultRequestHeaders.Authorization = null;

        var response = await Client.DeleteAsync($"/api/tasks/{id}");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.Unauthorized);
        Assert.Equal("1", await TaskCountAsync(id));
    }

    private void Authenticate(Guid userId, string email) =>
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpAssertions.CreateTokenFor(userId, email));

    private static async Task<Guid> InsertTaskAsync(Guid ownerId, string title)
    {
        var id = Guid.NewGuid();
        await TestDatabase.ExecuteAsync(
            $"""
             INSERT INTO tasks (id, owner_id, title, status, due_date)
             VALUES ('{id}', '{ownerId}', '{title}', 'Pending', DATE '2026-11-20')
             """);
        return id;
    }

    private static Task<string> TaskCountAsync(Guid id) =>
        TestDatabase.ScalarAsync($"SELECT count(*) FROM tasks WHERE id = '{id}'");
}
