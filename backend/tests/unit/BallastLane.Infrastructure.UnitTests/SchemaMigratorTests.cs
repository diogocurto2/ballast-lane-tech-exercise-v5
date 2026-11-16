using BallastLane.Infrastructure.Persistence;

namespace BallastLane.Infrastructure.UnitTests;

public sealed class SchemaMigratorTests : IDisposable
{
    private readonly string _directory =
        Path.Combine(Path.GetTempPath(), $"ballastlane-migrations-{Guid.NewGuid():N}");

    public SchemaMigratorTests() => Directory.CreateDirectory(_directory);

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    [Fact]
    public void Loading_scripts_orders_them_by_file_name()
    {
        // Written out of order on purpose: enumeration order must not decide application order.
        WriteScript("0002_create_tasks.sql", "CREATE TABLE tasks ();");
        WriteScript("0001_create_users.sql", "CREATE TABLE users ();");
        WriteScript("0010_add_index.sql", "CREATE INDEX ix ON users (id);");

        var scripts = SchemaMigrator.LoadScripts(_directory);

        Assert.Equal(
            ["0001_create_users.sql", "0002_create_tasks.sql", "0010_add_index.sql"],
            scripts.Select(script => script.Version));
        Assert.Equal("CREATE TABLE users ();", scripts[0].Sql);
    }

    [Fact]
    public void Loading_scripts_ignores_files_that_are_not_migrations()
    {
        WriteScript("0001_create_users.sql", "CREATE TABLE users ();");
        WriteScript("readme.md", "# notes");
        WriteScript("0001_create_users.sql.bak", "SELECT 1;");

        var scripts = SchemaMigrator.LoadScripts(_directory);

        var script = Assert.Single(scripts);
        Assert.Equal("0001_create_users.sql", script.Version);
    }

    [Fact]
    public void Loading_scripts_reports_a_migration_file_that_cannot_be_read()
    {
        var path = WriteScript("0001_create_users.sql", "CREATE TABLE users ();");

        using var exclusive = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

        var exception = Assert.Throws<MigrationLoadException>(() => SchemaMigrator.LoadScripts(_directory));

        Assert.Contains("0001_create_users.sql", exception.Message, StringComparison.Ordinal);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task Running_the_migrator_applies_pending_scripts_in_order_and_records_each_once()
    {
        WriteScript("0002_create_tasks.sql", "CREATE TABLE tasks ();");
        WriteScript("0001_create_users.sql", "CREATE TABLE users ();");
        var store = new RecordingMigrationStore();
        var migrator = new SchemaMigrator(store);

        await migrator.RunAsync(_directory, CancellationToken.None);

        Assert.Equal(1, store.InitializeCalls);
        Assert.Equal(["0001_create_users.sql", "0002_create_tasks.sql"], store.Applied);
        Assert.Equal(["0001_create_users.sql", "0002_create_tasks.sql"], store.RecordedVersions);
    }

    [Fact]
    public async Task Running_the_migrator_skips_versions_that_are_already_recorded()
    {
        WriteScript("0001_create_users.sql", "CREATE TABLE users ();");
        WriteScript("0002_create_tasks.sql", "CREATE TABLE tasks ();");
        WriteScript("0003_add_index.sql", "CREATE INDEX ix ON tasks (id);");
        var store = new RecordingMigrationStore(["0001_create_users.sql", "0003_add_index.sql"]);
        var migrator = new SchemaMigrator(store);

        await migrator.RunAsync(_directory, CancellationToken.None);

        Assert.Equal(["0002_create_tasks.sql"], store.Applied);
    }

    private string WriteScript(string fileName, string content)
    {
        var path = Path.Combine(_directory, fileName);
        File.WriteAllText(path, content);
        return path;
    }

    private sealed class RecordingMigrationStore : IMigrationStore
    {
        private readonly HashSet<string> _alreadyRecorded;

        internal RecordingMigrationStore(IEnumerable<string>? alreadyRecorded = null) =>
            _alreadyRecorded = alreadyRecorded is null ? [] : [.. alreadyRecorded];

        internal int InitializeCalls { get; private set; }

        internal List<string> Applied { get; } = [];

        internal List<string> RecordedVersions { get; } = [];

        public Task EnsureInitializedAsync(CancellationToken cancellationToken)
        {
            InitializeCalls++;
            return Task.CompletedTask;
        }

        public Task<IReadOnlySet<string>> GetAppliedVersionsAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlySet<string>>(_alreadyRecorded);

        public Task ApplyAsync(MigrationScript script, CancellationToken cancellationToken)
        {
            Applied.Add(script.Version);
            RecordedVersions.Add(script.Version);
            _alreadyRecorded.Add(script.Version);
            return Task.CompletedTask;
        }
    }
}
