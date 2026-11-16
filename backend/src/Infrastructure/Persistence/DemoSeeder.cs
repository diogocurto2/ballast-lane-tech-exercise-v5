using BallastLane.Application.Abstractions;
using Npgsql;

namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// Seeds the demonstration accounts and tasks. Every write is an upsert keyed on the same unique
/// expression index the schema defines (<c>lower(email)</c>), so repeated runs neither duplicate nor
/// fail. The whole seed is one transaction, so a partial seed cannot persist.
/// </summary>
public sealed class DemoSeeder(
    IDbConnectionFactory connectionFactory,
    IPasswordHasher passwordHasher,
    ITimeProvider timeProvider)
{
    private const string InsertUserSql =
        """
        INSERT INTO users (id, email, password_hash, created_at)
        VALUES (@id, @email, @passwordHash, @createdAt)
        ON CONFLICT (lower(email)) DO NOTHING
        """;

    private const string SelectUserIdSql = "SELECT id FROM users WHERE lower(email) = lower(@email)";

    private const string InsertTaskSql =
        """
        INSERT INTO tasks (id, owner_id, title, description, status, due_date, created_at, updated_at)
        SELECT @id, @ownerId, @title, @description, @status, @dueDate, @createdAt, @createdAt
        WHERE NOT EXISTS (
            SELECT 1 FROM tasks WHERE owner_id = @ownerId AND title = @title
        )
        """;

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var now = timeProvider.UtcNow;

        foreach (var account in DemonstrationData.Accounts)
        {
            await InsertUserAsync(connection, transaction, account, now, cancellationToken);
            var ownerId = await ReadUserIdAsync(connection, transaction, account.Email, cancellationToken);

            foreach (var task in account.Tasks)
            {
                await InsertTaskAsync(connection, transaction, ownerId, task, now, cancellationToken);
            }
        }

        await transaction.CommitAsync(cancellationToken);
    }

    private async Task InsertUserAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        DemoAccount account,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await connectionFactory.ExecuteAsync(
            connection,
            transaction,
            InsertUserSql,
            [
                new NpgsqlParameter("id", Guid.NewGuid()),
                new NpgsqlParameter("email", account.Email),
                new NpgsqlParameter("passwordHash", passwordHasher.Hash(account.Password)),
                new NpgsqlParameter("createdAt", now),
            ],
            cancellationToken);

    private async Task<Guid> ReadUserIdAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string email,
        CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand(SelectUserIdSql, connection, transaction);
        command.Parameters.AddWithValue("email", email);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return (Guid)result!;
    }

    private async Task InsertTaskAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        Guid ownerId,
        DemoTask task,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await connectionFactory.ExecuteAsync(
            connection,
            transaction,
            InsertTaskSql,
            [
                new NpgsqlParameter("id", Guid.NewGuid()),
                new NpgsqlParameter("ownerId", ownerId),
                new NpgsqlParameter("title", task.Title),
                new NpgsqlParameter("description", (object?)task.Description ?? DBNull.Value),
                new NpgsqlParameter("status", task.Status),
                new NpgsqlParameter("dueDate", task.DueDate),
                new NpgsqlParameter("createdAt", now),
            ],
            cancellationToken);
}
