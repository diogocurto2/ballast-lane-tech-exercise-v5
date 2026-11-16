# EV-0018 - Verification and convergence of 001-task-manager

> **Revision note (2026-11-16, after `EV-0024`).** Paths in this record reflect the repository layout
> at the time of the activity, when the backend lived at the root as `src/`, `tests/`, and `db/`. After
> the `backend/` restructure recorded in [EV-0024](./EV-0024-backend-folder-restructure.md), those
> locations are `backend/src/`, `backend/tests/`, and `backend/db/`. Nothing else in this record was
> altered, so it remains an accurate account of what was observed when it was written.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0018` |
| Date | `2026-11-16` |
| Development phase | Phase 7 - Review, validate, and converge (T097 to T106) |
| Activity type | `Review` and `Testing` |
| Related specification | `specs/001-task-manager/spec.md` |
| Related task identifiers | `T097` to `T106` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `PENDING` |

## Actual prompt

Continuation of the developer instruction that opened the session:

```text
/speckit-implement
```

`tasks.md` Phase 10 defined the verification work.

## Relevant context

- Standards: `clean-architecture.md`, `solid-principles.md`, `security.md` 5, `delivery-and-ci.md`
  2.5-2.8, `testing-and-tdd.md` 6.
- `quickstart.md` sections 4, 5, 6, 7, and 8 were executed against running containers.

## Original AI-generated output

`Not Applicable` - this activity produced verification results and documentation rather than new
generated code. The only generated code change was the React Router future flags, shown under
"Corrections and improvements".

## Validation performed

| Area | Which was checked | Result |
| --- | --- | --- |
| Clean Architecture | Every project reference, and which projects reference `Infrastructure` | `[VERIFIED]` |
| SOLID | No aggregate/repository/mediator abstraction was introduced; each boundary has a consumer | `[VERIFIED]` |
| TDD | Full suite executed; every story recorded its RED before its implementation | `[VERIFIED]` |
| Security | Cross-user access, ownership-field tampering, SQL-injection payload, credential secrecy, duplicate registration | `[VERIFIED]` |
| Functional requirements | Quickstart sections 4 and 5 executed step by step; section 6 driven in a real browser | `[VERIFIED]` with two items recorded as not manually verified |

## Review findings

- `[VERIFIED]` **T097 dependency direction.** `Domain` declares no reference; `Application` declares
  only `Domain`; `Infrastructure` declares `Application` and `Domain`; `SharedWeb` declares
  `Application` and `Domain`; both hosts declare `Application`, `Domain`, `Infrastructure`, and
  `SharedWeb`; `Migrator` declares `Infrastructure`. `Infrastructure` is declared by exactly
  `Migrator`, `TaskApi`, and `UserApi` - the three composition roots. The architecture test passes 9/9.
- `[VERIFIED]` **T098 prohibited libraries.** `dotnet list BallastLane.sln package --include-transitive`
  filtered for `EntityFramework`, `Dapper`, `MediatR`, and `Mediator` returned no rows, so none is
  referenced directly or transitively.
- `[VERIFIED]` **T099 security.** The Task API HTTP suite passes 53/53 including cross-user retrieve,
  update, and delete; ownership-field tampering; a stored SQL-injection payload read back verbatim;
  and the identifier-parameter injection attempt returning `404`. The User API suite passes 22/22
  including duplicate registration `409` and the absence of any password or hash in responses.
- `[VERIFIED]` **T100 frontend.** `npm run lint` reports no findings, `tsc --noEmit` exits 0, and the
  production build completes in 1.82 s.
- `[VERIFIED]` **T101 full suite.** Nine backend assemblies pass with 205 tests and no skips; eight
  frontend test files pass with 27 tests.
- `[VERIFIED]` **T102 quickstart.** Sections 4 and 5 were executed request by request with the
  documented results, and section 6 was driven in the browser; results are listed below.
- `[VERIFIED]` **T105 re-analysis.** Run against the implemented artifacts: 107 tasks, all complete;
  25 requirement identifiers, none absent from `tasks.md`; 30 implementation tasks, all 30 with an
  explicit RED reference; 33 `[P]` markers, none on an implementation task. One CRITICAL finding (a
  frontend assertion over unreachable code) had already been corrected during implementation, and no
  blocking finding remains.
- `[VERIFIED]` **Console cleanliness.** After opting into the React Router v7 future flags, the
  browser console recorded no warnings and no errors during login, list, create, view, edit, delete,
  and the empty state.
- `[PENDING]` **Keyboard-only traversal and focus visibility were not verified manually.** Trusted
  clicks and Tab traversal could not be performed in the headless harness used here (actionability
  waits timed out and focus stayed on `body`), so this remains an unverified manual check rather than
  a claimed pass. The automated suite asserts semantic controls, labels, and `role`-based assertions,
  which is not the same thing.
- `[PENDING]` **Expired-token redirect was not reproduced manually.** It is covered by an automated
  frontend test and by the `401` handling test, but waiting for a 60-minute token was not performed.
- `[VERIFIED]` **T106 hygiene.** `.env` is ignored; `.env.example` is tracked; no tracked symlinks
  (`git ls-files -s` has no mode `120000`); no `C:\Users\` path in tracked Markdown; a scan of the
  working tree for the local demo password and signing-key patterns returned zero hits.

`[VERIFIED]` **Follow-up check (2026-11-16, developer-requested).** `.gitignore` was verified against
the completed tree: no tracked `bin/`, `obj/`, `node_modules/`, `dist/`, coverage, or `.trx` path;
197 paths would be added and none is artifact-like; `.env` ignored, `.env.example` not. A bare `*.log`
rule was added because only three specific MSBuild log names were covered. The check also found that no
`.dockerignore` existed while all three images build with `context: .`; one was added, and the
patterns it declares cover 239.5 MB of a 240 MB tree. `[PENDING]` That context reduction is computed
rather than observed: Docker Desktop had stopped before a confirming `docker compose build` could run.

`[VERIFIED]` **Follow-up observation (2026-11-16, during `EV-0024`).** The computed estimate above was
confirmed by measurement once Docker Desktop was available again. Running
`docker compose build migrator` produced `#5 transferring context: 900B` and
`#6 transferring context: 108.19kB`, and a later run of the same command on a warm cache produced
`900B` and `6.30kB`. Every observation is orders of magnitude below the 240 MB working tree, so the
`[PENDING]` label on the computed claim is superseded by this observation; the estimate itself was not
adjusted to match the measurement after the fact.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Opt into the Router v7 future flags instead of ignoring the warnings | SC-008 asks for a clean console; the flags are the documented migration path | `[AI-PROPOSED]` |
| Record keyboard and expired-token checks as `[PENDING]` | They were not observed, and an unobserved check must not be reported as a pass | `[AI-PROPOSED]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| `BrowserRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}` | Two console warnings during ordinary use | The warnings are the router's own deprecation notices; enabling the flags is the sanctioned opt-in |
| README rewritten from a placeholder to a complete deliverable | `delivery-and-ci.md` 2.5 requires setup, rationale, credentials, API surface, limitations | A placeholder README would misrepresent the project's state |

## Commands and observed results

### T105 re-analysis of the implemented artifacts

Executed after implementation, read-only:

```text
check-prerequisites.ps1 -Json -RequireSpec -RequireTasks -IncludeTasks
  -> FEATURE_DIR specs/001-task-manager; AVAILABLE_DOCS research.md, data-model.md, contracts/, quickstart.md, tasks.md

tasks.md: unchecked 0; task ids 107; unique 107
requirement ids in spec: 25; absent from tasks.md: 0
implementation tasks: 30; without an explicit RED reference: 0
tasks with [P]: 33; [P] on implementation tasks: 0
evidence records present for EV-0010..EV-0018 and EV-0021: 10
```

Report summary:

| ID | Severity | Finding | State |
| --- | --- | --- | --- |
| C1 | CRITICAL (III) | T060's "invalid date" frontend assertion covered unreachable code | Resolved during implementation; the test targets the reachable case (`EV-0014`) |
| C2 | HIGH | Keyboard, focus visibility, and responsive layout are only manually verifiable | Keyboard recorded `[PENDING]`; narrow width verified at 375 px |
| C3 | MEDIUM | SC-001/SC-002 timing criteria are unmeasured | Accepted as scope, consistent with `EV-0023` H3 |
| C4 | LOW | T007 points at `EV-0009` for setup output | Output recorded in `EV-0010` with the reason |
| C5 | LOW | `TaskStatus` collides with `System.Threading.Tasks.TaskStatus` | Explicit alias, documented in `EV-0013` |

No blocking finding remains open. Coverage is 100% of the 25 requirement identifiers; constitution
alignment is clean on Principles II, IV, and VI, and Principle III's ordering held for all 30
implementation tasks.

### T097 project references

```text
BallastLane.Application.csproj   -> ..\Domain\BallastLane.Domain.csproj
BallastLane.Domain.csproj        -> (none)
BallastLane.Infrastructure.csproj-> ..\Application\...; ..\Domain\...
BallastLane.Migrator.csproj      -> ..\Infrastructure\...
BallastLane.SharedWeb.csproj     -> ..\Application\...; ..\Domain\...
BallastLane.TaskApi.csproj       -> Application; Domain; Infrastructure; SharedWeb
BallastLane.UserApi.csproj       -> Application; Domain; Infrastructure; SharedWeb

who declares Infrastructure: Migrator, TaskApi, UserApi
```

### T098 prohibited library scan

```text
dotnet list BallastLane.sln package --include-transitive  (filtered)
(no rows)
```

### T099 security subsets

```text
Task API HTTP (cross-user, ownership, 401 filters) -> Passed: 14
User API HTTP (duplicate, credential, 401 filters) -> Passed:  9
Task API HTTP full suite                           -> Passed: 53
```

### T100 frontend

```text
npm run lint    -> no findings
npx tsc --noEmit-> exit 0
npm run build   -> built in 1.82s
```

### T101 full suites

```text
dotnet test BallastLane.sln -m:1        (two consecutive runs)
Domain 31 | Application 44 | Infrastructure 18 | Api 18 | Architecture 9
PostgreSql 7 | TaskApi 53 | Migrations 9 | UserApi 22   -> 211 passed, 0 failed, 0 skipped

cd frontend; npx vitest run -> 8 files, 27 tests passed
```

`[VERIFIED]` A parallel `dotnet test` (without `-m:1`) intermittently failed one test per assembly
with `NpgsqlException: Exception while reading from stream` and ~2-minute durations, while every suite
passed when run alone. Two causes were found and fixed:

1. The migration suite created and dropped a database per test, and `CREATE DATABASE` copies the
   template and contends on its lock. It now creates a schema per test (`search_path`), so the tests
   no longer touch DDL that serializes across processes.
2. Five test processes against one Docker Desktop container can exhaust connection forwarding on this
   host. Each suite already has its own database and now caps `Max Pool Size` at 10; the documented
   command uses `-m:1` so projects run sequentially, which passed twice in a row.

### T102 quickstart section 4 (registration and authentication)

```text
1  register new email                 201  {"id":...,"email":"qs.user...@example.test","createdAt":...}
2  register same email, different case 409  {"title":"Conflict","detail":"An account already exists for this email address."}
3  7-character password               400  errors.password: "The password must be at least 8 characters long."
4  login correct                      200  accessToken, tokenType, expiresAtUtc
5  login wrong password               401  {"title":"Authentication required","detail":"Invalid email or password."}
6  login unknown email                401  {"title":"Authentication required","detail":"Invalid email or password."}
7  GET /api/tasks with no header      401
8  malformed / tampered token         401
9  GET /api/users/me with token       200  caller's own identity
10 GET /api/public/info anonymous     200  {"service":"ballastlane-user-api","status":"ok"}
```

Steps 5 and 6 return the same title and detail, differing only by correlation id, as designed.

### T102 quickstart section 5 (task CRUD and ownership)

```text
1  empty list for a fresh account     200  []
2  create with a future due date      201  status "Pending"
3  create with status InProgress      201  status "InProgress"
4  create without a description       201  description null
5  create with a past due date        201  dueDate 2020-01-01
6a blank title                        400
6b whitespace-only title              400
7  dueDate with a time component      400
8  status "Done"                      400
9  (covered by the automated HTTP test) supplied ownerId ignored -> 201, owner is the caller
10a Pending -> Completed              200
10b Completed -> InProgress           200
11 blank title on PUT                 400  and the subsequent GET shows the previous values
12 GET unknown UUID                   404
13 DELETE own task                    204  then a GET returns 404
14 DELETE the same id again           404
15 account B lists tasks              200  [] (account A's tasks absent)
16 B gets A's task                    404
17 B updates A's task                 404  and A's task is unchanged
18 B deletes A's task                 404  and A's task still exists
```

### T102 quickstart section 6 (frontend)

| Check | Observed result |
| --- | --- |
| Anonymous visitor is redirected to login | Yes, `/` and `/login` render the login form |
| Login with the documented credentials | Succeeded; the seeded tasks rendered in due-date order |
| List shows only the signed-in account's tasks | `demo.user` saw 3, `reviewer` saw 2, a new account saw the empty state |
| Create through the UI | `201`; the row appeared in the list without a reload |
| View a task | Detail view showed title, status, due date, and description |
| Edit through the UI | Form pre-populated; save updated title and status; the detail view reflected both |
| Delete requires confirmation | Dialog shown with the task title; cancelling sent no request; confirming removed the row |
| Empty account | "No tasks yet." with a "Create your first task" action |
| Slow request | "Loading tasks…" status rendered while the list was fetched |
| Invalid form input | "Title is required." and "A due date is required." rendered as alerts associated with their fields |
| Narrow viewport (375 px) | `scrollWidth == clientWidth`, so no horizontal scrolling |
| Browser console | No warnings, no errors after the router future flags were enabled |
| Keyboard-only traversal | **Not verified** - the harness could not perform trusted Tab input |
| Expired token redirect | **Not verified manually** - covered by automated tests only |

### T106 hygiene

```text
git check-ignore -v .env frontend/node_modules frontend/dist src/Domain/bin src/Domain/obj
  .gitignore:97:.env              .env
  .gitignore:54:node_modules/     frontend/node_modules
  .gitignore:68:dist/             frontend/dist
  .gitignore:4:[Bb]in/            src/Domain/bin
  .gitignore:5:[Oo]bj/            src/Domain/obj

git ls-files -s | mode 120000 (symlinks)  -> 0
git grep "C:\\Users\\" -- "*.md"          -> no matches
working-tree scan for local demo credentials -> 0 hits
```

## Final references

- Implementation files: `frontend/src/App.tsx`, `README.md`
- Test files: `tests/http/BallastLane.TaskApi.HttpTests/TaskSecurityTests.cs` (added for T099)
- Task identifiers: `T097` to `T106`
- Commits: `PENDING`

## Evidence status rationale

Status is `Captured`. Two manual checks are explicitly `[PENDING]` rather than claimed, and developer
review plus commit references are outstanding.

