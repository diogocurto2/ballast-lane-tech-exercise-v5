# EV-0020 - Analysis of 001-task-manager (pre-implementation gate)

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0020` |
| Date | `2026-11-16` |
| Development phase | Phase 5 - Analyze (mandatory pre-implementation gate) |
| Activity type | `Review` |
| Related specification | `specs/001-task-manager/spec.md` |
| Related task identifiers | `T001` to `T106` analyzed, none executed |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session |
| Evidence status | `Captured` |
| Commit references | `d9f3828` - `fix(spec): close the four blocking analysis findings and retarget .NET 10` |

## Actual prompt

Verbatim developer instruction:

```text
Analyze feature 001-task-manager following the constitution and engineering standards.

Pay particular attention to:

- Forward dependencies and executable RED-first TDD checkpoints.
- T020 depending on IPasswordHasher, introduced only in T031.
- Incorrect parallelization of T007.
- Explicit coverage of the mandatory GenAI API-generation exercise.
- Consistency between specification, plan, contracts, and tasks.
- The choice of unsupported .NET 8 instead of the installed .NET 10 LTS.
Record the analysis report using genai-evidence.

Do not implement or modify the analyzed artifacts. Report blockers and proposed corrections.
```

## Relevant context

- Artifacts analyzed: `specs/001-task-manager/spec.md`, `plan.md`, `tasks.md`, plus `research.md`,
  `data-model.md`, `contracts/user-api.md`, `contracts/task-api.md`, `quickstart.md`.
- Governing documents: `.specify/memory/constitution.md` v1.0.0, and the engineering standards
  `testing-and-tdd.md` section 3, `clean-architecture.md`, `api-standards.md`, `security.md`,
  `database-standards.md`, `frontend-standards.md`, `delivery-and-ci.md`, `genai.md`.
- Prerequisite check: `check-prerequisites.ps1 -Json -RequireSpec -RequireTasks -IncludeTasks`
  returned `FEATURE_DIR` `specs/001-task-manager` and `AVAILABLE_DOCS`
  `research.md, data-model.md, contracts/, quickstart.md, tasks.md`.
- Extension hooks: `.specify/extensions.yml` does not exist, so no `before_analyze` or
  `after_analyze` hooks were checked or executed.
- Read-only constraint honored: no analyzed artifact was modified. This record is the only file
  written.

## Original AI-generated output

The objects of this review are the previously generated `plan.md` and `tasks.md`. The specific
generated text that produced each finding is quoted in the analysis report below and in the
findings table.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | Task ordering against the dependency matrix; abstraction introduction points | Findings H5, C2 |
| SOLID | `Not Applicable` to a read-only analysis beyond abstraction placement, which H5 covers | `Not Applicable` |
| TDD | Executability of every RED checkpoint; test-to-implementation ordering; parallel markers | Findings C1, C2, C3, H1, H6, M3, M4 |
| Security | Claims-only identity, cross-user denial, token validation tasks present | `[VERIFIED]` present in T012, T018, T038, T071, T081, T099 |
| Functional requirements | All 17 FRs and 6 NFRs mapped to at least one task by story inference; explicit ID traceability absent | 100% inferred coverage, 0% explicit; finding H2 |

## Review findings

The complete findings table, coverage table, constitution alignment section, unmapped tasks, metrics,
and recommended actions are reproduced in the "Analysis report" section below, which is the report
delivered to the developer.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Run the pre-implementation analysis | Developer invoked `/speckit-analyze` with specific review targets | `Approved` - explicit instruction |
| Do not modify the analyzed artifacts | Explicit instruction: "Do not implement or modify the analyzed artifacts" | `Approved` - explicit instruction |
| Remediate the four CRITICAL findings only (C1 to C4), then re-analyze | Developer answered the remediation question with `critical-only` | `Approved` - explicit decision, 2026-11-16 |
| Retarget the plan and tasks from .NET 8 to .NET 10 LTS | Developer answered the .NET question with `net10`, resolving finding H4 | `Approved` - explicit decision, 2026-11-16 |
| Implement Setup + Foundational + US1 + US2 only, reporting P2 and P3 as not delivered | Developer answered the deadline question with `p1-only` | `Approved` - explicit decision, 2026-11-16 |
| Leave H1, H2, H3, H5, M1, M2, M3, M5, and M6 open | Outside the approved `critical-only` scope | `Pending` - recorded as unresolved findings |
| Remediations listed in the report | Not applied by this activity; applied in the follow-up remediation activity recorded in `EV-0022` | `Approved` - scope and decisions above |

## Corrections and improvements

`Not Applicable` - this activity is read-only by instruction. No correction was applied to any
analyzed artifact. Proposed corrections are recorded as recommendations only.

## Commands and observed results

Prerequisite check:

```text
Test-Path .specify/extensions.yml  ->  False
check-prerequisites.ps1 -Json -RequireSpec -RequireTasks -IncludeTasks
-> FEATURE_DIR specs/001-task-manager
   AVAILABLE_DOCS research.md, data-model.md, contracts/, quickstart.md, tasks.md
```

Forward-dependency scan of the abstractions named in `tasks.md`:

```text
IPasswordHasher  -> T013 (test), T020 (DemoSeeder, Phase 2), T031 (definition, Phase 3)
IUserReader/IUserWriter/ITokenIssuer/ITimeProvider -> T031 only (Phase 3)
NpgsqlUserRepository -> T032 only
UserId, TaskId, TaskStatus, TaskDraft -> 1 to 2 mentions each; AuthenticatedUser -> T041, T063 only
```

Requirement-identifier scan of `tasks.md`:

```text
FR-001 .. FR-017 : 0 mentions each
SC-001 .. SC-008 : 0 mentions each
```

Endpoint inventory:

```text
contracts/user-api.md : 4 operations (POST register, POST login, GET me, GET public/info)
contracts/task-api.md : 5 operations (GET list, GET by id, POST, PUT, DELETE)
plan.md L58           : "roughly 20 endpoints in total across both hosts"
```

.NET version statements:

```text
plan.md L27           : C# 12 on .NET 8 (net8.0, SDK 8.0.428)
research.md L29       : ".NET 8 passed its end-of-support date shortly before this plan"
research.md L25       : ".NET 10 (installed, current LTS): better long-term support"
```

Setup-phase parallel marker check:

```text
T001 (no [P]) -> T002..T006 [P] -> T007 [P] which validates "T001 to T006"
```

## Analysis report

# Specification Analysis Report - 001-task-manager

| ID | Category | Severity | Location(s) | Summary | Recommendation |
|----|----------|----------|-------------|---------|----------------|
| C1 | Constitution III (TDD) | CRITICAL | tasks.md T008-T014 | Every Phase 2 test references a type created later in the same phase, so the RED checkpoint T014 can only produce compilation failures, which the constitution explicitly excludes as valid RED evidence | Add a compile-only stub task before T014 (interfaces plus types that throw `NotImplementedException`), or split Phase 2 into per-group test/stub/RED/implement cycles |
| C2 | Dependency order | CRITICAL | tasks.md T020 vs T031 | `DemoSeeder` (T020, Phase 2) hashes passwords through `IPasswordHasher`, which is only defined in T031 (Phase 3, US1). Forward dependency across a phase boundary | Define `IPasswordHasher` and `ITimeProvider` in Phase 2 as application boundaries, or move the seeder to US7 |
| C3 | Dependency order | CRITICAL | tasks.md T012, T022, T033 | T012 asserts `GET /api/users/me`, but the User API host and controller arrive in T033 (US1); T022 claims Phase 2 GREEN across the UserApi HTTP tests, which is unachievable | Move T012 into US2 (Phase 4) where the host exists, or move the minimal Users host into Phase 2 |
| C4 | Coverage gap | CRITICAL | tasks.md (absent) vs spec DO-02, SUB-04, SUB-05 | No task performs the mandatory Generative AI API-generation exercise, although the specification maps it as deliverable DO-02 and the constitution requires it to remain pending only until performed | Add a documentation task that writes the prompt, captures representative generated output, records validation and corrections, and updates `GENAI.md` and its evidence record |
| H1 | Parallelization | HIGH | tasks.md T007 | T007 is marked `[P]` yet its content is the substitute validation for T001 to T006, so it depends on all of them; `[P]` states it can run concurrently | Remove `[P]` from T007 and declare it dependent on T001 to T006 |
| H2 | Traceability | HIGH | tasks.md (whole file) | No `FR-` or `SC-` identifier appears anywhere in the task list; mapping exists only through the `[USn]` label, so requirement coverage cannot be mechanically verified | Add a requirement reference to each task, or add a task-to-requirement table at the end of tasks.md |
| H3 | Coverage gap | HIGH | spec SC-001, SC-002 | Both success criteria are timing-based and no task measures them, automated or manual | Add a measured step to quickstart section 4 and reference it from T102, or restate the criteria as non-timed |
| H4 | Technology choice | HIGH | plan.md L27, research.md R1 | The plan targets .NET 8, whose end-of-support the plan itself records as having passed, while .NET 10 LTS is installed and verified | Retarget to `net10.0`, keep the design, and record the OpenAPI package implication; or record an explicit developer acceptance of an unsupported runtime |
| H5 | Missing artifact | HIGH | data-model.md (Domain value objects), tasks.md T030, T041 | `AuthenticatedUser` is declared as a Domain value object but no task creates it; T030 (Domain user) omits it and T041 (SharedWeb) only references it | Assign its creation explicitly, and state which project owns it |
| H6 | Acceptance fidelity | HIGH | tasks.md T038 vs T053 | T038 asserts that "every endpoint in `src/TaskApi` returns 401", but the task endpoints are not created until T053 (US3), so the assertion is vacuous at US2 time | Narrow T038 to the fallback-policy behaviour on a probe route, and let US3 assert the real endpoints |
| M1 | Inconsistency | MEDIUM | plan.md L58, contracts | The plan says "roughly 20 endpoints" while the contracts define 9 | Correct the count to 9 |
| M2 | Plan-to-task gap | MEDIUM | plan.md test strategy, Infrastructure unit row | Planned Infrastructure unit obligations (identifier allow-list, no string-interpolated SQL, cancellation propagation) have no corresponding task | Add those assertions to T010 or T045 |
| M3 | Forward reference | MEDIUM | tasks.md T019, T020 | T019's `--seed` action calls `DemoSeeder`, created in T020 | Reverse the order or have T019 accept a seeder delegate |
| M4 | RED hygiene | MEDIUM | tasks.md T039 | A RED checkpoint task also scaffolds the host, mixing implementation into the observation step | Move scaffolding into a separate preceding task |
| M5 | Coverage depth | MEDIUM | tasks.md T034 vs spec NFR-002 | Accessibility coverage is limited to labels; keyboard operation, focus visibility, and live-region announcements are not asserted | Add those assertions to T034 and T048 |
| M6 | Ambiguity | MEDIUM | tasks.md T008 | "Applying `db/migrations/` to an empty database" does not say whether the test invokes `SchemaMigrator` or an external tool, which determines whether RED is behavioural or compilation | State the mechanism in the task |

**Coverage Summary Table (aggregated by requirement):**

| Requirement Key | Has Task? | Task IDs | Notes |
|-----------------|-----------|----------|-------|
| FR-001 registration | Yes | T026, T027 | |
| FR-002 authentication | Yes | T026, T027 | |
| FR-003 persisted users | Yes | T032, T046, T091 | |
| FR-004 logout | Yes | T028, T034 | Client-side only, consistent with PDC-07 |
| FR-005 CRUD own tasks | Yes | T047, T053, T059, T065, T071, T076, T081, T086 | |
| FR-006 task fields | Yes | T046, T050, T058 | |
| FR-007 cross-user denial | Yes | T051, T071, T081 | |
| FR-008 two API surfaces | Yes | T027, T033, T038, T040 | See H6 |
| FR-009 ownership from claims | Yes | T059, T063 | |
| FR-010 frontend flows | Yes | T028, T034, T054, T066, T077, T087 | |
| FR-011 frontend states | Yes | T048, T054, T060, T072, T082 | |
| FR-012 seeded demo data | Yes | T013, T020, T091, T093 | See C2 |
| FR-013 statuses | Yes | T050, T058, T059, T070 | |
| FR-014 field rules | Yes | T058, T059, T062, T070 | |
| FR-015 password policy | Yes | T026, T027 | |
| FR-016 delete semantics | Yes | T081, T084, T085 | |
| FR-017 error categories | Yes | T011, T017, T059, T071 | |
| NFR-001 security | Yes | T012, T018, T027, T099 | |
| NFR-002 accessibility | Partial | T034 | See M5 |
| NFR-003 responsiveness | Manual only | T102 | Documented as manual |
| NFR-004 reproducibility | Yes | T005, T095, T104 | |
| NFR-005 durability | Yes | T046, T095 | |
| NFR-006 test levels | Yes | T008-T013, T045-T048, T058-T060, T070-T072, T081-T082 | |
| SC-001 registration timing | No | - | See H3 |
| SC-002 CRUD timing | No | - | See H3 |
| SC-003 cross-user denial | Yes | T099 | |
| SC-004 credential rejection | Yes | T012, T038, T099 | |
| SC-005 UI states | Yes | T102 | Manual |
| SC-006 clean-start demo | Yes | T095 | |
| SC-007 restart durability | Yes | T095 | |
| SC-008 clean console | Yes | T102 | Manual |
| DO-02 GenAI exercise | **No** | - | See C4 |
| DO-01 README | Yes | T104 | |
| DO-03 user story and presentation | Yes | T104 | |
| DO-04 single public repo | Yes | T106 | |
| DO-05 no symlinks | Yes | T106 | |

**Constitution Alignment Issues:**

- Principle III (TDD, non-negotiable): C1, C2, C3, H1, H6, M4. The constitution treats missing tests,
  missing RED checkpoints, and incorrect task ordering as blocking findings.
- Principle I (Assessment-first): C4. An assessment requirement with no task coverage is a blocking
  finding by definition.
- Principles II, IV, V, VI: no violation found. Dependency direction, security task coverage, seeded
  delivery, and evidence capture points are present.

**Unmapped Tasks:**

- T097, T098, T099, T100, T101, T102, T103, T105, T106 are verification tasks with no single FR; they
  map to NFR-006, Principle II, SC-003, SC-004, and Principles V and VI. This is expected for a
  verification phase and is not a defect.

**Metrics:**

- Total Requirements: 31 (17 FR, 6 NFR, 5 DO, plus 8 SC)
- Total Tasks: 106
- Coverage % (requirements with at least one task): 29 of 31 requirements, 93.5%; the two uncovered
  are DO-02 (GenAI exercise) and SC-001/SC-002 timing (counted as one item), so 29/31
- Explicit requirement-ID traceability in tasks.md: 0%
- Ambiguity Count: 1 (M6)
- Duplication Count: 0
- Critical Issues Count: 4

**Next Actions:**

- CRITICAL findings exist. `/speckit-implement` MUST NOT begin until C1 to C4 are resolved.
- C1, C2, C3, H1, H5, H6, M3, M4 are task-ordering and task-content corrections: edit `tasks.md`.
- C4 and H3 require new tasks: edit `tasks.md`, and consider a `quickstart.md` step for the timing
  criteria.
- H4 is a plan decision: edit `plan.md` and `research.md` R1, or record explicit developer acceptance.
- M1, M2, M5, M6 are low-risk corrections to `plan.md` and `tasks.md`.
- Recommended sequence: resolve C1 to C4 first, re-run `/speckit-analyze`, then run
  `/speckit-implement`.

## Final references

- Specification: [specs/001-task-manager/spec.md](../../../specs/001-task-manager/spec.md)
- Plan: [specs/001-task-manager/plan.md](../../../specs/001-task-manager/plan.md)
- Tasks: [specs/001-task-manager/tasks.md](../../../specs/001-task-manager/tasks.md)
- Data model: [specs/001-task-manager/data-model.md](../../../specs/001-task-manager/data-model.md)
- Contracts: [contracts/user-api.md](../../../specs/001-task-manager/contracts/user-api.md),
  [contracts/task-api.md](../../../specs/001-task-manager/contracts/task-api.md)
- Quickstart: [specs/001-task-manager/quickstart.md](../../../specs/001-task-manager/quickstart.md)
- Preceding records: [EV-0008](./EV-0008-implementation-plan-001-task-manager.md),
  [EV-0009](./EV-0009-task-generation-001-task-manager.md)
- Implementation files: `Not Applicable` - none exist, and none were created
- Test files: `Not Applicable` - none exist
- Task identifiers: `T001` to `T106`, analyzed only
- Commits: `d9f3828` - `fix(spec): close the four blocking analysis findings and retarget .NET 10`

## Evidence status rationale

Status is `Captured`: the instruction, the artifacts analyzed, the scans performed with their
observed output, the four blocking findings, and the full report are recorded.

It is not `Reviewed` or `Verified` because the developer has not reviewed the findings, no
remediation has been approved or applied, and the re-analysis that must follow any correction has
not happened.
