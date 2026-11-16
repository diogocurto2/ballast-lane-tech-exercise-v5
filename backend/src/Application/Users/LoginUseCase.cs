using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Users;

/// <summary>An issued bearer credential as the API exposes it.</summary>
public sealed record LoginView(string AccessToken, string TokenType, DateTimeOffset ExpiresAtUtc);

/// <summary>
/// Authenticates a user. Both an unknown email and a wrong password return the same generic failure,
/// so the endpoint cannot be used to discover which addresses are registered.
/// </summary>
public sealed class LoginUseCase(
    UserValidator validator,
    IUserReader userReader,
    IPasswordHasher passwordHasher,
    ITokenIssuer tokenIssuer)
{
    private const string GenericFailure = "Invalid email or password.";

    public async Task<OperationResult<LoginView>> ExecuteAsync(
        string? email,
        string? password,
        CancellationToken cancellationToken)
    {
        var errors = validator.ValidateLogin(email, password);
        if (errors.Count > 0)
        {
            return OperationResult<LoginView>.Validation(errors);
        }

        var user = await userReader.FindByEmailAsync(Email.Normalize(email), cancellationToken);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, password!))
        {
            return OperationResult<LoginView>.Unauthenticated(GenericFailure);
        }

        var token = tokenIssuer.Issue(user.Id, user.Email.Value);
        return OperationResult<LoginView>.Success(
            new LoginView(token.AccessToken, token.TokenType, token.ExpiresAtUtc));
    }
}
