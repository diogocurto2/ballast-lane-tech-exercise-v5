# API Standards

REST conventions, contracts, and error handling for the two required APIs.

## 1. Purpose and scope

This standard defines how the HTTP APIs are designed and implemented. The assessment requires a data CRUD API and a second API with user creation, login, and public and protected endpoints.

It does not fix endpoint paths or contracts. Those MUST be established in the Spec Kit implementation plan; this document constrains how they are shaped.

## 2. Mandatory rules

- Resources MUST be modeled as nouns with plural collection paths, and MUST use HTTP verbs for operations. Verbs MUST NOT appear in paths.
- Endpoints MUST use the appropriate HTTP verb and status code: `GET` for reads, `POST` for creation, `PUT` or `PATCH` for full or partial update, `DELETE` for removal.
- Collections MUST return `200 OK` with a collection, not a bare array wrapper invented per endpoint without reason.
- Creation MUST return `201 Created` with a `Location` header referencing the new resource where a read endpoint exists.
- Updates MUST return `200 OK` with the updated representation, or `204 No Content` where no representation is returned, and MUST be consistent across endpoints.
- Deletion MUST return `204 No Content` on success.
- Missing resources MUST return `404 Not Found`. Conflicting state MUST return `409 Conflict`. Invalid input MUST return `400 Bad Request`. Unauthenticated requests MUST return `401 Unauthorized`. Authenticated but unauthorized requests MUST return `403 Forbidden`.
- Request and response DTOs MUST be explicit types. Domain entities MUST NOT be serialized directly; see [clean-architecture.md](./clean-architecture.md).
- Input validation MUST occur server-side. Client-side validation MAY improve the experience and MUST NOT be relied on for correctness.
- Business-rule validation MUST be distinguished from input-shape validation. Shape errors return `400`; violated business rules return `409` or `422` consistently, per a documented convention.
- Error responses SHOULD use ProblemDetails (`application/problem+json`) so clients receive a consistent, machine-readable shape.
- A successful response MUST NOT include stack traces, driver messages, SQL, or internal identifiers that leak implementation detail.
- Every endpoint MUST be documented in OpenAPI, generated from the implementation where practical so documentation cannot drift.
- Endpoints MUST declare their authentication and authorization requirement explicitly. Public endpoints MUST be deliberately marked public rather than left unprotected by omission.
- Controllers MUST be thin: bind and validate the request, call one application use case, map the outcome to a status code. No business rules in controllers.
- Behavior MUST be testable without a browser: each endpoint MUST have HTTP-level tests asserting status codes and response bodies.

## 3. Required API surface

The assessment requires two API concerns. Whether they are separate hosts or separate route groups is a planning decision.

| API | Responsibility | Minimum endpoints |
| --- | --- | --- |
| Data CRUD API | Manage application records for the authenticated user | List, read by id, create, update, delete |
| User API | Registration, login, and access control demonstration | Register user, login, one public endpoint, at least one authenticated endpoint, at least one protected resource operation |

- The data API MUST scope access to the authenticated user. Reading or modifying another user's record MUST return `403` or `404`, per a documented, consistent choice.
- The user API MUST NOT return password hashes or other secrets in any response.
- Authentication responses MUST define the credential mechanism used and its expiry behavior.

## 4. Practical examples

**Resource-oriented routes**

```text
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}

POST   /api/users/register
POST   /api/users/login
GET    /api/users/me
GET    /api/public/info
```

**Status code shape**

```text
POST /api/tasks           201 Created + Location: /api/tasks/{id}
GET  /api/tasks/{unknown}  404 Not Found
POST /api/users/register   409 Conflict  (email already exists)
POST /api/tasks (invalid)  400 Bad Request (validation)
GET  /api/users/me (none)  401 Unauthorized
DELETE /api/tasks/{other}  403 Forbidden  (not the owner)
```

**ProblemDetails response**

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Validation failed",
  "status": 400,
  "detail": "Title is required.",
  "traceId": "00-8f1c...-01"
}
```

**Thin controller**

```csharp
[HttpPost]
public async Task<ActionResult<TaskResponse>> Create(
    [FromBody] CreateTaskRequest request,
    CancellationToken cancellationToken)
{
    var identity = User.ToAuthenticatedUser();
    var result = await _createTask.ExecuteAsync(identity, request.ToInput(), cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToResponse());
}
```

## 5. Prohibited practices and anti-patterns

- Verbs in paths such as `/api/getTasks` or `/api/tasks/create`.
- `POST` used for reads or `GET` used for mutations.
- Returning `200 OK` for creation or `200 OK` with a null body for deletion.
- Returning `500` for validation failures or `400` for unauthenticated requests.
- Returning domain entities or database rows directly.
- Exposing stack traces, SQL, or driver error text.
- Business rules in controllers or endpoint filters.
- Ownership taken from a client-supplied `userId` field in the request body; see [security.md](./security.md).
- Endpoints left unauthenticated by omission.
- Hand-maintained OpenAPI that diverges from the implementation.

## 6. Verification and review criteria

| Check | Method |
| --- | --- |
| Verb and status correctness | HTTP integration tests asserting status codes per case. |
| Resource modeling | Review routes for noun/verb conventions. |
| Validation separation | Tests distinguishing `400` from `409`/`422`. |
| Auth boundaries | Tests for `401`/`403` on each protected endpoint; explicit public markers on public ones. |
| Ownership scoping | Tests proving cross-user access is denied. |
| Error shape | Tests asserting ProblemDetails structure and absence of internal detail. |
| OpenAPI | Generated document covers every route and schema. |
| Thin controllers | Review for business logic outside application use cases. |
| Independence | Endpoints tested without a browser, in-process via the test host. |

## 7. Deferred decisions

Endpoint paths, request and response schemas, the authentication credential type and lifetime, the choice between `409` and `422` for business-rule violations, and whether the two APIs are separate hosts are planning decisions. They MUST be recorded in the plan before implementation.

## 8. References

- Layer and controller responsibilities: [clean-architecture.md](./clean-architecture.md)
- Authentication, authorization, and ownership: [security.md](./security.md)
- DTO mapping: [clean-architecture.md](./clean-architecture.md)
- HTTP test requirements: [testing-and-tdd.md](./testing-and-tdd.md)
- C# conventions for controllers: [coding-standards.md](./coding-standards.md)
