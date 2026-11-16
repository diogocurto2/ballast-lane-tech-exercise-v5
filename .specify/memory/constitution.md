# Ballast Lane Technical Interview Exercise V5 Constitution

The constitution governs software engineering for this repository: principles, architecture,
testing, quality, security, and AI-assisted development. Assessment and submission **process**
obligations are external requirements owned by the developer and recorded in
[docs/assessment/submission-requirements.md](../../docs/assessment/submission-requirements.md). They do
not appear in this constitution.

The original assessment
([docs/assessment/technical-exercise-v5.md](../../docs/assessment/technical-exercise-v5.md)) and the
submission requirements are authoritative. This constitution cannot weaken them.

## Core Principles

### I. Assessment-First and Specification-Driven Development

Every implemented feature MUST trace to the original assessment, the approved specification, and
testable acceptance criteria. Work that cannot be traced is out of scope.

Every feature MUST follow this lifecycle, without skipping a phase:

```text
Constitution -> Specify/Clarify -> Plan -> Tasks -> Analyze -> Implement -> Review/Converge -> Conclude
```

- Analysis of the specification, plan, tasks, and constitutional compliance MUST occur **before**
  implementation begins. It MAY be repeated afterwards; the pre-implementation analysis is
  mandatory.
- The specification MUST contain testable acceptance criteria, including relevant failure scenarios
  and edge cases.
- Requirements MUST be traceable to assessment obligations. An uncovered assessment requirement is
  a blocking finding.

**Rationale**: the assessment evaluates specification discipline and traceability. Defects are
cheapest to remove while the artifact is still prose.

**References**: [docs/assessment/technical-exercise-v5.md](../../docs/assessment/technical-exercise-v5.md),
[docs/assessment/submission-requirements.md](../../docs/assessment/submission-requirements.md),
[docs/engineering/README.md](../../docs/engineering/README.md)

### II. Clean Architecture and SOLID

Dependencies MUST point inward. `Domain` and `Application` MUST remain independent of HTTP,
database, and frontend frameworks: they MUST NOT reference ASP.NET Core, Npgsql, PostgreSQL, or
frontend libraries.

- Each type MUST have one nameable responsibility, and business rules MUST live in the domain or
  application layer, not in controllers or persistence adapters.
- Boundary interfaces MUST be owned by the consuming inner layer. Infrastructure MUST implement
  them; an inner layer MUST NOT construct infrastructure types.
- Controllers MUST be thin: bind, delegate to one application use case, map the outcome.
- Composition roots MAY reference Infrastructure for dependency registration. Business logic MUST
  NOT move into the API layer to avoid calling a use case.
- All five SOLID principles MUST be applied practically. An abstraction MUST be justified by a real
  seam, a testability boundary, or a recorded decision.
- The plan MUST demonstrate dependency direction and layer responsibilities. Tasks MUST include
  architecture verification. Analysis MUST identify architectural and SOLID violations before
  implementation.

**Rationale**: the assessment scores Clean Architecture and separation of concerns directly.
Unjustified abstraction is equally a defect: it hides intent and adds cost without behavior.

**References**: [docs/engineering/clean-architecture.md](../../docs/engineering/clean-architecture.md),
[docs/engineering/solid-principles.md](../../docs/engineering/solid-principles.md),
[docs/engineering/coding-standards.md](../../docs/engineering/coding-standards.md)

### III. Test-Driven Development (Non-Negotiable)

All new or changed runtime behavior MUST follow RED, GREEN, REFACTOR, in that order.

The complete TDD enforcement lifecycle defined in section 3 of
[docs/engineering/testing-and-tdd.md](../../docs/engineering/testing-and-tdd.md) is binding:

- **Specify** MUST establish testable acceptance criteria.
- **Plan** MUST define the test strategy, scenarios, tools, environments, and coverage for every
  applicable layer.
- **Tasks** MUST generate test-first, dependency-ordered work. Each implementation task MUST depend
  on its corresponding observed RED checkpoint. A test task and its implementation MUST NOT be
  marked parallelizable.
- **Analyze** MUST reject missing tests, missing RED checkpoints, or incorrect task ordering as
  blocking findings.
- **Implement** MUST execute and record actual RED and GREEN results.
- **Review** MUST verify the recorded evidence; an unsupported claim is not evidence.

Required test levels: unit tests for business logic, data access, and API endpoints; integration
tests against a real PostgreSQL instance wherever persistence behavior is involved; HTTP
integration tests for authentication, authorization, validation, and CRUD; and applicable frontend
component and user-flow tests.

An environmental, configuration, or compilation failure is NOT valid behavioral RED evidence.

Documentation-only and non-behavioral tasks MAY mark TDD `Not Applicable` with a written
justification and appropriate alternative validation.

**Rationale**: the assessment prefers TDD and evaluates testing rigour. Post-hoc tests cannot
demonstrate a test-first process, so the evidence must be captured when it occurs.

**References**: [docs/engineering/testing-and-tdd.md](../../docs/engineering/testing-and-tdd.md) section 3,
[docs/engineering/genai.md](../../docs/engineering/genai.md)

### IV. Secure REST APIs and Persistence

APIs MUST be resource-oriented and use correct HTTP semantics, status codes, request and response
DTOs, ProblemDetails error shapes, and generated OpenAPI documentation. Input validation and
business-rule validation MUST be distinguished and MUST both occur server-side.

The solution MUST provide a data CRUD API and a second API supplying user registration, login,
public endpoints, and protected endpoints.

- Passwords MUST be hashed with an established .NET component and MUST NEVER be stored, logged, or
  returned in plaintext.
- Authentication tokens MUST be validated on every protected request, with explicit issuer,
  audience, signature, and expiry checks.
- User identity MUST be derived from validated claims. Ownership MUST NOT be taken from a
  client-supplied field.
- Every operation on a user-owned record MUST enforce resource-level authorization. Cross-user
  access MUST be denied and MUST NOT return the record.
- All SQL MUST be parameterized. Database constraints MUST enforce uniqueness, required fields, and
  relationships.
- Secrets MUST come from configuration or environment variables and MUST NEVER be committed.

**Entity Framework, EF Core, Dapper, Mediator, and MediatR MUST NOT be used anywhere in the
solution**, including as transitive dependencies for any purpose.

**Rationale**: the assessment requires user creation, login, and authorized and non-authorized
endpoints, and explicitly forbids those libraries. Security defects are the highest-cost failures
in a code review.

**References**: [docs/engineering/api-standards.md](../../docs/engineering/api-standards.md),
[docs/engineering/security.md](../../docs/engineering/security.md),
[docs/engineering/database-standards.md](../../docs/engineering/database-standards.md)

### V. Full-Stack Quality and Reproducible Delivery

The frontend MUST be functional, responsive, and accessible, and MUST integrate with the backend
for all CRUD operations. Component, state, and API-access concerns MUST be separated.

Delivery MUST be reproducible and documented:

- The full stack MUST run from a documented command sequence on a clean checkout, with
  containerized PostgreSQL and idempotent seeded demonstration users and records.
- `README.md` MUST contain setup instructions, the user story, the engineering rationale, and the
  presentation content. It MUST NOT describe unimplemented functionality.
- `GENAI.md` MUST exist at the repository root as the assessment's Generative AI response.
- No exposed secrets, credentials, or machine-specific absolute paths may be committed.
- No symbolic links may be tracked.
- Automated build, lint, and test checks SHOULD run in CI, and integration tests MUST NOT be
  skipped to keep a pipeline green.

The engineering-reviewable submission constraints are binding: a single public repository delivered
by a single link, the README content above, a root-level `GENAI.md`, no symbolic links, and no
exposed secrets. Submission timing and assessment-process obligations are recorded in
[docs/assessment/submission-requirements.md](../../docs/assessment/submission-requirements.md) and are
owned by the developer; they are not engineering principles and do not appear in this constitution.

Scope MUST remain appropriate for a small technical assessment. Infrastructure such as AWS, Redis,
Kubernetes, message brokers, or service meshes MUST NOT be introduced without a functional need
recorded in the plan.

**Rationale**: the assessment evaluates full-stack ability and requires seeded demo data.
Reproducibility is what makes a demonstration and a review possible.

**References**: [docs/engineering/frontend-standards.md](../../docs/engineering/frontend-standards.md),
[docs/engineering/delivery-and-ci.md](../../docs/engineering/delivery-and-ci.md),
[docs/assessment/submission-requirements.md](../../docs/assessment/submission-requirements.md)

### VI. Evidence-Based Generative AI Engineering

GitHub Copilot is the primary AI coding tool. Every meaningful AI-assisted activity MUST apply the
`genai-evidence` skill and its three checkpoints: before execution, during execution, and before
task completion.

Evidence MUST capture:

- the actual prompt and the context supplied;
- original AI-generated artifacts, preserved **before** any correction;
- critical review findings and the developer's decisions;
- observed commands and results, including failures;
- corrections with technical justification.

- Evidence MUST remain proportional to the activity: one record per meaningful activity or related
  group of tasks, not per command or trivial edit.
- Missing mandatory evidence MUST block task completion.
- Generated code MUST NOT be committed without review and observed validation.
- Prompts, generated output, defects, corrections, test results, approvals, and commit references
  MUST NEVER be fabricated.
- `GENAI.md` MUST remain a concise, accurate assessment response based only on verified evidence.

The assessment's mandatory Task Management REST API exercise is required work for this repository
and MUST eventually include: the actual prompt, representative original generated code, validation
of the output, corrections or a justified acceptance, and the handling of authentication,
authorization, validation rules, and edge cases. Until that work is performed it MUST remain
explicitly pending in `GENAI.md`.

**Rationale**: the assessment grades fluency with AI tools and critical evaluation of generated
code. Neither can be demonstrated by narration after the fact.

**References**: [docs/engineering/genai.md](../../docs/engineering/genai.md),
[.github/skills/genai-evidence/SKILL.md](../../.github/skills/genai-evidence/SKILL.md),
[GENAI.md](../../GENAI.md)

## Additional Constraints

### Prohibited technologies

Entity Framework, EF Core, Dapper, Mediator, and MediatR are prohibited throughout the solution, not
only for data access. Data access MUST use Npgsql with ADO.NET directly.

### Technology choices

PostgreSQL with Npgsql, React with TypeScript, Docker Compose, and GitHub Actions are the current
proposed choices. Each is revisable in the implementation plan when a recorded reason justifies the
change.

### Decisions deferred to planning

The final .NET version, solution and project structure, class design, database schema, migration
files, API paths, and API contracts MUST be established in the Spec Kit implementation plan. The job
description references .NET 6, 7, and 8, and the assessment mandates no specific version.

### External requirements boundary

This constitution governs engineering. Product requirements originate from the assessment and
submission requirements documents and MUST NOT be invented here. Engineering standards implement
this constitution but cannot contradict it; where a standard is silent, this constitution governs.

## Development Workflow and Quality Gates

### Mandatory lifecycle

```text
Constitution -> Specify/Clarify -> Plan -> Tasks -> Analyze -> Implement -> Review/Converge -> Conclude
```

Each phase produces artifacts and evidence. A phase is complete only when its evidence exists and
is consistent with its artifacts.

### Constitution Check

A Constitution Check MUST be performed before implementation begins, and re-evaluated after design.

| Principle | Check |
| --- | --- |
| I. Assessment-first | Every requirement traces to the assessment and has testable acceptance criteria; pre-implementation analysis completed. |
| II. Clean Architecture and SOLID | Dependency direction documented and valid; layer responsibilities explicit; no unjustified abstraction. |
| III. TDD | Test strategy complete; RED-first task ordering; every implementation task depends on a RED checkpoint; no test/implementation parallelization. |
| IV. Secure APIs and persistence | Authn/authz boundaries defined; resource-level authorization planned; parameterized SQL; constraints; no prohibited libraries. |
| V. Full-stack delivery | Responsive frontend planned; seeded demo data; reproducible setup; README and `GENAI.md` obligations accepted. |
| VI. GenAI evidence | Evidence capture points identified for every activity type. |

### Blocking versus non-blocking findings

**Blocking** findings MUST be resolved before the next phase begins:

- violation of a MUST in Principles I through VI;
- an assessment requirement with no coverage;
- a functional requirement without planned tests, or with tests ordered after implementation;
- an implementation task whose dependency does not require an observed RED checkpoint;
- missing mandatory GenAI evidence for a completed activity;
- a claim of completion without observed validation.

**Non-blocking** findings MUST be recorded and MAY be deferred with a written reason:

- SHOULD-level guidance not met, with an acceptable trade-off;
- terminology drift and documentation clarity;
- quality improvements that no requirement depends on.

A blocking finding MUST NOT be silently deferred, reclassified, or downgraded. Deferring it
requires an amendment under Governance.

### Review and convergence

Review MUST verify the implementation against the specification and MUST check architectural
boundaries, security, recorded test results, and GenAI evidence. Readiness MUST NOT be claimed while
a mandatory assessment requirement is incomplete.

## Governance

### Authority and precedence

The constitution supersedes engineering standards and all other project practices. Engineering
standards implement it and MUST reference it rather than restating it. The original assessment and
the recorded submission requirements are external and cannot be weakened by this constitution or by
any project documentation; where they conflict with a project artifact, they prevail.

### Amendment procedure

1. Propose the amendment with the motivating problem and the affected principles.
2. Record the proposal and the developer's decision in a GenAI evidence record.
3. Update this document, incrementing the version per the policy below.
4. Update affected engineering standards so they remain consistent.
5. Re-run the Constitution Check on any in-flight feature.

### Versioning policy

Semantic versioning applies:

- **MAJOR**: a principle is removed or redefined incompatibly.
- **MINOR**: a principle or governance section is added, or guidance is materially expanded.
- **PATCH**: clarifications, wording, and non-semantic refinements.

### Compliance review

Every specification, plan, task list, and implementation MUST be checked against this constitution.
`/speckit-analyze` MUST treat a constitutional violation as blocking. `/speckit-converge` MUST
report unresolved violations as the highest severity. Non-compliance MUST NOT be resolved by
reinterpreting the principle; it requires fixing the artifact or amending this document.

### Developer approval

Constitutional changes REQUIRE explicit developer approval, recorded in an evidence record with its
date and basis. Completing a review, an interview, or a demo does not imply approval, and approval
MUST NOT be inferred or fabricated.

### Contradiction resolution

When artifacts contradict each other, resolve in this order: the original assessment, then the
recorded submission requirements, then this constitution, then the engineering standards, then the
specification and plan. The resolution and its reason MUST be recorded.

### Runtime guidance

Engineering standards index: [docs/engineering/README.md](../../docs/engineering/README.md).
AI-assisted development standard: [docs/engineering/genai.md](../../docs/engineering/genai.md).

**Version**: 1.0.0 | **Ratified**: 2026-11-16 | **Last Amended**: 2026-11-16
