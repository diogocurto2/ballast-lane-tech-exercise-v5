using BallastLane.Application.Abstractions;
using BallastLane.Domain.Users;
using Npgsql;

namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// PostgreSQL adapter for accounts. Email lookups compare <c>lower(email)</c> so they use the unique
/// expression index the schema defines, and a unique violation is translated into a conflict signal
/// rather than a driver exception.
/// </summary>
public sealed class NpgsqlUserRepository(IDbConnectionFactory connectionFactory) : IUserReader, IUserWriter
{
    private const string Columns = "id, email, password_hash, created_at";

    private const string SelectByEmailSql = $"SELECT {Columns} FROM users WHERE lower(email) = lower(@email)";

    private const string SelectByIdSql = $"SELECT {Columns} FROM users WHERE id = @id";

    private const string InsertSql =
        "INSERT INTO users (id, email, password_hash, created_at) VALUES (@id, @email, @passwordHash, @createdAt)";

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        await using var reader = await connectionFactory.ExecuteReaderAsync(
            connection,
            SelectByEmailSql,
            [new NpgsqlParameter("email", email)],
            cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<User?> FindByIdAsync(UserId userId, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        await using var reader = await connectionFactory.ExecuteReaderAsync(
            connection,
            SelectByIdSql,
            [new NpgsqlParameter("id", userId.Value)],
            cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<bool> AddAsync(User user, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);

        try
        {
            await connectionFactory.ExecuteAsync(
                connection,
                InsertSql,
                [
                    new NpgsqlParameter("id", user.Id.Value),
                    new NpgsqlParameter("email", user.Email.Value),
                    new NpgsqlParameter("passwordHash", user.PasswordHash),
                    new NpgsqlParameter("createdAt", user.CreatedAt),
                ],
                cancellationToken);

            return true;
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }
    }

    private static User Map(System.Data.Common.DbDataReader reader) =>
        new(
            new UserId(reader.GetGuid(0)),
            Email.Create(reader.GetString(1)),
            reader.GetString(2),
            reader.GetFieldValue<DateTimeOffset>(3));
}
