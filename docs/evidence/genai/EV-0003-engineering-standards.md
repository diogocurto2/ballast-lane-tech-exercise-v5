# EV-0003 - Engineering standards, submission requirements, and documentation index

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0003` |
| Date | `2026-11-15` |
| Development phase | Phase 1 - Constitution and engineering standards. The constitution is deliberately not ratified in this activity. |
| Activity type | Documentation and specification |
| Related specification | `PENDING` - no feature specification exists; that belongs to Phase 2. |
| Related task identifiers | `PENDING` - no Spec Kit task list exists. Work is identified by the six numbered items in the prompt below. |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session. |
| Evidence status | `Captured` |
| Commit references | `PENDING` - recorded after the commit exists; the developer instructed no commit before review. |

## Actual prompt

Verbatim developer prompt for this activity:

```text
We are continuing the Ballast Lane Senior .NET Technical Interview Exercise V5.

Repository:
[https://github.com/diogocurto2/ballast-lane-tech-exercise-v5](https://github.com/diogocurto2/ballast-lane-tech-exercise-v5)

The repository foundation, Spec Kit 1.4.0 installation, and Generative AI engineering standard have already been committed.

Our next objective is to establish all remaining engineering standards before ratifying the project constitution or implementing application features.

## 1. Establish context and GenAI evidence
Read:

- docs/assessment/technical-exercise-v5.md
- docs/engineering/genai.md
- .github/copilot-instructions.md
- .github/skills/genai-evidence/SKILL.md
- docs/evidence/genai/template.md
- GENAI.md
- .specify/memory/constitution.md
Execute the genai-evidence skill checkpoints for this documentation activity.

Create a new evidence record for this related group of engineering documentation tasks. Capture this actual prompt, representative generated documentation, significant engineering decisions, review findings, corrections, and observed validation results.

Do not fabricate application test results or developer approvals.

Verify the current Git history and synchronize stale commit references and evidence statuses in GENAI.md, EV-0001, EV-0002, and the evidence index. Record only actual developer approvals and observed facts.

The mandatory REST API GenAI exercise must remain pending.

## 2. Document submission requirements
Create docs/assessment/submission-requirements.md.

Document these additional requirements received from the recruiter:

- The solution must be delivered through one public GitHub repository.
- Send only one GitHub link for the completed code.
- Include a presentation of the development process and engineering rationale; README.md or a text file is acceptable.
- The Generative AI portion of the assessment is mandatory.
- Answer the GenAI question in a root-level GENAI.md file.
- Do not include symbolic links.
- Submission deadline: Monday, November 16, 2026, at 08:00 Brazil time (UTC-03:00).
- Once submitted, do not continue modifying the project until further notice.
Do not include the original recruiter email, personal information, or private correspondence.

Preserve the original technical-exercise-v5.md without modification.

## 3. Create the engineering standards
Create the following documents in English under docs/engineering/.

Each document must define:

- Purpose and scope.
- Mandatory engineering rules.
- Practical examples relevant to this assessment.
- Prohibited practices and anti-patterns.
- Objective verification and review criteria.
- References to other applicable engineering standards.
Use MUST for mandatory rules, SHOULD for recommendations, and MAY for optional practices.

Keep documents concise, actionable, and appropriate for a small Senior .NET technical assessment. Avoid speculative abstractions and unnecessary enterprise complexity.

### A. testing-and-tdd.md
Define:

- RED: write a meaningful executable test before implementing the behavior, run it, and observe the expected failure.
- GREEN: implement the minimum behavior required and rerun the test.
- REFACTOR: improve the implementation while preserving passing tests.
- Record observed RED, GREEN, and REFACTOR evidence.
- Distinguish a meaningful behavior failure from an unrelated build or environment failure.
- Require unit tests for business logic, data access components, and API endpoints, consistent with the assessment.
- Require integration tests against real PostgreSQL for SQL behavior, persistence, constraints, and migrations.
- Require HTTP integration tests for authentication, authorization, validation, and CRUD endpoints.
- Require appropriate frontend component and user-flow tests.
- Define when test doubles are useful and when real dependencies are necessary.
- Require regression tests for bug fixes.
- Define coverage expectations for critical behaviors rather than arbitrary coverage percentages.
- Require independent tests and reproducible execution.
Explain that documentation-only and infrastructure-only tasks may have TDD marked Not Applicable when justified, but still require applicable validation.

Integrate with the existing GenAI evidence workflow without creating duplicate evidence records.

### B. solid-principles.md
Define all five SOLID principles:

- Single Responsibility Principle.
- Open/Closed Principle.
- Liskov Substitution Principle.
- Interface Segregation Principle.
- Dependency Inversion Principle.
For each principle, include:

- A concise definition.
- Practical .NET examples.
- Common violations.
- Review questions or objective checks.
Explain how SOLID applies to controllers, application use cases, domain entities, persistence adapters, and tests.

Prefer simple designs. Do not require interfaces, factories, generic repositories, or additional layers without a justified need.

Distinguish SOLID from unnecessary abstraction.

### C. clean-architecture.md
Define the responsibilities of:

- Domain.
- Application.
- Infrastructure.
- API/presentation.
- Frontend.
- Composition roots.
Establish allowed dependency directions.

Require that business logic remains independent of ASP.NET Core, Npgsql, PostgreSQL, and frontend frameworks.

Define dependency inversion, repository boundaries, DTO mapping, application use cases, and framework-independent business rules.

Allow composition roots to reference infrastructure for dependency injection without moving business logic into the API layer.

Explain how two API hosts can share core application libraries without violating Clean Architecture.

Require verification of architectural dependency rules through project-reference inspection and suitable automated architecture checks.

Do not define the final application project structure, classes, database schema, or API contracts yet. These belong to the Spec Kit planning phase.

### D. coding-standards.md
Define practical standards for:

- C# naming, nullable reference types, asynchronous operations, and CancellationToken.
- Explicit responsibilities and readable methods.
- Exception handling and error propagation.
- Dependency injection.
- Logging without sensitive information.
- TypeScript strict typing and avoidance of unnecessary any.
- Maintainable React components.
- Consistent formatting, linting, and build warnings.
- Comments and documentation that explain decisions rather than repeat code.
- Small, understandable changes and meaningful Git commits.
Prohibit dead code, commented-out implementations, unnecessary abstractions, and silent exception handling.

### E. api-standards.md
Define:

- REST resource-oriented endpoints.
- HTTP verbs and status codes.
- Request and response DTOs.
- Input validation and business-rule validation.
- Consistent error responses using ProblemDetails where applicable.
- OpenAPI documentation.
- Authentication and authorization boundaries.
- Appropriate handling of missing resources, conflicts, invalid requests, and unauthorized access.
- Thin controllers that delegate business behavior to application use cases.
- API contracts that can be tested independently.
Require a data CRUD API and a second API supporting user registration, login, public endpoints, and authenticated endpoints, as required by the assessment.

The exact endpoint paths and contracts must be established in the implementation plan.

### F. security.md
Define:

- Secure password hashing using established .NET security components.
- Never storing plaintext passwords.
- Secure authentication and token validation.
- Explicit public and protected endpoints.
- Resource-level authorization.
- User identity derived from validated authentication claims, not trusted client-supplied ownership fields.
- Prevention of cross-user access to task data.
- SQL injection prevention.
- Server-side input validation.
- Safe handling of malformed, missing, expired, or invalid credentials.
- Secure configuration, environment variables, CORS, HTTPS, and secret handling.
- Appropriate handling of public demo credentials.
- No sensitive data in logs or GenAI evidence.
Require security-focused tests for authorization bypass, invalid authentication, and cross-user access.

Avoid implementing unnecessary custom cryptographic or authentication protocols.

### G. database-standards.md
Use PostgreSQL as the proposed persistence technology aligned with the job description.

Define:

- Direct database access with Npgsql and ADO.NET.
- No Entity Framework, Dapper, Mediator, or MediatR.
- Parameterized SQL.
- Connection management and disposal.
- Async database operations.
- Primary keys, foreign keys, uniqueness, and database constraints.
- Version-controlled SQL migrations.
- Reproducible database initialization.
- Idempotent seeded demo data.
- Transactions where atomicity is required.
- Correct persistence of users and application records.
- Appropriate indexing based on actual query requirements.
- Real PostgreSQL integration testing.
Do not create the actual schema, migration files, or database code yet.

### H. frontend-standards.md
Use React and TypeScript as the proposed frontend baseline.

Define:

- Component responsibilities.
- Separation of UI, application state, and API access.
- Authentication state and session expiration handling.
- Responsive design.
- Accessibility and keyboard navigation.
- Form validation and clear error messages.
- Loading, empty, success, and error states.
- CRUD user flows.
- Appropriate state management without unnecessary dependencies.
- Frontend testing and linting.
- Secure interaction with authenticated APIs.
Do not create application screens, install dependencies, or scaffold the React project yet.

### I. delivery-and-ci.md
Define:

- Docker and Docker Compose for reproducible local execution.
- Versioned configuration and example environment files.
- Dependency startup and health checks.
- GitHub Actions for backend build and tests, frontend build and lint, and integration validation.
- Seeded demo users and data.
- README setup, execution, testing, architecture, user story, and engineering rationale.
- Required GENAI.md content and supporting evidence.
- Public repository hygiene.
- No symbolic links in tracked Git files.
- Final submission checklist.
- No further repository changes after assessment submission until authorized.
Distinguish mandatory assessment requirements from project quality improvements.

Do not require AWS, Redis, Kubernetes, or other infrastructure without an actual functional need.

## 4. Create a documentation index
Create docs/engineering/README.md.

Provide:

- A concise index of all engineering standards.
- The purpose of each standard.
- Which standards apply to each Spec Kit phase.
- Cross-references between related documents.
- A proposed mapping from the future constitutional principles to the engineering standards.
The constitution will be created separately using /speckit-constitution after these documents are reviewed.

Do not ratify or modify the constitution in this task.

## 5. Update repository-wide Copilot instructions
Update .github/copilot-instructions.md to reference the engineering documentation index.

Require Copilot to consult the relevant engineering standards before any future specification, planning, implementation, testing, or review activity.

Keep these instructions concise. Do not duplicate the complete standards.

Preserve the existing genai-evidence obligations.

## 6. Validate the documentation
Review all new and existing engineering documents together.

Verify:

- Consistency between TDD, SOLID, Clean Architecture, security, API, database, frontend, and delivery standards.
- Coverage of every mandatory assessment and submission requirement.
- Correct internal links.
- No prohibited technologies.
- No contradictory dependency rules.
- No mandatory practices that introduce unnecessary complexity.
- Clear distinctions between established requirements, proposed technology choices, and future architecture decisions.
- No application implementation or fake test results.
- No secrets, private information, or symbolic links.
Capture actual validation results and review findings using the genai-evidence workflow.

Update the root GENAI.md only with relevant, verified information. Keep it concise and keep the mandatory REST API exercise pending.

## Scope restrictions

- Do not generate application code.
- Do not create .NET or React projects.
- Do not create feature specifications, plans, or tasks.
- Do not choose the final .NET version yet; the job description references .NET 6/7/8 and the assessment does not mandate a specific version. Record this as a planning decision.
- Do not modify the original Spec Kit skills or templates.
- Do not modify or ratify the constitution.
- Do not commit or push before developer review.
Return a report containing:

1. Files created and modified.
2. Summary of each engineering standard.
3. Proposed mapping to constitutional principles.
4. Actual validation commands and results.
5. GenAI evidence record created.
6. Decisions that still require developer approval.
7. Confirmation that the documentation is ready for the constitution step.
Stop after completing this documentation activity.
```

## Relevant context

- Assessment: [technical-exercise-v5.md](../../assessment/technical-exercise-v5.md), read in full; the Generative AI, testing, architecture, and frontend sections drive the standards.
- Standard applied: [genai.md](../../engineering/genai.md) - Phase 1 applies, with checkpoints before, during, and after execution.
- Previously committed baseline: `b3b6195` (standards and evidence workflow), `0439fbb` (commit references).
- Constitution read and confirmed unratified: `.specify/memory/constitution.md` still contains `[PROJECT_NAME]`, `[PRINCIPLE_1_NAME]`, and other placeholders.
- External facts observed before writing: submission deadline weekday and remaining time, computed with PowerShell (see commands below).

## Original AI-generated output

Representative excerpt of an originally generated standard, before correction. The section list lacked the required **Practical examples** section:

```text
clean-architecture.md headings (original):
## 1. Purpose and scope
## 2. Mandatory rules
## 3. Layer responsibilities
## 4. Allowed dependency directions
## 5. Dependency inversion and repository boundaries
## 6. DTO mapping
## 7. Application use cases
## 8. Framework-independent business rules
## 9. Composition roots and multiple hosts
## 10. Prohibited practices and anti-patterns
## 11. Verification and review criteria
## 12. Deferred decisions
## 13. References
```

```text
solid-principles.md headings (original):
## 1. Purpose and scope
## 2. Mandatory rules
## 3. The five principles
## 4. Application to each concern
## 5. SOLID versus unnecessary abstraction
## 6. Prohibited practices and anti-patterns
## 7. Verification and review criteria
## 8. References
```

Originally generated index row that used a non-ASCII section sign:

```markdown
| VIII | Proportional Simplicity (no unjustified complexity) | `solid-principles.md` §5, `delivery-and-ci.md` §3, `clean-architecture.md` §12 |
```

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | `Not Applicable` to application code - no code was produced. The standard's own rules were checked for internal consistency instead. | `Not Applicable` |
| SOLID | `Not Applicable` to application code - no code was produced. | `Not Applicable` |
| TDD | `Not Applicable` - documentation-only activity; the standard defines TDD for later phases. Applicable validation (links, structure, consistency) was performed instead. | `Not Applicable` |
| Security | Scanned new documents for secrets, credentials, tokens, and machine-specific absolute paths. | `[VERIFIED]` none found |
| Functional requirements | All six prompt items and scope restrictions checked against the produced files; prohibited technologies, symlinks, code presence, and untouched files verified. | `[VERIFIED]` with two defects found and corrected |

## Review findings

- `[VERIFIED]` All nine standards initially lacked nothing except that `clean-architecture.md` and `solid-principles.md` had no explicit **Practical examples** section, which the prompt requires for every document. Detected by a section-presence check; both were corrected.
- `[VERIFIED]` `docs/engineering/README.md` used a non-ASCII section sign (`U+00A7`) in three cross-references. Replaced with explicit `section N` wording, and the section numbers were updated for the inserted sections.
- `[VERIFIED]` Inserting sections shifted heading numbers; all standards were confirmed to have strictly sequential numbering after renumbering.
- `[VERIFIED]` `genai.md` does not use the section template required for standards A-I. This is deliberate: it was established and approved in earlier tasks with a different structure, and it is the governing process document rather than one of the new standards. Recorded rather than silently changed.
- `[VERIFIED]` Prohibited technologies (Entity Framework, EF Core, Dapper, MediatR, Mediator) appear in the documents only as prohibitions or as assessment statements; no document permits their use.
- `[AI-PROPOSED]` Technology choices recorded as **proposed** rather than established: PostgreSQL with Npgsql/ADO.NET, React with TypeScript, Docker Compose, GitHub Actions.
- `[AI-PROPOSED]` Eight proposed constitutional principles, with "Proportional Simplicity" added because several standards deliberately forbid abstraction a small assessment does not need.
- `[PENDING]` Developer review of all new standards and the proposed constitutional mapping.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| PostgreSQL as proposed persistence technology | Aligned with the job description; assessment prohibits EF/Dapper | `Pending` |
| React and TypeScript as proposed frontend baseline | Assessment allows a framework of choice; this is the most familiar and testable option | `Pending` |
| .NET version left undecided | Job description references 6/7/8; assessment mandates none; recorded as a planning decision per instruction | `Pending` |
| Prohibit extra infrastructure (AWS, Redis, Kubernetes) | No functional need; instruction forbids without justification | `Pending` |
| Add "Proportional Simplicity" as proposed principle VIII | Prevents best-practice drift toward unjustified complexity | `Pending` |
| Leave `genai.md` on its existing structure | It was established and approved earlier; rewriting it was out of scope | `Pending` |
| Defer project structure, schema, contracts, API paths | Explicitly deferred to the Spec Kit planning phase | `Pending` |

No developer approval was recorded for this activity. The developer instruction was to avoid committing before review.

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Added `## 4. Practical examples` to `solid-principles.md` with a create-task flow showing SRP, DIP, ISP, and LSP in one example | Prompt requires a practical examples section in every standard | A named section makes the requirement objectively checkable instead of implied by per-principle snippets |
| Added `## 6. Practical examples` to `clean-architecture.md` with project-reference snippets, a use case, a composition root, and boundary mapping | Same requirement | Project references are the most direct, verifiable expression of the dependency rules |
| Renumbered sections in both files after insertion | Inserted sections must not leave duplicate or skipped numbers | Sequential numbering keeps cross-references resolvable and auditable |
| Replaced `§5`, `§3`, `§12` with `section 6`, `section 3`, `section 13` in `docs/engineering/README.md` | Non-ASCII character in an English document; numbers shifted after insertion | Keeps documents ASCII-consistent and references accurate after renumbering |

## Commands and observed results

Deadline facts verified before documenting them:

```text
$d = Get-Date '2026-11-16T08:00:00-03:00'; $d.DayOfWeek; Get-Date -Format 'yyyy-MM-dd HH:mm K'; ($d - (Get-Date)).TotalHours
```

```text
deadline weekday: Monday
now: 2026-11-15 18:09 -03:00
hours remaining: 13.8
```

Git history verification:

```text
git log --oneline -3
git rev-list --left-right --count origin/main...main
```

```text
0439fbb docs: record commit reference in GenAI evidence records
b3b6195 docs: establish GenAI engineering standard and evidence workflow
859b141 chore: initialize repository foundation
0	0
```

Structure, encoding, and link validation after corrections:

```text
=== required sections (new standards A-I) ===
  all nine: missing: none

=== numbering sequential ===
  all standards sequential=True

=== non-ascii prose ===
  (none)

=== links ===
  checked: 159 | broken: 0
```

Anchor and cross-document consistency validation:

```text
=== anchor validation ===
  anchors checked: 1 | broken: 0

=== cross-document rule consistency ===
  dep direction stated in clean-architecture     OK
  API inner-layer only in composition root       OK
  PG integration tests in testing                OK
  PG integration tests in database               OK
  security tests referenced                      OK
  problem details in api                         OK
  two APIs required in api                       OK
  multi-host in clean-arch                       OK
  no extra infra in delivery                     OK
  abstraction limit in solid                     OK
  genai integration no duplicate records         OK
  net version deferred                           OK
  deadline recorded                              OK
  freeze recorded                                OK
  no symlinks recorded                           OK
```

Scope-restriction verification:

```text
git ls-files -s | Where-Object { $_ -match '^120000' }     -> (empty: no symlinks)
Get-ChildItem -Recurse -Include *.cs,*.csproj,*.sln,*.tsx,*.ts,*.jsx,package.json
                                                            -> (empty outside .specify/.github: no application code)
git diff --exit-code -- docs/assessment/technical-exercise-v5.md .specify/memory/constitution.md
                                                            -> exit 0 (both unchanged)
```

The first structural validation run, recorded because it produced the two defects:

```text
  clean-architecture.md        missing: ['Examples']
  solid-principles.md          missing: ['Examples']
  docs\engineering\README.md   ['0xa7']   (non-ASCII section sign)
```

## Final references

- Engineering standards: [docs/engineering/](../../engineering/README.md)
- Submission requirements: [submission-requirements.md](../../assessment/submission-requirements.md)
- Copilot instructions: [.github/copilot-instructions.md](../../../.github/copilot-instructions.md)
- Assessment response: [GENAI.md](../../../GENAI.md)
- Implementation files: `Not Applicable` - documentation only
- Test files: `Not Applicable` - no test project exists
- Task identifiers: `PENDING` - no Spec Kit task list exists
- Commits: `PENDING` - developer instructed no commit before review

## Evidence status rationale

Status is `Captured`: the prompt, context, originally generated excerpts, the two detected defects with their corrections, and the executed validation results are recorded.

It is not `Reviewed` or `Verified` because the developer has not yet reviewed these standards, no approval has been granted, and the developer explicitly instructed not to commit before review.
