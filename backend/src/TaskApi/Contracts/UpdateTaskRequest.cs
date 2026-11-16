using BallastLane.Application.Tasks;

namespace BallastLane.TaskApi.Contracts;

/// <summary>
/// Full update payload. Unlike creation, the status is required because the client now owns that
/// value. No owner is declared, so a supplied one is ignored rather than trusted.
/// </summary>
public sealed class UpdateTaskRequest
{
    public string? Title { get; init; }

    public string? Description { get; init; }

    public string? Status { get; init; }

    public string? DueDate { get; init; }

    public TaskInput ToInput() => new(Title, Description, Status, DueDate);
}
