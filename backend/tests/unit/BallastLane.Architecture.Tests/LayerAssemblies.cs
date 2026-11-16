using System.Reflection;

namespace BallastLane.Architecture.Tests;

internal static class LayerAssemblies
{
    internal static Assembly Domain { get; } = typeof(Domain.Users.User).Assembly;

    internal static Assembly Application { get; } = typeof(Application.Outcomes.OperationResult<>).Assembly;

    internal static Assembly Infrastructure { get; } = typeof(Infrastructure.Persistence.SchemaMigrator).Assembly;

    internal static Assembly SharedWeb { get; } = typeof(SharedWeb.AuthenticationSetup).Assembly;

    internal static Assembly UserApi { get; } = Load("BallastLane.UserApi");

    internal static Assembly TaskApi { get; } = Load("BallastLane.TaskApi");

    internal static Assembly Migrator { get; } = Load("BallastLane.Migrator");

    private static Assembly Load(string name) => Assembly.Load(name);
}
