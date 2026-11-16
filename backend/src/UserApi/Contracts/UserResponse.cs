using BallastLane.Application.Users;

namespace BallastLane.UserApi.Contracts;

/// <summary>Account representation. Contains no password or hash.</summary>
public sealed record UserResponse(string Id, string Email, DateTimeOffset CreatedAt)
{
    public static UserResponse From(UserView view) =>
        new(view.Id.ToString(), view.Email, view.CreatedAt);
}
