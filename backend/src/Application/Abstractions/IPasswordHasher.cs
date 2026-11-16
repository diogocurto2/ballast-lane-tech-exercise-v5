namespace BallastLane.Application.Abstractions;

/// <summary>
/// Hashes and verifies passwords. Declared here so the application layer depends on a boundary rather
/// than on an ASP.NET Core Identity component.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string passwordHash, string password);
}
