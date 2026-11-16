using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Users;

/// <summary>
/// A registered account as the API exposes it. Deliberately omits the password hash so a credential
/// cannot leak through a response.
/// </summary>
public sealed record UserView(Guid Id, string Email, DateTimeOffset CreatedAt);

/// <summary>
/// Registers an account: validates the input, normalizes the email, hashes the password, and reports
/// a conflict when the database already holds the email.
/// </summary>
public sealed class RegisterUserUseCase(
    UserValidator validator,
    IUserReader userReader,
    IUserWriter userWriter,
    IPasswordHasher passwordHasher,
    ITimeProvider timeProvider)
{
    public async Task<OperationResult<UserView>> ExecuteAsync(
        string? email,
        string? password,
        CancellationToken cancellationToken)
    {
        var errors = validator.ValidateRegistration(email, password);
        if (errors.Count > 0)
        {
            return OperationResult<UserView>.Validation(errors);
        }

        var normalizedEmail = Email.Create(email);

        // The database's unique index is the authority; this lookup only avoids a pointless insert.
        var existing = await userReader.FindByEmailAsync(normalizedEmail.Value, cancellationToken);
        if (existing is not null)
        {
            return OperationResult<UserView>.Conflict("An account already exists for this email address.");
        }

        var user = new User(
            UserId.New(),
            normalizedEmail,
            passwordHasher.Hash(password!),
            timeProvider.UtcNow);

        if (!await userWriter.AddAsync(user, cancellationToken))
        {
            return OperationResult<UserView>.Conflict("An account already exists for this email address.");
        }

        return OperationResult<UserView>.Success(new UserView(user.Id.Value, user.Email.Value, user.CreatedAt));
    }
}
