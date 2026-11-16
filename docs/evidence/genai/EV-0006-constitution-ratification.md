# EV-0006 - Constitution ratification and Spec Kit enforcement integration

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0006` |
| Date | `2026-11-16` |
| Development phase | Phase 1 - Constitution and engineering standards (ratification and tooling integration). |
| Activity type | Documentation and specification |
| Related specification | `PENDING` - no feature specification exists; that belongs to Phase 2. |
| Related task identifiers | `PENDING` - no Spec Kit task list exists. Work is identified by the developer instruction below. |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session. |
| Evidence status | `Reviewed` |
| Commit references | `ac604af` - `docs: ratify constitution v1.0.0 and enforce it in Spec Kit`, present on `origin/main` |

## Actual prompt

Verbatim developer instruction for this activity (constitution request; the full eight-section text is reproduced without omission):

```text
Create and ratify the project constitution for the Ballast Lane Senior .NET Technical Interview Exercise V5.

Repository:
[https://github.com/diogocurto2/ballast-lane-tech-exercise-v5](https://github.com/diogocurto2/ballast-lane-tech-exercise-v5)

GitHub Spec Kit 1.4.0 is already installed with Copilot skills and PowerShell integration.

All assessment requirements and engineering standards have been committed. The existing constitution is still the original unratified template.

## 1. Required context and evidence
Before modifying any files, read:

- docs/assessment/technical-exercise-v5.md
- docs/assessment/submission-requirements.md
- docs/engineering/README.md
- Every engineering standard under docs/engineering/
- .github/copilot-instructions.md
- .github/skills/genai-evidence/SKILL.md
- .specify/memory/constitution.md
- The active Spec Kit plan, spec, and task templates
- The active Spec Kit constitution, plan, tasks, analyze, implement, and converge skills
- GENAI.md and the existing evidence records
Execute the genai-evidence checkpoints for this constitution activity.

Capture this actual prompt, the original proposed constitution, meaningful review findings, corrections, and observed validation results.

Keep the mandatory REST API GenAI exercise pending.

## 2. Create the constitution
Replace the original template at:

.specify/memory/constitution.md

Create Constitution Version 1.0.0.

The constitution must be concise, normative, enforceable, and based on the existing engineering documentation.

Establish these six non-negotiable principles:

### I. Assessment-First and Specification-Driven Development
Every implemented feature MUST trace to the original assessment, the approved specification, and testable acceptance criteria.

Require this lifecycle:

Constitution → Specify/Clarify → Plan → Tasks → Analyze → Implement → Review/Converge → Conclude.

Pre-implementation analysis is mandatory.

The original assessment and submission requirements are authoritative.

Reference both assessment documents and the engineering standards index.

### II. Clean Architecture and SOLID
Domain and Application MUST remain independent of HTTP, database, and frontend frameworks.

Dependencies MUST point inward.

Require explicit responsibilities, dependency inversion, thin controllers, independent business rules, and practical application of all five SOLID principles.

Composition roots may reference Infrastructure for dependency registration.

The implementation plan MUST demonstrate dependency direction and responsibilities.

Tasks MUST include appropriate architecture verification and review activities.

Analyze MUST identify architectural and SOLID violations before implementation.

Reference:

- docs/engineering/clean-architecture.md
- docs/engineering/solid-principles.md
- docs/engineering/coding-standards.md
Avoid unnecessary abstractions and speculative enterprise complexity.

### III. Test-Driven Development — Non-Negotiable
All new or changed runtime behavior MUST follow RED, GREEN, and REFACTOR.

Require the complete TDD enforcement lifecycle defined in section 3 of docs/engineering/testing-and-tdd.md.

Specifically:

- Specify MUST establish testable acceptance criteria.
- Plan MUST define test strategy, scenarios, tools, environments, and coverage for every applicable layer.
- Tasks MUST generate test-first, dependency-ordered work.
- Each implementation task MUST depend on its corresponding observed RED checkpoint.
- A test and its corresponding implementation MUST NOT be parallelized.
- Analyze MUST reject missing tests, missing RED checkpoints, or incorrect task ordering.
- Implement MUST execute and record actual RED and GREEN results.
- Review MUST verify the evidence rather than accept unsupported claims.
Require unit tests, real PostgreSQL integration tests where persistence is involved, HTTP integration tests, and applicable frontend tests.

An environmental or compilation failure is not valid behavioral RED evidence.

Documentation-only and non-behavioral tasks may mark TDD Not Applicable with justification and alternative validation.

### IV. Secure REST APIs and Persistence
Require correct REST contracts, HTTP semantics, input validation, business-rule validation, authentication, authorization, and secure persistence.

Require a data CRUD API and a second API providing user registration, login, public endpoints, and protected endpoints.

Require password hashing, validated user identity, resource-level authorization, parameterized SQL, constraints, and secure secret handling.

Entity Framework, EF Core, Dapper, Mediator, and MediatR MUST NOT be used anywhere in the solution, including transitive dependencies where applicable.

Reference:

- docs/engineering/api-standards.md
- docs/engineering/security.md
- docs/engineering/database-standards.md

### V. Full-Stack Quality and Reproducible Delivery
Require a functional, responsive, accessible frontend integrated with the backend.

Require meaningful tests, seeded demonstration users and data, reproducible setup, accurate documentation, and appropriate automated delivery checks.

Reference:

- docs/engineering/frontend-standards.md
- docs/engineering/delivery-and-ci.md
- docs/assessment/submission-requirements.md
Preserve these mandatory submission constraints:

- One public GitHub repository.
- One repository link.
- README with setup instructions, user story, engineering rationale, and presentation content.
- Root-level GENAI.md.
- No symbolic links.
- No exposed secrets.
- No further repository changes after submission until further authorization is received.
Do not assume that completing the interview automatically authorizes repository modifications.

Keep scope appropriate for a small technical assessment.

### VI. Evidence-Based Generative AI Engineering
GitHub Copilot is the primary AI coding tool.

Require the genai-evidence skill and its three checkpoints for every meaningful AI-assisted activity.

Require capture of actual prompts, relevant context, original AI-generated artifacts before corrections, critical review, developer decisions, observed tests, and improvements.

Evidence MUST remain proportional to the activity.

Missing mandatory evidence MUST block task completion.

GENAI.md MUST be a concise, accurate assessment response based on verified evidence.

The mandatory Task Management REST API exercise MUST include the actual prompt, representative original generated code, validation, corrections or justified acceptance, authentication, authorization, validation rules, and edge cases.

Never fabricate evidence or approvals.

Reference:

- docs/engineering/genai.md
- .github/skills/genai-evidence/SKILL.md

## 3. Governance
Define:

- Constitution version and ratification metadata based on actual observed dates and developer approval.
- Amendment rules.
- Developer approval requirements.
- Constitution Check before implementation.
- Criteria for blocking versus non-blocking findings.
- Traceability to engineering standards.
- Resolution of contradictions between artifacts.
The original assessment and recruiter submission requirements cannot be weakened by project documentation or constitutional amendments.

Keep the final .NET version and concrete application architecture as planning decisions.

Do not add unnecessary infrastructure or introduce new business requirements.

## 4. Integrate the constitution with Spec Kit
Inspect the actual installed Spec Kit 1.4.0 integration and template-resolution mechanism.

Use supported project-level customizations or overrides where available. If no supported override exists for a required behavior, make a minimal, documented change to the relevant tracked template or skill.

Do not reinstall or upgrade Spec Kit.

Ensure the effective templates and skills enforce:

### Specify

- Testable acceptance criteria.
- Traceability to assessment requirements.

### Plan

- Explicit Constitution Check for all six principles.
- Clean Architecture dependency analysis.
- SOLID design verification.
- Complete TDD strategy.
- Security and persistence design.
- GenAI evidence capture points.

### Tasks

- Mandatory tests for new or changed behavior.
- Explicit RED-first task ordering.
- No parallelization of dependent tests and implementation.
- GREEN and REFACTOR checkpoints.
- Architecture, security, and evidence verification tasks.
Remove or override any default template instruction that treats required tests as optional.

### Analyze

- Mandatory pre-implementation analysis.
- Missing required tests and incorrect RED-first dependencies are blocking findings.
- Constitution violations cannot be silently deferred.
- Verify coverage of all mandatory assessment requirements.

### Implement

- Read the applicable engineering standards.
- Execute TDD checkpoints in order.
- Invoke or explicitly follow the genai-evidence skill.
- Preserve original generated code before corrections.
- Do not mark tasks complete without required evidence and observed validation.

### Review and Converge

- Verify actual implementation against the specification.
- Check architectural boundaries, security, test results, and GenAI evidence.
- Do not claim readiness while mandatory assessment requirements are incomplete.
Avoid maintaining duplicate sources of rules. Templates and skills should reference the constitution and engineering standards.

## 5. Correct existing inconsistencies
Review the current documentation for these specific issues:

- Ensure prohibited technologies are prohibited throughout the solution, not merely for data access.
- Ensure the post-submission freeze ends only upon further authorization, not automatically when the interview ends.
- Synchronize stale constitution and evidence statuses with actual repository facts.
- Keep historical evidence intact and distinguish historical observations from current status.
- Ensure pending skill-discovery verification is not presented as completed.
- Do not invent developer approvals.

## 6. Validate and report
Verify that:

- The constitution contains no unresolved template placeholders.
- Every constitutional principle references the correct engineering documents.
- The effective Plan and Tasks templates enforce TDD.
- The Analyze workflow blocks violations before implementation.
- The Implement workflow requires GenAI evidence.
- The original assessment document is unchanged.
- No application code was created.
- No symbolic links or secrets were introduced.
Record the actual commands and observed validation results.

Update the engineering index, Copilot instructions, evidence index, and GENAI.md as appropriate.

Return:

1. The complete constitution.
2. A mapping of principles to engineering standards.
3. The exact Spec Kit templates, skills, or overrides changed.
4. How TDD is enforced in Plan, Tasks, Analyze, and Implement.
5. How GenAI evidence is enforced.
6. Actual validation results.
7. Outstanding decisions and limitations.
8. The GenAI evidence record for this activity.
Do not create the product specification, plan, tasks, or application code yet.

Do not commit or push before developer review.

Stop after completing the constitution and its Spec Kit integration.
```

### Clarification requested during this activity

The instruction contradicted the previously applied and committed directive that the constitution MUST NOT contain a post-submission code freeze. The conflict was raised with the developer rather than resolved silently.

```text
Question: Where should the post-submission constraint live?
Options:
  1. In Principle V as a submission constraint (overrides f59bc6b)
  2. Principle V cites submission-requirements.md without restating the constraint
  3. Omit it from the constitution entirely (strict reading of f59bc6b)
Developer response: omit
```

Result: Principle V records only the engineering-reviewable submission constraints and refers
process obligations to `submission-requirements.md`. The freeze appears nowhere in the committed
constitution; the temporary Sync Impact Report that mentioned it was removed before staging.

## Relevant context

- Template resolution inspected: `.specify/scripts/powershell/resolve-template.ps1` and
  `common.ps1`. `Resolve-TemplateContent` applies, in priority order: project overrides
  (`.specify/templates/overrides/<name>.md`, full replacement), installed presets, extension
  layers, then the core template. A supported project-level override mechanism therefore exists.
- Constitution template resolved successfully through that stack before any edit.
- `.specify/extensions.yml` does not exist, so no pre- or post-execution hooks were checked or run.
- Context read: both assessment documents, the engineering index, all ten engineering standards,
  Copilot instructions, the `genai-evidence` skill, the constitution template, the spec, plan, and
  tasks templates, and the constitution, plan, tasks, analyze, implement, and converge skills.
- Evidence records EV-0001 through EV-0005 were reviewed for stale statuses.
- Git state observed: `origin/main` empty relative to local; commits `859b141`, `b3b6195`,
  `0439fbb`, `9906b6a`, `ba92121`, `fd309c7`, `f59bc6b`, `0018be0` present locally.

## Original AI-generated output

Representative excerpt of the first generated constitution, before correction. The links were
written repository-root-relative although the file lives in `.specify/memory/`:

```markdown
The original assessment
([docs/assessment/technical-exercise-v5.md](docs/assessment/technical-exercise-v5.md)) and the
submission requirements are authoritative.
```

That resolved to `.specify/memory/docs/assessment/...`, which does not exist. Detected by the link
resolver, which reported 23 broken files. All 23 were corrected by prefixing `../../`.

Second defect, in the same file, a markdown link wrapped in inline code so it would render
literally:

```markdown
`[.github/skills/genai-evidence/SKILL.md](.github/skills/genai-evidence/SKILL.md)`
```

Corrected before the file replaced the template.

Third defect, in `.github/skills/speckit-implement/SKILL.md`: inserting a new step produced two
steps numbered `8`. Renumbered to `8, 9, 10`.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | `Not Applicable` to application code - none produced. Principle II and the plan override define the dependency rules. | `Not Applicable` |
| SOLID | `Not Applicable` to application code - none produced. | `Not Applicable` |
| TDD | `Not Applicable` to this documentation activity. Enforcement wiring verified in the plan and tasks overrides and the analyze skill. | `Not Applicable` with alternative validation |
| Security | No secrets or machine-specific absolute paths introduced. | `[VERIFIED]` |
| Functional requirements | All six instruction sections checked: constitution content, Spec Kit integration, inconsistency fixes, validation, no code, no commit. | `[VERIFIED]` with four defects found and corrected |

## Review findings

- `[VERIFIED]` A supported override mechanism exists and is used. `.specify/templates/overrides/` takes priority 1 in `Resolve-TemplateContent`, so no installed template was modified. A resolver run confirmed all three overrides are the effective templates.
- `[VERIFIED]` Defect: 23 broken links in the constitution, caused by root-relative paths in a file two directories deep. Corrected; final link scan reports 0 broken files and 0 broken anchors across 242 documentation links.
- `[VERIFIED]` Defect: an inline-code-wrapped markdown link in Principle VI would have rendered literally. Corrected.
- `[VERIFIED]` Defect: duplicate step numbering in the implement skill after inserting the evidence step. Corrected to 8, 9, 10.
- `[VERIFIED]` Defect: the three override templates contained markdown links written as `../../docs/...`. Because those templates are copied into `specs/<feature>/` at runtime, the links were dangling while the template was stored in `.specify/templates/overrides/`. Converted to plain code-formatted paths, which is unambiguous in both locations.
- `[VERIFIED]` Final link scan after all corrections: 251 links, 0 broken files, 0 broken anchors.
- `[VERIFIED]` The constitution contains no unresolved `[ALL_CAPS]` placeholders and no non-ASCII characters.
- `[VERIFIED]` The six principles match the requested titles exactly and each references its engineering documents.
- `[VERIFIED]` The tasks template instruction "Tests are OPTIONAL" was replaced by an override that mandates test tasks, and the same change was applied in the tasks skill, which is the actual instruction source.
- `[VERIFIED]` The post-submission freeze is absent from the constitution body per the developer's explicit decision.
- `[VERIFIED]` Prohibited technologies are now stated solution-wide in the constitution, the database standard, and the Copilot instructions. The Copilot instruction previously limited them to "for data access", which was the exact inconsistency named in the instruction.
- `[VERIFIED]` No application code, project file, or test project was created. No symbolic link was introduced.
- `[AI-PROPOSED]` Six principles rather than the eight drafted earlier in the engineering index. Proportional simplicity and readable code were folded into Principles II, III, and V to keep the constitution concise, since the developer specified six.
- `[PENDING]` Developer review and commit of the constitution and integration changes. **Resolved 2026-11-16** - see the review section below: the changes were committed as `ac604af` and reviewed by the developer.
- `[PENDING]` Whether Copilot discovers and invokes the `genai-evidence` skill automatically. Not observed, and not claimed anywhere.

## Review of 2026-11-16 (recorded after the commit)

The developer reviewed the ratified constitution and the engineering standards as committed in
`ac604af` and accepted them as the project foundation. That review produced seven targeted
governance corrections, which are applied and recorded in `EV-0007`. This section separates what
the **reviewer observed** from what the **developer approved**, because they are not the same fact.

### Reviewer findings

- `[VERIFIED]` The tasks template override required RED-first ordering for user-story work but left
  the foundational phase (authentication, authorization, migrations, persistence, error handling,
  seed behavior) as plain implementation tasks. Behavioral work could therefore be implemented
  without a preceding RED checkpoint.
- `[VERIFIED]` Residual conditional-test wording survived in the effective workflows: the
  `speckit-tasks` skill still said "tests (if requested)" in three places, and `speckit-implement`
  still said "If you need to write tests".
- `[VERIFIED]` `speckit-implement` had no precondition requiring a completed `/speckit-analyze` run,
  so implementation could begin with no analysis of the specification, plan, or tasks.
- `[VERIFIED]` Analyze and Implement both referred to an "observed RED checkpoint", conflating the
  planned dependency in tasks.md with the behavioral failure observed during implementation.
- `[VERIFIED]` The plan template override listed Domain and Application unit tests plus real
  PostgreSQL and HTTP integration tests, but omitted unit tests for Infrastructure/data-access
  components and API controllers, which the constitution's Principle III requires.
- `[VERIFIED]` The Analyze skill's read-only rule did not state that its report may be preserved
  through the GenAI evidence workflow, leaving report preservation ambiguous.
- `[VERIFIED]` Constitution status was stale in four places: `docs/engineering/README.md` sections
  4 and 9, `docs/assessment/submission-requirements.md` section 4, `docs/engineering/genai.md`
  sections 2 and 3, and the `GENAI.md` supporting-evidence table still called the constitution the
  unratified template.

### Developer approval

`[DEVELOPER DECISION]` The developer's review statement, recorded verbatim: "The constitution and
engineering standards have been reviewed and accepted as the project foundation, subject to the
targeted corrections below." The seven corrections above are developer-directed findings, not
reviewer-invented scope: each names a specific defect and the correction to apply.

This approves the constitution and standards **as the foundation**. It does not approve the
product-level specification that follows, does not approve a commit of the correction work, and
does not imply approval of anything not named in the review.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Six principles, version 1.0.0, ratified `2026-11-16` | Developer instruction; actual date of this ratification | `Approved` - explicit instruction, and accepted in the 2026-11-16 review |
| Omit the post-submission freeze from the constitution | Developer answered the clarification with "omit", preserving the `f59bc6b` separation | `Approved` - explicit response |
| Use `.specify/templates/overrides/` rather than editing installed templates | Supported mechanism; avoids manifest hash drift on templates | `Approved` - the developer's 2026-11-16 corrections target those override files, which presupposes their retention; committed in `ac604af` |
| Edit five tracked skill files | No override mechanism exists for skills; the skills are the instruction source that must change | `Approved` - the developer's 2026-11-16 corrections target the `speckit-tasks`, `speckit-analyze`, and `speckit-implement` skills; committed in `ac604af` |
| Fold proportional simplicity and readable code into Principles II, III, V rather than standalone principles | Developer specified exactly six principles | `Approved` - constitution accepted as the foundation in the 2026-11-16 review |
| Commit these changes | Developer has not yet instructed a commit | `Approved` - committed in `ac604af`, present on `origin/main` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Temporary Sync Impact Report comment removed from the constitution before staging | The `/speckit-constitution` workflow defines it as scratch material expected to be removed before commit | Governance content must not carry amendment scratch notes; the amendment facts are preserved here |
| 23 constitution links rewritten with `../../` | Links resolved against `.specify/memory/`, not the repository root | Markdown links resolve relative to the containing file |
| Inline-code-wrapped link unwrapped | Would render as literal text | Markdown does not process links inside inline code |
| Implement skill steps renumbered 8, 9, 10 | Inserted step created a duplicate `8` | Deterministic step references keep the workflow auditable |
| Copilot instructions: prohibited technologies widened to solution-wide | Instruction required it; the file said "for data access" | The assessment forbids the libraries for the assignment, not only for persistence |
| Database standard wording widened and de-narrowed | Same inconsistency in the standard | Keeps the constitution and standards in agreement |
| Engineering index section 6 rewritten from "proposed mapping" to ratified mapping | The constitution now exists and is ratified | An index that calls a ratified mapping "proposed" is factually stale |
| Copilot instructions status updated to ratified | Was "still the unratified template" | Stale status contradicts observed repository facts |
| 2026-11-16: seven targeted governance corrections applied to the overrides and skills | Developer review found the gaps listed above | Recorded in `EV-0007` with before/after excerpts |
| 2026-11-16: constitution status synchronized in the engineering index, submission requirements, and the GenAI standard | The ratified, committed constitution was still described as an unratified template | Stale status contradicts observed repository facts |

## Commands and observed results

Pre-execution hook check:

```text
Test-Path .specify/extensions.yml   ->  False
```

Template resolution before editing:

```text
powershell -NoProfile -File .specify/scripts/powershell/resolve-template.ps1 constitution-template -Json
-> TEMPLATE_NAME constitution-template; TEMPLATE_CONTENT contains [PROJECT_NAME] and [PRINCIPLE_1_NAME]
```

Override resolution after creating the three overrides:

```text
plan-template    override_active=True chars=6595
tasks-template   override_active=True chars=7879
spec-template    override_active=True chars=3836
```

Constitution validation:

```text
placeholders [ALL_CAPS]: none
principles: I. Assessment-First and Specification-Driven Development
            II. Clean Architecture and SOLID
            III. Test-Driven Development (Non-Negotiable)
            IV. Secure REST APIs and Persistence
            V. Full-Stack Quality and Reproducible Delivery
            VI. Evidence-Based Generative AI Engineering
version line: **Version**: 1.0.0 | **Ratified**: 2026-11-16 | **Last Amended**: 2026-11-16
freeze present in constitution body: False
non-ascii: none
```

Link and anchor validation:

```text
Before correction: links 242 | broken files 23 | broken anchors 0
After correction:  links 242 | broken files 0  | broken anchors 0
```

Skill edit verification:

```text
speckit-tasks:    "Tests are MANDATORY" present; "Tests are OPTIONAL" gone
speckit-analyze:  "Pre-implementation gate" present
speckit-implement:"GenAI evidence obligations" present
speckit-converge: "Convergence MUST also verify" present
speckit-plan:     "all six principles" present
```

Scope verification:

```text
git status --porcelain -- .specify/templates/*.md          -> empty (installed templates untouched)
git status --porcelain -- .specify (tracked files)         -> only .specify/memory/constitution.md modified
application code (.cs/.csproj/.sln/.ts/.tsx outside tooling) -> none
tracked symbolic links                                      -> none
```

## Final references

- Constitution: [.specify/memory/constitution.md](../../../.specify/memory/constitution.md)
- Plan override: [.specify/templates/overrides/plan-template.md](../../../.specify/templates/overrides/plan-template.md)
- Tasks override: [.specify/templates/overrides/tasks-template.md](../../../.specify/templates/overrides/tasks-template.md)
- Spec override: [.specify/templates/overrides/spec-template.md](../../../.specify/templates/overrides/spec-template.md)
- Engineering index: [docs/engineering/README.md](../../engineering/README.md)
- Copilot instructions: [.github/copilot-instructions.md](../../../.github/copilot-instructions.md)
- Assessment response: [GENAI.md](../../../GENAI.md)
- Implementation files: `Not Applicable` - documentation and configuration only
- Test files: `Not Applicable` - no test project exists
- Task identifiers: `PENDING` - no Spec Kit task list exists
- Commits: `ac604af` - ratified constitution and Spec Kit enforcement integration on `origin/main`

## Evidence status rationale

Status is `Reviewed`: the instruction, the clarification and its answer, the three detected defects
with their corrections, the executed validation results, and the developer's 2026-11-16 review with
its seven targeted findings are all recorded, and the work is committed in `ac604af`.

It is not `Verified` because the seven governance corrections that the review produced are applied
and validated under a separate record (`EV-0007`) and have not themselves been reviewed or
committed. Advancing this record to `Verified` requires that follow-up work to be reviewed and its
commit reference recorded.
