using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Abstractions;

/// <summary>
/// Reads tasks. Every method is owner-scoped so a caller can never request another user's data, and a
/// missing or not-owned record is reported as <c>null</c> rather than an exception.
/// </summary>
public interface ITaskReader
{
    Task<IReadOnlyList<TaskItem>> ListByOwnerAsync(UserId ownerId, CancellationToken cancellationToken);

    Task<TaskItem?> FindOwnedAsync(TaskId taskId, UserId ownerId, CancellationToken cancellationToken);
}
