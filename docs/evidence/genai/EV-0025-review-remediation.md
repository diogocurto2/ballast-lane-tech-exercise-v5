# EV-0025 - Remediation of the second code review findings

> **Scope note.** This record covers the correction of the review findings listed in the prompt below.
> Earlier records (`EV-0015`, `EV-0018`, `GENAI.md`) are corrected in place where the prompt required
> it; those corrections are summarized in "Corrections and improvements" and preserve the original
> observed output.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0025` |
| Date | `2026-11-16` |
| Development phase | 10 - Post-review remediation (session handling, login consistency, styling) |
| Activity type | `Review`, `Code generation`, and `Testing` |
| Related specification | `specs/001-task-manager/spec.md` (US1, FR-004, FR-010, FR-011, NFR-002, NFR-003, PDC-07) |
| Related task identifiers | `T028`, `T034` (US1 frontend authentication), `T048`, `T054` (US3 list and detail), `T058` to `T069` (US4 create), `T070` to `T080` (US5 update), `T081` to `T090` (US6 delete), `T097` to `T106` (review and convergence) |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `7e2f1da` - `fix(web): end the session centrally and restyle the interface` - the code, test, and stylesheet change for findings 1, 2, 3, and 5. Recorded after the commit was created. The documentation change for finding 4 belongs to this record's own commit and is not self-referenced. |

## Actual prompt

Verbatim developer prompt that opened the activity:

```text
Review and fix the findings from the latest code review of the Ballast Lane Task Manager, currently on origin/main at commit a42d631.

Follow the existing constitution, engineering standards, TDD rules, and genai-evidence skill.

## Required corrections

1. **JWT logout security (HIGH)**

- Ensure logout clears both the React authentication state and the in-memory access token.
- Ensure expired or rejected sessions cannot reuse a previous token.
- Add regression tests verifying that requests after logout do not carry the previous JWT.
2. **Login consistency (MEDIUM)**

- Only report successful login after the authenticated user is successfully resolved through `/api/users/me`.
- If this second request fails for any reason, clear the token and authentication state and return an appropriate error.
- Add tests for network errors, HTTP 401, and HTTP 500 during this step.
3. **Centralized HTTP 401 handling (MEDIUM)**

- Ensure task list, detail, create, edit, and delete flows handle session expiration consistently.
- Centralize session invalidation instead of duplicating inconsistent redirect behavior.
- Avoid circular dependencies between the API client and authentication provider.
- Add regression tests for the affected flows.
4. **TDD evidence accuracy (MEDIUM)**

- Review EV-0015 and GENAI.md.
- Clearly distinguish original test-first RED cycles from regression/mutation checks performed after behavior already existed.
- Preserve the actual execution chronology. Do not fabricate or rewrite historical evidence.
5. **Frontend styling and responsiveness (MEDIUM)**

- Improve the existing interface with clean, professional, responsive CSS.
- Style forms, buttons, task lists, task details, dialogs, validation messages, and loading/error states.
- Ensure usability on mobile and desktop.
- Preserve accessibility, semantic HTML, and keyboard support.
- Do not introduce a new UI framework or unnecessary dependencies.

## Validation

- Follow RED-GREEN-REFACTOR for every new behavioral correction.
- Run the relevant frontend regression tests.
- Run the complete frontend lint, TypeScript, build, and test checks.
- Run the backend test suite to check for regressions.
- Verify the complete login → CRUD → logout journey.
- Update GenAI evidence with actual results.
- Do not claim manual checks passed unless they were actually performed.
- Do not modify the constitution, product scope, or unrelated architecture.
```

## Relevant context

- Requirements: FR-004 (logout ends the session), FR-010 (complete registration, login, logout, and
  CRUD in the frontend), FR-011 (loading, empty, success, validation, and error states), NFR-002
  (keyboard operation, labels, accessible errors), NFR-003 (usable from small mobile to desktop),
  PDC-07 (logout ends the web application's authenticated session).
- Standards: `frontend-standards.md` 2.3, 2.4, 2.5, 2.7, 2.8, 2.10; `testing-and-tdd.md` 2.1, 2.2,
  2.5, 2.6; `security.md` (token handling); `genai.md`.
- Constitution: Principle III (RED, GREEN, REFACTOR), Principle V (responsive, accessible frontend),
  Principle VI (evidence).
- Files supplied as context: the existing frontend under `frontend/src/`, the frontend tests under
  `frontend/tests/`, `docs/evidence/genai/EV-0015-us5-task-update.md`, `GENAI.md`, and the
  constitution at `.specify/memory/constitution.md`.
- Repository state: `origin/main` at `a42d631`, clean working tree.

## Original AI-generated output

The defects under review are the *existing* code, not code generated in this activity. The
representative pre-correction excerpts are therefore the files that were changed:

```tsx
// frontend/src/auth/AuthProvider.tsx - before (logout never touched the credential)
const logout = useCallback(() => {
  setUser(null);
  setError(null);
}, []);

// login reported success even when the identity lookup failed for a reason other than 401
const me = await api.currentUser();
if (me.ok) {
  setUser(me.data);
} else if (me.failure.status === 401) { /* ... */ }
return result;
```

```tsx
// frontend/src/pages/TasksPage.tsx - before (one of five inconsistent copies)
if (result.failure.status === 401) {
  auth.logout();
  navigate("/login");
  return;
}
```

```tsx
// frontend/src/api/client.ts - before (unused class, no invalidation)
export class SessionExpiredError extends Error { /* never referenced */ }
if (!response.ok) {
  return { ok: false, failure: await toFailure(response) };
}
```

The first AI-generated test file of this activity was `frontend/tests/flows/session.test.tsx`. It was
corrected twice after observed failures; both corrections are recorded below rather than rewritten
away.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | The session signal lives in `frontend/src/api/authToken.ts`, which imports nothing; the client publishes to it and the provider subscribes, so no client → provider or provider → client import was introduced | `[VERIFIED]` |
| SOLID | Token storage, invalidation publication, HTTP transport, and identity state keep one responsibility each; the provider no longer duplicates redirects owned by the route guard | `[VERIFIED]` |
| TDD | Six session tests plus two dialog tests observed failing for behavior reasons before the implementation; GREEN observed afterwards; REFACTOR (helper extraction, timeout configuration) reran clean | `[VERIFIED]` |
| Security | Logout clears the in-memory token; a `401` on an authenticated request clears it before notifying; no request after logout carried `Authorization` | `[VERIFIED]` |
| Functional requirements | Login with the documented demonstration account, list, create, view, edit, delete with confirmation, and logout executed against the running containers | `[VERIFIED]` |
| Styling and responsiveness | Measured in the Chromium-based VS Code integrated browser at 1024 px and 288 px: `scrollWidth == clientWidth` on the list, the detail view, and the dialog; keyboard traversal with visible focus on both pages | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` **Finding 1, logout left the credential live.** `AuthProvider.logout` cleared the
  identity and the notice but never the token, so `getAccessToken()` still returned the previous JWT
  and any later request through `tasksApi` would have sent it. Observed: `expected 'token-value' to be null`.
- `[VERIFIED]` **Finding 2, login reported success without an identity.** When `/api/users/me` failed
  with a network error or `500`, `login` fell through and returned the successful `/login` result, so
  the caller navigated to `/tasks` and the route guard bounced it back. Observed: the probe recorded
  `success` where `failure:0`, `failure:401`, and `failure:500` were expected.
- `[VERIFIED]` **Finding 3, five inconsistent expiration paths.** `TaskDetailPage` redirected without
  clearing state, `TaskEditPage`, `TaskForm`, and `TaskDelete` showed ordinary error text on a `401`,
  and `TasksPage` performed its own logout-then-redirect. Observed: no login heading for the edit,
  create, and delete flows, and no session notice for list and detail. Task `T034` had explicitly
  required "`frontend/src/api/client.ts` with centralized 401 handling", so the per-page copies were
  also a departure from the approved task list, not only an inconsistency.
- `[VERIFIED]` **Finding 4, EV-0015 and GENAI.md mislabelled a mutation check as RED.** The US5
  frontend tests passed on their first run because the edit-mode behavior already existed from US4;
  the failure observed afterwards by disabling edit mode is a mutation check, not a test-first RED
  cycle. The record's TDD row and `GENAI.md` stated "every story recorded an observed RED before
  implementation", which is not accurate for that story.
- `[VERIFIED]` **Finding 5, the interface was effectively unstyled.** `styles.css` contained 22 lines
  with a page shell class that no component used; forms, buttons, lists, detail views, dialogs, and
  state panels had no visual treatment beyond browser defaults.
- `[AI-PROPOSED]` **Observation, deferred.** `aria-invalid` and the field error message persist while
  the user corrects the field, because errors are only recomputed on submit. It predates this activity
  and no review finding covers it, so it was not changed; recorded here so it is not mistaken for
  behavior introduced by the styling work.
- `[VERIFIED]` **First test draft defect.** `tests/flows/session.test.tsx` used `Array.prototype.at`,
  which the project's `ES2020` library target does not provide:
  `error TS2550: Property 'at' does not exist on type 'RecordedCall[]'`. Replaced with an index
  expression.
- `[VERIFIED]` **Test budget defect.** Full-app flows in jsdom exceeded the default 5 s per-test
  timeout under parallel load (`Test timed out in 5000ms` in `LoginForm.test.tsx` and in the edit
  flow). A per-test override was added first, then replaced by a single `testTimeout: 15000` in
  `vite.config.ts`, which covers the whole suite without weakening any assertion.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Invalidation is published by a dependency-free module (`api/authToken.ts`) that the client calls and the provider subscribes to | Satisfies centralization while making a client ↔ provider cycle structurally impossible | `[AI-PROPOSED]` |
| Only a `401` on a request that carried a credential invalidates the session | A `401` from `POST /api/users/login` means bad credentials, not an expired session | `[AI-PROPOSED]` |
| Pages stop handling `401` themselves and rely on the guard | One redirect path; the guard already redirects when `isAuthenticated` becomes false | `[AI-PROPOSED]` |
| The provider still clears the session when `refreshCurrentUser` reports `401` | The provider owns identity state and must not depend on the transport to notice a rejected credential; the two paths are idempotent | `[AI-PROPOSED]` |
| Keep `api/authToken.ts` as the module name instead of renaming it | The module owns the credential and its invalidation; renaming would have churned test imports for no behavioral gain | `[AI-PROPOSED]` |
| Add initial focus on the safe action and Escape-to-cancel to `ConfirmDialog` | The dialog is now visually modal; a modal that ignores Escape and starts focus outside itself is a keyboard trap in practice (NFR-002) | `[AI-PROPOSED]` |
| No UI framework, CSS library, or new dependency | Explicit instruction, and `frontend-standards.md` 2.9 forbids unjustified additions | `[DEVELOPER DECISION]` |
| Correct `EV-0015`, `EV-0018`, and `GENAI.md` wording instead of deleting the original claims | The chronology must survive; only the labels were wrong | `[DEVELOPER DECISION]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| `logout` calls `setAccessToken(null)` before clearing state | The credential outlived the session | FR-004, PDC-07, `frontend-standards.md` 2.3 |
| `invalidateSession()` clears the token, then notifies subscribers | A listener that issues another request cannot reuse a rejected credential | `security.md`, Principle IV |
| `request()` invalidates when a `401` arrives on an authenticated request | One place decides that a credential is dead | `frontend-standards.md` 2.3 ("Protected API calls MUST be centralized") |
| `AuthProvider` subscribes to the invalidation signal | The React state follows the transport decision without importing it | Avoids the cycle the review named |
| `TasksPage`, `TaskDetailPage`, `TaskEditPage`, `TaskForm`, and `TaskDelete` ignore `401` failures | Reporting "not found", "could not be deleted", or a load error on an expired session misdescribes the failure | Consistency across all five flows |
| `login` returns a failure whenever `/api/users/me` fails, after `endSession(null)` | A token without a resolvable identity is not a session | Review finding 2 |
| `ConfirmDialog` focuses the cancel button and cancels on Escape | Keyboard operability of a modal | NFR-002, `frontend-standards.md` 2.5 |
| `styles.css` rewritten with tokens, light/dark palettes, and responsive rules | The interface was unstyled | NFR-003, `frontend-standards.md` 2.4 |
| `data-status`, `button-primary`, `notice`, and `page-note` classes added to existing markup | Semantic hooks for the new stylesheet; roles, labels, and accessible names unchanged | `frontend-standards.md` 2.5 |
| `testTimeout: 15000` in `vite.config.ts` | jsdom flow tests cannot reliably finish inside 5 s under parallel load | `testing-and-tdd.md` 2.7 |
| `EV-0015` and `GENAI.md` relabelled | Distinguish original test-first RED from post-implementation mutation checks | Review finding 4, `testing-and-tdd.md` 3.6 |

## Commands and observed results

### Baseline, before any change

```text
cd frontend
npm run lint        -> no findings (exit 0)
npx tsc --noEmit    -> exit 0
npx vitest run      -> Test Files 8 passed (8), Tests 27 passed (27), duration 47.98s

dotnet test BallastLane.sln -m:1
  -> 91 failures, every one: 28P01: password authentication failed for user "postgres"
```

The backend failures were environmental: the suites read `ConnectionStrings__TestDatabase` and fell
back to a default credential when it was unset. Re-running with the value exported from the local
environment file produced the real baseline. The value is not reproduced here.

```text
dotnet test BallastLane.sln -m:1
TaskApi 53 | UserApi 22 | Migrations 9 | PostgreSql 7 | Api 18
Application 44 | Architecture 9 | Domain 31 | Infrastructure 18
  -> 211 passed, 0 failed, 0 skipped
```

### RED - session handling, before the implementation

```text
cd frontend; npx vitest run tests/components/auth-context.test.tsx tests/flows/session.test.tsx
  × clears the token on logout and sends no credential afterwards
      -> expected 'issued-token' to be null
  × ends the session when the task list reports 401
      -> Unable to find an element with the text: /session has expired/i
  × ends the session when a task detail request reports 401
  × ends the session when the task edit form cannot load its task
  × ends the session when creating a task reports 401
  × ends the session when deleting a task reports 401
  × clears the in-memory access token on logout
      -> expected 'token-value' to be null
  × does not report a successful login when the identity lookup fails with a network error
      Expected element to have text content: failure:0
      Received: success
  × does not report a successful login when the identity lookup fails with HTTP 401
  × does not report a successful login when the identity lookup fails with HTTP 500

Test Files  2 failed (2)
     Tests  10 failed | 2 passed (12)
```

The two passes are the two pre-existing `auth-context` tests; every failure is an assertion about
behavior, not a compilation or fixture problem.

### RED - dialog keyboard operation

```text
cd frontend; npx vitest run tests/components/ConfirmDialog.test.tsx
     -> expect(element).toHaveFocus()
     -> expected "spy" to be called 1 times, but got 0 times
      Tests  2 failed | 1 passed (3)
```

### GREEN - after the implementation

```text
cd frontend; npx vitest run tests/components/auth-context.test.tsx tests/flows/session.test.tsx
      Tests  12 passed (12)

cd frontend; npx vitest run tests/components/ConfirmDialog.test.tsx tests/components/TaskDelete.test.tsx
      Tests  7 passed (7)
```

### Final checks

```text
cd frontend
npm run lint      -> no findings (exit 0)
npx tsc --noEmit  -> exit 0
npm run build     -> dist/assets/index-XbiwL4Pj.css 9.08 kB | dist/assets/index-Ba--26DX.js 182.29 kB
                     built in 1.81s
npx vitest run    -> Test Files 10 passed (10), Tests 40 passed (40), duration 30.30s

dotnet test BallastLane.sln -m:1
TaskApi 53 | UserApi 22 | Migrations 9 | PostgreSql 7 | Api 18
Application 44 | Architecture 9 | Domain 31 | Infrastructure 18
  -> 211 passed, 0 failed, 0 skipped
```

### Browser journey against the running containers

`docker compose up -d --build` started PostgreSQL, the migrator, and both API hosts;
`docker compose ps` reported the migrator `Exited` and both hosts `Started`;
`GET /api/public/info` returned `{"service":"ballastlane-user-api","status":"ok"}` and
`GET /api/tasks` without a token returned `401`. The client ran from `npm run dev` at
`http://localhost:5173` and was driven in the Chromium-based VS Code integrated browser.

| Step | Observed result |
| --- | --- |
| Log in with the documented demonstration account | `/tasks` rendered with the three seeded tasks |
| Create | Validation empty-state rendered; after filling, `201` and the row appeared without a reload |
| Read | Detail view showed title, `Completed`/`Pending` status, due date, and description |
| Edit | Form pre-populated; changing the title and status to `Completed` updated the detail view |
| Delete, first attempt | Dialog opened with focus on `Cancel`; `Escape` closed it, no request was made, the task remained |
| Delete, confirmed | Returned to `/tasks` with the created task gone; a token-authenticated `GET /api/tasks` then returned exactly the three seeded tasks |
| Log out | Redirected to `/login`; `localStorage` and `sessionStorage` empty; navigating to `/tasks` redirected to `/login` and issued **zero** task API requests |
| Session expiry | With the task list intercepted to answer `401` after a real login, the app cleared the session, redirected to `/login`, and displayed `Your session has expired. Log in again.` |
| Layout, 1024 px | `scrollWidth 1024 == clientWidth 1024`, row height 54 px |
| Layout, 288 px | `scrollWidth 288 == clientWidth 288`, rows stacked (`"title title" "status due"`), row height 112 px |
| Dialog, 288 px | 244 px wide, fully inside the 512 px viewport, no horizontal overflow |
| Keyboard, login page | Tab order Email → Password → Log in → Register → Email; every stop matched `:focus-visible` with a solid accent outline |
| Keyboard, tasks page | Tab order New task → Log out → three task rows → New task; every stop matched `:focus-visible` with a solid accent outline |

The keyboard checks above were performed with real key events in the Chromium-based VS Code integrated browser. They are the two items
`EV-0018` had recorded as manually unverified for keyboard and focus visibility; the expired-token
redirect has now also been exercised against a real `401` response rather than by waiting out the
60-minute token lifetime.

The containers used for the journey were stopped afterwards; PostgreSQL was left running as it was
before the activity.

## Final references

- Implementation files: `frontend/src/api/authToken.ts`, `frontend/src/api/client.ts`,
  `frontend/src/auth/AuthProvider.tsx`, `frontend/src/components/{TaskForm,TaskDelete,TaskList,ConfirmDialog}.tsx`,
  `frontend/src/pages/{TasksPage,TaskDetailPage,TaskEditPage,LoginPage,RegisterPage}.tsx`,
  `frontend/src/styles.css`, `frontend/vite.config.ts`
- Test files: `frontend/tests/flows/session.test.tsx` (new),
  `frontend/tests/components/auth-context.test.tsx`, `frontend/tests/components/ConfirmDialog.test.tsx` (new)
- Corrected evidence: `docs/evidence/genai/EV-0015-us5-task-update.md`,
  `docs/evidence/genai/EV-0018-review-and-convergence.md`, `GENAI.md`
- Task identifiers: `T028`, `T034`, `T048`, `T054`, `T058` to `T069`, `T070` to `T080`,
  `T081` to `T090`, `T097` to `T106`
- Commits: `7e2f1da` - `fix(web): end the session centrally and restyle the interface`, recorded after
  it was created. The record, index, and `GENAI.md` corrections are delivered by the documentation
  commit that follows and are not self-referenced.

## Evidence status rationale

Status is `Captured`. Every correction has an observed RED failure and an observed GREEN result, the
full frontend and backend suites pass, and the journey was executed in a real browser. The code commit
is recorded. Developer review and approval of the AI-proposed decisions remain outstanding, which is
why the record has not advanced to `Reviewed` or `Verified`.
