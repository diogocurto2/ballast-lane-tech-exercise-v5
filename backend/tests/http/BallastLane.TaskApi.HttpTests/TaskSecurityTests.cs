using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BallastLane.TaskApi.HttpTests;

/// <summary>
/// Security behaviors that must hold regardless of feature work: a payload that looks like SQL is
/// stored and returned as data, and ownership fields cannot be used to reach another account.
/// </summary>
[Collection(TaskApiCollection.Name)]
public sealed class TaskSecurityTests : IAsyncLifetime
{
    private const string InjectionPayload = "Robert'); DROP TABLE tasks; --";

    private readonly TaskApiFactory _factory;

    public TaskSecurityTests(TaskApiFactory factory) => _factory = factory;

    private HttpClient Client { get; set; } = null!;

    private Guid OwnerId { get; set; }

    public async Task InitializeAsync()
    {
        await TestDatabase.ResetAsync();
        Client = _factory.CreateClient();
        OwnerId = await TestDatabase.CreateUserAsync("security.owner@example.test", "unused-hash");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            HttpAssertions.CreateTokenFor(OwnerId, "security.owner@example.test"));
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task A_sql_injection_payload_is_stored_and_read_back_as_data()
    {
        var create = await Client.PostAsJsonAsync(
            "/api/tasks",
            new { title = InjectionPayload, description = InjectionPayload, dueDate = "2026-11-20" });

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await HttpAssertions.ReadJsonAsync(create);
        var id = created.GetProperty("id").GetString();

        var read = await Client.GetAsync($"/api/tasks/{id}");
        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
        var body = await HttpAssertions.ReadJsonAsync(read);
        Assert.Equal(InjectionPayload, body.GetProperty("title").GetString());
        Assert.Equal(InjectionPayload, body.GetProperty("description").GetString());

        // The table still exists and still holds the row, so the payload did not change query semantics.
        Assert.Equal("1", await TestDatabase.ScalarAsync($"SELECT count(*) FROM tasks WHERE id = '{id}'"));
    }

    [Fact]
    public async Task An_injection_payload_in_the_identifier_parameter_does_not_change_the_query()
    {
        var response = await Client.GetAsync($"/api/tasks/{(int)0}x' OR '1'='1");

        await HttpAssertions.AssertProblemAsync(response, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task The_owner_field_in_a_create_payload_cannot_assign_another_account()
    {
        var otherId = await TestDatabase.CreateUserAsync("security.other@example.test", "unused-hash");

        var create = await Client.PostAsJsonAsync(
            "/api/tasks",
            new { title = "Mine", dueDate = "2026-11-20", ownerId = otherId });

        var created = await HttpAssertions.ReadJsonAsync(create);
        var id = created.GetProperty("id").GetString();

        Assert.Equal(
            OwnerId.ToString(),
            await TestDatabase.ScalarAsync($"SELECT owner_id FROM tasks WHERE id = '{id}'"));
    }
}
