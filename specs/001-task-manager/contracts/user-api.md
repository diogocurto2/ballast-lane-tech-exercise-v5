# Contract: User API (`BallastLane.UserApi`)

**Host**: `BallastLane.UserApi` | **Base URL (local)**: `http://localhost:5081`
**Spec**: [../spec.md](../spec.md) | **Plan**: [../plan.md](../plan.md) | **Data model**: [../data-model.md](../data-model.md)

Satisfies the assessment's "second API" obligation: user creation, login, a public endpoint, and an
authorized endpoint (FR-008, US1, US2).

## Conventions

- All request and response bodies are `application/json`.
- All errors are `application/problem+json` (ProblemDetails). Shape validation errors add an
  `errors` extension object keyed by field name.
- Status convention: `400` shape validation, `401` authentication, `404` missing-or-not-owned,
  `409` conflict. `422` is deliberately unused (research R14).
- Every endpoint is explicitly public (`[AllowAnonymous]`) or protected by the default policy.
  None is left unclassified.
- No response ever contains a password or password hash (NFR-001).

## POST /api/users/register

Public. Creates an account.

**Request**

```json
{ "email": "demo.user@example.test", "password": "DemoPassw0rd!" }
```

| Field | Type | Rules |
|---|---|---|
| `email` | string | required, single-`@` address shape, at most 320 characters, compared case-insensitively |
| `password` | string | required, at least 8 characters (PDC-01) |

**Responses**

| Status | Meaning | Body |
|---|---|---|
| `201 Created` | account created | `UserResponse` |
| `400 Bad Request` | shape validation failed | ProblemDetails with `errors` keyed by field |
| `409 Conflict` | an account already exists for that email (case-insensitive) | ProblemDetails, with no detail that reveals the stored credential |

`Location` is omitted deliberately: no read endpoint exists for an arbitrary user id, and adding
`/api/users/{id}` would create a resource the specification does not require. This deviation from the
`Location` convention is documented here rather than left implicit.

**`UserResponse`**

```json
{ "id": "3f1e...", "email": "demo.user@example.test", "createdAt": "2026-11-16T09:00:00+00:00" }
```

## POST /api/users/login

Public. Exchanges credentials for a bearer token.

**Request**

```json
{ "email": "demo.user@example.test", "password": "DemoPassw0rd!" }
```

**Responses**

| Status | Meaning | Body |
|---|---|---|
| `200 OK` | credentials valid | `LoginResponse` |
| `400 Bad Request` | missing email or password | ProblemDetails with `errors` |
| `401 Unauthorized` | unknown email **or** wrong password, indistinguishably | ProblemDetails with one generic title and no field detail |

**`LoginResponse`**

```json
{
  "accessToken": "<jwt>",
  "tokenType": "Bearer",
  "expiresAtUtc": "2026-11-16T10:00:00+00:00"
}
```

Token claims: `sub` = user id, `email`, `iss` = `ballastlane-user-api`, `aud` =
`ballastlane-clients`, `exp` = issue time + 60 minutes, `jti` = a new UUID.

## GET /api/users/me

Protected (`401` without a valid token). Returns the calling user.

| Status | Meaning | Body |
|---|---|---|
| `200 OK` | authenticated | `UserResponse` for the identity in the token |
| `401 Unauthorized` | missing, malformed, bad-signature, or expired token | ProblemDetails |

The user is resolved from the `sub` claim only. No query parameter selects a user.

## GET /api/public/info

Public. Demonstrates a non-authorized endpoint as the assessment requires.

| Status | Meaning | Body |
|---|---|---|
| `200 OK` | always | `{ "service": "ballastlane-user-api", "status": "ok" }` |

## Authentication contract shared with the Task API

| Property | Value |
|---|---|
| Algorithm | HS256 |
| Issuer | `ballastlane-user-api` |
| Audience | `ballastlane-clients` |
| Lifetime | 60 minutes |
| Signing key source | `Jwt__SigningKey` environment variable; placeholder in `.env.example` |
| Validation | `ValidateIssuer`, `ValidateAudience`, `ValidateIssuerSigningKey`, `ValidateLifetime` all `true`; `ClockSkew = TimeSpan.Zero` |
| Failure | every invalid or missing credential produces `401`, never `403` |

## OpenAPI

Served at `/swagger/v1/swagger.json` and `/swagger` (UI) in Development, and in any other
environment only when that environment sets `Swagger__Enabled=true` explicitly (the local
container stack does; a deployed environment does not). Generated from the implementation; no
hand-maintained document.
