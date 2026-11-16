namespace BallastLane.Domain.Tasks;

/// <summary>
/// Identifies a task. A value object so a task identifier cannot be confused with a user identifier.
/// </summary>
public readonly record struct TaskId(Guid Value)
{
    public static TaskId New() => new(Guid.NewGuid());

    public static TaskId Parse(string value) => new(Guid.Parse(value));

    /// <summary>
    /// Attempts to read an identifier from its text form. A value that is not a UUID is not an
    /// identifier at all, which is why the API answers such a request with 404 rather than 400.
    /// </summary>
    public static bool TryParse(string? value, out TaskId taskId)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            taskId = new TaskId(parsed);
            return true;
        }

        taskId = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}
