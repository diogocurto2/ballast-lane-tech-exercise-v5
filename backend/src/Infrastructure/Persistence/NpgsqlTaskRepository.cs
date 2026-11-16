using System.Data.Common;
using BallastLane.Application.Abstractions;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;
using Npgsql;
using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;

namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// PostgreSQL adapter for tasks. Every read is owner-scoped and every value is a parameter: the
/// owner filter is the authorization boundary, so it is never optional.
/// </summary>
public sealed class NpgsqlTaskRepository(IDbConnectionFactory connectionFactory) : ITaskReader, ITaskWriter
{
    private const string Columns = "id, owner_id, title, description, status, due_date, created_at, updated_at";

    private const string ListByOwnerSql =
        $"SELECT {Columns} FROM tasks WHERE owner_id = @ownerId ORDER BY due_date, created_at, id";

    private const string FindOwnedSql =
        $"SELECT {Columns} FROM tasks WHERE id = @id AND owner_id = @ownerId";

    private const string InsertSql =
        """
        INSERT INTO tasks (id, owner_id, title, description, status, due_date, created_at, updated_at)
        VALUES (@id, @ownerId, @title, @description, @status, @dueDate, @createdAt, @updatedAt)
        """;

    private const string UpdateSql =
        """
        UPDATE tasks
        SET title = @title, description = @description, status = @status, due_date = @dueDate, updated_at = @updatedAt
        WHERE id = @id AND owner_id = @ownerId
        """;

    private const string DeleteSql = "DELETE FROM tasks WHERE id = @id AND owner_id = @ownerId";

    public async Task<IReadOnlyList<TaskItem>> ListByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        await using var reader = await connectionFactory.ExecuteReaderAsync(
            connection,
            ListByOwnerSql,
            [new NpgsqlParameter("ownerId", ownerId.Value)],
            cancellationToken);

        var tasks = new List<TaskItem>();
        while (await reader.ReadAsync(cancellationToken))
        {
            tasks.Add(Map(reader));
        }

        return tasks;
    }

    public async Task<TaskItem?> FindOwnedAsync(
        TaskId taskId,
        UserId ownerId,
        CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        await using var reader = await connectionFactory.ExecuteReaderAsync(
            connection,
            FindOwnedSql,
            [new NpgsqlParameter("id", taskId.Value), new NpgsqlParameter("ownerId", ownerId.Value)],
            cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);

        await connectionFactory.ExecuteAsync(
            connection,
            InsertSql,
            [
                new NpgsqlParameter("id", task.Id.Value),
                new NpgsqlParameter("ownerId", task.OwnerId.Value),
                new NpgsqlParameter("title", task.Title),
                new NpgsqlParameter("description", (object?)task.Description ?? DBNull.Value),
                new NpgsqlParameter("status", task.Status.ToString()),
                new NpgsqlParameter("dueDate", task.DueDate),
                new NpgsqlParameter("createdAt", task.CreatedAt),
                new NpgsqlParameter("updatedAt", task.UpdatedAt),
            ],
            cancellationToken);
    }

    /// <summary>
    /// Replaces the editable columns of an owned row. The owner and creation time appear only in the
    /// WHERE clause, never in the update, so neither can be changed by a payload.
    /// </summary>
    public async Task<bool> UpdateAsync(TaskItem task, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);

        var affected = await connectionFactory.ExecuteAsync(
            connection,
            UpdateSql,
            [
                new NpgsqlParameter("id", task.Id.Value),
                new NpgsqlParameter("ownerId", task.OwnerId.Value),
                new NpgsqlParameter("title", task.Title),
                new NpgsqlParameter("description", (object?)task.Description ?? DBNull.Value),
                new NpgsqlParameter("status", task.Status.ToString()),
                new NpgsqlParameter("dueDate", task.DueDate),
                new NpgsqlParameter("updatedAt", task.UpdatedAt),
            ],
            cancellationToken);

        return affected == 1;
    }

    /// <summary>
    /// Removes an owned row. Both the identifier and the owner are in the WHERE clause, so a
    /// cross-user attempt deletes nothing and reports <c>false</c>.
    /// </summary>
    public async Task<bool> DeleteAsync(TaskId taskId, UserId ownerId, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);

        var affected = await connectionFactory.ExecuteAsync(
            connection,
            DeleteSql,
            [new NpgsqlParameter("id", taskId.Value), new NpgsqlParameter("ownerId", ownerId.Value)],
            cancellationToken);

        return affected == 1;
    }

    private static TaskItem Map(DbDataReader reader) =>
        new(
            new TaskId(reader.GetGuid(0)),
            new UserId(reader.GetGuid(1)),
            reader.GetString(2),
            reader.IsDBNull(3) ? null : reader.GetString(3),
            Enum.Parse<TaskStatus>(reader.GetString(4), ignoreCase: false),
            reader.GetFieldValue<DateOnly>(5),
            reader.GetFieldValue<DateTimeOffset>(6),
            reader.GetFieldValue<DateTimeOffset>(7));
}



