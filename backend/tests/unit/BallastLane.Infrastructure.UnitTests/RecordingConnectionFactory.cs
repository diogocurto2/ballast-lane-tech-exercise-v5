using System.Data.Common;
using BallastLane.Infrastructure.Persistence;
using Npgsql;

namespace BallastLane.Infrastructure.UnitTests;

/// <summary>
/// A recording connection factory. The repository's SQL, parameter names, and row mapping are asserted
/// without a live server; the same statements are executed against real PostgreSQL in
/// <c>BallastLane.PostgreSql.Tests</c>.
/// </summary>
internal sealed class RecordingConnectionFactory : IDbConnectionFactory
{
    private readonly Func<DbDataReader>? _readerFactory;

    internal RecordingConnectionFactory(Func<DbDataReader>? readerFactory = null) => _readerFactory = readerFactory;

    internal List<RecordedCommand> Commands { get; } = [];

    public NpgsqlConnection Create() => new();

    public Task<NpgsqlConnection> OpenAsync(CancellationToken cancellationToken) =>
        Task.FromResult(new NpgsqlConnection());

    public Task<int> ExecuteAsync(
        NpgsqlConnection connection,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken)
    {
        Commands.Add(new RecordedCommand(sql, parameters, Transaction: null, cancellationToken));
        return Task.FromResult(0);
    }

    public Task<int> ExecuteAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken)
    {
        Commands.Add(new RecordedCommand(sql, parameters, transaction, cancellationToken));
        return Task.FromResult(0);
    }

    public Task<DbDataReader> ExecuteReaderAsync(
        NpgsqlConnection connection,
        string sql,
        IReadOnlyList<NpgsqlParameter> parameters,
        CancellationToken cancellationToken)
    {
        Commands.Add(new RecordedCommand(sql, parameters, Transaction: null, cancellationToken));
        return Task.FromResult(
            _readerFactory?.Invoke()
            ?? throw new InvalidOperationException("No reader was configured for this test."));
    }
}

internal sealed record RecordedCommand(
    string Sql,
    IReadOnlyList<NpgsqlParameter> Parameters,
    NpgsqlTransaction? Transaction,
    CancellationToken CancellationToken)
{
    internal IReadOnlyList<string> ParameterNames => Parameters.Select(parameter => parameter.ParameterName).ToArray();

    internal bool HasValue(string name) => Parameters.Any(parameter => parameter.ParameterName == name);
}
