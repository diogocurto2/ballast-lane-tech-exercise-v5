using BallastLane.Application.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.TaskApi.Contracts;

/// <summary>Task representation. The owner is deliberately not exposed.</summary>
public sealed record TaskResponse(
    string Id,
    string Title,
    string? Description,
    string Status,
    string DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static TaskResponse From(Domain.Tasks.TaskItem task) => new(
        task.Id.ToString(),
        task.Title,
        task.Description,
        task.Status.ToString(),
        task.DueDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
        task.CreatedAt,
        task.UpdatedAt);
}
