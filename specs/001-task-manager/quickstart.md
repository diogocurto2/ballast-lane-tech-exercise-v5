# Quickstart: Personal Task Manager (001-task-manager)

**Phase**: 1 - Design | **Date**: 2026-11-16 | **Plan**: [plan.md](./plan.md) | **Spec**: [spec.md](./spec.md)

Runnable validation scenarios proving the feature works end to end. This is a validation and run
guide, not an implementation guide: it intentionally contains no model, service, or controller
bodies, no migration SQL, and no test-suite source. Those belong to `tasks.md` and the
implementation phase. Behavior detail lives in [data-model.md](./data-model.md) and
[contracts/](./contracts/).

## Prerequisites

| Requirement | Version | Verified locally |
|---|---|---|
| .NET SDK | 8.0.x | `8.0.428` |
| Node.js | 20 or newer | `v25.9.0` |
| Docker + Compose | any current | `28.3.3` / `v2.39.2` |

## 1. Configure and start the backend

```bash
cp .env.example .env          # then fill in the placeholders
docker compose up --build     # postgres, migrator, user-api, task-api
```

Expected outcome:

- `postgres` becomes healthy; the migrator exits `0` after applying migrations and the seed.
- `user-api` responds on `http://localhost:5081`, `task-api` on `http://localhost:5082`.
- OpenAPI is reachable at `http://localhost:5081/swagger` and `http://localhost:5082/swagger`.

## 2. Start the frontend

```bash
cd frontend
npm ci
npm run dev
```

Expected outcome: the app serves on `http://localhost:5173` and redirects an anonymous visitor to the
login page.

## 3. Verify the seeded demonstration (US7, FR-012, NFR-004, SC-006)

1. Log in with the demonstration credentials documented in `README.md`.
2. The task list shows the seeded tasks for that account.

Then verify idempotency:

```bash
docker compose run --rm migrator --seed
```

Expected outcome: the command succeeds, and the task list is unchanged — no duplicates, no errors.

## 4. Verify registration and authentication (US1, US2)

| Step | Action | Expected |
|---|---|---|
| 1 | Register a new email with a valid password | `201`, and the response contains no password or hash |
| 2 | Register the same email again, with different letter case | `409 Conflict`; only one account exists |
| 3 | Register with a 7-character password | `400 Bad Request` with a password field error |
| 4 | Log in with the correct credentials | `200` with `accessToken`, `tokenType`, `expiresAtUtc` |
| 5 | Log in with a wrong password | `401` with one generic title |
| 6 | Log in with an unregistered email | `401` with the **same** body as step 5 |
| 7 | Call `GET /api/tasks` with no `Authorization` header | `401` |
| 8 | Call it with a malformed token, and with an expired token | `401` in both cases |
| 9 | Call `GET /api/users/me` with the valid token | `200` and the caller's own identity |
| 10 | Call `GET /api/public/info` with no credentials | `200` |

## 5. Verify task CRUD and ownership (US3, US4, US5, US6)

With a valid token for account A:

| Step | Action | Expected |
|---|---|---|
| 1 | `GET /api/tasks` on a fresh account | `200` and `[]` — an empty state, not an error |
| 2 | `POST /api/tasks` with title, description, `dueDate` in the future | `201` with a `Location` header and `status: "Pending"` |
| 3 | `POST /api/tasks` with `status: "InProgress"` | `201` with `status: "InProgress"` |
| 4 | `POST /api/tasks` with no description | `201`, and `description` is `null` |
| 5 | `POST /api/tasks` with a past `dueDate` | `201` — past dates are accepted |
| 6 | `POST /api/tasks` with a blank and with a whitespace-only title | `400` in both cases; nothing persisted |
| 7 | `POST /api/tasks` with `dueDate: "2026-11-20T10:00:00Z"` | `400` — a calendar date is required |
| 8 | `POST /api/tasks` with `status: "Done"` | `400` — only three statuses exist |
| 9 | `POST /api/tasks` including `ownerId` of another user | `201`, and the task is owned by the caller |
| 10 | `PUT /api/tasks/{id}` moving Pending to Completed, then Completed to InProgress | `200` in both directions |
| 11 | `PUT /api/tasks/{id}` with a blank title | `400`, and a subsequent `GET` shows the old title |
| 12 | `GET /api/tasks/{unknown-uuid}` | `404` |
| 13 | `DELETE /api/tasks/{id}` | `204`; a later `GET` of that id returns `404` |
| 14 | `DELETE` the same id again | `404` |

Then, with a valid token for account B:

| Step | Action | Expected |
|---|---|---|
| 15 | `GET /api/tasks` | `200`, and account A's tasks are absent |
| 16 | `GET /api/tasks/{a-task-id}` | `404` |
| 17 | `PUT /api/tasks/{a-task-id}` | `404`, and account A's task is unchanged |
| 18 | `DELETE /api/tasks/{a-task-id}` | `404`, and account A's task still exists |

## 6. Verify the frontend flows (FR-010, FR-011, NFR-002, NFR-003, SC-005)

| Check | Expected |
|---|---|
| Register, log in, list, create, edit, delete through the UI | Each flow completes without a page reload |
| Delete a task | A confirmation is required before the request is sent |
| Empty account | An empty state with a next action, not a blank screen |
| Slow request | A visible loading state, not a frozen screen |
| Invalid form input | Field-level messages associated with the field |
| Server rejection | A human-readable error, no raw response body or stack trace |
| Resize to a narrow mobile width | No horizontal scrolling; controls remain operable |
| Keyboard only | Every control is reachable and operable; focus is visible |
| Expired token | The session clears and the user is redirected to login with a message |
| Browser console during the full flow | No unexpected warnings or errors (`SC-008`) |

## 7. Run the automated tests

```bash
dotnet test                                   # unit, integration, and HTTP tests
cd frontend && npm run lint && npm run build && npm test
```

Expected outcome:

- Every test passes. Integration and HTTP tests run against the real PostgreSQL container; none is
  skipped for a missing database.
- The backend build reports no warnings, and the frontend lint and strict type check pass.

If PostgreSQL is not running, start it first:

```bash
docker compose up -d postgres
```

## 8. Verify a clean-checkout run (NFR-004, SC-006, SC-007)

1. Clone the repository to a fresh directory with no prior containers or volumes.
2. Follow sections 1 and 2.
3. Log in with the demonstration credentials and confirm seeded tasks are present.
4. Stop the stack with `docker compose down` (keeping the volume), start it again, and confirm the
   same users and tasks are still present.

## Traceability of this guide

| Scenario group | Requirements and criteria |
|---|---|
| Section 3 | US7, FR-012, NFR-004, SC-006 |
| Section 4 | US1, US2, FR-001 to FR-004, FR-015, FR-017, NFR-001, SC-001, SC-004 |
| Section 5 | US3 to US6, FR-005 to FR-009, FR-013, FR-014, FR-016, SC-002, SC-003 |
| Section 6 | FR-010, FR-011, NFR-002, NFR-003, SC-005, SC-008 |
| Sections 7 and 8 | NFR-006, NFR-005, SC-007 |
