# EV-0015 - User Story 5: update a task (001-task-manager)

> **Revision note (2026-11-16, after `EV-0025`).** The labels in this record were corrected to
> distinguish the original test-first RED cycle from a mutation check performed after the behavior
> already existed. No observed command output, date, or sequence of events was altered; only the
> wording that described what kind of check each failure was. The activity is otherwise unchanged.
>
> **Revision note (2026-11-16, after `EV-0024`).** Paths in this record reflect the repository layout
> at the time of the activity, when the backend lived at the root as `src/`, `tests/`, and `db/`. After
> the `backend/` restructure recorded in [EV-0024](./EV-0024-backend-folder-restructure.md), those
> locations are `backend/src/`, `backend/tests/`, and `backend/db/`. Nothing else in this record was
> altered, so it remains an accurate account of what was observed when it was written.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0015` |
| Date | `2026-11-16` |
| Development phase | Phase 6 - Implement (User Story 5, T070 to T080) |
| Activity type | `Code generation` |
| Related specification | `specs/001-task-manager/spec.md` (US5, FR-006, FR-007, FR-013, FR-014, PDC-06) |
| Related task identifiers | `T070` to `T080` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9cd6b3a` - `feat(web): implement the React task manager client`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires. |

## Actual prompt

Continuation of the developer instruction that opened the session:

```text
/speckit-implement
```

`tasks.md` T070 to T080 and the `PUT /api/tasks/{id}` section of `contracts/task-api.md` defined the
behavior.

## Relevant context

- Requirements: US5 acceptance scenarios 1 to 5, FR-006, FR-007, FR-013 (any transition), FR-014,
  PDC-06 (one operation updates all editable values).
- Standards: `api-standards.md`, `database-standards.md` 2.2, `security.md` 2.3, `frontend-standards.md`.
- Preceding records: `EV-0013`, `EV-0014`.

## Original AI-generated output

```csharp
// src/Application/Tasks/UpdateTaskUseCase.cs - original stub
public Task<OperationResult<TaskItem>> ExecuteAsync(
    AuthenticatedUser identity, TaskId taskId, TaskInput input, CancellationToken cancellationToken) =>
    Task.FromResult(OperationResult<TaskItem>.NotFound("Task was not found."));
```

```csharp
// src/Infrastructure/Persistence/NpgsqlTaskRepository.cs - original stub
public Task<bool> UpdateAsync(TaskItem task, CancellationToken cancellationToken) =>
    Task.FromResult(false);
```

The frontend edit mode was generated as part of the US4 form, so all four US5 frontend tests passed
at the first run. No test-first RED cycle exists for that frontend behavior: the tests were written
after the behavior already existed. Afterwards, edit mode was temporarily disabled
(`const isEdit = false;` and the edit branch of `initialValues` unreachable), the four tests were
observed failing, and the implementation was restored. That observed failure is a
**post-implementation mutation check** - it demonstrates that the tests can fail - and it is not a
RED checkpoint in the TDD sequence.

The first `UpdateTaskUseCase` also reused `TaskValidator.Validate(input)`, which treats an omitted
status as `Pending` because that is correct for creation. The HTTP suite caught the consequence: a
`PUT` without a status returned `200` and silently reset the status instead of `400`.

## Validation performed

| Area | Which was checked | Result |
| --- | --- | --- |
| Clean Architecture | The use case loads through the owner-scoped reader, applies a validated draft, and delegates persistence; the controller binds and maps only | `[VERIFIED]` |
| SOLID | `TaskItem with { ... }` replaces editable values while carrying identity, owner, and creation time, so an update cannot move a task between users | `[VERIFIED]` |
| TDD | Backend: test-first RED observed as 13 application and 15 HTTP failures from stubs written before the implementation. Frontend: no test-first RED exists; a post-implementation mutation check produced 4 failures when edit mode was disabled | `[VERIFIED]` |
| Security | The `UPDATE` statement uses `owner_id` only in the `WHERE` clause; a cross-user update returns `404` and leaves the row unchanged; a supplied `ownerId` cannot change ownership | `[VERIFIED]` |
| Functional requirements | All six status transitions in both directions, every editable field, unchanged `id`/owner/`createdAt`, and `updatedAt` advancing are asserted | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` The shared validator's create semantics (`status` optional, defaults to Pending) are
  wrong for update. `Validate(input, requireStatus: true)` now reports `status` as required on the
  update path, which is what the contract says.
- `[VERIFIED]` The repository update returns `affected == 1`, so a row that no longer matches the
  owner scope is reported as not-found instead of being silently treated as a success.
- `[VERIFIED]` An invalid edit is rejected before the row is read, and an invalid payload after
  validation cannot reach the writer, so a rejected update leaves the stored task untouched
  (asserted by re-reading `title` and `status` from the database).

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| `requireStatus` flag on the validator rather than a second validator | One rule set, one error vocabulary, two documented call sites | `[AI-PROPOSED]` |
| Disable edit mode after the fact to prove the frontend tests can fail | The first run passed, which proves nothing about the tests; the failure observed this way is a mutation check, not a RED checkpoint | `[AI-PROPOSED]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Added `requireStatus` to `TaskValidator.Validate` | A `PUT` without a status silently reset it | `contracts/task-api.md` states the status is required on update |
| Repository update scoped by `id` and `owner_id` with an affected-row check | Cross-user and stale identifiers must be not-found, not success | Resource-level authorization in `security.md` 2.3 |
| Disabled edit mode after the frontend tests had already passed, then restored it | The tests must demonstrably fail without the behavior; recorded as a mutation check rather than a RED cycle | `testing-and-tdd.md` 2.2 |

## Commands and observed results

### RED (backend, test-first)

The stubs in the "Original AI-generated output" section were written before the implementation, and
these failures are the observed RED for the backend behavior.

```text
dotnet test tests\unit\BallastLane.Application.Tests   -> Failed: 13, Passed: 31
dotnet test tests\http\BallastLane.TaskApi.HttpTests   -> Failed: 15, Passed: 28
```

### Post-implementation mutation check (frontend, not a RED)

The four US5 frontend tests passed before this step, because edit mode already existed from US4. The
failure below was produced afterwards by disabling that behavior, so it is recorded as a mutation
check and not as a RED checkpoint.

```text
cd frontend; npx vitest run tests/components/TaskFormEdit.test.tsx
Test Files  1 failed (1)
     Tests  4 failed (4)
```

### GREEN

```text
dotnet build BallastLane.sln -> Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test tests\unit\BallastLane.Application.Tests -> Passed: 44
dotnet test tests\http\BallastLane.TaskApi.HttpTests -> Passed: 43

cd frontend; npx vitest run -> Test Files 7 passed, Tests 23 passed
cd frontend; npm run build  -> built in 1.68s
cd frontend; npm run lint   -> no findings
```

## Final references

- Implementation files: `src/Application/Tasks/{UpdateTaskUseCase,TaskValidator}.cs`,
  `src/Application/Abstractions/ITaskWriter.cs`,
  `src/Infrastructure/Persistence/NpgsqlTaskRepository.cs` (`UpdateAsync`),
  `src/TaskApi/{Controllers/TasksController.cs,Contracts/UpdateTaskRequest.cs,Program.cs}`,
  `frontend/src/components/{TaskForm,TaskDetail}.tsx`, `frontend/src/pages/TaskEditPage.tsx`,
  `frontend/src/App.tsx`
- Test files: `tests/unit/BallastLane.Application.Tests/UpdateTaskUseCaseTests.cs`,
  `tests/http/BallastLane.TaskApi.HttpTests/TaskUpdateTests.cs`,
  `frontend/tests/components/TaskFormEdit.test.tsx`
- Task identifiers: `T070` to `T080`
- Commits: `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9cd6b3a` - `feat(web): implement the React task manager client`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires.

## Evidence status rationale

Status is `Captured`. Only the backend cycle in this record is a test-first RED. The frontend failure
recorded under "Post-implementation mutation check" was produced after the behavior already existed,
so it is not presented as a RED cycle; both are recorded so the frontend tests are not mistaken for an
unverified first-attempt pass. Developer review and a commit reference remain outstanding.
