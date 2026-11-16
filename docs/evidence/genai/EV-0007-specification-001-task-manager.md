# EV-0007 - Governance corrections and feature specification 001-task-manager

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0007` |
| Date | `2026-11-16` |
| Development phase | Phase 2 - Specify (with Phase 1 governance corrections applied first) |
| Activity type | `Documentation and specification` |
| Related specification | `specs/001-task-manager/spec.md` |
| Related task identifiers | `PENDING` - tasks.md does not exist yet; this activity covers governance corrections plus the specification |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session |
| Evidence status | `Captured` |
| Commit references | `PENDING` - the developer instructed no commit or push before review |

## Actual prompt

Verbatim developer instruction for this activity:

```text
We are continuing the Ballast Lane Senior .NET Technical Interview Exercise V5.

Repository:
[https://github.com/diogocurto2/ballast-lane-tech-exercise-v5](https://github.com/diogocurto2/ballast-lane-tech-exercise-v5)

The project constitution v1.0.0 and Spec Kit integration are committed on origin/main at ac604af.

The constitution and engineering standards have been reviewed and accepted as the project foundation, subject to the targeted corrections below.

Execute this activity using the genai-evidence skill. Capture the actual prompt, generated artifacts, review findings, corrections, and observed validation.

## Part 1 — Targeted governance corrections
Before creating the feature specification, apply these limited corrections:

1. In the active tasks template, require RED-first TDD for foundational tasks that change runtime behavior, including authentication and authorization, migrations, persistence, error handling, and seed behavior. Only non-behavioral scaffolding and configuration may be marked TDD Not Applicable with justification.
2. Remove residual instructions such as "tests (if requested)" or "if you need to write tests" from the effective Tasks and Implement workflows. Required tests are mandatory.
3. Add an explicit precondition to speckit-implement: implementation MUST NOT begin until speckit-analyze has been executed against the current specification, plan, and tasks, with no unresolved blocking findings.
4. Distinguish planned RED checkpoints from observed RED results. Analyze validates the planned test ordering before implementation. Implement must observe the actual behavioral failure before writing production behavior.
5. In the active plan template, explicitly include unit tests for Infrastructure/data-access components and API controllers in addition to real PostgreSQL and HTTP integration tests.
6. Ensure that the Analyze skill remains read-only for specification, plan, tasks, and application code, while its report can be preserved through the GenAI evidence workflow.
7. Synchronize stale constitution status statements in engineering documents, submission requirements, and GENAI.md. Update EV-0006 with the actual existing commit reference and this review. Distinguish reviewer findings from developer approval.
Do not rewrite the constitution or expand its scope.

Do not change the original assessment document.

Keep submission timing and post-submission restrictions outside the constitution and engineering enforcement.

## Part 2 — Create the product specification
Use the installed Spec Kit specification workflow to create one feature for the complete technical assessment:

001-task-manager

Read the original assessment, submission requirements, constitution, and engineering standards.

Use the active specification template override.

### Product vision
Build a simple personal Task Manager web application where users can register, authenticate, and securely manage their own tasks.

The application should be small enough to demonstrate the complete user journey while showcasing senior full-stack .NET engineering practices.

### Primary informal user story
As a registered user, I want to create, view, update, and delete my personal tasks, including their status and due date, so that I can organize my work and track my progress securely.

### User journeys
Define independently testable, prioritized user stories for:

1. User registration and authentication.
2. Listing and viewing personal tasks.
3. Creating a task.
4. Updating a task.
5. Deleting a task.
6. Accessing public and protected API functionality.
7. Demonstrating the complete application with seeded users and data.

### Functional requirements

- Visitors can register a user account.
- Registered users can authenticate.
- User information is persisted.
- Authenticated users can log out from the web application.
- Authenticated users can list, retrieve, create, update, and delete their own tasks.
- Each task has a unique identifier, title, description, status, due_date, and an associated user.
- Users cannot access or modify tasks belonging to other users.
- The system provides a task CRUD API and a second API for user creation, login, public endpoints, and authorized endpoints.
- The frontend supports the full registration, login, and task CRUD flows.
- The frontend provides meaningful loading, empty, success, validation, and error states.
- Seeded demo credentials and data support a reproducible demonstration.

### Proposed business rules
Use these as explicit, proposed product decisions for developer review:

- Title is required and cannot be blank.
- Description is optional.
- Due date is required and represents a calendar date, not a timestamp.
- Tasks may have a past due date.
- Task statuses are Pending, InProgress, and Completed.
- New tasks default to Pending.
- Users may change tasks between any valid statuses.
- Every task belongs to exactly one user.
- Task ownership is assigned using the authenticated user identity and cannot be changed through client-supplied data.
- Registration requires a unique email and valid password.
- Deleting a task removes it from the user's task collection.
- No teams, collaboration, notifications, attachments, Kanban, or additional business domains.
Do not silently introduce additional business rules. Document meaningful unresolved decisions for clarification.

### Acceptance scenarios
Include Given/When/Then scenarios covering successful behavior and relevant failures:

- Successful registration and login.
- Duplicate email registration.
- Incorrect credentials.
- Invalid registration input.
- Anonymous access to protected functionality.
- Expired or invalid authentication.
- Successful task creation, retrieval, update, and deletion.
- Invalid task title, status, or due date.
- Missing task identifiers.
- Cross-user retrieval, update, and deletion attempts.
- Empty task lists.
- Responsive frontend behavior.
- Persistence of users and tasks.
Do not invent performance targets that are unnecessary for this assessment.

### Assessment traceability
Map all mandatory requirements from the original assessment and submission requirements to the relevant specification requirements, acceptance criteria, or delivery obligations.

Distinguish:

- Functional requirements.
- Non-functional quality requirements.
- Engineering constraints.
- Submission deliverables.
- Proposed product decisions.
The mandatory GenAI API-generation exercise must be explicitly traceable to the project, but do not add generative AI functionality to the Task Manager.

### Specification boundaries
Do not create application code, database schemas, API route definitions, .NET projects, React projects, or implementation classes.

Do not generate plan.md or tasks.md yet.

Do not choose the final .NET version during specification. Concrete architecture, technology versions, database design, and API contracts belong to the planning phase.

Keep the mandatory API-generation prompt, code sample, and validation in GENAI.md pending until the actual exercise is performed.

## Part 3 — Validate and report
Generate the specification and the applicable requirements-quality checklist.

Verify:

- Every mandatory assessment requirement is covered.
- Every functional requirement has testable acceptance criteria.
- User stories are independently testable.
- Security and ownership scenarios are explicit.
- Proposed business decisions are clearly distinguishable from original assessment requirements.
- Scope remains small and practical.
- The specification complies with the constitution.
- No application implementation was created.
Record the GenAI evidence for this activity.

Return:

1. Targeted governance corrections applied.
2. Feature directory and generated files.
3. User stories and priorities.
4. Functional and non-functional requirements.
5. Acceptance criteria.
6. Assessment traceability results.
7. Proposed decisions and any clarification questions.
8. Actual validation results.
9. GenAI evidence reference.
10. Readiness for the planning phase.
Do not commit or push before developer review.

Stop after completing the specification.
```

## Relevant context

- Requirement or assessment section: the complete original assessment
  (`docs/assessment/technical-exercise-v5.md`) and the submission requirements
  (`docs/assessment/submission-requirements.md`), both read before any edit.
- Engineering standard applied: `docs/engineering/genai.md` (evidence workflow),
  `docs/engineering/testing-and-tdd.md` section 3 (TDD enforcement lifecycle),
  `docs/engineering/clean-architecture.md`, `docs/engineering/security.md`,
  `docs/engineering/api-standards.md`, `docs/engineering/database-standards.md`,
  `docs/engineering/frontend-standards.md`, `docs/engineering/delivery-and-ci.md`.
- Governing artifact: `.specify/memory/constitution.md` v1.0.0 (ratified 2026-11-16).
- Files read before editing: the constitution, the installed spec/tasks/plan templates, the three
  project template overrides, `resolve-template.ps1` and `common.ps1`, the `speckit-tasks`,
  `speckit-implement`, and `speckit-analyze` skills, `GENAI.md`, the evidence index and template,
  `EV-0006`, `.github/copilot-instructions.md`, `docs/engineering/README.md`, and
  `docs/engineering/genai.md`.
- Template resolution mechanism: `Resolve-TemplateContent` applies project overrides at priority 1,
  so the overrides in `.specify/templates/overrides/` are the effective templates.
- Extension hooks: `.specify/extensions.yml` does not exist, so no `before_specify` or
  `after_specify` hooks were checked or executed.

## Original AI-generated output

### Governance corrections: original wording before correction

`speckit-tasks` skill, task organization and phase structure (excerpt as it existed at `ac604af`):

```text
   - Each phase includes: story goal, independent test criteria, tests (if requested), implementation tasks
   - If tests requested: Tests specific to that story
   - If tests requested: Each interface contract → contract test task [P] before implementation in that story's phase
  - Within each story: Tests (if requested) → Models → Services → Endpoints → Integration
```

`speckit-implement` skill, implementation execution rules:

```text
   - **Tests before code**: If you need to write tests for contracts, entities, and integration scenarios
   - **Polish and validation**: Unit tests, performance optimization, documentation
```

`.specify/templates/overrides/tasks-template.md`, foundational phase:

```text
- [ ] T006 Create versioned SQL migrations for the schema in plan.md
- [ ] T007 Implement connection handling and migration runner
- [ ] T008 Implement authentication and authorization wiring
- [ ] T009 Implement error handling and ProblemDetails mapping
- [ ] T010 Implement structured logging with sensitive-data exclusion
- [ ] T011 Implement idempotent seed for demonstration users and records
- [ ] T012 Write architecture verification test asserting dependency direction
```

`.specify/templates/overrides/plan-template.md`, test strategy table:

```text
| Domain | Unit | | | |
| Application | Unit | | | |
| Infrastructure | Integration | Real PostgreSQL required | | |
| API endpoints | HTTP integration | In-process test host | | |
| Frontend | Component and user flow | | | |
```

`speckit-analyze` skill, operating constraint and blocking findings (excerpt):

```text
**STRICTLY READ-ONLY**: Do **not** modify any files. Output a structured analysis report.
...
- an implementation task that does not depend on an observed RED checkpoint;
```

### Specification: first generated version, before validation corrections

The first generated version of `specs/001-task-manager/spec.md` is preserved unmodified in the
session artifacts (`files/spec-001-task-manager-original.md`). The three defects found by validation
iteration 1 were:

1. A literal clarification marker token in explanatory prose, which the template forbids outside a
   requirement:

```text
No `[NEEDS CLARIFICATION]` markers
are carried in this specification because each open decision has a workable proposed default
```

2. Traceability tables referencing constraint and deliverable identifiers that were never defined
   anywhere in the document. `EC-01` through `EC-05` and `DO-01` through `DO-05` appeared only as
   references:

```text
| Web application with an API and a data layer, using ASP.NET with a database or data store | Functional + constraint | FR-001..FR-017, EC-01, EC-02, EC-05 | yes |
```

3. Test-Driven Development, an explicit assessment methodology requirement, was mapped only to the
   undefined `EC-03` and never named in the document; a keyword scan for `TDD` returned no match.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | The specification states the assessment's dependency-independence requirement as EC-02 and defers layering to planning; no layering or project structure is prescribed. | `[VERIFIED]` - no layer, project, or namespace named |
| SOLID | `Not Applicable` to a specification activity. No class design exists to evaluate. | `Not Applicable` |
| TDD | The specification carries testable acceptance criteria with failure scenarios and edge cases per Principle III. Test ordering is a plan/tasks concern and is not specified here. | `[VERIFIED]` - all 17 FRs carry an explicit failure behavior; 7 stories each carry a negative scenario |
| Security | Ownership and authentication scenarios checked as explicit: cross-user retrieve, update, and delete; anonymous, malformed, and expired credentials; identity never from client data. | `[VERIFIED]` - FR-007, FR-009, NFR-001 and US2/US3/US5/US6 scenarios |
| Functional requirements | Every mandatory assessment row mapped; SUB-01..SUB-08 all accounted for; `EC`/`DO` identifiers defined; no undefined references. | `[VERIFIED]` after iteration 2 |

## Review findings

- `[VERIFIED]` Correction 1 (tasks template): the foundational phase in the override contained only
  implementation tasks. Replaced with a RED-first sequence: five test tasks, an explicit RED
  checkpoint task, four implementation tasks that each depend on the RED checkpoint, one
  configuration task marked `Not Applicable` with justification, the architecture test, and a GREEN
  checkpoint. Only non-behavioral scaffolding and configuration remain eligible for `Not
  Applicable`.
- `[VERIFIED]` Correction 2 (residual optional-test wording): five occurrences removed from the
  effective workflows - four in `speckit-tasks` and one in `speckit-implement`. A resolver run
  confirmed the effective tasks template no longer contains `Tests are OPTIONAL` or
  `(if requested)`.
- `[VERIFIED]` Correction 3 (analyze precondition): `speckit-implement` gained a blocking
  `Precondition` section before Outline step 1 requiring a completed `/speckit-analyze` run with no
  unresolved blocking findings.
- `[VERIFIED]` Correction 4 (planned versus observed RED): `speckit-analyze` now states that it
  validates the **planned** test ordering and does not execute tests, while `speckit-implement`
  must **observe** each behavioral failure before writing the production behavior. The tasks
  template and tasks skill state that a RED checkpoint in tasks.md is a planned dependency.
- `[VERIFIED]` Correction 5 (plan template): added row-level unit tests for Infrastructure
  (data access) and for API controllers, alongside the existing real-PostgreSQL integration and
  HTTP integration rows, plus an explicit statement that all levels are mandatory.
- `[VERIFIED]` Correction 6 (analyze read-only): the constraint now names the specification, plan,
  tasks, constitution, and application code, and permits exactly one output artifact - the report -
  preserved through the GenAI evidence workflow into `docs/evidence/genai/`.
- `[VERIFIED]` Correction 7 (status synchronization): stale constitution statuses corrected in
  `docs/engineering/README.md` sections 4 and 9, `docs/assessment/submission-requirements.md`
  section 4, and `docs/engineering/genai.md` sections 2 and 3. `EV-0006` now records `ac604af` as
  the commit reference and the 2026-11-16 review, with reviewer findings separated from developer
  approval.
- `[VERIFIED]` The constitution was not modified and was not rewritten; `git status` reports it
  unmodified. The original assessment document is unmodified.
- `[VERIFIED]` The installed (non-override) `.specify/templates/tasks-template.md` still contains
  `Tests are OPTIONAL`. It is intentionally left untouched: the override at priority 1 is the
  effective template, and the resolver confirms the override wins. The residual text is therefore
  not part of the effective workflow.
- `[VERIFIED]` Specification defects found and corrected: three, listed under "Original
  AI-generated output" above.
- `[VERIFIED]` A fourth defect found during the final review pass, after iteration 2 passed: the
  requirement-to-story coverage table assigned `FR-003` (user persistence) to US7 only, although
  US1's first acceptance scenario asserts that user information is persisted. Corrected to
  `US1, US7`. This shows the limits of the automated checks: identifier definition and reference
  consistency are machine-checkable, but correct requirement-to-story mapping is a semantic
  property that only a read reveals.
- `[AI-PROPOSED]` Task-status priority assignment: US1 and US2 are both P1 (authentication and the
  access boundary are inseparable prerequisites for ownership enforcement), US3 to US6 are P2 (the
  CRUD lifecycle), and US7 is P3 (demonstration reproducibility). This is an ordering proposal, not
  an assessment requirement.
- `[AI-PROPOSED]` The eight proposed product decisions `PDC-01..PDC-08` were derived from gaps where
  the assessment is silent. They are isolated in their own section and marked as requiring review;
  none is enforced elsewhere in the specification as though it were an assessment requirement.
- `[PENDING]` Developer review of the specification, the governance corrections, and the `PDC`
  defaults. No commit has been made.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Apply the seven targeted governance corrections before specifying | Developer instruction, Part 1 | `Approved` - explicit instruction |
| Keep the constitution and its scope unchanged | Developer instruction: "Do not rewrite the constitution or expand its scope" | `Approved` - explicit instruction |
| Keep submission timing and post-submission restrictions outside engineering enforcement | Developer instruction, consistent with `EV-0005` | `Approved` - explicit instruction |
| One feature, `001-task-manager`, covering the whole assessment | Developer instruction, Part 2 | `Approved` - explicit instruction |
| Seven user stories matching the seven requested journeys, prioritized P1/P1/P2/P2/P2/P2/P3 | Developer listed the journeys; the priority ordering is the AI proposal above | `Pending` - priority order requires review |
| Proposed business rules `PDC-01..PDC-08` | Developer supplied twelve rules, which the specification records verbatim in substance and numbers as `PDC` where the assessment is silent; the extra defaults fill gaps the developer asked to have documented | `Pending` - defaults require review |
| Defer .NET version, schema, API contracts, and structure to planning | Developer instruction, Part 2 boundaries | `Approved` - explicit instruction |
| Do not commit or push | Developer instruction, Part 3 | `Approved` - explicit instruction |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Foundational phase of the tasks override rebuilt RED-first | Foundational behavior could be implemented with no preceding RED checkpoint | Constitution Principle III requires RED before any behavior change, and names authentication, migrations, persistence, error handling, and seeds |
| Restricted `Not Applicable` to non-behavioral scaffolding and configuration | The prior wording allowed any "non-behavioral infrastructure task" to skip TDD without a clear boundary | A behavioral task can never be proven without a failing test first |
| Removed five optional-test instructions from the effective workflows | Residual conditional wording contradicted mandatory testing | Constitution Principle III makes tests mandatory; conditional wording reopened the loophole |
| Added a blocking precondition section to `speckit-implement` | Nothing prevented implementation before analysis | Constitution Principle I mandates pre-implementation analysis |
| Introduced the planned/observed RED distinction in analyze, implement, tasks template, and tasks skill | Both workflows used "observed RED checkpoint" for a task-list dependency | Analysis is static and cannot observe test failures; only implementation can |
| Added Infrastructure unit tests and API controller unit tests to the plan test strategy | The required levels were incomplete relative to Principle III | Principle III requires unit tests for business logic, data access, and API endpoints, in addition to integration tests |
| Clarified analyze read-only scope and permitted report preservation | Read-only scope and evidence preservation appeared to conflict | The report is a record, not an artifact edit, and the constitution requires evidence capture |
| Corrected constitution status in four documents and `EV-0006` | Ratified, committed constitution was described as an unratified template | Stale status contradicts observed repository facts |
| Corrected the FR-003 coverage mapping from `US7` to `US1, US7` | US1's first scenario asserts that user information is persisted | The coverage table understated where the requirement is verified |
| Defined `EC-01..EC-05` and `DO-01..DO-05` in the specification | Traceability referenced undefined identifiers | A traceability matrix that cites undefined IDs cannot be verified |
| Removed the literal clarification marker token from spec prose | The token is a template instruction, not content | A marker scan would report an unresolved clarification that does not exist |
| Added the TDD methodology row to the assessment traceability table | An explicit assessment requirement was only implicitly covered | Every mandatory assessment requirement must trace to a requirement ID |

## Commands and observed results

Extension hook check:

```text
Test-Path .specify/extensions.yml  ->  False
```

Effective-template verification after the corrections:

```text
effective tasks-template contains 'Tests are OPTIONAL': False
effective tasks-template contains '(if requested)':   False
effective tasks-template contains RED-first rule:     True
resolved spec-template equals the override verbatim:  True
```

Non-ASCII scan of every edited governance file:

```text
.specify/templates/overrides/tasks-template.md  -> non-ascii: none
.specify/templates/overrides/plan-template.md   -> non-ascii: U+2500 U+2514 U+251C (pre-existing tree glyphs)
.specify/templates/overrides/spec-template.md   -> non-ascii: none
.github/skills/speckit-tasks/SKILL.md           -> pre-existing glyphs only (em dash, arrow, check/cross marks)
.github/skills/speckit-implement/SKILL.md       -> pre-existing glyphs only
.github/skills/speckit-analyze/SKILL.md         -> pre-existing glyphs only
```

Searched for residual conditional-test wording across all Markdown:

```text
remaining 'Tests are OPTIONAL' matches:
  GENAI.md:152, EV-0006:368, EV-0006:448  -> historical verification statements, correct to keep
  .specify/templates/tasks-template.md:12,157 -> installed core template, superseded by the override
remaining 'if requested' matches: .specify/scripts/powershell/*.ps1 help comments only
```

Specification validation, iteration 1:

```text
non-ascii: none
placeholder absent: [FEATURE NAME]
placeholder absent: $ARGUMENTS
PLACEHOLDER FOUND: [NEEDS CLARIFICATION
link ../../docs/assessment/technical-exercise-v5.md      -> OK
link ../../docs/assessment/submission-requirements.md    -> OK
covered keywords: CRUD, second API, user creation, login, authorized, seed, README,
                  unit test, frontend, responsive, Clean Architecture, Entity Framework,
                  Dapper, Mediator, presentation, GENAI
MISSING: TDD
stories=7 priorities=7 FRs=17 NFRs=0 SCs=8
```

The `NFRs=0` reading was a scan-pattern artifact, not a defect: the non-functional requirements use
the heading form `- **NFR-001 Security**:` rather than a bare identifier, so the first pattern did
not match. The identifier check below confirms all six exist.

Specification validation, iteration 2:

```text
placeholder absent: [NEEDS CLARIFICATION, $ARGUMENTS, [FEATURE NAME], TODO
TDD mentioned: True
referenced but not defined: none
FR : 17 defined (1..17)
NFR : 6 defined (1..6)
SC : 8 defined (1..8)
PDC : 8 defined (1..8)
EC : 5 defined (1..5)
DO : 5 defined (1..5)
```

Scope verification:

```text
git status --porcelain
 M .github/skills/speckit-analyze/SKILL.md
 M .github/skills/speckit-implement/SKILL.md
 M .github/skills/speckit-tasks/SKILL.md
 M .specify/templates/overrides/plan-template.md
 M .specify/templates/overrides/tasks-template.md
 M docs/assessment/submission-requirements.md
 M docs/engineering/README.md
 M docs/engineering/genai.md
 M docs/evidence/genai/EV-0006-constitution-ratification.md
?? specs/
```

`git diff --stat` reports 9 files changed, 153 insertions, 52 deletions before `specs/` is added.
`.specify/memory/constitution.md` and `docs/assessment/technical-exercise-v5.md` are absent from the
list, confirming neither was modified.

## Final references

- Specification: [specs/001-task-manager/spec.md](../../../specs/001-task-manager/spec.md)
- Requirements checklist:
  [specs/001-task-manager/checklists/requirements.md](../../../specs/001-task-manager/checklists/requirements.md)
- Feature pointer: `.specify/feature.json`
- Tasks template override:
  [.specify/templates/overrides/tasks-template.md](../../../.specify/templates/overrides/tasks-template.md)
- Plan template override:
  [.specify/templates/overrides/plan-template.md](../../../.specify/templates/overrides/plan-template.md)
- Tasks skill: [.github/skills/speckit-tasks/SKILL.md](../../../.github/skills/speckit-tasks/SKILL.md)
- Implement skill:
  [.github/skills/speckit-implement/SKILL.md](../../../.github/skills/speckit-implement/SKILL.md)
- Analyze skill: [.github/skills/speckit-analyze/SKILL.md](../../../.github/skills/speckit-analyze/SKILL.md)
- Engineering index: [docs/engineering/README.md](../../engineering/README.md)
- GenAI standard: [docs/engineering/genai.md](../../engineering/genai.md)
- Submission requirements: [docs/assessment/submission-requirements.md](../../assessment/submission-requirements.md)
- Superseded prior record: [EV-0006](./EV-0006-constitution-ratification.md)
- Implementation files: `Not Applicable` - no application code was created
- Test files: `Not Applicable` - no test project exists
- Task identifiers: `PENDING` - tasks.md does not exist yet
- Commits: `PENDING` - the developer instructed no commit or push before review

## Evidence status rationale

Status is `Captured`: the verbatim instruction, the context supplied, the pre-correction wording of
every governance change, the three specification defects, and the executed validation results are
recorded.

It is not `Reviewed` or `Verified` because the developer has not yet reviewed the specification, the
`PDC` defaults, or the governance corrections, and the instruction prohibits committing before that
review.
