namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// Raised when a migration file cannot be read. The message names the file so an operator can act on
/// it without reading a driver stack trace.
/// </summary>
public sealed class MigrationLoadException : Exception
{
    public MigrationLoadException(string fileName, Exception innerException)
        : base($"The migration file '{fileName}' could not be read.", innerException)
    {
        FileName = fileName;
    }

    public string FileName { get; }
}
