using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Tasks;

/// <summary>
/// Creates a task for the authenticated user. The owner comes from the validated identity, never from
/// the payload, and a task starts as Pending when no status is supplied.
/// </summary>
public sealed class CreateTaskUseCase(
    TaskValidator validator,
    ITaskWriter taskWriter,
    ITimeProvider timeProvider)
{
    public async Task<OperationResult<TaskItem>> ExecuteAsync(
        AuthenticatedUser identity,
        TaskInput input,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(input);
        if (errors.Count > 0)
        {
            return OperationResult<TaskItem>.Validation(errors);
        }

        TaskValidator.TryParseDueDate(input.DueDate, out var dueDate);

        var draft = TaskDraft.Create(input.Title, input.Description, input.Status, dueDate);
        var task = TaskItem.Create(identity.UserId, draft, timeProvider.UtcNow);

        await taskWriter.AddAsync(task, cancellationToken);

        return OperationResult<TaskItem>.Success(task);
    }
}
