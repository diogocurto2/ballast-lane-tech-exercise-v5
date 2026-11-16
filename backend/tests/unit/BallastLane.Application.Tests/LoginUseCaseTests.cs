using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Application.Users;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Tests;

public sealed class LoginUseCaseTests
{
    private const string Email = "demo.user@example.test";
    private const string Password = "ValidPassw0rd!";

    private readonly InMemoryUserStore _users = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly FakeTokenIssuer _issuer = new();

    [Fact]
    public async Task Valid_credentials_yield_a_token()
    {
        var useCase = await CreateUseCaseWithRegisteredUserAsync();

        var result = await useCase.ExecuteAsync(Email, Password, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("issued-token", result.Value!.AccessToken);
        Assert.Equal("Bearer", result.Value.TokenType);
        Assert.Equal(_issuer.ExpiresAtUtc, result.Value.ExpiresAtUtc);
        Assert.Equal(1, _issuer.IssueCount);
    }

    [Fact]
    public async Task Credentials_are_matched_case_insensitively_for_the_email()
    {
        var useCase = await CreateUseCaseWithRegisteredUserAsync();

        var result = await useCase.ExecuteAsync("DEMO.USER@Example.Test", Password, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task A_wrong_password_and_an_unknown_email_produce_the_same_generic_failure()
    {
        var useCase = await CreateUseCaseWithRegisteredUserAsync();

        var wrongPassword = await useCase.ExecuteAsync(Email, "WrongPassw0rd!", CancellationToken.None);
        var unknownEmail = await useCase.ExecuteAsync("nobody@example.test", Password, CancellationToken.None);

        Assert.Equal(OperationStatus.Unauthenticated, wrongPassword.Status);
        Assert.Equal(wrongPassword.Status, unknownEmail.Status);
        Assert.Equal(wrongPassword.Detail, unknownEmail.Detail);
        Assert.Equal("Invalid email or password.", wrongPassword.Detail);
        Assert.Empty(wrongPassword.Errors);
        Assert.Empty(unknownEmail.Errors);
        Assert.Equal(0, _issuer.IssueCount);
    }

    [Theory]
    [InlineData("", Password)]
    [InlineData(Email, "")]
    public async Task A_missing_field_yields_a_validation_outcome(string email, string password)
    {
        var useCase = await CreateUseCaseWithRegisteredUserAsync();

        var result = await useCase.ExecuteAsync(email, password, CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        Assert.NotEmpty(result.Errors);
    }

    private async Task<LoginUseCase> CreateUseCaseWithRegisteredUserAsync()
    {
        var register = new RegisterUserUseCase(
            new UserValidator(),
            _users,
            _users,
            _hasher,
            new FixedTimeProvider(new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero)));

        await register.ExecuteAsync(Email, Password, CancellationToken.None);

        return new LoginUseCase(new UserValidator(), _users, _hasher, _issuer);
    }

    private sealed class InMemoryUserStore : IUserReader, IUserWriter
    {
        internal List<User> Users { get; } = [];

        public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken) =>
            Task.FromResult(Users.SingleOrDefault(
                user => string.Equals(user.Email.Value, email, StringComparison.OrdinalIgnoreCase)));

        public Task<User?> FindByIdAsync(UserId userId, CancellationToken cancellationToken) =>
            Task.FromResult(Users.SingleOrDefault(user => user.Id == userId));

        public Task<bool> AddAsync(User user, CancellationToken cancellationToken)
        {
            Users.Add(user);
            return Task.FromResult(true);
        }
    }
}
