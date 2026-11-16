using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;

namespace BallastLane.Domain.Tasks;

/// <summary>
/// A validated create or update payload. It deliberately carries no identifier and no owner, so
/// ownership cannot arrive from a request and a payload can never address another user's record.
/// </summary>
public sealed record TaskDraft
{
    public const int MaximumTitleLength = 200;

    public const int MaximumDescriptionLength = 2000;

    private TaskDraft(string title, string? description, TaskStatus status, DateOnly dueDate)
    {
        Title = title;
        Description = description;
        Status = status;
        DueDate = dueDate;
    }

    public string Title { get; }

    public string? Description { get; }

    public TaskStatus Status { get; }

    public DateOnly DueDate { get; }

    /// <summary>
    /// Validates and normalizes a draft. <paramref name="status"/> may be omitted, in which case the
    /// task starts as Pending; a supplied value must be one of the three documented statuses.
    /// </summary>
    public static bool TryCreate(
        string? title,
        string? description,
        string? status,
        DateOnly dueDate,
        out TaskDraft draft)
    {
        draft = null!;

        var trimmedTitle = (title ?? string.Empty).Trim();
        if (trimmedTitle.Length is 0 or > MaximumTitleLength)
        {
            return false;
        }

        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (normalizedDescription is not null && normalizedDescription.Length > MaximumDescriptionLength)
        {
            return false;
        }

        if (!TryParseStatus(status, out var parsedStatus))
        {
            return false;
        }

        draft = new TaskDraft(trimmedTitle, normalizedDescription, parsedStatus, dueDate);
        return true;
    }

    public static TaskDraft Create(string? title, string? description, string? status, DateOnly dueDate)
    {
        if (!TryCreate(title, description, status, dueDate, out var draft))
        {
            throw new ArgumentException("The supplied task values do not satisfy the task rules.", nameof(title));
        }

        return draft;
    }

    private static bool TryParseStatus(string? status, out TaskStatus parsed)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            parsed = TaskStatus.Pending;
            return true;
        }

        // Case-sensitive on purpose: the API's vocabulary is exactly the three documented names.
        return Enum.TryParse(status, ignoreCase: false, out parsed) && Enum.IsDefined(parsed);
    }
}