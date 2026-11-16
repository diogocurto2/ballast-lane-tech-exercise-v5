using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;
using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Application.Tasks;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Tests;

public sealed class ListAndGetTasksUseCaseTests
{
    private static readonly UserId Owner = new(TestIdentities.AuthUserId);

    private static readonly UserId OtherOwner = new(TestIdentities.OtherUserId);

    [Fact]
    public async Task Listing_returns_only_the_callers_tasks()
    {
        var reader = new InMemoryTaskReader(
        [
            NewTask(Owner, "Mine", new DateOnly(2026, 11, 20)),
            NewTask(OtherOwner, "Not mine", new DateOnly(2026, 11, 19)),
        ]);
        var useCase = new ListTasksUseCase(reader);

        var result = await useCase.ExecuteAsync(Identity(Owner), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var task = Assert.Single(result.Value!);
        Assert.Equal("Mine", task.Title);
    }

    [Fact]
    public async Task Listing_an_account_with_no_tasks_returns_an_empty_collection_not_an_error()
    {
        var useCase = new ListTasksUseCase(new InMemoryTaskReader([]));

        var result = await useCase.ExecuteAsync(Identity(Owner), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task Listing_is_ordered_by_due_date_then_creation_time()
    {
        var sharedDueDate = new DateOnly(2026, 11, 20);
        var reader = new InMemoryTaskReader(
        [
            NewTask(Owner, "Later due", new DateOnly(2026, 11, 25), createdAt: TaskTime(1)),
            NewTask(Owner, "Same due, later created", sharedDueDate, createdAt: TaskTime(2)),
            NewTask(Owner, "Same due, earlier created", sharedDueDate, createdAt: TaskTime(1)),
        ]);
        var useCase = new ListTasksUseCase(reader);

        var result = await useCase.ExecuteAsync(Identity(Owner), CancellationToken.None);

        Assert.Equal(
            ["Same due, earlier created", "Same due, later created", "Later due"],
            result.Value!.Select(task => task.Title));
    }

    [Fact]
    public async Task Getting_an_owned_task_by_identifier_returns_it()
    {
        var task = NewTask(Owner, "Mine", new DateOnly(2026, 11, 20));
        var useCase = new GetTaskUseCase(new InMemoryTaskReader([task]));

        var result = await useCase.ExecuteAsync(Identity(Owner), task.Id, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(task.Id, result.Value!.Id);
    }

    [Fact]
    public async Task Getting_another_users_task_returns_not_found_and_no_data()
    {
        var task = NewTask(OtherOwner, "Not mine", new DateOnly(2026, 11, 20));
        var useCase = new GetTaskUseCase(new InMemoryTaskReader([task]));

        var result = await useCase.ExecuteAsync(Identity(Owner), task.Id, CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Getting_an_unknown_identifier_returns_not_found()
    {
        var useCase = new GetTaskUseCase(new InMemoryTaskReader([]));

        var result = await useCase.ExecuteAsync(Identity(Owner), TaskId.New(), CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    private static AuthenticatedUser Identity(UserId owner) => new(owner, "owner@example.test");

    private static DateTimeOffset TaskTime(int hour) =>
        new(2026, 11, 1, hour, 0, 0, TimeSpan.Zero);

    private static TaskItem NewTask(
        UserId owner,
        string title,
        DateOnly dueDate,
        DateTimeOffset? createdAt = null) =>
        new(
            TaskId.New(),
            owner,
            title,
            Description: null,
            TaskStatus.Pending,
            dueDate,
            createdAt ?? TaskTime(0),
            createdAt ?? TaskTime(0));

    private sealed class InMemoryTaskReader(IReadOnlyList<TaskItem> tasks) : ITaskReader
    {
        public Task<IReadOnlyList<TaskItem>> ListByOwnerAsync(
            UserId ownerId,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<TaskItem>>(tasks
                .Where(task => task.OwnerId == ownerId)
                .OrderBy(task => task.DueDate)
                .ThenBy(task => task.CreatedAt)
                .ThenBy(task => task.Id.Value)
                .ToArray());

        public Task<TaskItem?> FindOwnedAsync(
            TaskId taskId,
            UserId ownerId,
            CancellationToken cancellationToken) =>
            Task.FromResult(tasks.SingleOrDefault(task => task.Id == taskId && task.OwnerId == ownerId));
    }
}

