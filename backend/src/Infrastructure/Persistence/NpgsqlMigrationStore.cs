using Npgsql;

namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// PostgreSQL implementation of the migration store. Each script is applied inside its own
/// transaction together with its version record, so a script cannot be applied without being recorded.
/// </summary>
public sealed class NpgsqlMigrationStore(IDbConnectionFactory connectionFactory) : IMigrationStore
{
    private const string EnsureTableSql =
        """
        CREATE TABLE IF NOT EXISTS schema_migrations (
            version text PRIMARY KEY,
            applied_at timestamptz NOT NULL DEFAULT now()
        )
        """;

    private const string SelectVersionsSql = "SELECT version FROM schema_migrations";

    private const string InsertVersionSql = "INSERT INTO schema_migrations (version) VALUES (@version)";

    public async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        await connectionFactory.ExecuteAsync(connection, EnsureTableSql, [], cancellationToken);
    }

    public async Task<IReadOnlySet<string>> GetAppliedVersionsAsync(CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        await using var reader = await connectionFactory.ExecuteReaderAsync(
            connection,
            SelectVersionsSql,
            [],
            cancellationToken);

        var versions = new HashSet<string>(StringComparer.Ordinal);
        while (await reader.ReadAsync(cancellationToken))
        {
            versions.Add(reader.GetString(0));
        }

        return versions;
    }

    public async Task ApplyAsync(MigrationScript script, CancellationToken cancellationToken)
    {
        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        await connectionFactory.ExecuteAsync(connection, transaction, script.Sql, [], cancellationToken);
        await connectionFactory.ExecuteAsync(
            connection,
            transaction,
            InsertVersionSql,
            [new NpgsqlParameter("version", script.Version)],
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}

