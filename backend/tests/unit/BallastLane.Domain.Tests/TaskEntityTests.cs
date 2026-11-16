using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Domain.Tests;

public sealed class TaskEntityTests
{
    [Fact]
    public void A_task_exposes_its_identifier_owner_title_description_status_and_dates()
    {
        var id = TaskId.New();
        var owner = UserId.New();
        var createdAt = new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero);
        var dueDate = new DateOnly(2026, 11, 20);

        var task = new TaskItem(
            id,
            owner,
            "Write the specification",
            "Cover traceability",
            TaskStatus.InProgress,
            dueDate,
            createdAt,
            createdAt);

        Assert.Equal(id, task.Id);
        Assert.Equal(owner, task.OwnerId);
        Assert.Equal("Write the specification", task.Title);
        Assert.Equal("Cover traceability", task.Description);
        Assert.Equal(TaskStatus.InProgress, task.Status);
        Assert.Equal(dueDate, task.DueDate);
        Assert.Equal(createdAt, task.CreatedAt);
        Assert.Equal(createdAt, task.UpdatedAt);
    }

    [Fact]
    public void Exactly_three_statuses_exist_and_no_other_value_is_representable()
    {
        Assert.Equal(
            ["Pending", "InProgress", "Completed"],
            Enum.GetNames<TaskStatus>());
    }

    [Fact]
    public void A_task_identifier_round_trips_from_its_text_form()
    {
        var id = TaskId.New();

        Assert.Equal(id, TaskId.Parse(id.ToString()));
        Assert.True(TaskId.TryParse(id.ToString(), out var parsed));
        Assert.Equal(id, parsed);
        Assert.False(TaskId.TryParse("not-a-uuid", out _));
    }
}

