# Feature Specification: Personal Task Manager (001-task-manager)

**Feature Branch**: `001-task-manager`

**Created**: 2026-11-16

**Status**: Draft

**Input**: User description: "Build a simple personal Task Manager web application where users can register, authenticate, and securely manage their own tasks. The application should be small enough to demonstrate the complete user journey while showcasing senior full-stack engineering practices. Primary informal user story: As a registered user, I want to create, view, update, and delete my personal tasks, including their status and due date, so that I can organize my work and track my progress securely." The full developer instruction for this activity, including the governance corrections and reporting requirements, is preserved verbatim in `docs/evidence/genai/EV-0007-specification-001-task-manager.md`.

**Project override** of the installed specification template. Governed by
`.specify/memory/constitution.md` Principles I and III.

**Traceability is mandatory.** Every functional requirement MUST cite the assessment obligation it
satisfies, and MUST carry testable acceptance criteria including failure scenarios and edge cases.

## Traceability to Assessment

Source documents: [technical-exercise-v5.md](../../docs/assessment/technical-exercise-v5.md)
and [submission-requirements.md](../../docs/assessment/submission-requirements.md).
An assessment obligation with no requirement is a blocking gap.

Requirement ID prefixes: `FR-` functional requirement, `NFR-` non-functional quality requirement,
`EC-` engineering constraint, `DO-` submission deliverable, `PDC-` proposed product decision
(awaiting developer review, not an original assessment requirement).

### Original assessment obligations

| Assessment obligation | Type | Requirement IDs | Covered |
|---|---|---|---|
| Web application with an API and a data layer, using ASP.NET with a database or data store | Functional + constraint | FR-001..FR-017, EC-02, EC-05 | yes |
| CRUD operations on application data via API endpoints, with appropriate verbs, parameters, and return values | Functional | FR-005, FR-008, FR-017 | yes |
| Create a user, log in as the user, and store user information in the data | Functional | FR-001, FR-002, FR-003, FR-008 | yes |
| A second API with user creation, login, and authorized and non-authorized endpoints | Functional | FR-008, FR-017 | yes |
| At least one table/object/container for application data plus an additional one for users; unique identifier (primary key) and at least two other fields | Functional | FR-003, FR-006, Key Entities | yes (concrete schema is a planning deliverable) |
| A data access layer providing the CRUD operations | Functional + constraint | FR-005, EC-02 | yes |
| A business logic layer holding business rules and validation, independent of the data layer and the API | Functional + constraint | FR-009, FR-013, FR-014, FR-015, EC-02 | yes |
| Development driven by Test-Driven Development methodologies | Quality constraint | EC-03, NFR-006 | yes |
| Unit tests for all components, including data access, business logic, and API endpoints | Non-functional (quality) | NFR-006, EC-03 | yes |
| Frontend framework integration, responsive and user-friendly, implementing CRUD, structured code | Functional + quality | FR-010, FR-011, NFR-002, NFR-003 | yes |
| README with setup instructions and other necessary documentation | Submission deliverable | DO-01 | yes |
| Seeded data / credentials for demo purposes | Functional + deliverable | FR-012, US7 | yes |
| Informal user story created by the candidate and included in the presentation | Deliverable | DO-03 (story stated above and in this specification) | yes |
| Generative AI exercise: prompt, output code or representative sample, validation, corrections, handling of edge cases, authentication, and validations | Submission deliverable | DO-02 | yes - documentation deliverable only; the exercise is explicitly PENDING in GENAI.md and adds no AI functionality to this product |
| Presentation and code-review criteria: Clean Architecture, application testing, code quality, functionality, optional browser-console cleanliness, GenAI fluency | Quality + delivery | EC-01..EC-04, NFR-006, DO-03 | yes |

Entity Framework, Dapper, and Mediator are prohibited by the original assessment; the prohibition
is carried as engineering constraint EC-01 and is not restated as a product rule.

### Submission requirements

| Submission requirement | Requirement IDs | Covered |
|---|---|---|
| SUB-01: one public GitHub repository | DO-04 | yes |
| SUB-02: exactly one repository link | DO-04 | yes |
| SUB-03: presentation of process and rationale (README or text file acceptable) | DO-01, DO-03 | yes |
| SUB-04: Generative AI portion is mandatory | DO-02 | yes - tracked as PENDING until performed |
| SUB-05: root-level `GENAI.md` | DO-02 | yes |
| SUB-06: no symbolic links | DO-05 | yes |
| SUB-07: submission deadline | Process obligation | owned by the developer; recorded in submission-requirements.md and intentionally outside engineering enforcement |
| SUB-08: post-submission repository freeze | Process obligation | owned by the developer; recorded in submission-requirements.md and intentionally outside engineering enforcement |

### Engineering constraints referenced above

| ID | Constraint | Source |
|---|---|---|
| EC-01 | Entity Framework, EF Core, Dapper, Mediator, and MediatR MUST NOT be used anywhere in the solution, including as transitive dependencies | Original assessment prohibition; carried into the constitution as a prohibited technology |
| EC-02 | Business rules and validation MUST live independently of the data layer and the API, with inward-pointing dependencies (Clean Architecture) | Original assessment; concrete layering is a planning deliverable |
| EC-03 | All new or changed runtime behavior MUST follow the Test-Driven Development lifecycle (RED, GREEN, REFACTOR) with observed evidence, enforced across the Spec Kit phases | Original assessment; constitution Principle III |
| EC-04 | Scope MUST stay appropriate for a small assessment: no additional business domains and no infrastructure beyond a demonstrated functional need | Constitution Principle V; keeps the assessment practical |
| EC-05 | The .NET version, concrete stack versions, database design, migrations, and API contracts MUST be chosen in the planning phase, not by this specification | Constitution defers these decisions to planning |

### Submission deliverables referenced above

| ID | Deliverable | Source |
|---|---|---|
| DO-01 | `README.md` containing setup instructions, the user story, engineering rationale, and presentation content | SUB-03; assessment submission guidelines |
| DO-02 | Root-level `GENAI.md` answering the Generative AI exercise (prompt, output, validation, corrections, edge cases, authentication, validations), backed by evidence records; explicitly PENDING until performed | SUB-04, SUB-05 |
| DO-03 | The informal user story and the presentation of process and rationale | Assessment presentation section; SUB-03 |
| DO-04 | Delivery through one public GitHub repository, submitted as exactly one link | SUB-01, SUB-02 |
| DO-05 | No symbolic links tracked in the repository | SUB-06 |

### Requirement-to-story coverage

| Requirements | User stories |
|---|---|
| FR-001, FR-002, FR-003, FR-004, FR-015 | US1 |
| FR-008 (access-control half), FR-017 | US2 |
| FR-005 (list, retrieve), FR-006, FR-007 | US3 |
| FR-005 (create), FR-006, FR-009, FR-013, FR-014 | US4 |
| FR-005 (update), FR-007, FR-013, FR-014 | US5 |
| FR-005 (delete), FR-007, FR-016 | US6 |
| FR-003, FR-012 | US1, US7 |
| FR-010, FR-011 | US1, US3, US4, US5, US6 |

## User Scenarios and Testing *(mandatory)*

### User Story 1 - Register and Authenticate (Priority: P1)

A visitor registers a personal account with an email address and a password. A registered user
authenticates with those credentials, stays authenticated while using the application, and can log
out from the web application. Account information is persisted, so the same user can return and
authenticate again later.

**Why this priority**: every other story depends on an authenticated identity. Ownership
enforcement, task isolation, and the demonstration journey are impossible without registration and
authentication, so this story is the foundation the rest of the product builds on.

**Independent Test**: using only the registration and login flows: register a new account, log out,
log in again with the same credentials, and verify duplicate-email, wrong-password, and
invalid-input rejections. No task functionality is needed to verify this story end to end.

**Acceptance Scenarios**:

1. **Given** no account exists for an email address, **When** a visitor registers with that email
   and a valid password, **Then** the account is created, the user information is persisted, and
   the visitor can immediately authenticate with those credentials.
2. **Given** an account already exists for an email address, **When** anyone registers again with
   the same email address, **Then** registration fails with a duplicate-email error, no second
   account is created, and the existing account is unchanged.
3. **Given** invalid registration input (malformed email address, missing password, or a password
   that does not meet the password policy), **When** the registration is submitted, **Then** the
   system rejects it with field-level validation errors and persists nothing.
4. **Given** an existing account, **When** the user authenticates with the correct email and
   password, **Then** authentication succeeds and protected functionality becomes available.
5. **Given** an existing account, **When** authentication is attempted with an incorrect password
   or an unregistered email, **Then** authentication fails with a single generic
   invalid-credentials error that does not reveal whether the email address exists.
6. **Given** an authenticated user, **When** the user logs out from the web application, **Then**
   the authenticated session ends and access to protected functionality requires re-authentication.

---

### User Story 2 - Public and Protected API Access (Priority: P1)

The product exposes two API surfaces with distinct access rules: public functionality (registration
and login) that anyone may call, and protected task functionality that only authenticated users may
call. Requests carrying missing, malformed, or expired credentials are rejected before any user
data is considered, and the acting user's identity is always derived from validated authentication
data rather than from anything the client supplies.

**Why this priority**: this is the security boundary of the entire product. Every task operation
relies on a trustworthy identity and a reliable public/protected split, so a defect here
invalidates the correctness of every later story.

**Independent Test**: with at least one registered account, call each public endpoint anonymously
(expect success), call each protected endpoint with no credentials, an invalid token, and an
expired token (expect rejection each time), and call a protected endpoint with valid credentials
(expect acceptance).

**Acceptance Scenarios**:

1. **Given** the public registration and login endpoints, **When** an unauthenticated caller
   invokes them, **Then** they respond normally without requiring credentials.
2. **Given** any protected task endpoint, **When** a request arrives with no credentials, **Then**
   the request is rejected with an authentication-required error and no task data is returned.
3. **Given** any protected task endpoint, **When** a request arrives with malformed or expired
   credentials, **Then** the request is rejected with an authentication error and no task data is
   returned.
4. **Given** a registered user with valid credentials, **When** the user calls a protected endpoint,
   **Then** the request is accepted and the response reflects that user's data only.
5. **Given** any protected operation, **When** the system resolves the acting user, **Then** the
   identity comes exclusively from validated authentication data; identity values supplied in the
   request body or parameters are ignored.

---

### User Story 3 - List and View Personal Tasks (Priority: P2)

An authenticated user sees the collection of their own tasks and can open any one of them to see
its identifier, title, description, status, and due date. Tasks belonging to other users are never
visible. A user with no tasks sees an empty list rather than an error.

**Why this priority**: viewing is the read half of the CRUD core and the first capability a user
needs after authenticating; it validates the ownership model with the least write risk.

**Independent Test**: with two accounts, one owning tasks and one owning none: list and retrieve
tasks for the first account, confirm the empty result for the second, and confirm neither account
can see the other's tasks.

**Acceptance Scenarios**:

1. **Given** a user who owns tasks, **When** the user lists their tasks, **Then** exactly their own
   tasks are returned, each showing its identifier, title, description, status, and due date.
2. **Given** a user who owns a task, **When** the user retrieves that task by its identifier,
   **Then** the full task details are returned.
3. **Given** a user who owns no tasks, **When** the user lists their tasks, **Then** an empty
   result is returned and the interface shows an empty state rather than an error.
4. **Given** a task owned by another user, **When** a user requests, lists for, or otherwise
   references that task's identifier, **Then** the request is denied with a not-found or forbidden
   error, and the other user's task is not returned or described.
5. **Given** a task identifier that does not exist, **When** a user retrieves it, **Then** a
   task-not-found error is returned.

---

### User Story 4 - Create a Task (Priority: P2)

An authenticated user creates a task with a required title, an optional description, a required due
date (a calendar date), and no client-chosen owner. New tasks default to the Pending status. Past
due dates are accepted, because recording an obligation you already owed is a valid use.

**Why this priority**: creation is the write half of CRUD that produces the data every other story
reads and modifies; without it the product has nothing to manage.

**Independent Test**: as a single authenticated user, create tasks exercising the accepted cases
(full data, description omitted, past due date) and the rejected cases (blank title, invalid
status, missing or malformed due date), then verify only the accepted tasks exist and each is owned
by the creating user.

**Acceptance Scenarios**:

1. **Given** an authenticated user, **When** the user creates a task with a valid title, an
   optional description, and a valid due date, **Then** the task is created with a unique
   identifier, the Pending status, and the authenticated user as its owner.
2. **Given** an authenticated user, **When** the user creates a task with a blank, whitespace-only,
   or missing title, **Then** the creation is rejected with a validation error and no task is
   persisted.
3. **Given** an authenticated user, **When** the user creates a task with a missing or malformed
   due date, or an unrecognized status value, **Then** the creation is rejected with a validation
   error and no task is persisted.
4. **Given** an authenticated user, **When** the user creates a task with a due date in the past,
   **Then** the task is created and accepted (past due dates are allowed).
5. **Given** an authenticated user, **When** a creation request includes an owner field naming a
   different user, **Then** the supplied value is ignored and the task is owned by the
   authenticated user.

---

### User Story 5 - Update a Task (Priority: P2)

An authenticated user edits their own task: title, description, due date, and status. The status
may move between any of the valid statuses in any direction. Ownership never changes through an
update. Invalid edits are rejected and leave the stored task unchanged.

**Why this priority**: updates complete the write lifecycle (progress tracking is the core value of
a task manager) and exercise validation and ownership on existing data.

**Independent Test**: as a single authenticated user, update an owned task across each valid status
transition and each editable field, attempt invalid updates (blank title, unknown status, malformed
due date), and attempt to update another user's task.

**Acceptance Scenarios**:

1. **Given** a user's own task, **When** the user updates any combination of title, description,
   due date, and status with valid values, **Then** the task reflects the changes, its identifier
   and owner are unchanged, and the change is visible on the next read.
2. **Given** a task in any valid status, **When** the user changes its status to any other valid
   status (Pending, InProgress, Completed), **Then** the change is accepted.
3. **Given** a user's own task, **When** the user submits an update with a blank title, an
   unrecognized status, or a missing or malformed due date, **Then** the update is rejected with a
   validation error and the stored task is unchanged.
4. **Given** a task owned by another user, **When** a user attempts to update it, **Then** the
   request is denied, the task is unchanged, and the other user's data is not returned.
5. **Given** a task identifier that does not exist, **When** a user attempts to update it, **Then**
   a task-not-found error is returned and nothing changes.

---

### User Story 6 - Delete a Task (Priority: P2)

An authenticated user deletes one of their own tasks. The task is removed from the user's task
collection and no longer appears anywhere in that user's views. Deletion never reaches other
users' tasks.

**Why this priority**: deletion closes the CRUD lifecycle and its ownership check is the last
security surface; users need it to remove obsolete work.

**Independent Test**: as a single authenticated user with two tasks, delete one, verify it
disappears from the list and by direct retrieval, verify the other task is untouched, and attempt
to delete another user's task.

**Acceptance Scenarios**:

1. **Given** a user's own task, **When** the user deletes it, **Then** the task is removed from the
   user's task collection and a later retrieval of that identifier returns a task-not-found error.
2. **Given** a user who owns several tasks, **When** the user deletes one of them, **Then** the
   remaining tasks are unchanged.
3. **Given** a task owned by another user, **When** a user attempts to delete it, **Then** the
   request is denied, the task still exists, and the other user's data is not returned.
4. **Given** a task identifier that does not exist, **When** a user attempts to delete it, **Then**
   a task-not-found error is returned and nothing changes.

---

### User Story 7 - Seeded Demonstration Environment (Priority: P3)

A reviewer can start the complete application from a clean state using documented instructions.
The startup process seeds demonstration users and demonstration tasks, and the documented
credentials log straight into a populated account, so the full journey can be demonstrated without
manual data entry. Re-running the startup does not duplicate or corrupt the seeded data.

**Why this priority**: the seed makes the assessment's required demonstration reproducible, but it
adds no capability the other stories do not already provide, so it ranks below all user-facing
functionality.

**Independent Test**: from a clean environment, follow only the documented startup instructions,
log in with the documented demonstration credentials, and confirm the demonstration tasks are
present; stop and restart the environment and confirm the same data is still present and not
duplicated.

**Acceptance Scenarios**:

1. **Given** a clean environment, **When** the application is started using only the documented
   instructions, **Then** demonstration users and demonstration tasks exist and are usable.
2. **Given** the documented demonstration credentials, **When** they are used to authenticate,
   **Then** login succeeds and the seeded tasks for that account are visible.
3. **Given** a previously started and seeded environment, **When** the startup process runs again,
   **Then** the seeded data is neither duplicated nor corrupted.
4. **Given** the seeded demonstration data, **When** any seeded user performs the registration,
   login, and task CRUD flows, **Then** all flows behave as specified in US1 through US6.

---

### Edge Cases

- What happens when a request references a task identifier that does not exist? A task-not-found
  error is returned for retrieve, update, and delete; no other data changes.
- What happens when a user references another user's task identifier? The request is denied and the
  other user's task is never returned, described, or confirmable to exist.
- What happens when a protected endpoint receives no credentials, malformed credentials, or expired
  credentials? The request is rejected with an authentication error before any data is considered.
- What happens when registration is attempted with an email address that already exists? The
  registration fails with a duplicate-email error and the existing account is unchanged.
- What happens when a title is only whitespace? It is treated as blank and rejected, exactly like a
  missing title.
- What happens when a due date is supplied with a time-of-day component or in an unparseable
  format? The value is rejected as a validation error; due dates are calendar dates.
- What happens with a due date in the past? It is accepted by design (US4, US5); the product does
  not block or auto-complete overdue tasks.
- What happens when two requests from the same account run simultaneously? Out of scope for this
  assessment (see Assumptions); the specification defines no conflict resolution.
- What happens when a status value outside the three valid statuses is submitted? A validation
  error is returned.
- What happens when the demonstration seed runs more than once? It is idempotent (US7).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow a visitor to register a user account with an email address and a
  password, where the email address is unique across accounts and the password satisfies the
  password policy. On failure (malformed email, missing or policy-violating password, duplicate
  email): the system rejects the registration with a field-specific error, persists nothing, and
  leaves existing accounts unchanged.
- **FR-002**: System MUST allow a registered user to authenticate with their email address and
  password. On failure (unknown email, wrong password): the system returns one generic
  invalid-credentials error that does not reveal whether the email address is registered.
- **FR-003**: System MUST persist user account information so that an account survives application
  restarts and can authenticate again later. On failure: authentication and ownership-dependent
  operations fail closed with an error; no partially created account is exposed as usable.
- **FR-004**: System MUST allow an authenticated user to log out from the web application, ending
  their authenticated session. On failure: the session remains and the interface reports the
  logout failure; protected access does not silently appear logged out.
- **FR-005**: System MUST allow an authenticated user to list, retrieve, create, update, and delete
  their own tasks. On failure of any operation (unknown identifier, validation error, ownership
  violation, authentication error): the operation returns a category-appropriate error, changes
  nothing, and never returns or describes another user's task.
- **FR-006**: Every task MUST have a unique identifier, a title, a description, a status, a due
  date, and exactly one associated user. On failure to satisfy field rules (blank title, invalid
  status, invalid due date): the write operation is rejected with a validation error and nothing is
  persisted.
- **FR-007**: System MUST prevent users from accessing or modifying tasks belonging to other users.
  On a cross-user attempt: the system denies the operation and MUST NOT return, describe, or
  confirm the existence of the other user's task.
- **FR-008**: System MUST expose two API surfaces: a task CRUD API over the authenticated user's
  tasks, and a second API providing user creation, login, public endpoints, and authorized
  endpoints. On failure: public endpoints respond to unauthenticated callers; protected endpoints
  reject unauthenticated callers with an authentication error.
- **FR-009**: System MUST assign task ownership from the authenticated user identity at creation
  time and MUST NOT allow ownership to be set or changed through client-supplied data. On an
  attempt to supply a different owner: the supplied value is ignored and the authenticated user
  remains the owner.
- **FR-010**: The frontend MUST support the complete registration, login, logout, and task CRUD
  flows against the APIs. On failure of any backend operation: the frontend shows the
  corresponding error state rather than a silent failure or misleading success.
- **FR-011**: The frontend MUST present meaningful loading, empty, success, validation, and error
  states for every flow it supports. On failure of a state to render: the flow MUST NOT appear to
  have succeeded or destroyed data when it has not.
- **FR-012**: System MUST provide seeded demonstration credentials and demonstration data so the
  complete journey can be demonstrated from documented startup instructions alone, and the seeding
  MUST be idempotent across repeated startups. On failure: startup reports the seeding failure
  rather than starting against an unseeded, undocumented state.
- **FR-013**: A task status MUST be one of exactly three values: Pending, InProgress, Completed. A
  new task MUST default to Pending, and a user MAY change a task between any valid statuses in any
  direction. On an unrecognized status value: the write is rejected with a validation error.
- **FR-014**: A task title MUST be required and MUST NOT be blank. A task description MUST be
  optional. A task due date MUST be required and MUST represent a calendar date without a
  time-of-day or timezone component. Past due dates MUST be accepted. On violation: the write is
  rejected with a field-specific validation error and nothing is persisted.
- **FR-015**: Registration MUST require a valid password as defined by the password policy
  (PDC-01). On violation: the registration is rejected with a password-policy validation error and
  no account is created.
- **FR-016**: Deleting a task MUST remove it from the owning user's task collection, after which
  the task is no longer retrievable by that user. On failure: the task remains and the user
  receives an error.
- **FR-017**: Every error response MUST be distinguishable by category (validation,
  authentication, authorization/not-found for cross-user cases, and task-not-found) so callers and
  the frontend can react appropriately. On failure: responses do not leak other users' data,
  stack details, or whether a cross-user record exists.

### Non-Functional Requirements

- **NFR-001 Security**: Passwords MUST be stored only in a form that does not allow recovery of the
  plaintext; plaintext passwords MUST NEVER be stored, logged, displayed, or returned by any
  endpoint. Authentication data MUST be validated on every protected request, and user identity
  MUST come from validated authentication data, never from client-supplied fields. Cross-user
  access MUST be denied in 100% of cases.
- **NFR-002 Accessibility**: Registration, login, and task flows MUST be operable by keyboard,
  MUST label every form field, and MUST surface validation and errors in an accessible way.
- **NFR-003 Responsiveness**: The interface MUST be usable across viewport widths from a small
  mobile screen to a desktop display, per the assessment's responsive and user-friendly criterion.
- **NFR-004 Reproducibility**: The complete application MUST be startable from a clean checkout
  using only documented instructions, with demonstration data seeded automatically (FR-012).
- **NFR-005 Durability**: Users and tasks MUST persist across application restarts; the
  demonstration in US7 MUST show data intact after a restart.
- **NFR-006 Test coverage**: Every functional requirement above MUST be covered by automated
  tests at the levels mandated by the constitution (unit, real-database integration, HTTP
  integration, and applicable frontend tests), including every failure scenario named in this
  specification.

### Key Entities

- **User**: a person who registers and authenticates. Key attributes: a unique email address used
  as the login identifier, and a password stored only in a non-recoverable form. A user owns zero
  or more tasks.
- **Task**: a personal work item belonging to exactly one user. Key attributes: a unique
  identifier, a required title, an optional description, a status (Pending, InProgress, or
  Completed, defaulting to Pending), and a required due date held as a calendar date. All of a
  user's tasks together form that user's task collection; no task exists without an owner.

Concrete storage design (tables, columns, constraints, migrations) is a planning-phase deliverable
and is intentionally not specified here.

## Proposed Business Rules and Open Decisions

Everything in this section is `[PROPOSED - developer review required]`. None of these rules appear
in the original assessment or submission requirements; they are product decisions recorded here so
that nothing is silently invented. No additional business rules have been introduced anywhere else
in this specification.

| ID | Proposed decision | Proposed default | Basis and effect if rejected |
|---|---|---|---|
| PDC-01 | Password policy | Valid = minimum 8 characters; anything longer accepted | Assessment says only "valid password"; rejection means defining another policy in clarification |
| PDC-02 | Title maximum length | 200 characters | No assessment limit exists; rejection means choosing another bound or no bound |
| PDC-03 | Description maximum length | 2,000 characters | Same basis as PDC-02 |
| PDC-04 | Email uniqueness comparison | Case-insensitive (standard email behavior) | Assessment says "unique email" without case rules; rejection means case-sensitive uniqueness |
| PDC-05 | Task list ordering | Due date ascending, then creation order | Assessment specifies no ordering; rejection means another ordering or user-selectable ordering |
| PDC-06 | Update shape | One update operation changes title, description, status, and due date together | Assessment does not prescribe separate status updates; rejection means a dedicated status-change operation |
| PDC-07 | Logout semantics | Logout ends the web application's authenticated session; protected access then requires re-authentication | Assessment requires logout from the web application only; server-side credential revocation semantics are a planning decision |
| PDC-08 | Overdue behavior | A past due date does not change status automatically; status changes only when the user acts | Assessment says past dates are allowed and statuses are user-controlled; rejection means adding automatic status rules (new product behavior) |

Unresolved decisions intentionally left for the `/speckit-clarify` phase: the default task list
ordering (PDC-05) and the password policy detail (PDC-01) are the two decisions most likely to
affect user experience if the proposed defaults are rejected. This specification carries no
blocking clarification markers because each open decision has a workable proposed default; the
clarification phase may still override any of them.

## Scope and Boundaries

- In scope: one product - the personal Task Manager described by the primary user story above -
  with its two API surfaces and its frontend flows.
- Out of scope: teams, sharing, collaboration, notifications, attachments, Kanban boards, tags,
  search beyond listing one's own tasks, reporting, additional business domains, bulk
  import/export, internationalization, password recovery, email change, account deletion, and any
  generative-AI feature inside the product.
- The assessment's Generative AI API-generation exercise (DO-02) is a documentation deliverable
  answered in `GENAI.md`. It shares the task domain with this product and its outcome informs the
  developer's judgment, but it is not a feature of the Task Manager and no AI functionality is
  added to the product.
- Explicitly deferred to planning: .NET version, solution and project structure, database schema,
  migrations, API paths and contracts, authentication mechanism details, frontend framework
  configuration, and seed data values. This specification deliberately does not choose any of them.

## Success Criteria *(mandatory)*

- **SC-001**: A first-time visitor completes registration and login in under 2 minutes using only
  what the interface provides.
- **SC-002**: An authenticated user completes the create, view, update, and delete journey for a
  task in under 3 minutes.
- **SC-003**: 100% of tested cross-user access attempts (list, retrieve, update, delete) are
  denied, and none of them returns or describes another user's task.
- **SC-004**: 100% of tested unauthenticated, malformed-credential, and expired-credential
  requests to protected functionality are rejected.
- **SC-005**: Every supported flow demonstrably shows each required interface state (loading,
  empty, success, validation, error) at least once in the demonstration.
- **SC-006**: Starting from a clean environment, a reviewer reaches the seeded demonstration
  (login with the documented credentials and see populated tasks) by following only the documented
  instructions.
- **SC-007**: Registered users and their tasks remain intact across an application restart.
- **SC-008**: The demonstration runs end to end without unexpected client-console errors
  (the assessment lists a clean console as a desired quality bar).

## Assumptions

- Single-user personal context: each account belongs to one person; there are no shared scopes,
  roles, or administrative users beyond what authentication and ownership already require.
- The email address doubles as the login identifier; no separate username exists.
- Account lifecycle management (password reset, email change, account deletion, email verification)
  is not required by the assessment and is out of scope.
- Due dates carry no timezone semantics: they are calendar dates, displayed as entered.
- Concurrent modification of the same task from two sessions is not addressed by this assessment;
  the specification defines no conflict resolution and none is claimed.
- The interface language is English; no localization is required.
- Demonstration seed values (credential strings and sample tasks) are chosen during implementation
  and documented in the README; no real credentials are committed to the repository.
- Performance targets beyond ordinary interactive use are not defined: the assessment does not ask
  for load, concurrency, or throughput goals, so none are invented here.
- The optional browser-console cleanliness criterion (assessment) is treated as a quality goal
  (SC-008), not a functional requirement.
