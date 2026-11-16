# EV-0008 - Implementation plan, research, and Phase 1 design for 001-task-manager

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0008` |
| Date | `2026-11-16` |
| Development phase | Phase 3 - Plan (Phase 0 research and Phase 1 design) |
| Activity type | `Documentation and specification` |
| Related specification | `specs/001-task-manager/spec.md` |
| Related task identifiers | `PENDING` - tasks.md has not been generated yet |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session |
| Evidence status | `Captured` |
| Commit references | `4ca023d` - `feat(spec): plan and RED-first task list for 001-task-manager` |

## Actual prompt

The developer invoked the `/speckit-plan` skill with **empty arguments**. The literal user input was:

```text
/speckit-plan
```

No additional instruction text, constraints, or corrections were supplied with the invocation. The
skill's own instructions selected the active feature from `.specify/feature.json` and directed the
plan workflow. The governing product instruction remains the one recorded verbatim in `EV-0007`.

## Relevant context

- Feature selected: `specs/001-task-manager` via `.specify/feature.json`.
- Read before designing: the full specification (`spec.md`), the constitution
  (`.specify/memory/constitution.md`), and every engineering standard:
  `clean-architecture.md`, `solid-principles.md`, `testing-and-tdd.md`, `api-standards.md`,
  `security.md`, `database-standards.md`, `frontend-standards.md`, `coding-standards.md`,
  `delivery-and-ci.md`, `genai.md`, and the index `docs/engineering/README.md`.
- Template: `setup-plan.ps1 -Json` copied the effective plan template to
  `specs/001-task-manager/plan.md`. A byte comparison confirmed the copied file matched
  `.specify/templates/overrides/plan-template.md`, so the project override was the template in use.
  The template copy was then replaced by the actual plan.
- Extension hooks: `.specify/extensions.yml` does not exist, so no `before_plan` or `after_plan`
  hooks were checked or executed.

## Original AI-generated output

### Design decisions taken in the first draft

The first version of `plan.md` is preserved in the session artifacts. Its technical decisions are
unchanged by the later correction; the correction affected requirement traceability only. The
decisions were:

```text
.NET 8 (net8.0) on SDK 8.0.428; two API hosts (UserApi, TaskApi) over shared
Domain/Application/Infrastructure/SharedWeb; JWT HS256 bearer tokens with explicit
validation and ClockSkew zero; PasswordHasher<T> behind IPasswordHasher; Npgsql +
ADO.NET with hand-written parameterized SQL; ordered SQL migrations applied by a
Migrator console host; React 18.3 + TS 5.6 + Vite + Vitest; sessionStorage token with
documented XSS trade-off; Swashbuckle OpenAPI; DateOnly/date for due dates.
```

### Defect 1 - requirement traceability gap

The first draft asserted in its Constitution Check that it covered "all 17 FRs and 6 NFRs", but a
scripted scan found ten requirement identifiers that never appeared anywhere in the plan:

```text
FR-003, FR-004, FR-005, FR-008, FR-010,
NFR-002, NFR-003, NFR-004, NFR-005, NFR-006
```

The test-strategy table mapped scenarios to acceptance criteria, but it did not map every
requirement identifier to a verification level, so the claim of full coverage was not checkable and
was in fact unsupported. This is exactly the class of finding the analysis phase must treat as
blocking ("a functional requirement with no planned test").

### Defect 2 - literal clarification token in research.md

`research.md` opened with a sentence containing the literal marker token:

```text
Every `NEEDS CLARIFICATION` in the plan's Technical Context is resolved here.
```

A placeholder scan flags that token whether or not it is meant as prose, so the artifact would
report an unresolved clarification it does not have.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | Dependency matrix completed with a justification per non-empty cell; prohibited edges named; architecture test planned; `SharedWeb` explicitly barred from holding business rules | `[VERIFIED]` - no prohibited edge in the design |
| SOLID | All five principles answered; every abstraction tied to a seam (database, hashing, token, time); five named abstractions deliberately not added | `[VERIFIED]` |
| TDD | Seven test layers including Infrastructure unit and API controller unit; requirement coverage map added; RED-first ordering and planned-versus-observed distinction stated | `[VERIFIED]` after correcting defect 1 |
| Security | Token validation parameters enumerated; ownership enforced in the Application layer from claims; hashing component and parameters recorded; parameterized SQL; secrets from environment; prohibited libraries verified by test | `[VERIFIED]` |
| Functional requirements | All 23 requirement identifiers present in the coverage map after correction; every one mapped to at least one test level | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` Defect 1: ten requirement identifiers were absent from the first plan draft. Corrected
  by adding a requirement coverage map with one row per FR and NFR, naming the verification levels
  and where each is asserted.
- `[VERIFIED]` Defect 2: `research.md` contained a literal clarification token in prose. Rephrased.
- `[VERIFIED]` Toolchain choices were grounded in observed output rather than assumption: four .NET
  SDKs are installed (5.0.408, 8.0.428, 9.0.304, 10.0.404, default 10.0.404), Node `v25.9.0`, npm
  `11.12.1`, Docker `28.3.3`, Compose `v2.39.2-desktop.1`.
- `[VERIFIED]` .NET 8 was chosen over the installed .NET 10 and .NET 9. .NET 9 is rejected because its
  support has ended. .NET 8 follows the assessment's stated stack (job description references 6, 7,
  and 8). The honest trade-off is recorded: .NET 8 passed end-of-support shortly before this
  activity, which is acceptable for an assessment deliverable targeting that stack and would not be
  acceptable for production. .NET 10 remains a one-line change if the developer prefers it.
- `[VERIFIED]` Template provenance checked: the copied plan template was byte-identical to the
  project override, so the Constitution Check, dependency matrix, SOLID table, and mandatory test
  levels in the plan came from the enforced override rather than from a default template.
- `[AI-PROPOSED]` Two API hosts rather than one host with route groups, a separate `Migrator` console
  host, a `SharedWeb` project for shared authentication and error mapping, and an `ITimeProvider`
  abstraction. Each is recorded in the plan's Complexity Tracking with the rejected alternative, and
  each is `Pending` developer approval.
- `[AI-PROPOSED]` Adopting all eight `PDC` defaults so implementation is unambiguous. They remain
  provisional until the developer confirms them; changing one is a specification change.
- `[VERIFIED]` The quickstart records honestly that NFR-002 (accessibility), NFR-003
  (responsiveness), and NFR-005 (durability across restart) are only partly automatable, and names
  the manual scenarios instead of implying automated coverage.
- `[PENDING]` Developer review of the plan, the four complexity-tracking decisions, and the adopted
  `PDC` defaults.
- `[PENDING]` The `.NET 8` end-of-support trade-off requires a developer decision if the panel's
  expectation is a currently supported runtime.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Run the plan phase for `001-task-manager` | Developer invoked `/speckit-plan` | `Approved` - explicit invocation |
| .NET 8 target | Matches the assessment's stack; recorded trade-off for support status | `Pending` |
| Two API hosts | Assessment requires a second API | `Pending` |
| Separate `Migrator` console host | Keeps migration explicit and separate from seeding | `Pending` |
| Shared `SharedWeb` project | Avoids duplicating security wiring across hosts | `Pending` |
| Adopt `PDC-01` to `PDC-08` defaults | Removes implementation ambiguity | `Pending` |
| Generate `research.md`, `data-model.md`, `contracts/`, `quickstart.md` | Required Phase 0 and Phase 1 outputs of the plan workflow | `Approved` - implied by the invoked workflow |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Added the requirement coverage map to `plan.md` | Ten requirement identifiers were unmapped, so claimed coverage was uncheckable | Traceability must be mechanical: every requirement needs a named verification level, or the analysis phase cannot distinguish covered from uncovered |
| Recorded that NFR-002, NFR-003, and NFR-005 are partly manual | Automated suites cannot assert visual responsiveness or a container restart | Claiming test coverage that does not exist would be a fabricated result |
| Rephrased the `research.md` opening sentence | A literal clarification token trips placeholder scans | An artifact must not report a clarification it does not have |
| Stated the `Location` omission for registration explicitly in the user-API contract | `api-standards.md` expects `Location` on creation where a read endpoint exists; here none does | Documenting a deliberate deviation is required; leaving it implicit reads as an oversight |
| Added a documented cross-user `404` choice and an explicit no-`422` status convention | The API standard requires these choices to be recorded and consistent | Two hosts and two readers must not diverge on status semantics |

## Commands and observed results

Extension hook check and template setup:

```text
Test-Path .specify/extensions.yml  ->  False
setup-plan.ps1 -Json ->
  FEATURE_SPEC specs/001-task-manager/spec.md
  IMPL_PLAN    specs/001-task-manager/plan.md
  FEATURE_DIR  specs/001-task-manager
  BRANCH       001-task-manager
  "Copied plan template to specs/001-task-manager/plan.md"
copied template equals override: True
```

Toolchain observation:

```text
dotnet --version        -> 10.0.404
dotnet --list-sdks      -> 5.0.408, 8.0.428, 9.0.304, 10.0.404
node --version          -> v25.9.0
npm --version           -> 11.12.1
docker --version        -> Docker version 28.3.3, build 980b856
docker compose version  -> Docker Compose version v2.39.2-desktop.1
```

Requirement traceability scan, first draft:

```text
FRs: 17  NFRs: 6
IDs absent from plan: FR-003, FR-004, FR-005, FR-008, FR-010,
                      NFR-002, NFR-003, NFR-004, NFR-005, NFR-006
```

Requirement traceability scan, after correction:

```text
requirement IDs: 23; absent from plan: none
coverage rows: 23
```

Artifact validation:

```text
links: 17 checked, 0 broken
placeholders: only the checklist's own "[NEEDS CLARIFICATION]" item text remains, by design
ASCII: research.md, data-model.md, both contracts, spec.md and requirements.md are ASCII-only.
       plan.md reproduces the plan template's box-drawing tree glyphs in its project-structure
       section; quickstart.md contains em dashes, matching existing repository prose. Neither
       affects rendering, and no ASCII-only rule is imposed by the repository.
```

Plan structure verification:

```text
headings: Summary, Technical Context, Constitution Check, Test Strategy (Principle III),
          Security and Persistence Design (Principle IV), GenAI Evidence Capture Points
          (Principle VI), Project Structure, Complexity Tracking, Post-Design Constitution
          Check, Adopted Specification Decisions, Delivery Context
six principles present in the Constitution Check: True
dependency matrix present: True
SOLID verification table present: True
test strategy rows: 7 (Domain, Application, Infrastructure unit, Infrastructure integration,
          API controllers, API endpoints, Frontend) plus an Architecture row
```

Scope verification:

```text
No application code, project file, solution, migration SQL, or test project was created.
Only specs/001-task-manager/* documentation artifacts were written.
The constitution and the original assessment were not modified.
```

## Final references

- Plan: [specs/001-task-manager/plan.md](../../../specs/001-task-manager/plan.md)
- Research: [specs/001-task-manager/research.md](../../../specs/001-task-manager/research.md)
- Data model: [specs/001-task-manager/data-model.md](../../../specs/001-task-manager/data-model.md)
- Contracts: [contracts/user-api.md](../../../specs/001-task-manager/contracts/user-api.md),
  [contracts/task-api.md](../../../specs/001-task-manager/contracts/task-api.md)
- Quickstart: [specs/001-task-manager/quickstart.md](../../../specs/001-task-manager/quickstart.md)
- Specification: [specs/001-task-manager/spec.md](../../../specs/001-task-manager/spec.md)
- Preceding record: [EV-0007](./EV-0007-specification-001-task-manager.md)
- Implementation files: `Not Applicable` - no application code was created
- Test files: `Not Applicable` - no test project exists
- Task identifiers: `PENDING` - tasks.md has not been generated
- Commits: `4ca023d` - `feat(spec): plan and RED-first task list for 001-task-manager`

## Evidence status rationale

Status is `Captured`: the invocation, the context read, the toolchain observations, both defects with
their corrections, and the executed validation results are recorded.

It is not `Reviewed` or `Verified` because the developer has not reviewed the plan, its four
complexity-tracking decisions, or the adopted `PDC` defaults, and no commit exists for this work.
