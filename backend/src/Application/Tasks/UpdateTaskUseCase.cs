using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Tasks;

/// <summary>
/// Replaces the editable values of a task the caller owns. The task is loaded through the owner-scoped
/// reader first, so another user's task is indistinguishable from a missing one, and an invalid edit
/// leaves the stored task untouched.
/// </summary>
public sealed class UpdateTaskUseCase(
    TaskValidator validator,
    ITaskReader taskReader,
    ITaskWriter taskWriter,
    ITimeProvider timeProvider)
{
    private const string NotFoundDetail = "Task was not found.";

    public async Task<OperationResult<TaskItem>> ExecuteAsync(
        AuthenticatedUser identity,
        TaskId taskId,
        TaskInput input,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(input, requireStatus: true);
        if (errors.Count > 0)
        {
            return OperationResult<TaskItem>.Validation(errors);
        }

        var existing = await taskReader.FindOwnedAsync(taskId, identity.UserId, cancellationToken);
        if (existing is null)
        {
            return OperationResult<TaskItem>.NotFound(NotFoundDetail);
        }

        TaskValidator.TryParseDueDate(input.DueDate, out var dueDate);
        var draft = TaskDraft.Create(input.Title, input.Description, input.Status, dueDate);

        // Identity, owner, and creation time are carried over from the stored task: an update replaces
        // editable values only.
        var updated = existing with
        {
            Title = draft.Title,
            Description = draft.Description,
            Status = draft.Status,
            DueDate = draft.DueDate,
            UpdatedAt = timeProvider.UtcNow,
        };

        if (!await taskWriter.UpdateAsync(updated, cancellationToken))
        {
            return OperationResult<TaskItem>.NotFound(NotFoundDetail);
        }

        return OperationResult<TaskItem>.Success(updated);
    }
}