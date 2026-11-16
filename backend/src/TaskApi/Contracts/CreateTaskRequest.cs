using BallastLane.Application.Tasks;

namespace BallastLane.TaskApi.Contracts;

/// <summary>
/// Creation payload. It declares no owner: a supplied <c>ownerId</c> is not bound, so it is ignored
/// rather than trusted.
/// </summary>
public sealed class CreateTaskRequest
{
    public string? Title { get; init; }

    public string? Description { get; init; }

    public string? Status { get; init; }

    public string? DueDate { get; init; }

    public TaskInput ToInput() => new(Title, Description, Status, DueDate);
}
