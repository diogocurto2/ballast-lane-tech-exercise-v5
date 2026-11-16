using BallastLane.Application.Outcomes;

namespace BallastLane.Application.Users;

/// <summary>
/// Shape and business-rule validation for registration and login input. Rules live here so a
/// controller never contains one.
/// </summary>
public sealed class UserValidator
{
    public const int MinimumPasswordLength = 8;

    public IReadOnlyList<FieldError> ValidateRegistration(string? email, string? password)
    {
        var errors = new List<FieldError>();

        if (!Domain.Users.Email.TryCreate(email, out _))
        {
            errors.Add(new FieldError("email", "A valid email address is required."));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add(new FieldError("password", "A password is required."));
        }
        else if (password.Length < MinimumPasswordLength)
        {
            errors.Add(new FieldError(
                "password",
                $"The password must be at least {MinimumPasswordLength} characters long."));
        }

        return errors;
    }

    public IReadOnlyList<FieldError> ValidateLogin(string? email, string? password)
    {
        var errors = new List<FieldError>();

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(new FieldError("email", "An email address is required."));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add(new FieldError("password", "A password is required."));
        }

        return errors;
    }
}
