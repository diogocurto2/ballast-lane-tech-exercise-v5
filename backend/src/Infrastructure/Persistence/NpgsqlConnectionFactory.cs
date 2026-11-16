using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// Creates PostgreSQL connections from configuration and runs parameterized commands.
/// </summary>
public sealed class NpgsqlConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    internal const string ConnectionStringName = "ConnectionStrings:Database";

    public NpgsqlConnection Create()
    {
        var connectionString = configuration[ConnectionStringName];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"The connection string '{ConnectionStringName}' is not configured. "
                + "Set the ConnectionStrings__Database environment variable.");
        }

        return new NpgsqlConnection(connectionString);
    }

    public async Task<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken)
    {
        var connection = Create();
        try
        {
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    public Task<int> ExecuteAsync(
        NpgsqlConnection connection,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken) =>
        ExecuteCoreAsync(connection, transaction: null, sql, parameters, cancellationToken);

    public Task<int> ExecuteAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken) =>
        ExecuteCoreAsync(connection, transaction, sql, parameters, cancellationToken);

    public async Task<DbDataReader> ExecuteReaderAsync(
        NpgsqlConnection connection,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken)
    {
        var command = CreateCommand(connection, transaction: null, sql, parameters);
        try
        {
            return await command.ExecuteReaderAsync(cancellationToken);
        }
        catch
        {
            await command.DisposeAsync();
            throw;
        }
    }

    private static async Task<int> ExecuteCoreAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction? transaction,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(connection, transaction, sql, parameters);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static NpgsqlCommand CreateCommand(
        NpgsqlConnection connection,
        NpgsqlTransaction? transaction,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters)
    {
        var command = new NpgsqlCommand(sql, connection, transaction);
        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        return command;
    }
}
