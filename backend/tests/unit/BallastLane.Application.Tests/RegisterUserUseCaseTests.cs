using BallastLane.Application.Abstractions;
using BallastLane.Application.Outcomes;
using BallastLane.Application.Users;
using BallastLane.Domain.Users;

namespace BallastLane.Application.Tests;

public sealed class RegisterUserUseCaseTests
{
    private readonly InMemoryUserStore _users = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly FixedTimeProvider _time = new(new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero));

    [Fact]
    public async Task Registering_stores_a_hash_and_never_the_plaintext_password()
    {
        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync("new.user@example.test", "ValidPassw0rd!", CancellationToken.None);

        Assert.True(result.IsSuccess);
        var stored = Assert.Single(_users.Users);
        Assert.NotEqual("ValidPassw0rd!", stored.PasswordHash);
        Assert.DoesNotContain("ValidPassw0rd!", stored.PasswordHash, StringComparison.Ordinal);
        Assert.True(_hasher.Verify(stored.PasswordHash, "ValidPassw0rd!"));
        Assert.Equal("new.user@example.test", stored.Email.Value);
        Assert.Equal("new.user@example.test", result.Value!.Email);
        Assert.Equal(_time.UtcNow, result.Value.CreatedAt);
    }

    [Fact]
    public async Task Registering_an_email_that_already_exists_yields_a_conflict()
    {
        var useCase = CreateUseCase();
        await useCase.ExecuteAsync("taken@example.test", "ValidPassw0rd!", CancellationToken.None);

        var result = await useCase.ExecuteAsync("taken@example.test", "AnotherPassw0rd!", CancellationToken.None);

        Assert.Equal(OperationStatus.Conflict, result.Status);
        Assert.Single(_users.Users);
    }

    [Fact]
    public async Task An_email_that_differs_only_in_case_is_treated_as_a_duplicate()
    {
        var useCase = CreateUseCase();
        await useCase.ExecuteAsync("Case.User@Example.Test", "ValidPassw0rd!", CancellationToken.None);

        var result = await useCase.ExecuteAsync("case.user@example.test", "AnotherPassw0rd!", CancellationToken.None);

        Assert.Equal(OperationStatus.Conflict, result.Status);
        Assert.Single(_users.Users);
    }

    [Theory]
    [InlineData("short12")]           // 7 characters
    [InlineData("")]
    public async Task A_password_below_the_policy_yields_a_validation_outcome(string password)
    {
        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync("new.user@example.test", password, CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        var error = Assert.Single(result.Errors);
        Assert.Equal("password", error.Field);
        Assert.Empty(_users.Users);
    }

    [Fact]
    public async Task An_eight_character_password_is_accepted()
    {
        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync("new.user@example.test", "Passw0rd", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(_users.Users);
    }

    [Fact]
    public async Task A_malformed_email_yields_a_field_level_validation_outcome()
    {
        var useCase = CreateUseCase();

        var result = await useCase.ExecuteAsync("not-an-email", "ValidPassw0rd!", CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        Assert.Contains(result.Errors, error => error.Field == "email");
        Assert.Empty(_users.Users);
    }

    [Fact]
    public async Task A_race_lost_at_the_database_translates_to_a_conflict()
    {
        var store = new InMemoryUserStore { RejectNextInsert = true };
        var useCase = new RegisterUserUseCase(
            new UserValidator(),
            store,
            store,
            _hasher,
            _time);

        var result = await useCase.ExecuteAsync("new.user@example.test", "ValidPassw0rd!", CancellationToken.None);

        Assert.Equal(OperationStatus.Conflict, result.Status);
    }

    private RegisterUserUseCase CreateUseCase() =>
        new(new UserValidator(), _users, _users, _hasher, _time);

    private sealed class InMemoryUserStore : IUserReader, IUserWriter
    {
        internal List<User> Users { get; } = [];

        internal bool RejectNextInsert { get; init; }

        public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken) =>
            Task.FromResult(Users.SingleOrDefault(
                user => string.Equals(user.Email.Value, email, StringComparison.OrdinalIgnoreCase)));

        public Task<User?> FindByIdAsync(UserId userId, CancellationToken cancellationToken) =>
            Task.FromResult(Users.SingleOrDefault(user => user.Id == userId));

        public Task<bool> AddAsync(User user, CancellationToken cancellationToken)
        {
            if (RejectNextInsert)
            {
                return Task.FromResult(false);
            }

            Users.Add(user);
            return Task.FromResult(true);
        }
    }
}
