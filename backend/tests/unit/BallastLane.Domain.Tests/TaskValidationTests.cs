using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;
using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;

namespace BallastLane.Domain.Tests;

public sealed class TaskValidationTests
{
    private static readonly UserId Owner = UserId.New();

    private static readonly DateOnly DueDate = new(2026, 11, 20);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n ")]
    public void A_blank_or_whitespace_only_title_is_rejected(string title)
    {
        Assert.False(TaskDraft.TryCreate(title, null, null, DueDate, out _));
        Assert.Throws<ArgumentException>(() => TaskDraft.Create(title, null, null, DueDate));
    }

    [Fact]
    public void A_missing_title_is_rejected()
    {
        Assert.False(TaskDraft.TryCreate(null, null, null, DueDate, out _));
    }

    [Fact]
    public void A_200_character_title_is_accepted_and_201_is_rejected()
    {
        Assert.True(TaskDraft.TryCreate(new string('a', 200), null, null, DueDate, out _));
        Assert.False(TaskDraft.TryCreate(new string('a', 201), null, null, DueDate, out _));
    }

    [Fact]
    public void A_title_is_trimmed_before_it_is_stored()
    {
        var draft = TaskDraft.Create("   Write the specification   ", null, null, DueDate);

        Assert.Equal("Write the specification", draft.Title);
    }

    [Fact]
    public void A_2000_character_description_is_accepted_and_2001_is_rejected()
    {
        Assert.True(TaskDraft.TryCreate("Title", new string('b', 2000), null, DueDate, out _));
        Assert.False(TaskDraft.TryCreate("Title", new string('b', 2001), null, DueDate, out _));
    }

    [Fact]
    public void A_blank_description_is_stored_as_absent()
    {
        var draft = TaskDraft.Create("Title", "   ", null, DueDate);

        Assert.Null(draft.Description);
    }

    [Fact]
    public void An_omitted_status_defaults_to_Pending()
    {
        var draft = TaskDraft.Create("Title", null, null, DueDate);

        Assert.Equal(TaskStatus.Pending, draft.Status);
    }

    [Fact]
    public void An_unrecognized_status_text_is_rejected()
    {
        Assert.False(TaskDraft.TryCreate("Title", null, "Done", DueDate, out _));
        Assert.False(TaskDraft.TryCreate("Title", null, "completed", DueDate, out _));
    }

    [Fact]
    public void Each_of_the_three_documented_statuses_is_accepted()
    {
        foreach (var status in new[] { "Pending", "InProgress", "Completed" })
        {
            Assert.True(TaskDraft.TryCreate("Title", null, status, DueDate, out var draft));
            Assert.Equal(status, draft.Status.ToString());
        }
    }

    [Fact]
    public void A_draft_cannot_carry_an_owner()
    {
        // The type has no owner member, so ownership cannot come from a request payload.
        Assert.DoesNotContain(
            typeof(TaskDraft).GetProperties(),
            property => property.Name.Contains("Owner", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(
            typeof(TaskDraft).GetProperties(),
            property => property.Name.Contains("User", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void A_past_due_date_is_accepted()
    {
        var past = new DateOnly(2020, 1, 1);

        var draft = TaskDraft.Create("Title", null, null, past);

        Assert.Equal(past, draft.DueDate);
    }

    [Fact]
    public void Creating_a_task_assigns_the_supplied_owner_and_the_draft_values()
    {
        var draft = TaskDraft.Create("Title", "Description", "InProgress", DueDate);
        var createdAt = new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero);

        var task = TaskItem.Create(Owner, draft, createdAt);

        Assert.Equal(Owner, task.OwnerId);
        Assert.Equal("Title", task.Title);
        Assert.Equal("Description", task.Description);
        Assert.Equal(TaskStatus.InProgress, task.Status);
        Assert.Equal(DueDate, task.DueDate);
        Assert.Equal(createdAt, task.CreatedAt);
        Assert.Equal(createdAt, task.UpdatedAt);
    }
}

