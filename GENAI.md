# Generative AI - Assessment Response

Living response to the Generative AI section of the Ballast Lane Senior .NET technical assessment ([technical-exercise-v5.md](docs/assessment/technical-exercise-v5.md)).

This document records what was actually asked, what the AI tool actually produced, how it was validated, and what was decided. It is not a generic AI policy and not a chat transcript. Detailed evidence lives in [docs/evidence/genai/](docs/evidence/genai/README.md).

**Current status: the Task Management REST API is implemented and verified.** The backend (two API hosts over a shared domain core, PostgreSQL through Npgsql and ADO.NET) and the React frontend are complete. `dotnet test` passes 211 tests across nine assemblies with no skips, and `npm test` passes 27 frontend tests; `npm run lint`, `tsc --noEmit`, and the production build all pass. Every evidence record from `EV-0010` to `EV-0018` is `Captured`; developer review and commit references remain outstanding, so nothing here is claimed as `Verified` at the record level. Two post-implementation follow-ups requested by the developer are recorded in `EV-0024`: the backend now lives under `backend/` for symmetry with `frontend/`, and the Docker build context was measured rather than estimated.

Provenance labels used throughout: `[AI-PROPOSED]`, `[DEVELOPER DECISION]`, `[VERIFIED]`, `[PENDING]`. Only `[VERIFIED]` marks an observed result. Labels describe provenance, not approval: design choices introduced by the AI in these artifacts are marked `[AI-PROPOSED]` until the developer approves them, and approval status is tracked in the evidence records, where it currently reads `Pending` for final approval. A choice becomes `[DEVELOPER DECISION]` only once the developer has directed or approved it.

## 1. AI Tool and Development Approach

- **Primary AI coding tool:** GitHub Copilot in VS Code.
- **Specification-driven development:** GitHub Spec Kit 1.4.0, installed and configured in this repository with the Copilot integration, PowerShell scripts, and sequential feature numbering. See `.specify/init-options.json` and `.specify/integration.json`.
- **Constitution state:** `.specify/memory/constitution.md` is **ratified** at version 1.0.0 (2026-11-16) with six non-negotiable principles, and committed in `ac604af`. Feature `001-task-manager` has a specification, an implementation plan, a RED-first task list, and a completed implementation under `specs/001-task-manager/` and `src/`/`tests/`/`frontend/`.
- **Spec Kit enforcement:** project overrides in `.specify/templates/overrides/` replace the plan, tasks, and spec templates so the Constitution Check, dependency analysis, test strategy, and mandatory RED-first task ordering are enforced by the effective templates rather than by convention.
- **Evidence standard:** [docs/engineering/genai.md](docs/engineering/genai.md) defines the mandatory workflow, evidence capture rules, proportionality rules by activity type, provenance labels, evidence lifecycle, and completion gate.
- **Engineering standards:** [docs/engineering/README.md](docs/engineering/README.md) indexes the testing/TDD, SOLID, Clean Architecture, coding, API, security, database, frontend, and delivery standards, plus their applicability per Spec Kit phase and a proposed constitutional mapping.
- **Evidence skill:** [.github/skills/genai-evidence/SKILL.md](.github/skills/genai-evidence/SKILL.md) defines the three checkpoints before, during, and before completion of each meaningful activity.
- **Repository-wide instructions:** [.github/copilot-instructions.md](.github/copilot-instructions.md) states the evidence obligations and requires consulting the relevant engineering standard before each activity.

`[DEVELOPER DECISION]` The workflow order is fixed: constitution and engineering standards, specify and clarify, plan, tasks, analyze the specification, plan, tasks, and constitutional compliance, implement with TDD and the evidence checkpoints, review and converge, then conclude. Analysis may be repeated after implementation, but the pre-implementation analysis is mandatory. The rationale is that inconsistencies are cheapest to fix before code exists.

`[AI-PROPOSED]` The approach is evidence-based rather than narration-based: each meaningful activity produces a record containing the actual prompt, the original AI output, the relevant validation commands and observed results, and the developer's review decisions. Evidence is scaled to the activity type so the workflow stays practical for a small assessment. The rationale is that the assessment evaluates critical thinking about AI output, which cannot be demonstrated by asserting quality after the fact.

`[VERIFIED]` The workflow documents were created and validated on `2026-11-14`, reviewed by the developer, and corrected against six specific findings; see section 8, `EV-0001`, and `EV-0002`.

`[VERIFIED]` The constitution was ratified on `2026-11-16` at version 1.0.0 with six non-negotiable principles, the Spec Kit plan, tasks, and spec templates are overridden to enforce it, and the work is committed in `ac604af`. The developer reviewed the constitution and engineering standards as the project foundation on `2026-11-16` and directed seven targeted corrections; they are applied under `EV-0007`. See `EV-0006`.

`[PENDING]` Whether Copilot discovers and automatically invokes the `genai-evidence` skill has not been verified. The skill files exist and their frontmatter is structurally valid, but discovery and invocation in a session have not been observed and are not claimed.

## 2. Task Management REST API — Assessment Exercise

The assessment requires generating a RESTful API for a simple task management system with:

- create, read, update, and delete tasks;
- a task with a title, description, status, and due date;
- tasks associated with a user, assuming a basic User model exists.

The assessment also requires the prompt used, the output code or a representative sample, and an explanation of how the output was validated, corrected, and how edge cases, authentication, and validation were handled.

`[VERIFIED]` This exercise was executed through the Spec Kit workflow. The deliverable is the product described in [specs/001-task-manager/spec.md](specs/001-task-manager/spec.md): two API hosts over a shared application core, PostgreSQL persistence through Npgsql with ADO.NET (no ORM), and a React + TypeScript frontend.

| Assessment requirement | Where it is satisfied | Evidence |
| --- | --- | --- |
| Create, read, update, and delete tasks | `TaskApi` `TasksController` with `POST`, `GET`, `GET {id}`, `PUT`, `DELETE` | `EV-0013` to `EV-0016` |
| Title, description, status, and due date | `TaskDraft` (domain rules) and `TaskValidator` (shape rules), plus database `CHECK` constraints | `EV-0014`, `EV-0015` |
| Tasks associated with a user | `owner_id` foreign key; ownership from validated claims only | `EV-0011`, `EV-0014` |
| User model with creation and login | `UserApi` register/login plus a protected `/api/users/me` and a public `/api/public/info` | `EV-0011`, `EV-0012` |
| Prompt, output, validation, corrections, edge cases | Sections 3 to 7 and the records | `EV-0010` to `EV-0017` |

`[AI-PROPOSED]` Repository setup and documentation work is recorded separately from the API implementation so that preparatory material is never mistaken for the assessed generation task.

## 3. Actual Prompt

The developer prompt that opened the implementation activity, reproduced verbatim:

```text
/speckit-implement
```

No additional developer text was supplied for the implementation. The behaviour to build came from
the approved artifacts the command loads: [spec.md](specs/001-task-manager/spec.md),
[plan.md](specs/001-task-manager/plan.md), [tasks.md](specs/001-task-manager/tasks.md),
[data-model.md](specs/001-task-manager/data-model.md), and
[contracts/](specs/001-task-manager/contracts/), under the ratified
[constitution](.specify/memory/constitution.md) and the engineering standards.

`[VERIFIED]` That is the whole prompt. The only later developer input in the session was the command
itself; no follow-up instruction was given about architecture, libraries, or test approach, so those
decisions are the AI's proposals reviewed against the standards, not developer directions.

For completeness: the earlier prompts that produced the specification, plan, task list, and
governance corrections are reproduced in `EV-0001`, `EV-0007`, `EV-0008`, `EV-0009`, and `EV-0023`.
They are not the API generation prompt and are not presented as such.

## 4. Original AI-Generated Code

Every record preserves the original output **before** correction. Representative excerpts:

**Scaffolding defect (`EV-0010`).** The generated project files omitted the target framework, so
restore failed with `NETSDK1013: The TargetFramework value '' was not recognized`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>BallastLane.Domain</RootNamespace>
  </PropertyGroup>
</Project>
```

**Interface generated ahead of its story (`EV-0014`).** `ITaskWriter` was first produced with
`AddAsync`, `UpdateAsync`, and `DeleteAsync`, which would have implemented US5 and US6 behaviour before
their RED checkpoints:

```csharp
public interface ITaskWriter
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(TaskItem task, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(TaskId taskId, UserId ownerId, CancellationToken cancellationToken);
}
```

**Controller defect (`EV-0011`).** Data-annotation validation produced `Email`/`Password` field keys,
which do not match the documented lowercase contract:

```csharp
public sealed class RegisterRequest
{
    [Required] public string? Email { get; init; }
    [Required] public string? Password { get; init; }
}
```

**Authentication defect (`EV-0011`).** The framework's default challenge wrote an empty body, so a
`401` was not a ProblemDetails:

```csharp
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = CreateTokenValidationParameters(configuration);
    options.MapInboundClaims = false;
});
```

**Frontend defect (`EV-0011`).** The first generated provider could never authenticate, and the first
login page re-authenticated with empty credentials:

```tsx
const value: AuthState = {
  isAuthenticated: false, user: null, error: null,
  login: async () => {}, logout: () => {}, refreshCurrentUser: async () => {},
};

<LoginForm api={usersApi} onAuthenticated={() => { void auth.login({ email: "", password: "" }); }} />
```

Recorded working implementation, for comparison (`EV-0013`), showing the owner-scoped read that the
architecture requires:

```csharp
private const string FindOwnedSql =
    $"SELECT {Columns} FROM tasks WHERE id = @id AND owner_id = @ownerId";
```

## 5. Validation of AI Suggestions

`[VERIFIED]` Every suggestion was evaluated against these gates, with the result recorded per
activity rather than asserted here:

| Gate | What is checked | How it was checked in this work |
| --- | --- | --- |
| Clean Architecture | Dependency direction, separation of concerns | An architecture test reads every `.csproj`; `Domain`/`Application` reference no ASP.NET Core or Npgsql assembly; `Infrastructure` is declared only by the three composition roots (`EV-0018`) |
| SOLID | Single responsibility, interface segregation, dependency inversion | One use case per operation; boundaries exist only where there is a seam; `ITaskWriter` was narrowed when it demanded behaviour belonging to a later story (`EV-0014`) |
| TDD | A test exists, it fails without the change, it asserts meaningful outcomes | Every story recorded an observed RED before implementation; several first-attempt passes were rejected and their RED re-established (`EV-0011`, `EV-0012`, `EV-0015`, `EV-0017`) |
| Security | Authentication and authorization placement, validation, secrets, no leakage | Cross-user access, ownership-field tampering, SQL-injection payloads, token failures, and credential secrecy are automated HTTP tests (`EV-0018`) |
| Functional requirements | Behaviour matches the specification | Requirement mapping in the plan plus the executed quickstart, sections 4 and 5 (`EV-0018`) |

`[VERIFIED]` The gates were applied to real defects, not hypothetically. The compiler rejected an
`Infrastructure → SharedWeb` reference (`EV-0011`); the architecture test failed when a prohibited
`Dapper` reference was added deliberately (`EV-0010`); and the HTTP suite caught a `PUT` that silently
reset a status (`EV-0015`).

`[VERIFIED]` Validation commands and their observed output are recorded in each record, including
failures: a `NETSDK1013` restore failure, an `NpgsqlException: The operation has timed out` run that
passed unchanged on retry, and three cross-suite test failures caused by a shared database (`EV-0013`).

## 6. Corrections and Improvements

`[VERIFIED]` Corrections made to AI output during implementation, each with the observed failure that
prompted it:

| Correction | Failure that prompted it | Record |
| --- | --- | --- |
| Added `net10.0` to all 15 project files; regenerated `BallastLane.sln` | `NETSDK1013`; `dotnet new sln` emitted `.slnx` while the plan names `.sln` | `EV-0010` || Replaced data-annotation validation with the application validator | `400` bodies used `Email`/`Password` keys instead of the documented lowercase ones | `EV-0011` |
| Added a JWT challenge handler writing ProblemDetails | `401` responses had no body and no content type | `EV-0011` |
| Moved the token contract into `Application.Abstractions` | The compiler rejected `Infrastructure → SharedWeb` | `EV-0011` |
| Rewrote `AuthProvider` and `LoginPage` | The generated pair could never establish a session | `EV-0011` |
| Replaced global `Program` with per-host entry-point types | `CS0433: The type 'Program' exists in both` hosts | `EV-0012` |
| Narrowed `ITaskWriter` to the operation that existed | The interface demanded US5/US6 behaviour before their RED | `EV-0014` |
| `Created(relativeUrl)` instead of `CreatedAtAction` | Absolute `Location` leaked the internal host and ignored port mapping | `EV-0014` |
| Added `requireStatus` to the update validation path | A `PUT` without a status returned `200` and reset it | `EV-0015` |
| One database per test suite, plus a `CREATE DATABASE` advisory lock | Three suites truncated each other's rows when run together | `EV-0013` |
| Schema per test instead of a database per test; `Max Pool Size` 10; documented `dotnet test -m:1` | Parallel runs intermittently failed with `Exception while reading from stream` | `EV-0018` |
| Installed `libgssapi-krb5-2` in the runtime images | `libgssapi_krb5.so.2: cannot open shared object file` on every run | `EV-0017` |
| Enabled the Router v7 future flags | Two console warnings during ordinary use | `EV-0018` |

`[VERIFIED]` Corrections to the *process*, not the code, are recorded with the same prominence:

- `EV-0011`: the US1 domain and application types were generated as working implementations instead of
  compile-only stubs, so most of their tests passed at the first run. The implementations were backed
  up, replaced with deliberately wrong stubs, and the tests were re-run to observe genuine assertion
  failures before restoring them.
- `EV-0012`, `EV-0015`, `EV-0017`: three stories passed at first attempt because the behaviour already
  existed. In each case the protecting behaviour was temporarily removed, the tests were observed
  failing, and it was restored. A first-attempt pass proves nothing about the tests.

`[VERIFIED]` One test was corrected rather than the code: a native `type="date"` input cannot hold a
non-calendar value, so the frontend assertion for an "invalid date" covered unreachable code. The test
now asserts the reachable case (a missing date) (`EV-0014`).

## 7. Authentication, Authorization, Validation, and Edge Cases

**How users are created and credentials stored.** `POST /api/users/register` validates shape in
`UserValidator`, normalizes the email (lower case, single `@`, 320 characters at most), and hashes the
password with ASP.NET Core Identity's `PasswordHasher<T>` behind an `IPasswordHasher` boundary. Only
the hash is stored, in a `text` column; a unique index on `lower(email)` makes case-insensitive
uniqueness a database rule, and a unique violation is translated to `409`. No response contains a
password or hash, which the HTTP suite asserts (`EV-0011`).

**How credentials are issued and endpoints protected.** Login returns an HS256 bearer token with
`sub`, `email`, `jti`, issuer `ballastlane-user-api`, audience `ballastlane-clients`, and a 60-minute
lifetime. Both hosts share one `AuthenticationSetup` and use explicit
`TokenValidationParameters` with `ValidateIssuer`, `ValidateAudience`, `ValidateIssuerSigningKey`,
`ValidateLifetime`, and `ClockSkew = TimeSpan.Zero`. Missing, malformed, bad-signature, and expired
tokens all return `401` as ProblemDetails. A fallback authorization policy denies anonymous access to
any route without an explicit `[AllowAnonymous]`, so protection never depends on remembering an
attribute (`EV-0011`, `EV-0012`).

**Authorization rules for another user's task.** Ownership is derived only from validated claims
(`ClaimsPrincipalExtensions.ToAuthenticatedUser`) and enforced twice: in the application use case, and
in SQL as `WHERE ... owner_id = @ownerId`. Cross-user retrieve, update, and delete return `404` - the
single documented choice - so existence is never confirmed. A client-supplied `ownerId` is not bound to
the request DTO at all, so it cannot be trusted by accident (`EV-0013` to `EV-0016`).

**Which validation rules exist and where they live.** Shape and business rules live in
`Application` (`TaskValidator`, `UserValidator`) and `Domain` (`TaskDraft`, `Email`): title required,
trimmed, 1-200 characters; description optional, at most 2,000; `dueDate` exactly `yyyy-MM-dd` with no
time or timezone; status one of exactly three names (case-sensitive), defaulting to `Pending` on create
and required on update. Database `CHECK` constraints repeat the invariants as a second line of defence.
A `400` body carries an `errors` object keyed by the same field names the frontend binds to.

**Edge cases handled.**

| Edge case | Behaviour | Evidence |
| --- | --- | --- |
| Duplicate registration, including a different letter case | `409`, one account remains | `EV-0011` |
| Wrong password vs unknown email | Identical `401` body apart from the per-request correlation id | `EV-0011` |
| Blank, whitespace-only, or missing title | `400` with a `title` error; nothing persisted | `EV-0014` |
| `dueDate` with a time component, or a non-date | `400` with a `dueDate` error | `EV-0014`, `EV-0015` |
| Status `Done`, or a lower-cased status | `400` with a `status` error | `EV-0014`, `EV-0015` |
| Past due date | Accepted, no automatic status change | `EV-0014` |
| Unknown identifier, and a non-UUID identifier | `404` in both cases | `EV-0013`, `EV-0015`, `EV-0016` |
| Another user's task, for read, update, delete | `404`, and the record is unchanged | `EV-0013` to `EV-0016` |
| Ownership field supplied in a payload | Ignored; the token subject stays the owner | `EV-0014`, `EV-0018` |
| SQL-injection payload in a text field | Stored and returned verbatim; the table survives | `EV-0018` |
| Empty account | `200` with `[]`, and an empty state in the UI | `EV-0013` |
| Re-running the seed | No duplicates, no failure | `EV-0017` |

**Edge cases deliberately not handled.**

- Concurrent edits by the same user: the last write wins; the specification places this out of scope.
- Token revocation on logout: logout clears the client session only; there is no server-side deny list.
- Token refresh: the user signs in again after 60 minutes.
- Account lifecycle (password reset, email change, deletion): explicitly out of scope.

## 8. Testing and Verification Results

`[VERIFIED]` Final full-suite result, executed from the repository root against a running PostgreSQL
container:

```text
dotnet build BallastLane.sln        -> Build succeeded. 0 Warning(s) 0 Error(s)
dotnet test BallastLane.sln -m:1    -> 211 passed, 0 failed, 0 skipped (nine assemblies)

BallastLane.Domain.Tests             31
BallastLane.Application.Tests        44
BallastLane.Infrastructure.UnitTests 18
BallastLane.Api.UnitTests            18
BallastLane.Architecture.Tests        9
BallastLane.PostgreSql.Tests          7
BallastLane.TaskApi.HttpTests        53
BallastLane.Migrations.Tests          9
BallastLane.UserApi.HttpTests        22

cd frontend
npm run lint      -> no findings
npx tsc --noEmit  -> exit 0
npm run build     -> built in 1.82s
npx vitest run    -> 8 files, 27 tests passed
```

`[VERIFIED]` Quickstart sections 4 and 5 were executed request by request against the running
containers and every documented expectation was observed, including the `409` duplicate, the
indistinguishable `401`s, calendar-date rejection, all six status transitions, cross-user `404`s, and
`204` followed by `404`. Section 6 was driven in a browser: login with the documented credentials, list,
create, view, edit, and delete with confirmation, plus the empty and loading states, field-level
validation, and a 375-pixel viewport with no horizontal scrolling (`EV-0018`).

`[PENDING]` Two checks were not performed and are not claimed:

- **Keyboard-only traversal and focus visibility.** The headless harness used here could not perform
  trusted Tab input; focus remained on `body`. The automated tests assert semantic controls and labels,
  which is not the same evidence.
- **Expired-token redirect in the browser.** It is covered by an automated frontend test and by the
  `401` handling tests, but no 60-minute wait was performed.

### Earlier evidence (documentation and specification work)

| Command or check | Observed result |
| --- | --- |
| `python -c "import yaml; print(...)"` | `pyyaml 6.0.3` available for frontmatter validation |
| Frontmatter parse of `SKILL.md` | Parsed keys: `name`, `description`, `compatibility`, `metadata` |
| Skill name vs directory | `genai-evidence` matches `.github/skills/genai-evidence`; pattern valid; description 696 of 1024 characters |
| ASCII-only scan of the created documents | No non-ASCII characters in the five workflow documents; the only non-ASCII character in `GENAI.md` is the em dash of the required section title |

### Documentation work performed so far

| Command or check | Observed result |
| --- | --- |
| `python -c "import yaml; print(...)"` | `pyyaml 6.0.3` available for frontmatter validation |
| Frontmatter parse of `SKILL.md` | Parsed keys: `name`, `description`, `compatibility`, `metadata` |
| Skill name vs directory | `genai-evidence` matches `.github/skills/genai-evidence`; pattern valid; description 696 of 1024 characters |
| ASCII-only scan of the created documents | No non-ASCII characters in the five workflow documents; the only non-ASCII character in `GENAI.md` is the em dash of the required section title |
| Relative-link resolution scan | Four broken links in `copilot-instructions.md` plus pending targets; all resolved after correction and after `GENAI.md` and `EV-0001` were created. Final prose-only scan: 40 links checked, 0 broken |
| `git diff -- docs/assessment/technical-exercise-v5.md` | Empty output; the assessment document is unchanged |

`[VERIFIED]` These are actual observed results from this session.

### Documentation work: refinement per developer review (`EV-0002`)

| Command or check | Observed result |
| --- | --- |
| Spec Kit state inspection | `init-options.json`: `speckit_version` `1.4.0`, `integration` `copilot`, `script` `ps`. `integration.json`: version `1.4.0`, `installed_integrations` `["copilot"]` |
| Workflow order check | Parsed headings: Phase 1 Constitution, Phase 2 Specify, Phase 3 Plan, Phase 4 Tasks, Phase 5 Analyze, Phase 6 Implement, Phase 7 Review, Phase 8 Conclude. `analyze before implement: True` |
| Skill frontmatter re-parse | Keys `compatibility`, `description`, `metadata`, `name`; `name==dir: True`; description 696 of 1024 characters |
| Relative-link resolution scan | 53 prose links checked, 0 broken |
| Non-ASCII prose scan | No non-ASCII in any document except the em dash of the required GENAI.md section title |
| Inaccurate-claim scan | All regex hits inspected; each is a prohibition or a description of the corrected defect, not a live claim. No absolute Windows paths; no claim that a commit must exist before it is created |
| `git status --porcelain -- .specify .github/skills/speckit-*` | Empty output: the Spec Kit installation and its bundled skills were not modified |
| `git diff --exit-code -- docs/assessment/technical-exercise-v5.md` | Exit code `0`; the assessment document is unchanged |

`[VERIFIED]` These are actual observed results from this session. `[PENDING]` Copilot skill discovery and automatic invocation, which have not been observed.

### Documentation work: engineering standards (`EV-0003`)

| Command or check | Observed result |
| --- | --- |
| Deadline verification | `2026-11-16` is a Monday; `08:00 -03:00`; approximately 13.8 hours remained when the documentation was written |
| Git history verification | `origin/main` contains `859b141`, `b3b6195`, `0439fbb`; `rev-list --left-right --count` returned `0 0`, so local and remote agree |
| Required sections in the nine standards | All nine contain purpose, mandatory rules, practical examples, prohibited practices, verification criteria, and references |
| Section numbering | Sequential in every standard after inserting the missing example sections |
| Internal links and anchors | 159 links checked, 0 broken; 1 internal anchor checked, valid |
| Cross-document rule consistency | 15 targeted assertions pass, including dependency direction, PostgreSQL integration tests, security test linkage, two-API requirement, and absence of extra infrastructure |
| Prohibited technologies | Entity Framework, EF Core, Dapper, MediatR, and Mediator appear only as prohibitions or assessment statements |
| Scope restrictions | No `.cs`, `.csproj`, `.sln`, `.ts`, `.tsx`, or `package.json` outside Spec Kit tooling; no tracked symbolic links; assessment and constitution unchanged (`git diff --exit-code` returned `0`) |
| Defects found and corrected | Two standards lacked the required practical-examples section; the index used a non-ASCII section sign with now-stale numbers |

`[VERIFIED]` These are actual observed results from this session. No application test was executed, because no application exists.

### Constitution ratification and Spec Kit integration (`EV-0006`)

| Command or check | Observed result |
| --- | --- |
| Extension hook check | `.specify/extensions.yml` absent; no hooks registered or executed |
| Constitution template resolution | `resolve-template.ps1 constitution-template -Json` succeeded and returned the unratified scaffold |
| Override resolution after creation | `plan-template override_active=True chars=6595`; `tasks-template override_active=True chars=7879`; `spec-template override_active=True chars=3836` |
| Constitution content | No placeholders remaining; six principles present; version 1.0.0, ratified 2026-11-16; no non-ASCII; no post-submission freeze in the body |
| Link and anchor scan | 242 links, 0 broken files, 0 broken anchors, after correcting 23 root-relative links |
| Skill enforcement markers | `speckit-tasks` contains "Tests are MANDATORY" and no longer contains "Tests are OPTIONAL"; analyze, implement, converge, and plan edits present |
| Installed templates | `git status --porcelain -- .specify/templates/*.md` empty; the overrides are additive |
| Scope | No application code, no project files, no tracked symbolic links |
| Defects found and corrected | 23 broken constitution links; one inline-code-wrapped markdown link; duplicate step numbering in the implement skill |

`[VERIFIED]` These are actual observed results from this session.

### Implementation plan and Phase 1 design (`EV-0008`)

| Command or check | Observed result |
| --- | --- |
| Template setup (`setup-plan.ps1 -Json`) | Copied plan template to `specs/001-task-manager/plan.md`; copied content identical to the project override |
| Toolchain observation | .NET SDKs 5.0.408, 8.0.428, 9.0.304, 10.0.404 (default 10.0.404); Node v25.9.0; npm 11.12.1; Docker 28.3.3; Compose v2.39.2 |
| Requirement traceability scan, first draft | 17 FRs and 6 NFRs declared, but 10 identifiers absent from the plan: FR-003, FR-004, FR-005, FR-008, FR-010, NFR-002, NFR-003, NFR-004, NFR-005, NFR-006 |
| Requirement traceability scan, after correction | 23 requirement IDs, 0 absent, 23 coverage rows |
| Artifact links | 17 links checked, 0 broken |
| Plan structure | All required sections present: Constitution Check with six principles, dependency matrix, SOLID table, seven test layers plus an architecture test, security and persistence design, GenAI capture points, complexity tracking, post-design re-check |
| Scope | No application code, solution, migration SQL, or test project created; constitution and original assessment unmodified |

`[VERIFIED]` These are actual observed results from this session. The first-draft traceability gap is
the notable one: the plan asserted complete requirement coverage that a scan disproved.

### Task generation (`EV-0009`)

| Command or check | Observed result |
| --- | --- |
| Template provenance (`setup-tasks.ps1 -Json`) | `TASKS_TEMPLATE` resolved to `.specify/templates/overrides/tasks-template.md`, 9,836 characters |
| First-list validation | 107 task lines, 33 missing a file path, duplicate identifier from an inserted `T005a`, 36 tasks with non-explicit RED dependencies |
| Final-list validation | 106 task lines, 0 format violations, 106 unique sequential identifiers, no gaps, no duplicates |
| Per-story counts | US1 13, US2 7, US3 13, US4 12, US5 11, US6 10, US7 6; 34 Setup/Foundational/Polish |
| RED lineage | 8 direct RED dependencies plus 24 inherited lineage statements; 0 implementation tasks without a documented RED checkpoint |
| Parallel markers | 34 `[P]` tasks, all test-authoring or independent verification; none crosses a test/implementation pair |

`[VERIFIED]` These are actual observed results from this session.

### API implementation

`[VERIFIED]` Build, test, and container output observed in this session:

| Command | Observed result |
| --- | --- |
| `dotnet build BallastLane.sln` | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| `dotnet test BallastLane.sln -m:1` | `211 passed, 0 failed, 0 skipped` across nine assemblies, twice consecutively |
| `docker compose run --rm migrator --seed` | `Migrations applied from '/app/db/migrations'. Demonstration data seeded.` |
| `docker compose run --rm migrator --seed` (second run) | Same output; `users = 2, tasks = 5`, unchanged |
| `docker compose down` then `up -d postgres` | `health: healthy`; `users = 2, tasks = 5` after the restart |
| `npm run lint` / `npx tsc --noEmit` / `npm run build` | No findings / exit 0 / built in 1.82 s |
| `npx vitest run` | `8 files, 27 tests passed` |

Failures encountered and their resolutions are listed in section 6 and in each story record.

### Governance corrections and feature specification (`EV-0007`)

| Command or check | Observed result |
| --- | --- |
| Extension hook check (`Test-Path .specify/extensions.yml`) | `False`; no hooks registered or executed |
| Effective tasks template after corrections | `Tests are OPTIONAL`: `False`; `(if requested)`: `False`; RED-first foundational rule present: `True` |
| Template resolver output versus override file | Identical; the override is the effective spec template |
| Residual conditional-wording scan | Remaining hits are historical verification statements in `GENAI.md` and `EV-0006`, and the superseded installed core template |
| Specification validation, iteration 1 | Found three defects: a literal clarification marker token in prose, undefined `EC`/`DO` identifiers, and TDD mapped without being named |
| Specification validation, iteration 2 | Placeholders absent; referenced-but-undefined IDs: none; `FR` 17, `NFR` 6, `SC` 8, `PDC` 8, `EC` 5, `DO` 5 all defined |
| Specification link targets | Both assessment links resolve |
| Link resolution across all changed documents | 109 live links checked with fenced code excluded, 0 broken; the two apparent hits in `EV-0006` are quoted defect evidence inside code fences |
| Final semantic review | One further defect found and corrected: `FR-003` was mapped to US7 only, although US1's first scenario asserts user persistence |
| Scope verification | 9 tracked files modified, 2 governance/spec paths added; the constitution and the original assessment are unmodified; no application code and no tracked symbolic links |

`[VERIFIED]` These are actual observed results from this session. No application test was executed, because no application exists.

## 9. Engineering Decisions and Lessons Learned

`[AI-PROPOSED]` Decisions embodied in this work, pending final developer approval recorded in the evidence records:

- Separate the AI proposal, the developer decision, and the verified result, using explicit labels in every record.
- Gate status transitions on evidence: a record advances from `Captured` to `Reviewed` to `Verified` only when the corresponding evidence exists.
- Keep one record per meaningful activity or related task group, so a single activity is not fragmented across files.
- Make the evidence workflow enforceable through repository instructions and a skill, rather than relying on convention.
- Treat the documentation work as its own record, distinct from the assessed API exercise.
- Scale evidence to the activity type and mark inapplicable requirements `Not Applicable`, so the workflow stays practical for a small assessment.
- Record commit references only after the commit exists, and track capture, review, validation, and committed state separately.

`[DEVELOPER DECISION]` The workflow order was corrected on review so that analysis of the specification, plan, tasks, and constitutional compliance precedes implementation, with post-implementation analysis kept as an optional repeat.

Observations from this work:

- `[VERIFIED]` Three defects were found by executed checks rather than by inspection: four broken relative links in the Copilot instructions, a provenance mislabeling where unreviewed AI choices were marked as developer decisions, and an evidence record that asserted a wording change that never occurred. All three are recorded with their corrections rather than removed.
- `[VERIFIED]` The first version of the standard placed analysis after implementation and understated the repository state by implying Spec Kit was not installed. Both were corrected on developer review; see `EV-0002`.
- `[VERIFIED]` Evidence discipline is easier to satisfy when validation is automated. Each defect above surfaced from a script or a review, not from reading the documents again.
- `[VERIFIED]` On 2026-11-16 the same pattern repeated at a larger scale: all three specification defects were found by scripted checks (placeholder scan, identifier definition check, keyword coverage scan), not by re-reading the generated document. Writing an identifier in a traceability table does not mean it exists.
- `[VERIFIED]` A governance rule expressed in a template can still leak through the skill that generates the artifact, because the skill is the instruction source. Correcting only the template would have left "tests (if requested)" in the effective workflow.
- `[VERIFIED]` The same traceability failure recurred at the plan phase: the plan claimed full requirement coverage while ten requirement identifiers were absent. The lesson is that a coverage claim is only credible when a mechanical scan can enumerate every requirement and point to where it is verified.
- `[PENDING]` That Copilot automatically selects the `genai-evidence` skill has not been observed and is not claimed. Per vendor documentation, skill selection is advisory; the repository instructions exist partly as a backstop, but this has not been tested here. Whether the directory name must equal the frontmatter `name` is likewise documented behavior, not a locally verified finding.

Lessons from implementing and testing the API, and from reviewing generated code:

- `[VERIFIED]` **A first-attempt pass is not a TDD cycle.** Four stories produced tests that passed immediately, either because the implementation was generated alongside them (US1 domain/application) or because the behaviour already existed from an earlier story (US2, US5, US7). Each time, the protecting behaviour was temporarily removed to observe the real failure. The habit is to distrust a green run on new tests and prove the test can fail.
- `[VERIFIED]` **The compiler is the cheapest architecture reviewer.** It rejected `Infrastructure → SharedWeb` (`CS0103`) and two hosts exposing a global `Program` to one test project (`CS0433`). Both were design defects that a review may have missed.
- `[VERIFIED]` **Asserting declarations beats asserting assemblies.** The architecture test first read runtime assembly references, but the compiler omits an unused reference, so a deliberately added `Dapper` reference went unnoticed until code used it. Reading the `.csproj` files catches the declaration itself.
- `[VERIFIED]` **Test isolation is a real defect class, not housekeeping.** Three suites passed alone and failed together because they shared one database; two `CREATE DATABASE` statements contended on the template lock. One database per suite, then a schema per test, plus a capped pool and a documented sequential command, made the full run deterministic.
- `[VERIFIED]` **The same defect pattern recurred three times before it was fixed properly.** Sharing a database, then `CREATE DATABASE` per test, then connection pressure across five concurrent processes each produced the same symptom (a passing suite that fails in a full run). Each fix was verified by an observed failing full run, not by reasoning alone.
- `[VERIFIED]` **Environmental failures must be distinguished from product failures.** Two runs failed with `NpgsqlException: The operation has timed out` and passed unchanged on retry. Recording them as product defects would have been wrong; hiding them would have been dishonest. Both are recorded with the reasoning. One full run also failed after the documenting command ran the suite twice concurrently - the failure of the verification procedure rather than of the code.
- `[VERIFIED]` **Generated frontend code tends to look finished while being inert.** The first `AuthProvider` could never authenticate and the first login page submitted empty credentials, yet both compiled, linted, and rendered. Driving the real UI in a browser was what exposed the difference between "renders" and "works".
- `[VERIFIED]` **A shared validator can carry the wrong story's semantics.** `TaskValidator.Validate` treating an omitted status as `Pending` is right for creation and wrong for update, and it silently reset a status until an HTTP test caught it.
- `[VERIFIED]` **Stub bodies must be wrong, not merely empty.** A stub that returned `200` with the value satisfied the success-path assertion, so that test could never fail; making the stub return `501` produced a genuine RED.
- `[PENDING]` Lessons that require developer review of these records, and the two unverified manual checks (keyboard traversal, expired-token redirect).

### Repository and build hygiene (follow-up, 2026-11-16)

`[VERIFIED]` A developer-requested check of `.gitignore` against the completed tree confirmed it
already covered every artifact the project produces: `git ls-files` matched no `bin/`, `obj/`,
`node_modules/`, `dist/`, coverage, or `.trx` path; `git add -An .` listed 197 paths with none
artifact-like; and `.env` was ignored while `.env.example` was not. One gap was closed: a bare `*.log`,
since only `msbuild.log`, `msbuild.err`, and `msbuild.wrn` were named explicitly.

`[VERIFIED]` The same check exposed a related omission rather than a `.gitignore` defect: **no
`.dockerignore` existed**, while all three images build with `context: .`. That sent the entire working
tree to the daemon on every build, including `frontend/node_modules`. A `.dockerignore` was added; the
patterns it declares cover 239.5 MB of a 240 MB tree, leaving an expected context of about 0.5 MB
across 182 files.

`[PENDING]` That reduction is **computed by applying the declared patterns locally, not observed**.
Docker Desktop stopped between the earlier container runs and this check
(`open //./pipe/dockerDesktopLinuxEngine: The system cannot find the file specified`), so no
`docker compose build` was run to confirm the transferred context size. Re-running
`docker compose build migrator` with the daemon available would confirm or refute it.

`[VERIFIED]` The confirming run was performed once Docker Desktop was reachable again, during the
`backend/` restructure recorded in `EV-0024`. `docker compose build migrator` reported
`#5 transferring context: 900B` and `#6 transferring context: 108.19kB`, and a later run of the same
command on a warm cache reported `900B` and `6.30kB`. Every observation is orders of magnitude below
the 240 MB working tree, so the reduction is real. The computed estimate of about 0.5 MB across 182
files was in the right range but not exact; the measurements stand as the observed values.

### Repository layout restructure (follow-up, 2026-11-16)

`[VERIFIED]` A developer review noted an asymmetry: a `frontend/` folder existed but the backend was
scattered across `src/`, `tests/`, and `db/` at the repository root. The backend was moved under
`backend/`, keeping `backend/src`, `backend/tests`, and `backend/db` while `BallastLane.sln`,
`docker-compose.yml`, `Directory.Build.props`, `.gitignore`, and `.dockerignore` stayed at the root
because they describe the whole repository. Sixteen solution project paths, nine test project
references, three Dockerfiles, the compose file, four test helpers, and seven architecture-test
path constants were updated. `dotnet build` succeeded, all 211 backend tests passed, and
`docker compose run --rm migrator --seed` succeeded after the move. Living documents (`README.md`,
`plan.md`, `research.md`, and the two engineering standards that cite concrete paths) were updated;
historical artifacts (`tasks.md` and the pre-move evidence records) were left intact with a dated note
pointing here, because rewriting them would misrepresent what was actually executed and observed. See
`EV-0024`.

## 10. Supporting Evidence

| Evidence | Location | Status |
| --- | --- | --- |
| `EV-0001` - repository GenAI standards and evidence workflow | [docs/evidence/genai/EV-0001-repository-genai-standards.md](docs/evidence/genai/EV-0001-repository-genai-standards.md) | `Verified` - committed in `b3b6195`, `0439fbb` |
| `EV-0002` - refinement per developer review | [docs/evidence/genai/EV-0002-standard-refinement.md](docs/evidence/genai/EV-0002-standard-refinement.md) | `Verified` - committed in `b3b6195`, `0439fbb` |
| `EV-0003` - engineering standards and submission requirements | [docs/evidence/genai/EV-0003-engineering-standards.md](docs/evidence/genai/EV-0003-engineering-standards.md) | `Verified` - committed in `9906b6a` |
| `EV-0004` - TDD enforcement lifecycle across Spec Kit phases | [docs/evidence/genai/EV-0004-tdd-enforcement-lifecycle.md](docs/evidence/genai/EV-0004-tdd-enforcement-lifecycle.md) | `Verified` - committed in `ba92121` |
| `EV-0005` - separation of engineering governance and submission process rules | [docs/evidence/genai/EV-0005-governance-separation.md](docs/evidence/genai/EV-0005-governance-separation.md) | `Verified` - committed in `f59bc6b` |
| `EV-0006` - constitution ratification and Spec Kit integration | [docs/evidence/genai/EV-0006-constitution-ratification.md](docs/evidence/genai/EV-0006-constitution-ratification.md) | `Reviewed` - committed in `ac604af`; seven review findings applied under `EV-0007` |
| `EV-0007` - targeted governance corrections and feature specification `001-task-manager` | [docs/evidence/genai/EV-0007-specification-001-task-manager.md](docs/evidence/genai/EV-0007-specification-001-task-manager.md) | `Captured` - committed in `3e8d0c9`; developer review `PENDING` |
| `EV-0008` - implementation plan, research, and Phase 1 design | [docs/evidence/genai/EV-0008-implementation-plan-001-task-manager.md](docs/evidence/genai/EV-0008-implementation-plan-001-task-manager.md) | `Captured` - developer review `PENDING` |
| `EV-0009` - task generation with RED-first ordering | [docs/evidence/genai/EV-0009-task-generation-001-task-manager.md](docs/evidence/genai/EV-0009-task-generation-001-task-manager.md) | `Captured` - developer review `PENDING` |
| `EV-0020` / `EV-0022` / `EV-0023` - analysis and remediation before implementation | [EV-0020](docs/evidence/genai/EV-0020-analysis-001-task-manager.md), [EV-0022](docs/evidence/genai/EV-0022-critical-remediation-and-reanalysis.md), [EV-0023](docs/evidence/genai/EV-0023-final-corrections-and-second-reanalysis.md) | `Captured` - no unresolved blocking finding remained |
| `EV-0010` - setup scaffolding and foundational layer (T001-T024) | [docs/evidence/genai/EV-0010-setup-and-foundational-tdd.md](docs/evidence/genai/EV-0010-setup-and-foundational-tdd.md) | `Captured` - RED and GREEN observed |
| `EV-0011` - US1 registration and authentication (T025-T037) | [docs/evidence/genai/EV-0011-us1-registration-authentication.md](docs/evidence/genai/EV-0011-us1-registration-authentication.md) | `Captured` - RED re-established after a process deviation |
| `EV-0012` - US2 public and protected access (T038-T044) | [docs/evidence/genai/EV-0012-us2-access-boundary.md](docs/evidence/genai/EV-0012-us2-access-boundary.md) | `Captured` - RED observed by removing the protections |
| `EV-0013` - US3 list and view tasks (T045-T057) | [docs/evidence/genai/EV-0013-us3-task-read.md](docs/evidence/genai/EV-0013-us3-task-read.md) | `Captured` - includes the cross-suite concurrency defect |
| `EV-0014` - US4 create a task (T058-T069) | [docs/evidence/genai/EV-0014-us4-task-create.md](docs/evidence/genai/EV-0014-us4-task-create.md) | `Captured` |
| `EV-0015` - US5 update a task (T070-T080) | [docs/evidence/genai/EV-0015-us5-task-update.md](docs/evidence/genai/EV-0015-us5-task-update.md) | `Captured` - required-status defect found by HTTP tests |
| `EV-0016` - US6 delete a task (T081-T090) | [docs/evidence/genai/EV-0016-us6-task-delete.md](docs/evidence/genai/EV-0016-us6-task-delete.md) | `Captured` |
| `EV-0017` - US7 seeded demonstration (T091-T096) | [docs/evidence/genai/EV-0017-us7-demonstration-environment.md](docs/evidence/genai/EV-0017-us7-demonstration-environment.md) | `Captured` - includes executed quickstart sections 3 and 8 |
| `EV-0018` - verification and convergence (T097-T106) | [docs/evidence/genai/EV-0018-review-and-convergence.md](docs/evidence/genai/EV-0018-review-and-convergence.md) | `Captured` - two manual checks explicitly pending |
| `EV-0024` - backend folder restructure and observed Docker build context | [docs/evidence/genai/EV-0024-backend-folder-restructure.md](docs/evidence/genai/EV-0024-backend-folder-restructure.md) | `Captured` - build, 211 tests, and container run re-verified after the move |
| Generative AI engineering standard | [docs/engineering/genai.md](docs/engineering/genai.md) | `Verified` - committed |
| Engineering standards index and nine standards | [docs/engineering/README.md](docs/engineering/README.md) | Established and accepted as the project foundation on 2026-11-16; targeted corrections recorded in `EV-0007` |
| Submission requirements | [docs/assessment/submission-requirements.md](docs/assessment/submission-requirements.md) | Recorded and accepted; constitution status synchronized 2026-11-16 |
| Evidence skill | [.github/skills/genai-evidence/SKILL.md](.github/skills/genai-evidence/SKILL.md) | `Verified` - committed; discovery and invocation unverified |
| Evidence index and template | [docs/evidence/genai/README.md](docs/evidence/genai/README.md), [template.md](docs/evidence/genai/template.md) | `Verified` - committed |
| Repository Copilot instructions | [.github/copilot-instructions.md](.github/copilot-instructions.md) | Updated with the standards index; review `PENDING` |
| Spec Kit installation | `.specify/init-options.json`, `.specify/integration.json` | Installed and configured, version 1.4.0, Copilot integration |
| Constitution | `.specify/memory/constitution.md` | **Ratified** - version 1.0.0, 2026-11-16; committed in `ac604af` |
| Feature specification `001-task-manager` | [specs/001-task-manager/spec.md](specs/001-task-manager/spec.md) | Draft - developer review `PENDING` |
| Assessment document | [docs/assessment/technical-exercise-v5.md](docs/assessment/technical-exercise-v5.md) | Unmodified |
| Git history | `859b141` foundation, `b3b6195` standards and workflow, `0439fbb` commit references, `9906b6a` standards, `ba92121` TDD lifecycle, `fd309c7` evidence reference, `f59bc6b` governance separation, `0018be0` evidence reference, `ac604af` constitution v1.0.0; all present on `origin/main` | `[VERIFIED]` |

### Coverage of the assessment's Generative AI questions

| Assessment question | Where it is answered | Status |
| --- | --- | --- |
| Write the prompt used to generate the API scaffold or implementation | Section 3; `EV-0010` to `EV-0017` | `[VERIFIED]` |
| Show the output code or a representative sample | Section 4; original excerpts preserved in each record before correction | `[VERIFIED]` |
| Describe how AI suggestions were validated | Section 5; per-activity validation tables | `[VERIFIED]` |
| Describe how output was corrected or improved | Section 6; twelve code corrections and four process corrections with their triggering failures | `[VERIFIED]` |
| Describe how edge cases, authentication, and validations were handled | Section 7, with the 13 handled and 4 deliberately unhandled cases | `[VERIFIED]` |
| Demonstrate fluency with AI tools and prompt engineering | Sections 1, 5, 6, and 9; the prompt was a single command whose constraints came from ratified artifacts, and the review findings show where the output was rejected or reworked | `[VERIFIED]` |
| Keyboard-only operation and focus visibility | Section 8 | `[PENDING]` - not performed |
| Expired-token redirect observed in the browser | Section 8 | `[PENDING]` - covered by automated tests only |
| Developer review and approval of the evidence records | `docs/evidence/genai/` | `[PENDING]` |
