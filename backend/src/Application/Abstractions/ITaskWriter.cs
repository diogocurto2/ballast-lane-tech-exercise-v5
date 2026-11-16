using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Abstractions;

/// <summary>
/// Stores tasks. The entity already carries its owner, so an implementation cannot invent one. An
/// update reports <c>false</c> when no owned row matched, which the use case maps to not-found.
/// </summary>
public interface ITaskWriter
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(TaskItem task, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(TaskId taskId, UserId ownerId, CancellationToken cancellationToken);
}
