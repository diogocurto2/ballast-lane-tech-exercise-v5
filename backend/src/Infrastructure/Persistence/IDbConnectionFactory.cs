using System.Data.Common;
using Npgsql;

namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// Creates and opens connections and executes parameterized commands. Declared in Infrastructure so
/// the application layer never sees a driver type, and abstracted so data access components can be
/// unit-tested without a live server.
/// </summary>
public interface IDbConnectionFactory
{
    NpgsqlConnection Create();

    Task<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken);

    Task<int> ExecuteAsync(
        NpgsqlConnection connection,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken);

    Task<int> ExecuteAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken);

    Task<DbDataReader> ExecuteReaderAsync(
        NpgsqlConnection connection,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken);
}
