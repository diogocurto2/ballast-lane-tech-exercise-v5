namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// One demonstration task. Status is stored as the same textual value the API exposes.
/// </summary>
public sealed record DemoTask(string Title, string? Description, string Status, DateOnly DueDate);

/// <summary>
/// One demonstration account and the tasks seeded for it. The credentials are deliberately public:
/// they are demonstration-only and valid only against the local container.
/// </summary>
public sealed record DemoAccount(string Email, string Password, IReadOnlyList<DemoTask> Tasks);

/// <summary>
/// The demonstration dataset. Fixed identifiers are not required because the seed resolves the owner
/// by email and skips rows that already exist.
/// </summary>
public static class DemonstrationData
{
    public const string PrimaryAccountEmail = "demo.user@example.test";

    public static IReadOnlyList<DemoAccount> Accounts { get; } =
    [
        new(
            PrimaryAccountEmail,
            "DemoPassw0rd!",
            [
                new("Prepare the submission package", "README, GENAI.md, and the evidence records.", "InProgress", new DateOnly(2026, 11, 18)),
                new("Walk through the assessment checklist", null, "Pending", new DateOnly(2026, 11, 20)),
                new("Review the recorded test evidence", "Unit, PostgreSQL, HTTP, and frontend results.", "Completed", new DateOnly(2026, 11, 12)),
            ]),
        new(
            "reviewer@example.test",
            "ReviewPassw0rd!",
            [
                new("Review the task manager solution", "Focus on authentication and ownership.", "Pending", new DateOnly(2026, 11, 21)),
                new("Verify the seeded demonstration", "Log in and confirm the seeded tasks are listed.", "Pending", new DateOnly(2026, 11, 22)),
            ]),
    ];
}
