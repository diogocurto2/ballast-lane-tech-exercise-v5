# EV-0022 - Remediation of CRITICAL findings and re-analysis of 001-task-manager

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0022` |
| Date | `2026-11-16` |
| Development phase | Phase 5 - Analyze (remediation and re-run of the pre-implementation gate) |
| Activity type | `Review` |
| Related specification | `specs/001-task-manager/spec.md` |
| Related task identifiers | `T001` to `T107` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session |
| Evidence status | `Captured` |
| Commit references | `PENDING` - no commit was requested for this activity |

## Actual prompt

No new free-text prompt was supplied. The developer answered the remediation questions presented at
the end of `EV-0020`. Recorded verbatim as the answers received:

```text
remediation=critical-only
dotnet=net10
deadline_scope=p1-only
```

The governing instruction remains the `/speckit-analyze` instruction recorded in `EV-0020`.

## Relevant context

- Prerequisite analysis: `EV-0020`, which recorded four CRITICAL findings (C1 to C4) and twelve
  non-critical findings.
- Approved scope: fix only C1 to C4, plus the developer's .NET decision, then re-analyze. All other
  findings remain open and are reported as such.
- Files changed: `specs/001-task-manager/tasks.md`, `specs/001-task-manager/plan.md`,
  `specs/001-task-manager/research.md`. No other artifact was modified.

## Original AI-generated output

The defective generated text is quoted in `EV-0020`. The specific lines changed here, as they stood
before correction:

```text
T014 Run the Phase 2 tests in ... tests/unit/BallastLane.Infrastructure.UnitTests/,
     tests/unit/BallastLane.Api.UnitTests/, tests/integration/BallastLane.Migrations.Tests/,
     and tests/http/BallastLane.UserApi.HttpTests/ ...

T022 Run the Phase 2 suite across tests/unit/, tests/integration/, and
     tests/http/BallastLane.UserApi.HttpTests/ ...

T031 Implement src/Application/Abstractions/IUserReader.cs, IUserWriter.cs,
     IPasswordHasher.cs, ITokenIssuer.cs, and ITimeProvider.cs, plus ...

T012 Write HTTP tests asserting GET /api/public/info answers anonymously,
     GET /api/users/me returns 401 ...
```

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | Interface ownership after moving the boundaries into Phase 2: `IPasswordHasher`, `ITimeProvider`, `IUserReader`, `IUserWriter`, and `ITokenIssuer` all live in `src/Application/Abstractions/`, the consuming layer | `[VERIFIED]` |
| SOLID | `Not Applicable` to this correction; abstraction placement is unchanged apart from the phase in which the interfaces are declared | `Not Applicable` |
| TDD | Re-ran the executability checks: a compile-only contract task now precedes the RED checkpoint; no Phase 2 test depends on a User API controller; no Phase 2 task references `NpgsqlTaskRepository` | `[VERIFIED]` |
| Security | `/api/users/me` 401 assertions preserved, relocated to US2 where the endpoint exists; token-validation parameter assertions added as unit tests | `[VERIFIED]` |
| Functional requirements | The mandatory GenAI exercise (DO-02) now has a task; all previously covered FRs remain covered | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` C1 resolved: T014 now creates the compile-only contracts that the Phase 2 tests need
  (`IPasswordHasher`, `ITimeProvider`, `IUserReader`, `IUserWriter`, `ITokenIssuer`, the outcome
  types, and `NotImplementedException` bodies for `SchemaMigrator`, `NpgsqlConnectionFactory`, and
  `DemoSeeder`) before running them, so the observed failures can be behavioural assertions rather
  than compilation errors.
- `[VERIFIED]` C2 resolved: `IPasswordHasher` is declared in T014, before T020 consumes it. T031 no
  longer re-creates the interfaces and instead implements the concrete use cases against them.
- `[VERIFIED]` C3 resolved: Phase 2 no longer references the User API host or `UsersController`; the
  `/api/users/me` assertions moved into T038 (US2), and T022's GREEN scope is limited to
  `tests/unit/` and `tests/integration/`. Verified against the 17 Phase 2 task lines: zero matches.
- `[VERIFIED]` C4 resolved: T107 performs the mandatory Generative AI REST API generation exercise,
  including the prompt, representative original output, validation, corrections, and handling of
  authentication, authorization, validation, and edge cases, with its own evidence record. T103 was
  narrowed to README verification so it does not claim to verify a `GENAI.md` that does not yet
  contain the exercise.
- `[VERIFIED]` H4 resolved by developer decision: the target is now .NET 10 (`net10.0`, C# 14) in
  `plan.md` Technical Context, `research.md` R1 with the rejected .NET 8 alternative retained for the
  record, and T001/T002 in `tasks.md`. Package versions are deliberately left unpinned in prose and
  are pinned at implementation time, so no version is invented.
- `[VERIFIED]` H6 resolved as a side effect of the C3 rewrite: T038 no longer asserts that "every
  endpoint in `src/TaskApi` returns 401" before any such endpoint exists; it asserts the fallback
  policy on a probe route plus the real User API endpoints.
- `[VERIFIED]` A stale reference in the "Parallel Example: Phase 2" block still listed T012's old
  test path; corrected to `tests/unit/BallastLane.Api.UnitTests/TokenValidationParametersTests.cs`.
- `[VERIFIED]` Remaining `.NET 8` mentions in `research.md` are confined to the rejected-alternative
  record and the decision note, which is the correct place for them.
- `[PENDING]` H1, H2, H3, H5, M1, M2, M3, M5, and M6 remain unresolved by the approved scope. H5 in
  particular will be reached during implementation: `AuthenticatedUser` is still declared without an
  owning creation task.
- `[PENDING]` Developer review of the corrected artifacts and re-approval of the gate.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Remediate C1 to C4 only | Explicit answer `critical-only` | `Approved` |
| Target .NET 10 LTS | Explicit answer `net10`; the plan had recorded .NET 8 as past end-of-support | `Approved` |
| Implement P1 only, report P2 and P3 as not delivered | Explicit answer `p1-only` | `Approved` |
| Fold compile-only contract creation into the RED checkpoint task T014 rather than inserting a new task | Inserting a task would renumber 93 identifiers and every cross-reference, creating more risk than it removes; the plan's test-first strategy already permits minimal symbols so that RED is behavioural | `Pending` - recorded trade-off |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| T014 now creates compile-only contracts before running tests | C1: compilation failure is not valid RED | Constitution Principle III excludes environmental and compilation failures as RED evidence |
| T010 no longer asserts repository cancellation | It referenced `NpgsqlTaskRepository`, created in T052 | A Phase 2 test must not depend on a later phase |
| T012 re-scoped to token-validation parameter unit tests | The HTTP form needed a controller created in T033 | Keeps the security assertions and removes the forward dependency |
| `/api/users/me` assertions moved to T038 | US2 owns the access boundary and depends on US1 | Preserves coverage without breaking the phase boundary |
| T022 GREEN scope narrowed | It included HTTP tests that cannot pass in Phase 2 | A checkpoint that cannot close is not a checkpoint |
| T031 no longer creates the interfaces | They are declared in T014 | One declaration point avoids a duplicate definition |
| T107 added for the GenAI API exercise | C4: no task performed a mandatory assessment deliverable | An assessment requirement with no coverage is blocking |
| T103 narrowed to README | It claimed to verify `GENAI.md` content that T107 produces later | Ordering must match what each task can actually observe |
| .NET 10 in plan, research, and tasks | Developer decision on H4 | A supported LTS runtime is the defensible baseline |
| Password-hashing parameters no longer state a specific iteration count | The previous count was tied to .NET 8 defaults and would be unverified after retargeting | Recording an unobserved number would be a fabricated detail |

## Commands and observed results

Re-analysis checks after correction:

```text
C1 stub task before RED:            True
C2 IPasswordHasher defined at T014: True
C3 Phase 2 excludes UserApi HTTP:   True (17 Phase 2 task lines, 0 matches for
                                          UserApi.HttpTests or UsersController)
C4 GenAI exercise task present:     True
net10 target in tasks:              True
forward-dependency sweep, Phase 1-2:
  NpgsqlTaskRepository   referenced in Phase 1-2: False
  UsersController        referenced in Phase 1-2: False
  IPasswordHasher        referenced in Phase 1-2: True (declared at T014)
  SchemaMigrator         referenced in Phase 1-2: True (stub at T014)
  NpgsqlConnectionFactory referenced in Phase 1-2: True (stub at T014)
  DemoSeeder             referenced in Phase 1-2: True (stub at T014)
format/id integrity:
  tasks: 107; unique: 107; missing: none
  format violations: 2
  -> both are scanner false positives: T006 and T103 name root-level files
     (`.env.example`, `.gitignore`, `README.md`) without a directory separator
```

Residual .NET 8 mentions:

```text
research.md L24, L26, L207 - all inside the rejected-alternative record and the
decision note for R1; no live target references remain.
```

## Final references

- Task list: [specs/001-task-manager/tasks.md](../../../specs/001-task-manager/tasks.md)
- Plan: [specs/001-task-manager/plan.md](../../../specs/001-task-manager/plan.md)
- Research: [specs/001-task-manager/research.md](../../../specs/001-task-manager/research.md)
- Prerequisite analysis: [EV-0020](./EV-0020-analysis-001-task-manager.md)
- Implementation files: `Not Applicable` - none exist
- Test files: `Not Applicable` - none exist
- Task identifiers: `T001` to `T107`, none executed
- Commits: `PENDING` - no commit was requested for this activity

## Evidence status rationale

Status is `Captured`: the developer's answers, the four corrections and the runtime retarget, the
observed re-analysis output, and the explicitly unresolved findings are recorded.

It is not `Reviewed` or `Verified` because the developer has not reviewed the corrected artifacts,
and the gate must be re-approved before implementation begins.
