# Contract: Task API (`BallastLane.TaskApi`)

**Host**: `BallastLane.TaskApi` | **Base URL (local)**: `http://localhost:5082`
**Spec**: [../spec.md](../spec.md) | **Plan**: [../plan.md](../plan.md) | **Data model**: [../data-model.md](../data-model.md)

Satisfies the assessment's data CRUD API obligation with correct verbs, parameters, and return
values (FR-005, FR-008; US3, US4, US5, US6).

## Conventions

- **Every endpoint is protected.** There are no public endpoints on this host. A request without a
  valid bearer token produces `401` before any data is read.
- The acting user comes from the `sub` claim of the validated token. No request body or query
  parameter can select another owner; a supplied `ownerId` is ignored (FR-009).
- Cross-user access returns `404`, identical to "does not exist", so existence is never confirmed
  (FR-007, research R13).
- Errors are `application/problem+json`. Status convention: `400` shape validation, `401`
  authentication, `404` missing-or-not-owned, `409` conflict, `204` on a successful delete.
- A task representation never appears in an error body.

## Task representation

```json
{
  "id": "9b2c...",
  "title": "Write the specification",
  "description": "Cover traceability",
  "status": "InProgress",
  "dueDate": "2026-11-20",
  "createdAt": "2026-11-16T09:00:00+00:00",
  "updatedAt": "2026-11-16T09:30:00+00:00"
}
```

`description` is `null` when absent. `dueDate` is an ISO `YYYY-MM-DD` calendar date. `status` is one
of `Pending`, `InProgress`, `Completed`. The owner is not exposed: the collection is already
owner-scoped, and exposing an identifier the client cannot use adds only surface.

## GET /api/tasks

Lists the authenticated user's tasks.

| Status | Meaning | Body |
|---|---|---|
| `200 OK` | always, including when the user has no tasks | JSON array of task representations, ordered by `dueDate` then `createdAt` then `id` |
| `401 Unauthorized` | missing, malformed, bad-signature, or expired token | ProblemDetails |

Returns `200` with `[]` for an empty collection; an empty list is not an error (US3.3).

## GET /api/tasks/{id}

| Status | Meaning |
|---|---|
| `200 OK` | task owned by the caller |
| `401 Unauthorized` | invalid or missing token |
| `404 Not Found` | identifier unknown **or** owned by another user |

`id` must be a UUID. A non-UUID value is a `404` (it cannot identify a resource), not a `400`.

## POST /api/tasks

Creates a task for the authenticated user.

**Request**

```json
{
  "title": "Write the specification",
  "description": "Cover traceability",
  "status": "InProgress",
  "dueDate": "2026-11-20"
}
```

| Field | Type | Rules |
|---|---|---|
| `title` | string | required, not blank after trimming, at most 200 characters (FR-014, PDC-02) |
| `description` | string \| null | optional, at most 2,000 characters when present (PDC-03) |
| `status` | string \| null | optional on create; when omitted the server assigns `Pending`; when present it must be one of the three values (FR-013) |
| `dueDate` | string | required, `YYYY-MM-DD`, no time or timezone (FR-014) |
| `ownerId` | any | **not part of the contract**; if supplied it is ignored (FR-009, US4.5) |

**Responses**

| Status | Meaning | Body |
|---|---|---|
| `201 Created` | created | task representation; `Location: /api/tasks/{id}` |
| `400 Bad Request` | shape or rule validation failed | ProblemDetails with `errors` keyed by field |
| `401 Unauthorized` | invalid or missing token | ProblemDetails |

A past `dueDate` is accepted and produces `201` (US4.4).

## PUT /api/tasks/{id}

Full update of title, description, status, and due date in one operation (PDC-06).

**Request**: the same body as `POST`, except `status` is required, because the client now owns the
value. The owner is immutable and is not part of the body.

| Status | Meaning | Body |
|---|---|---|
| `200 OK` | updated | the updated task representation |
| `400 Bad Request` | shape or rule validation failed; the stored task is unchanged | ProblemDetails |
| `401 Unauthorized` | invalid or missing token | ProblemDetails |
| `404 Not Found` | identifier unknown or owned by another user; nothing changes | ProblemDetails |

`updatedAt` is set to the current time on success. `createdAt`, `id`, and the owner never change.

## DELETE /api/tasks/{id}

| Status | Meaning | Body |
|---|---|---|
| `204 No Content` | deleted | empty |
| `401 Unauthorized` | invalid or missing token | ProblemDetails |
| `404 Not Found` | identifier unknown or owned by another user; nothing changes | ProblemDetails |

## Error shape

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Validation failed",
  "status": 400,
  "detail": "Title is required.",
  "errors": { "title": ["Title is required."] },
  "traceId": "00-8f1c...-01"
}
```

Successful responses contain no stack traces, SQL, driver text, or internal identifiers. `traceId`
is the W3C activity id, present for correlation and carrying no business information.

## CORS

An allow-list of the frontend origin only (`http://localhost:5173` locally, from configuration). No
wildcard origin, and no credentials are used because the token travels in the `Authorization` header
(security standard 2.5).

## OpenAPI

Served at `/swagger/v1/swagger.json` and `/swagger` (UI) in Development, and in any other
environment only when that environment sets `Swagger__Enabled=true` explicitly, generated from the
implementation, with the bearer security scheme declared so protected endpoints are documented as
protected.
