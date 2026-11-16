using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Tasks;

/// <summary>
/// Deletes one of the caller's own tasks. The delete is scoped by identifier and owner in a single
/// statement, so a cross-user attempt cannot remove anything and is reported as not-found.
/// </summary>
public sealed class DeleteTaskUseCase(ITaskWriter taskWriter)
{
    private const string NotFoundDetail = "Task was not found.";

    public async Task<OperationResult<Unit>> ExecuteAsync(
        AuthenticatedUser identity,
        TaskId taskId,
        CancellationToken cancellationToken)
    {
        var deleted = await taskWriter.DeleteAsync(taskId, identity.UserId, cancellationToken);

        return deleted
            ? OperationResult<Unit>.Success(Unit.Value)
            : OperationResult<Unit>.NotFound(NotFoundDetail);
    }
}