using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Tasks;

/// <summary>
/// Lists the caller's own tasks. The owner comes from the validated identity, so no request value can
/// widen the scope.
/// </summary>
public sealed class ListTasksUseCase(ITaskReader taskReader)
{
    public async Task<OperationResult<IReadOnlyList<TaskItem>>> ExecuteAsync(
        AuthenticatedUser identity,
        CancellationToken cancellationToken)
    {
        var tasks = await taskReader.ListByOwnerAsync(identity.UserId, cancellationToken);
        return OperationResult<IReadOnlyList<TaskItem>>.Success(tasks);
    }
}

/// <summary>
/// Reads one of the caller's own tasks. A missing task and another user's task are deliberately the
/// same outcome, so existence is never confirmed.
/// </summary>
public sealed class GetTaskUseCase(ITaskReader taskReader)
{
    private const string NotFoundDetail = "Task was not found.";

    public async Task<OperationResult<TaskItem>> ExecuteAsync(
        AuthenticatedUser identity,
        TaskId taskId,
        CancellationToken cancellationToken)
    {
        var task = await taskReader.FindOwnedAsync(taskId, identity.UserId, cancellationToken);

        return task is null
            ? OperationResult<TaskItem>.NotFound(NotFoundDetail)
            : OperationResult<TaskItem>.Success(task);
    }
}
