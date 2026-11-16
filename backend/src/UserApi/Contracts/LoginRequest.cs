namespace BallastLane.UserApi.Contracts;

/// <summary>
/// Login payload. Field rules live in the application validator so a 400 body always uses the same
/// field keys the contract documents.
/// </summary>
public sealed class LoginRequest
{
    public string? Email { get; init; }

    public string? Password { get; init; }
}
