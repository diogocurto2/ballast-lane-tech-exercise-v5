using BallastLane.Domain.Users;
using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;

namespace BallastLane.Domain.Tasks;

/// <summary>
/// A personal work item belonging to exactly one user.
/// </summary>
public sealed record TaskItem(
    TaskId Id,
    UserId OwnerId,
    string Title,
    string? Description,
    TaskStatus Status,
    DateOnly DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    /// <summary>
    /// Creates a task for the supplied owner. The owner is a parameter rather than part of the draft,
    /// so a request payload can never choose it.
    /// </summary>
    public static TaskItem Create(UserId ownerId, TaskDraft draft, DateTimeOffset now) =>
        new(
            TaskId.New(),
            ownerId,
            draft.Title,
            draft.Description,
            draft.Status,
            draft.DueDate,
            now,
            now);
}