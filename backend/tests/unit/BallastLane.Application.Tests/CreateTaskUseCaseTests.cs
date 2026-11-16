using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;
using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Application.Tasks;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Tests;

public sealed class CreateTaskUseCaseTests
{
    private static readonly UserId Owner = new(TestIdentities.AuthUserId);

    private static readonly DateOnly DueDate = new(2026, 11, 20);

    private static readonly FixedTimeProvider Time =
        new(new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero));

    [Fact]
    public async Task A_valid_task_is_created_with_the_authenticated_owner_and_Pending_by_default()
    {
        var writer = new RecordingTaskWriter();
        var useCase = new CreateTaskUseCase(new TaskValidator(), writer, Time);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            new TaskInput("Write the specification", "Cover traceability", null, "2026-11-20"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var stored = Assert.Single(writer.Added);
        Assert.Equal(Owner, stored.OwnerId);
        Assert.Equal(TaskStatus.Pending, stored.Status);
        Assert.Equal(DueDate, stored.DueDate);
        Assert.Equal(Time.UtcNow, stored.CreatedAt);
        Assert.Equal(Time.UtcNow, stored.UpdatedAt);
    }

    [Fact]
    public async Task A_supplied_status_is_honoured()
    {
        var writer = new RecordingTaskWriter();
        var useCase = new CreateTaskUseCase(new TaskValidator(), writer, Time);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            new TaskInput("Title", null, "InProgress", "2026-11-20"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TaskStatus.InProgress, Assert.Single(writer.Added).Status);
    }

    [Theory]
    [InlineData("", "2026-11-20", "title")]
    [InlineData("   ", "2026-11-20", "title")]
    [InlineData("Title", "2026-11-20T10:00:00Z", "dueDate")]
    [InlineData("Title", "not-a-date", "dueDate")]
    [InlineData("Title", "", "dueDate")]
    public async Task Invalid_input_yields_a_field_level_validation_outcome_and_persists_nothing(
        string title,
        string dueDate,
        string expectedField)
    {
        var writer = new RecordingTaskWriter();
        var useCase = new CreateTaskUseCase(new TaskValidator(), writer, Time);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            new TaskInput(title, null, null, dueDate),
            CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        Assert.Contains(result.Errors, error => error.Field == expectedField);
        Assert.Empty(writer.Added);
    }

    [Fact]
    public async Task An_unrecognized_status_yields_a_validation_outcome()
    {
        var writer = new RecordingTaskWriter();
        var useCase = new CreateTaskUseCase(new TaskValidator(), writer, Time);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            new TaskInput("Title", null, "Done", "2026-11-20"),
            CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "status");
        Assert.Empty(writer.Added);
    }

    [Fact]
    public async Task A_past_due_date_is_accepted()
    {
        var writer = new RecordingTaskWriter();
        var useCase = new CreateTaskUseCase(new TaskValidator(), writer, Time);

        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            new TaskInput("Overdue obligation", null, null, "2020-01-01"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateOnly(2020, 1, 1), Assert.Single(writer.Added).DueDate);
    }

    [Fact]
    public async Task An_owner_supplied_in_the_payload_is_ignored()
    {
        var writer = new RecordingTaskWriter();
        var useCase = new CreateTaskUseCase(new TaskValidator(), writer, Time);

        // TaskInput has no owner member, so the caller can only ever create for itself.
        var result = await useCase.ExecuteAsync(
            Identity(Owner),
            new TaskInput("Title", null, null, "2026-11-20"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(Owner, Assert.Single(writer.Added).OwnerId);
        Assert.NotEqual(new UserId(TestIdentities.OtherUserId), Assert.Single(writer.Added).OwnerId);
    }

    private static AuthenticatedUser Identity(UserId owner) => new(owner, "owner@example.test");

    private sealed class RecordingTaskWriter : ITaskWriter
    {
        internal List<TaskItem> Added { get; } = [];

        public Task AddAsync(TaskItem task, CancellationToken cancellationToken)
        {
            Added.Add(task);
            return Task.CompletedTask;
        }

        public Task<bool> UpdateAsync(TaskItem task, CancellationToken cancellationToken) =>
            Task.FromResult(true);

        public Task<bool> DeleteAsync(TaskId taskId, UserId ownerId, CancellationToken cancellationToken) =>
            Task.FromResult(true);
    }
}


