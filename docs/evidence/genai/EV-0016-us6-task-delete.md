# EV-0016 - User Story 6: delete a task (001-task-manager)

> **Revision note (2026-11-16, after `EV-0024`).** Paths in this record reflect the repository layout
> at the time of the activity, when the backend lived at the root as `src/`, `tests/`, and `db/`. After
> the `backend/` restructure recorded in [EV-0024](./EV-0024-backend-folder-restructure.md), those
> locations are `backend/src/`, `backend/tests/`, and `backend/db/`. Nothing else in this record was
> altered, so it remains an accurate account of what was observed when it was written.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0016` |
| Date | `2026-11-16` |
| Development phase | Phase 6 - Implement (User Story 6, T081 to T090) |
| Activity type | `Code generation` |
| Related specification | `specs/001-task-manager/spec.md` (US6, FR-005, FR-007, FR-016) |
| Related task identifiers | `T081` to `T090` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9cd6b3a` - `feat(web): implement the React task manager client`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires. |

## Actual prompt

Continuation of the developer instruction that opened the session:

```text
/speckit-implement
```

`tasks.md` T081 to T090 and the `DELETE /api/tasks/{id}` section of `contracts/task-api.md` defined
the behavior.

## Relevant context

- Requirements: US6 acceptance scenarios 1 to 4, FR-005 (delete), FR-007, FR-016.
- Standards: `api-standards.md` (204 on delete), `security.md` 2.3, `frontend-standards.md` 2.8
  (destructive actions require explicit confirmation).
- Preceding record: `EV-0015`.

## Original AI-generated output

```csharp
// src/Application/Tasks/DeleteTaskUseCase.cs - original stub (always succeeds)
public Task<OperationResult<Unit>> ExecuteAsync(
    AuthenticatedUser identity, TaskId taskId, CancellationToken cancellationToken) =>
    Task.FromResult(OperationResult<Unit>.Success(Unit.Value));
```

```csharp
// src/Infrastructure/Persistence/NpgsqlTaskRepository.cs - original stub
public Task<bool> DeleteAsync(TaskId taskId, UserId ownerId, CancellationToken cancellationToken) =>
    Task.FromResult(false);
```

```tsx
// frontend/src/components/TaskDelete.tsx - original stub
export function TaskDelete(_props: TaskDeleteProps) {
  return <div />;
}
```

During implementation an edit replaced the working `UpdateAsync` with a second `DeleteAsync` stub,
producing two identical members. The build failed immediately with `CS0111`-class duplication, and the
method body region was rewritten to hold both `UpdateAsync` and the new `DeleteAsync`.

## Validation performed

| Area | Which was checked | Result |
| --- | --- | --- |
| Clean Architecture | The use case delegates a single owner-scoped delete; the controller maps `Success` to `204` through `ToNoContentResult` | `[VERIFIED]` |
| SOLID | `DeleteAsync` reports success as "exactly one row removed", so the adapter cannot claim a delete it did not perform | `[VERIFIED]` |
| TDD | RED observed: 5 HTTP and 4 frontend failures | `[VERIFIED]` |
| Security | `DELETE ... WHERE id = @id AND owner_id = @ownerId`; a cross-user delete returns `404` and the row survives; an unauthenticated delete removes nothing | `[VERIFIED]` |
| Functional requirements | `204` then `404` on retrieval, `404` on a second delete, sibling tasks untouched, and `404` for unknown or non-UUID identifiers | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` A `204` response must carry no body; the test asserts an empty body explicitly.
- `[VERIFIED]` The frontend requires a confirmation dialog (`role="dialog"`), sends no request when
  cancelled, and keeps the task visible with an error when the delete fails.
- `[VERIFIED]` The list updates through router state (`deletedTaskId`), so the row disappears without a
  refetch.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Confirmation implemented as a `ConfirmDialog` component | Reusable for any future destructive action and testable by role | `[AI-PROPOSED]` |
| Delete result carried to the list through router state | Avoids a refetch while keeping the server authoritative on the next load | `[AI-PROPOSED]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Rewrote the repository method region after a bad edit | An edit replaced `UpdateAsync` with a duplicate `DeleteAsync` | The compiler rejected duplicate members; the region now holds both operations once |
| Delete mapped through `ToNoContentResult` | `204` is the documented success status and must not include a representation | `api-standards.md` deletion convention |

## Commands and observed results

### RED

```text
dotnet test tests\http\BallastLane.TaskApi.HttpTests -> Failed: 5, Passed: 45

cd frontend; npx vitest run tests/components/TaskDelete.test.tsx
Test Files  1 failed (1)
     Tests  4 failed (4)
```

### GREEN

```text
dotnet build BallastLane.sln -> Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test BallastLane.sln
Domain 31 | Application 44 | Infrastructure 18 | Api 18 | Architecture 9
PostgreSql 7 | TaskApi 50 | Migrations 6 | UserApi 22   -> 205 passed, 0 failed

cd frontend; npx vitest run -> Test Files 8 passed, Tests 27 passed
cd frontend; npm run build  -> built in 1.58s
cd frontend; npm run lint   -> no findings
```

## Final references

- Implementation files: `src/Application/Tasks/DeleteTaskUseCase.cs`,
  `src/Application/Abstractions/ITaskWriter.cs`,
  `src/Infrastructure/Persistence/NpgsqlTaskRepository.cs` (`DeleteAsync`),
  `src/TaskApi/Controllers/TasksController.cs` (`DELETE`), `src/TaskApi/Program.cs`,
  `frontend/src/components/{TaskDelete,ConfirmDialog}.tsx`, `frontend/src/pages/{TaskDetailPage,TasksPage}.tsx`
- Test files: `tests/http/BallastLane.TaskApi.HttpTests/TaskDeleteTests.cs`,
  `frontend/tests/components/TaskDelete.test.tsx`
- Task identifiers: `T081` to `T090`
- Commits: `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9cd6b3a` - `feat(web): implement the React task manager client`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires.

## Evidence status rationale

Status is `Captured`. Developer review and a commit reference remain outstanding.
