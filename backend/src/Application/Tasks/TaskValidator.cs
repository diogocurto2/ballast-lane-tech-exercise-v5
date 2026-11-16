using BallastLane.Application.Outcomes;
using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;

namespace BallastLane.Application.Tasks;

/// <summary>
/// The shape-validation input for creating or updating a task. Deliberately carries no owner: an
/// owner cannot arrive from a request payload.
/// </summary>
public sealed record TaskInput(string? Title, string? Description, string? Status, string? DueDate);

/// <summary>
/// Validates task input. Shape rules live here so a controller never holds one and every endpoint
/// reports the same field keys.
/// </summary>
public sealed class TaskValidator
{
    public const string DueDateFormat = "yyyy-MM-dd";

    public IReadOnlyList<FieldError> Validate(TaskInput input, bool requireStatus = false)
    {
        ArgumentNullException.ThrowIfNull(input);

        var errors = new List<FieldError>();

        var title = (input.Title ?? string.Empty).Trim();
        if (title.Length == 0)
        {
            errors.Add(new FieldError("title", "Title is required."));
        }
        else if (title.Length > Domain.Tasks.TaskDraft.MaximumTitleLength)
        {
            errors.Add(new FieldError(
                "title",
                $"Title must be at most {Domain.Tasks.TaskDraft.MaximumTitleLength} characters long."));
        }

        if (input.Description is { Length: > 0 }
            && input.Description.Trim().Length > Domain.Tasks.TaskDraft.MaximumDescriptionLength)
        {
            errors.Add(new FieldError(
                "description",
                $"Description must be at most {Domain.Tasks.TaskDraft.MaximumDescriptionLength} characters long."));
        }

        if (!TryParseDueDate(input.DueDate, out _))
        {
            errors.Add(new FieldError("dueDate", "Due date must be a calendar date in the format yyyy-MM-dd."));
        }

        if (string.IsNullOrWhiteSpace(input.Status))
        {
            // Creation may omit the status because Pending is a sensible default; an update may not,
            // because the client now owns the value and silence would silently reset it.
            if (requireStatus)
            {
                errors.Add(new FieldError("status", "Status is required."));
            }
        }
        else if (!TryParseStatus(input.Status, out _))
        {
            errors.Add(new FieldError(
                "status",
                "Status must be one of Pending, InProgress, or Completed."));
        }

        return errors;
    }

    /// <summary>
    /// Parses a calendar date. A value carrying a time or timezone component is rejected rather than
    /// truncated, because a due date has no time of day.
    /// </summary>
    public static bool TryParseDueDate(string? value, out DateOnly dueDate)
    {
        dueDate = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return DateOnly.TryParseExact(
            value.Trim(),
            DueDateFormat,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out dueDate);
    }

    public static bool TryParseStatus(string? value, out TaskStatus status)
    {
        status = TaskStatus.Pending;

        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return Enum.TryParse(value, ignoreCase: false, out status) && Enum.IsDefined(status);
    }
}
