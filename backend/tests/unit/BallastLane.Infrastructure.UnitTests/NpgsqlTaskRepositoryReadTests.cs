using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;
using System.Data;
using System.Data.Common;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;
using BallastLane.Infrastructure.Persistence;

namespace BallastLane.Infrastructure.UnitTests;

public sealed class NpgsqlTaskRepositoryReadTests
{
    private static readonly UserId Owner = new(Guid.Parse("11111111-1111-4111-8111-111111111111"));

    [Theory]
    [InlineData("Pending", TaskStatus.Pending)]
    [InlineData("InProgress", TaskStatus.InProgress)]
    [InlineData("Completed", TaskStatus.Completed)]
    public async Task Each_documented_status_maps_from_its_stored_text(string stored, TaskStatus expected)
    {
        var repository = CreateRepository(
            Row(stored: stored, description: "Cover traceability"));

        var tasks = await repository.ListByOwnerAsync(Owner, CancellationToken.None);

        Assert.Equal(expected, Assert.Single(tasks).Status);
    }

    [Fact]
    public async Task A_null_description_maps_to_null_and_a_date_column_maps_to_a_date_only_value()
    {
        var repository = CreateRepository(
            Row(stored: "Pending", description: null, dueDate: new DateOnly(2026, 11, 20)));

        var tasks = await repository.ListByOwnerAsync(Owner, CancellationToken.None);

        var task = Assert.Single(tasks);
        Assert.Null(task.Description);
        Assert.Equal(new DateOnly(2026, 11, 20), task.DueDate);
    }

    [Fact]
    public async Task The_list_query_filters_by_owner_and_uses_the_documented_ordering()
    {
        var factory = new RecordingConnectionFactory(() => Row(stored: "Pending", description: null));

        var repository = new NpgsqlTaskRepository(factory);
        await repository.ListByOwnerAsync(Owner, CancellationToken.None);

        var command = Assert.Single(factory.Commands);
        Assert.Contains("WHERE owner_id = @ownerId", command.Sql, StringComparison.Ordinal);
        Assert.Contains("ORDER BY due_date, created_at, id", command.Sql, StringComparison.Ordinal);
        Assert.True(command.HasValue("ownerId"));
        Assert.Equal(Owner.Value, command.Parameters.Single(p => p.ParameterName == "ownerId").Value);
        Assert.DoesNotContain("SELECT *", command.Sql, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task The_get_query_filters_by_both_identifier_and_owner()
    {
        var factory = new RecordingConnectionFactory(() => Row(stored: "Completed", description: "x"));

        var repository = new NpgsqlTaskRepository(factory);
        var taskId = TaskId.New();
        await repository.FindOwnedAsync(taskId, Owner, CancellationToken.None);

        var command = Assert.Single(factory.Commands);
        Assert.Contains("WHERE id = @id AND owner_id = @ownerId", command.Sql, StringComparison.Ordinal);
        Assert.True(command.HasValue("id"));
        Assert.True(command.HasValue("ownerId"));
    }

    [Fact]
    public async Task A_missing_row_returns_null_rather_than_throwing()
    {
        var repository = CreateRepository(Row(stored: "Pending", description: null, isEmpty: true));

        var task = await repository.FindOwnedAsync(TaskId.New(), Owner, CancellationToken.None);

        Assert.Null(task);
    }

    [Fact]
    public async Task The_owners_identifier_is_never_taken_from_the_stored_row_for_authorization()
    {
        // The mapped owner must be the row's owner, and the query must already have been filtered by
        // the caller's identifier, so a row for another owner cannot be returned by a scoped lookup.
        var otherOwner = Guid.Parse("22222222-2222-4222-8222-222222222222");
        var factory = new RecordingConnectionFactory(() => Row(stored: "Pending", description: null, ownerId: otherOwner));

        var repository = new NpgsqlTaskRepository(factory);
        await repository.FindOwnedAsync(TaskId.New(), Owner, CancellationToken.None);

        Assert.Equal(Owner.Value, factory.Commands.Single().Parameters.Single(p => p.ParameterName == "ownerId").Value);
    }

    [Fact]
    public async Task The_cancellation_token_is_forwarded_to_the_reader_command()
    {
        var factory = new RecordingConnectionFactory(() => Row(stored: "Pending", description: null));
        using var cancellation = new CancellationTokenSource();

        await new NpgsqlTaskRepository(factory).ListByOwnerAsync(Owner, cancellation.Token);

        Assert.Equal(cancellation.Token, factory.Commands.Single().CancellationToken);
    }

    private static NpgsqlTaskRepository CreateRepository(params DbDataReader[] readers)
    {
        var queue = new Queue<DbDataReader>(readers);
        return new NpgsqlTaskRepository(new RecordingConnectionFactory(() => queue.Dequeue()));
    }

    private static DbDataReader Row(
        string stored,
        string? description,
        DateOnly? dueDate = null,
        Guid? ownerId = null,
        bool isEmpty = false)
    {
        var table = new DataTable();
        table.Columns.Add("id", typeof(Guid));
        table.Columns.Add("owner_id", typeof(Guid));
        table.Columns.Add("title", typeof(string));
        table.Columns.Add("description", typeof(string));
        table.Columns.Add("status", typeof(string));
        table.Columns.Add("due_date", typeof(DateOnly));
        table.Columns.Add("created_at", typeof(DateTimeOffset));
        table.Columns.Add("updated_at", typeof(DateTimeOffset));

        if (!isEmpty)
        {
            table.Rows.Add(
                Guid.NewGuid(),
                ownerId ?? Owner.Value,
                "Write the specification",
                (object?)description ?? DBNull.Value,
                stored,
                dueDate ?? new DateOnly(2026, 11, 20),
                new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 11, 16, 9, 30, 0, TimeSpan.Zero));
        }

        return table.CreateDataReader();
    }
}


