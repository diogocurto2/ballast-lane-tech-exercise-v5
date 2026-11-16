namespace BallastLane.Domain.Tasks;

/// <summary>
/// The three statuses a task may hold. A closed set, enforced here in code and again by a database
/// CHECK constraint.
/// </summary>
public enum TaskStatus
{
    Pending,
    InProgress,
    Completed,
}
