using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Users;

/// <summary>
/// Resolves the caller's own account from a claim-derived identity. There is no parameter that could
/// select another user.
/// </summary>
public sealed class GetCurrentUserUseCase(IUserReader userReader)
{
    public async Task<OperationResult<UserView>> ExecuteAsync(
        AuthenticatedUser identity,
        CancellationToken cancellationToken)
    {
        var user = await userReader.FindByIdAsync(identity.UserId, cancellationToken);

        // A token whose account no longer exists is treated as unauthenticated rather than not found,
        // because the credential itself can no longer be honoured.
        if (user is null)
        {
            return OperationResult<UserView>.Unauthenticated("The authenticated account was not found.");
        }

        return OperationResult<UserView>.Success(
            new UserView(user.Id.Value, user.Email.Value, user.CreatedAt));
    }
}
