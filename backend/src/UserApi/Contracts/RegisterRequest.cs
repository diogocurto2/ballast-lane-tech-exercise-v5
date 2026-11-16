namespace BallastLane.UserApi.Contracts;

/// <summary>
/// Registration payload. Only the two fields the contract defines are bound; any additional field the
/// client sends, including an ownership or role field, is ignored. Field rules live in the application
/// validator so a 400 body always uses the same field keys the contract documents.
/// </summary>
public sealed class RegisterRequest
{
    public string? Email { get; init; }

    public string? Password { get; init; }
}
