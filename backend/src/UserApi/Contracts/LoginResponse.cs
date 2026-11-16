using BallastLane.Application.Users;

namespace BallastLane.UserApi.Contracts;

/// <summary>The bearer credential issued by a successful login.</summary>
public sealed record LoginResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAtUtc)
{
    public static LoginResponse From(LoginView view) =>
        new(view.AccessToken, view.TokenType, view.ExpiresAtUtc);
}
