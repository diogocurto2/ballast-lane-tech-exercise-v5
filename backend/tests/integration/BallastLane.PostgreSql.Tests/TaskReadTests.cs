using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;
using BallastLane.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.PostgreSql.Tests;

[Collection(PostgreSqlCollection.Name)]
public sealed class TaskReadTests : IAsyncLifetime
{
    private readonly NpgsqlTaskRepository _repository = new(new NpgsqlConnectionFactory(
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = TestDatabase.ConnectionString,
            })
            .Build()));

    private UserId _owner;

    private UserId _otherOwner;

    public async Task InitializeAsync()
    {
        await PostgreSqlDatabase.ResetAsync();
        _owner = await PostgreSqlDatabase.CreateUserAsync("read.owner@example.test");
        _otherOwner = await PostgreSqlDatabase.CreateUserAsync("read.other@example.test");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task An_inserted_task_reads_back_with_identical_fields()
    {
        var id = TaskId.New();
        var dueDate = new DateOnly(2026, 11, 20);
        await PostgreSqlDatabase.InsertTaskAsync(id, _owner, "Write the specification", "Cover traceability", "InProgress", dueDate);

        var task = await _repository.FindOwnedAsync(id, _owner, CancellationToken.None);

        Assert.NotNull(task);
        Assert.Equal(id, task.Id);
        Assert.Equal(_owner, task.OwnerId);
        Assert.Equal("Write the specification", task.Title);
        Assert.Equal("Cover traceability", task.Description);
        Assert.Equal(TaskStatus.InProgress, task.Status);
        Assert.Equal(dueDate, task.DueDate);
    }

    [Fact]
    public async Task A_null_description_round_trips_as_null()
    {
        var id = TaskId.New();
        await PostgreSqlDatabase.InsertTaskAsync(id, _owner, "No description", null, "Pending", new DateOnly(2026, 11, 21));

        var task = await _repository.FindOwnedAsync(id, _owner, CancellationToken.None);

        Assert.NotNull(task);
        Assert.Null(task.Description);
    }

    [Fact]
    public async Task Listing_returns_only_the_callers_rows_in_due_date_order_with_a_creation_tie_break()
    {
        var sameDueDate = new DateOnly(2026, 11, 20);
        await PostgreSqlDatabase.InsertTaskAsync(
            TaskId.New(), _owner, "Second by creation", null, "Pending", sameDueDate,
            createdAt: new DateTimeOffset(2026, 11, 1, 10, 0, 0, TimeSpan.Zero));
        await PostgreSqlDatabase.InsertTaskAsync(
            TaskId.New(), _owner, "First by creation", null, "Pending", sameDueDate,
            createdAt: new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero));
        await PostgreSqlDatabase.InsertTaskAsync(
            TaskId.New(), _owner, "Later due date", null, "Pending", new DateOnly(2026, 12, 1));
        await PostgreSqlDatabase.InsertTaskAsync(
            TaskId.New(), _otherOwner, "Another owners task", null, "Pending", new DateOnly(2026, 11, 1));

        var tasks = await _repository.ListByOwnerAsync(_owner, CancellationToken.None);

        Assert.Equal(
            ["First by creation", "Second by creation", "Later due date"],
            tasks.Select(task => task.Title));
    }

    [Fact]
    public async Task An_account_with_no_tasks_returns_an_empty_list()
    {
        var empty = await PostgreSqlDatabase.CreateUserAsync("read.empty@example.test");

        var tasks = await _repository.ListByOwnerAsync(empty, CancellationToken.None);

        Assert.Empty(tasks);
    }

    [Fact]
    public async Task Another_users_task_is_not_returned_by_an_owner_scoped_lookup()
    {
        var id = TaskId.New();
        await PostgreSqlDatabase.InsertTaskAsync(id, _otherOwner, "Not mine", null, "Pending", new DateOnly(2026, 11, 20));

        var task = await _repository.FindOwnedAsync(id, _owner, CancellationToken.None);

        Assert.Null(task);
    }
}

