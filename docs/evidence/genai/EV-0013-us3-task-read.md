# EV-0013 - User Story 3: list and view personal tasks (001-task-manager)

> **Revision note (2026-11-16, after `EV-0024`).** Paths in this record reflect the repository layout
> at the time of the activity, when the backend lived at the root as `src/`, `tests/`, and `db/`. After
> the `backend/` restructure recorded in [EV-0024](./EV-0024-backend-folder-restructure.md), those
> locations are `backend/src/`, `backend/tests/`, and `backend/db/`. Nothing else in this record was
> altered, so it remains an accurate account of what was observed when it was written.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0013` |
| Date | `2026-11-16` |
| Development phase | Phase 6 - Implement (User Story 3, T045 to T057) |
| Activity type | `Code generation` |
| Related specification | `specs/001-task-manager/spec.md` (US3, FR-005, FR-007, FR-011, PDC-05) |
| Related task identifiers | `T045` to `T057` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `PENDING` |

## Actual prompt

Continuation of the developer instruction that opened the session:

```text
/speckit-implement
```

`tasks.md` T045 to T057 defined the behavior; `contracts/task-api.md` defined the read contract.

## Relevant context

- Requirements: US3 acceptance scenarios 1 to 5, FR-005, FR-007, FR-011, PDC-05, SC-003.
- Standards: `database-standards.md` (2.2, 2.9), `api-standards.md`, `frontend-standards.md` (2.7),
  `testing-and-tdd.md`.
- Preceding records: `EV-0010`, `EV-0011`, `EV-0012`.

## Original AI-generated output

Compile-only contracts were created first. Two of them are preserved here before correction:

```csharp
// src/Domain/Tasks/TaskId.cs - original stub
public static bool TryParse(string? value, out TaskId taskId)
{
    taskId = default;
    return false;
}
```

```csharp
// src/Application/Tasks/ListAndGetTasksUseCases.cs - original stub
public Task<OperationResult<IReadOnlyList<TaskItem>>> ExecuteAsync(
    AuthenticatedUser identity,
    CancellationToken cancellationToken) =>
    Task.FromResult(OperationResult<IReadOnlyList<TaskItem>>.Success([]));
```

```csharp
// frontend/src/components/TaskList.tsx - original stub
export function TaskList(_props: TaskListProps) {
  return <div />;
}
```

The first `TaskStatus` stub contained only `Pending` and `InProgress`; the tests reference
`Completed`, so the suite failed to compile (`error CS0117: 'TaskStatus' does not contain a
definition for 'Completed'`). A compilation failure is not valid RED evidence, so the enum was
completed to all three documented values before the RED run.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | `ITaskReader` is application-owned; `NpgsqlTaskRepository` implements it; the controller delegates to one use case per action | `[VERIFIED]` |
| SOLID | Owner scoping is a parameter of the boundary rather than a convention; a missing row returns `null` as the interface declares, so the adapter satisfies Liskov substitution | `[VERIFIED]` |
| TDD | RED observed for every suite: 1 domain, 3 application, 8 infrastructure unit, 3 PostgreSQL, 6 HTTP, and 6 frontend failures, all assertion failures | `[VERIFIED]` |
| Security | Every read is filtered by `owner_id`; another user's task returns `404` with no data; the owner is not exposed in the representation | `[VERIFIED]` |
| Functional requirements | FR-005 (read half), FR-007, FR-011 (loading, empty, success, error) and PDC-05 ordering verified | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` `TaskStatus` collides with `System.Threading.Tasks.TaskStatus` through implicit
  usings. Rather than renaming the domain type away from the plan, an explicit
  `using TaskStatus = BallastLane.Domain.Tasks.TaskStatus;` alias is used in the files that name it.
- `[VERIFIED]` A non-UUID identifier is answered with `404`, not `400`, because it cannot identify a
  resource; the contract documents this choice.
- `[VERIFIED]` Two test-infrastructure defects appeared only when the whole solution ran at once:
  three test assemblies truncated and asserted on the same database, and two `CREATE DATABASE`
  statements contended on the template lock. Running each project alone passed, so the collision was
  real concurrency interference rather than a product defect.
- `[VERIFIED]` Fixes: each suite now derives its own database name from the configured connection
  string (`_userapi`, `_taskapi`, `_migrations`, `_postgres`), `CREATE DATABASE` is serialized with a
  session advisory lock, and the test connection strings use a 60-second connect timeout.
- `[VERIFIED]` One unit test asserted `null` from a recording factory that had no reader configured;
  the fake threw instead. The test now supplies an empty reader, which is what "no row" means.
- `[VERIFIED]` The frontend task pages are reachable through `/tasks` and `/tasks/:id`, both behind
  `RequireAuth`.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Keep the domain type named `TaskStatus` and alias it where needed | The plan and data model name it that; a rename would diverge from the design for a namespace coincidence | `[AI-PROPOSED]` |
| One database per test suite | Test independence is constitutional; sharing one database made the suite order-dependent | `[AI-PROPOSED]` |
| Advisory lock around `CREATE DATABASE` | PostgreSQL serializes DDL on the template database; a cross-process lock makes the suite deterministic | `[AI-PROPOSED]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Completed `TaskStatus` before the RED run | The stub caused a compilation failure, not a behavioral one | `testing-and-tdd.md` 2.2 excludes build failures as RED evidence |
| Added the `TaskStatus` alias | Ambiguity with `System.Threading.Tasks.TaskStatus` | Keeps the documented domain name without renaming |
| Per-suite test databases, advisory lock, 60-second connect timeout | Concurrent assemblies interfered | Tests must be independent and reproducible (`testing-and-tdd.md` 2.7) |
| Replaced the null-reader fake with an empty reader | The fake threw where the contract says "no row" | The test must express the absence of a row, not a missing dependency |

## Commands and observed results

### RED

```text
dotnet test tests\unit\BallastLane.Domain.Tests         -> Failed: 1, Passed: 16
dotnet test tests\unit\BallastLane.Application.Tests    -> Failed: 3, Passed: 16
dotnet test tests\unit\BallastLane.Infrastructure.UnitTests -> Failed: 8, Passed: 10
dotnet test tests\integration\BallastLane.PostgreSql.Tests  -> Failed: 3, Passed: 4
dotnet test tests\http\BallastLane.TaskApi.HttpTests        -> Failed: 6, Passed: 9

cd frontend; npx vitest run
Test Files  2 failed | 3 passed (5)
     Tests  6 failed | 9 passed (15)
```

### Concurrency defect reproduced

```text
dotnet test BallastLane.sln --no-build
Failed!  - Failed: 1, Passed: 14, Total: 15   BallastLane.TaskApi.HttpTests.dll
Failed!  - Failed: 1, Passed: 5,  Total: 6    BallastLane.Migrations.Tests.dll
Failed!  - Failed: 1, Passed: 21, Total: 22   BallastLane.UserApi.HttpTests.dll
```

Each project then passed when run alone, which identified the interference as cross-suite rather
than a product defect.

### GREEN

```text
dotnet build BallastLane.sln        -> Build succeeded. 0 Warning(s) 0 Error(s)
dotnet test BallastLane.sln         -> 131 passed, 0 failed across nine assemblies

BallastLane.Api.UnitTests            18
BallastLane.Application.Tests        19
BallastLane.Domain.Tests             17
BallastLane.Architecture.Tests        9
BallastLane.Infrastructure.UnitTests 18
BallastLane.PostgreSql.Tests          7
BallastLane.TaskApi.HttpTests        15
BallastLane.Migrations.Tests          6
BallastLane.UserApi.HttpTests        22

cd frontend; npx vitest run         -> Test Files 5 passed, Tests 15 passed
cd frontend; npm run build          -> built in 1.66s
cd frontend; npm run lint           -> no findings
```

## Final references

- Implementation files: `src/Domain/Tasks/{TaskItem,TaskId,TaskStatus}.cs`,
  `src/Application/Abstractions/ITaskReader.cs`,
  `src/Application/Tasks/ListAndGetTasksUseCases.cs`,
  `src/Infrastructure/Persistence/NpgsqlTaskRepository.cs`,
  `src/TaskApi/{Program.cs,Controllers/TasksController.cs,Contracts/TaskResponse.cs}`,
  `frontend/src/api/{tasks,authToken}.ts`, `frontend/src/components/{TaskList,TaskDetail,Feedback}.tsx`,
  `frontend/src/pages/{TasksPage,TaskDetailPage}.tsx`, `frontend/src/state/tasksReducer.ts`,
  `frontend/src/App.tsx`
- Test files: `tests/unit/BallastLane.Domain.Tests/TaskEntityTests.cs`,
  `tests/unit/BallastLane.Application.Tests/ListAndGetTasksUseCaseTests.cs`,
  `tests/unit/BallastLane.Infrastructure.UnitTests/{NpgsqlTaskRepositoryReadTests,RecordingConnectionFactory}.cs`,
  `tests/integration/BallastLane.PostgreSql.Tests/{TaskReadTests,PostgreSqlDatabase}.cs`,
  `tests/http/BallastLane.TaskApi.HttpTests/TaskReadTests.cs`,
  `frontend/tests/components/{TaskListStates,TaskDetail}.test.tsx`
- Task identifiers: `T045` to `T057`
- Commits: `PENDING`

## Evidence status rationale

Status is `Captured`. Developer review and a commit reference remain outstanding. The concurrency
defect and the invalid-RED stub are recorded because they happened, including the corrective steps.
