using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Application.Tasks;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Tests;

using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;

public sealed class UpdateTaskUseCaseTests
{
    private static readonly UserId Owner = new(TestIdentities.AuthUserId);

    private static readonly UserId OtherOwner = new(TestIdentities.OtherUserId);

    private static readonly DateOnly OriginalDueDate = new(2026, 11, 20);

    private static readonly FixedTimeProvider Time =
        new(new DateTimeOffset(2026, 11, 16, 12, 0, 0, TimeSpan.Zero));

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
    public async Task Every_status_transition_in_both_directions_is_accepted(string from, string to)
    {
        var store = new InMemoryTaskStore(ExistingTask(from));
        var useCase = CreateUseCase(store);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            store.Tasks[0].Id,
            new TaskInput("Write the specification", null, to, "2026-11-20"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(to, Assert.Single(store.Updated).Status.ToString());
    }

    [Fact]
    public async Task Every_editable_field_can_change_in_one_operation()
    {
        var store = new InMemoryTaskStore(ExistingTask("Pending"));
        var useCase = CreateUseCase(store);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            store.Tasks[0].Id,
            new TaskInput("Revised title", "Revised description", "Completed", "2027-01-15"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var updated = Assert.Single(store.Updated);
        Assert.Equal("Revised title", updated.Title);
        Assert.Equal("Revised description", updated.Description);
        Assert.Equal(TaskStatus.Completed, updated.Status);
        Assert.Equal(new DateOnly(2027, 1, 15), updated.DueDate);
    }

    [Fact]
    public async Task The_identifier_owner_and_creation_time_are_unchanged_by_an_update()
    {
        var existing = ExistingTask("Pending");
        var store = new InMemoryTaskStore(existing);
        var useCase = CreateUseCase(store);

        await useCase.ExecuteAsync(
            Identity(Owner),
            existing.Id,
            new TaskInput("Revised title", null, "InProgress", "2027-01-15"),
            CancellationToken.None);

        var updated = Assert.Single(store.Updated);
        Assert.Equal(existing.Id, updated.Id);
        Assert.Equal(existing.OwnerId, updated.OwnerId);
        Assert.Equal(existing.CreatedAt, updated.CreatedAt);
        Assert.Equal(Time.UtcNow, updated.UpdatedAt);
    }

    [Theory]
    [InlineData("", "2026-11-20", "InProgress", "title")]
    [InlineData("   ", "2026-11-20", "InProgress", "title")]
    [InlineData("Title", "2026-11-20", "Done", "status")]
    [InlineData("Title", "2026-11-20T10:00:00Z", "InProgress", "dueDate")]
    [InlineData("Title", "nonsense", "InProgress", "dueDate")]
    public async Task Invalid_edits_yield_a_field_level_validation_outcome_and_change_nothing(
        string title,
        string dueDate,
        string status,
        string expectedField)
    {
        var store = new InMemoryTaskStore(ExistingTask("Pending"));
        var useCase = CreateUseCase(store);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            store.Tasks[0].Id,
            new TaskInput(title, null, status, dueDate),
            CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        Assert.Contains(result.Errors, error => error.Field == expectedField);
        Assert.Empty(store.Updated);
    }

    [Fact]
    public async Task Another_users_task_yields_not_found_and_is_not_changed()
    {
        var existing = ExistingTask("Pending", OtherOwner);
        var store = new InMemoryTaskStore(existing);
        var useCase = CreateUseCase(store);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            existing.Id,
            new TaskInput("Hijacked", null, "Completed", "2027-01-15"),
            CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
        Assert.Empty(store.Updated);
    }

    [Fact]
    public async Task An_unknown_identifier_yields_not_found()
    {
        var store = new InMemoryTaskStore();
        var useCase = CreateUseCase(store);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            TaskId.New(),
            new TaskInput("Title", null, "Pending", "2026-11-20"),
            CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    private static UpdateTaskUseCase CreateUseCase(InMemoryTaskStore store) =>
        new(new TaskValidator(), store, store, Time);

    private static AuthenticatedUser Identity(UserId owner) => new(owner, "owner@example.test");

    private static TaskItem ExistingTask(string status, UserId? owner = null) =>
        new(
            TaskId.New(),
            owner ?? Owner,
            "Write the specification",
            "Cover traceability",
            Enum.Parse<TaskStatus>(status),
            OriginalDueDate,
            new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero));

    private sealed class InMemoryTaskStore(params TaskItem[] tasks) : ITaskReader, ITaskWriter
    {
        internal List<TaskItem> Tasks { get; } = [.. tasks];

        internal List<TaskItem> Updated { get; } = [];

        public Task<IReadOnlyList<TaskItem>> ListByOwnerAsync(
            UserId ownerId,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<TaskItem>>(Tasks.Where(task => task.OwnerId == ownerId).ToArray());

        public Task<TaskItem?> FindOwnedAsync(
            TaskId taskId,
            UserId ownerId,
            CancellationToken cancellationToken) =>
            Task.FromResult(Tasks.SingleOrDefault(task => task.Id == taskId && task.OwnerId == ownerId));

        public Task AddAsync(TaskItem task, CancellationToken cancellationToken)
        {
            Tasks.Add(task);
            return Task.CompletedTask;
        }

        public Task<bool> UpdateAsync(TaskItem task, CancellationToken cancellationToken)
        {
            Updated.Add(task);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(TaskId taskId, UserId ownerId, CancellationToken cancellationToken)
        {
            var removed = Tasks.RemoveAll(task => task.Id == taskId && task.OwnerId == ownerId);
            return Task.FromResult(removed == 1);
        }
    }
}
