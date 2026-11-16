using BallastLane.Domain.Users;

namespace BallastLane.Domain.Tests;

public sealed class UserTests
{
    [Theory]
    [InlineData("demo.user@example.test", "demo.user@example.test")]
    [InlineData("Demo.User@Example.Test", "demo.user@example.test")]
    [InlineData("  mixed.CASE@Example.Test  ", "mixed.case@example.test")]
    public void An_email_address_is_normalized_to_lower_case_without_surrounding_whitespace(
        string input,
        string expected)
    {
        Assert.Equal(expected, Email.Normalize(input));
        Assert.Equal(expected, Email.Create(input).Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("no-at-sign.example.test")]
    [InlineData("@example.test")]
    [InlineData("user@")]
    [InlineData("two@at@example.test")]
    [InlineData("user name@example.test")]
    [InlineData("user@example")]
    public void A_malformed_email_address_is_rejected(string input)
    {
        Assert.False(Email.TryCreate(input, out _));
        Assert.Throws<ArgumentException>(() => Email.Create(input));
    }

    [Fact]
    public void An_email_address_at_the_320_character_limit_is_accepted_and_one_longer_is_rejected()
    {
        // Four 63-character labels joined by dots give the 255-character domain maximum.
        var local = new string('a', 64);
        var domain = string.Join('.', Enumerable.Repeat(new string('b', 63), 4));
        var atLimit = $"{local}@{domain}";
        Assert.Equal(320, atLimit.Length);

        Assert.True(Email.TryCreate(atLimit, out _));
        Assert.False(Email.TryCreate($"{atLimit}x", out _));
    }

    [Fact]
    public void A_user_exposes_its_identifier_email_hash_and_creation_time_only()
    {
        var id = UserId.New();
        var createdAt = new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero);

        var user = new User(id, Email.Create("Demo.User@Example.Test"), "hashed-value", createdAt);

        Assert.Equal(id, user.Id);
        Assert.Equal("demo.user@example.test", user.Email.Value);
        Assert.Equal("hashed-value", user.PasswordHash);
        Assert.Equal(createdAt, user.CreatedAt);
    }

    [Fact]
    public void An_authenticated_user_carries_a_user_id_and_email()
    {
        var id = UserId.New();

        var identity = new AuthenticatedUser(id, "demo.user@example.test");

        Assert.Equal(id, identity.UserId);
        Assert.Equal("demo.user@example.test", identity.Email);
    }
}
