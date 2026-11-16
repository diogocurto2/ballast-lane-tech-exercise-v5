# EV-0023 - Final pre-implementation corrections and second re-analysis of 001-task-manager

> **Revision note (2026-11-16, after `EV-0024`).** Paths in this record reflect the repository layout
> at the time of the activity, when the backend lived at the root as `src/`, `tests/`, and `db/`. After
> the `backend/` restructure recorded in [EV-0024](./EV-0024-backend-folder-restructure.md), those
> locations are `backend/src/`, `backend/tests/`, and `backend/db/`. Nothing else in this record was
> altered, so it remains an accurate account of what was observed when it was written.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0023` |
| Date | `2026-11-16` |
| Development phase | Phase 5 - Analyze (second re-run of the pre-implementation gate) |
| Activity type | `Review` |
| Related specification | `specs/001-task-manager/spec.md` |
| Related task identifiers | `T001` to `T107` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session |
| Evidence status | `Captured` |
| Commit references | `PENDING` - no commit was requested for this activity |

## Actual prompt

Verbatim developer instruction:

```text
Apply the final pre-implementation review corrections:

- Fix the Foundation dependencies: implement the actual password hasher before DemoSeeder, order Migrator and Seeder correctly, and fix ON CONFLICT to match the lower(email) unique index.
- Ensure compilable scaffolding and genuine behavioral RED checkpoints for every user story, not only Foundation.
- Remove T007's parallel marker and fix its dependencies.
- Align all backend and test package versions with .NET 10.
- Capture the mandatory GenAI API-generation prompt and original output during actual API implementation; use T107 to consolidate the evidence in GENAI.md.
- Treat P1 as the first implementation increment, not the complete assessment. CRUD, frontend task management, seeded demo data, and GENAI.md remain mandatory deliverables.
Run /speckit-analyze again, record the findings, and resolve any remaining execution blockers. Follow the existing constitution and engineering standards.

Do not create application code during this correction.
```

## Relevant context

- Prior records: `EV-0020` (first analysis, four CRITICAL findings) and `EV-0022` (remediation of those
  four and the .NET 10 retarget).
- Files changed: `specs/001-task-manager/tasks.md` and `specs/001-task-manager/plan.md` only.
- No application code, project file, solution, or test project was created, per the instruction.
- Extension hooks: `.specify/extensions.yml` does not exist, so no `before_analyze` or `after_analyze`
  hooks were checked or executed.

## Original AI-generated output

The defective generated text as it stood before this correction:

```text
T019 Implement `src/Migrator/Program.cs` with a `migrate` action by default and a `--seed`
     action, returning a non-zero exit code on failure (depends on T018; RED via T014)

T020 Implement `src/Infrastructure/Persistence/DemoSeeder.cs` with idempotent upserts
     (`ON CONFLICT (email) DO NOTHING`), seeded passwords hashed through `IPasswordHasher`
     inside one explicit transaction (depends on T019; RED via T014)

T032 ... `src/Infrastructure/Security/IdentityPasswordHasher.cs`, ... and
     `src/Infrastructure/Time/SystemTimeProvider.cs`

T007 [P] Add `.editorconfig`, ... then run the substitute validation for T001 to T006 ...

T029 [US1] Run the US1 tests in ... (no compile-only contracts of its own)
```

Four defects are visible in that text: the migrator host was implemented before the seeder it
calls, the seeder used an `ON CONFLICT (email)` target that does not match the `lower(email)` unique
expression index, the concrete password hasher was implemented two phases after the seeder that
requires it, and only Phase 2 had compile-only scaffolding.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | Hasher and time provider remain Infrastructure adapters behind Application boundaries; `AuthenticatedUser` is a Domain type created in T014 | `[VERIFIED]` |
| SOLID | `Not Applicable` to this correction; abstraction placement is unchanged | `Not Applicable` |
| TDD | Every user story's RED checkpoint now creates its own compile-only contracts, so each can produce behavioural assertion failures | `[VERIFIED]` |
| Security | Password hashing now exists before any code that seeds credentials; `ON CONFLICT` target matches the unique expression index, so idempotent seeding cannot fail on a missing constraint | `[VERIFIED]` |
| Functional requirements | All 36 requirement identifiers (17 FR, 6 NFR, 5 DO, 8 SC) now appear in a task-level traceability table | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` Foundation dependency order corrected. T019 now implements the concrete
  `IdentityPasswordHasher` and `SystemTimeProvider`; T020 then implements the seeder and the migrator
  host together, with the host applying migrations before seeding. T032 no longer re-implements the
  hasher or time provider.
- `[VERIFIED]` `ON CONFLICT` corrected from `(email)` to `(lower(email))`, matching the unique
  expression index created in T015. The old form would have raised at runtime because no unique
  constraint exists on the bare `email` column.
- `[VERIFIED]` Compile-only scaffolding added to every story's RED checkpoint: T029 (user domain,
  application, controllers, frontend auth), T039 (Task API host and probe routes), T049 (task domain,
  read use cases, repository, controller, frontend list and state components), T061 (draft,
  validator, create use case, create contract, task form), T073 (update use case and contract, edit
  form), T083 (delete use case, confirm dialog), and T092 (demonstration dataset constants).
- `[VERIFIED]` H1 resolved: T007 no longer carries `[P]` and now declares its dependency on T001 to
  T006. A scan for `[P]` on run or record tasks returns zero.
- `[VERIFIED]` Packages aligned with .NET 10: `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.x,
  `Microsoft.Extensions.Identity.Core` 10.0.x, `Microsoft.AspNetCore.Mvc.Testing` 10.0.x, `Npgsql`
  10.x, `Swashbuckle.AspNetCore` latest stable targeting `net10.0`, `xunit` 2.x with the current
  stable `Microsoft.NET.Test.Sdk`; the build must fail on any package below the .NET 10 line or any
  prohibited library. `plan.md` Technical Context states the same alignment.
- `[VERIFIED]` GenAI capture placement corrected: the prompt and representative original generated
  code are captured during T033, T053, T065, T076, and T086 (the API implementation tasks), before any
  correction, and T107 only consolidates them into `GENAI.md` and the exercise record. A task-list
  note states that T107 must not reconstruct them afterwards.
- `[VERIFIED]` Scope framing corrected: "MVP first" replaced by "First increment: P1", with an explicit
  statement that US3 to US6 (CRUD and frontend task management), US7 (seeded demonstration data), and
  T107 (`GENAI.md`) remain mandatory deliverables. "Scope honesty" now says the priority order
  decides build order, not delivery scope.
- `[VERIFIED]` H2 resolved: a "Requirement Traceability" section maps all 36 identifiers to tasks. A
  scan reports zero requirement identifiers missing from `tasks.md`.
- `[PENDING]` H3 (SC-001 and SC-002 timing) is now referenced to a timed manual recording in T102 but
  no measured threshold automation exists; recorded rather than claimed.
- `[PENDING]` M1, M2, M3, M5, and M6 remain open by the previously approved scope. None of them
  prevents execution.
- `[VERIFIED]` No application code was created. Only `tasks.md` and `plan.md` changed.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Apply the six correction groups | Explicit developer instruction | `Approved` |
| P1 is the first increment, not the assessment scope | Explicit developer instruction; CRUD, frontend, seed, and GENAI.md remain mandatory | `Approved` |
| Capture GenAI prompt and output during implementation, consolidate in T107 | Explicit developer instruction | `Approved` |
| Keep H3 and M1, M2, M3, M5, M6 open | Not execution blockers; instruction asked only for remaining blockers | `Pending` - recorded |
| Create no application code during this correction | Explicit developer instruction | `Approved` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| T019 re-scoped to the concrete hasher and time provider; T020 hosts the seeder and migrator | The seeder consumed a hasher implemented later, and the migrator host called a seeder that did not yet exist | An implementation task must not depend on a later artifact, and the runner must not precede what it runs |
| `ON CONFLICT (lower(email))` | The unique index is an expression index on `lower(email)` | PostgreSQL requires the conflict target to match a real unique constraint or index; the previous form would fail at runtime |
| Compile-only contracts in every story's RED checkpoint | Only Phase 2 could produce behavioural RED | Constitution Principle III excludes compilation failures as RED evidence |
| T007 loses `[P]` and gains dependencies | It validates T001 to T006 | A task cannot run in parallel with its own prerequisites |
| Packages aligned to .NET 10 line, with a build rule | Backend and test packages had no version alignment after the retarget | A mixed package set below the target line produces runtime and analyzer drift |
| GenAI capture moved into implementation tasks | Capturing after the fact is reconstruction, not evidence | The genai standard requires the prompt and original output to be preserved when produced |
| "First increment" framing replaces "MVP first" | The earlier wording implied P1 was sufficient scope | CRUD, frontend, seed, and GENAI.md are mandatory assessment deliverables |
| Requirement traceability table added | No `FR-`/`SC-` identifier appeared in the task list | Traceability must be checkable without inference |

## Commands and observed results

Second re-analysis:

```text
T007 parallel marker removed:        True
T007 declares dependencies:          True
hasher implemented before seeder:    True
seeder then migrator host in T020:   True
ON CONFLICT matches lower(email):    True / stale (email) form: False
.NET 10 package alignment stated:    True
GenAI capture during implementation: True
P1 framed as first increment:        True
mandatory deliverables restated:     True

per-story RED scaffolding:
  T029 True  T039 True  T049 True  T061 True
  T073 True  T083 True  T092 True

requirement ids: 36; missing from tasks: none
tasks: 107; unique ids: 107
format violations: 0
[P] on run/validate tasks: 0
```

## Final references

- Task list: [specs/001-task-manager/tasks.md](../../../specs/001-task-manager/tasks.md)
- Plan: [specs/001-task-manager/plan.md](../../../specs/001-task-manager/plan.md)
- Prior records: [EV-0020](./EV-0020-analysis-001-task-manager.md),
  [EV-0022](./EV-0022-critical-remediation-and-reanalysis.md)
- Implementation files: `Not Applicable` - none exist and none were created
- Test files: `Not Applicable` - none exist
- Task identifiers: `T001` to `T107`, none executed
- Commits: `PENDING` - no commit was requested for this activity

## Evidence status rationale

Status is `Captured`: the instruction, the four defects in the pre-correction text, the six correction
groups, and the observed second re-analysis output are recorded.

It is not `Reviewed` or `Verified` because the developer has not reviewed the corrected artifacts and
no implementation has begun.
