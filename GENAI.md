# Generative AI - Assessment Response

Living response to the Generative AI section of the Ballast Lane Senior .NET technical assessment ([technical-exercise-v5.md](docs/assessment/technical-exercise-v5.md)).

This document records what was actually asked, what the AI tool actually produced, how it was validated, and what was decided. It is not a generic AI policy and not a chat transcript. Detailed evidence lives in [docs/evidence/genai/](docs/evidence/genai/README.md).

**Current status: the Task Management REST API exercise and its evidence are pending. No API prompt has been written, no application code has been generated, and no tests have been executed. Specification, plan, and task list (`EV-0001` through `EV-0009`) are complete through the Tasks phase; no task has been executed and application code does not exist yet.**

Provenance labels used throughout: `[AI-PROPOSED]`, `[DEVELOPER DECISION]`, `[VERIFIED]`, `[PENDING]`. Only `[VERIFIED]` marks an observed result. Labels describe provenance, not approval: design choices introduced by the AI in these artifacts are marked `[AI-PROPOSED]` until the developer approves them, and approval status is tracked in the evidence records, where it currently reads `Pending` for final approval. A choice becomes `[DEVELOPER DECISION]` only once the developer has directed or approved it.

## 1. AI Tool and Development Approach

- **Primary AI coding tool:** GitHub Copilot in VS Code.
- **Specification-driven development:** GitHub Spec Kit 1.4.0, installed and configured in this repository with the Copilot integration, PowerShell scripts, and sequential feature numbering. See `.specify/init-options.json` and `.specify/integration.json`.
- **Constitution state:** `.specify/memory/constitution.md` is **ratified** at version 1.0.0 (2026-11-16) with six non-negotiable principles, and committed in `ac604af`. Spec Kit 1.4.0 is installed and configured with the Copilot integration. Feature `001-task-manager` has a specification, an implementation plan, and a RED-first task list under `specs/001-task-manager/`; no task has been executed and no application code exists yet.
- **Spec Kit enforcement:** project overrides in `.specify/templates/overrides/` replace the plan, tasks, and spec templates so the Constitution Check, dependency analysis, test strategy, and mandatory RED-first task ordering are enforced by the effective templates rather than by convention.
- **Evidence standard:** [docs/engineering/genai.md](docs/engineering/genai.md) defines the mandatory workflow, evidence capture rules, proportionality rules by activity type, provenance labels, evidence lifecycle, and completion gate.
- **Engineering standards:** [docs/engineering/README.md](docs/engineering/README.md) indexes the testing/TDD, SOLID, Clean Architecture, coding, API, security, database, frontend, and delivery standards, plus their applicability per Spec Kit phase and a proposed constitutional mapping.
- **Evidence skill:** [.github/skills/genai-evidence/SKILL.md](.github/skills/genai-evidence/SKILL.md) defines the three checkpoints before, during, and before completion of each meaningful activity.
- **Repository-wide instructions:** [.github/copilot-instructions.md](.github/copilot-instructions.md) states the evidence obligations and requires consulting the relevant engineering standard before each activity.

`[DEVELOPER DECISION]` The workflow order is fixed: constitution and engineering standards, specify and clarify, plan, tasks, analyze the specification, plan, tasks, and constitutional compliance, implement with TDD and the evidence checkpoints, review and converge, then conclude. Analysis may be repeated after implementation, but the pre-implementation analysis is mandatory. The rationale is that inconsistencies are cheapest to fix before code exists.

`[AI-PROPOSED]` The approach is evidence-based rather than narration-based: each meaningful activity produces a record containing the actual prompt, the original AI output, the relevant validation commands and observed results, and the developer's review decisions. Evidence is scaled to the activity type so the workflow stays practical for a small assessment. The rationale is that the assessment evaluates critical thinking about AI output, which cannot be demonstrated by asserting quality after the fact.

`[VERIFIED]` The workflow documents were created and validated on `2026-11-14`, reviewed by the developer, and corrected against six specific findings; see section 8, `EV-0001`, and `EV-0002`.

`[VERIFIED]` The constitution was ratified on `2026-11-16` at version 1.0.0 with six non-negotiable principles, the Spec Kit plan, tasks, and spec templates are overridden to enforce it, and the work is committed in `ac604af`. The developer reviewed the constitution and engineering standards as the project foundation on `2026-11-16` and directed seven targeted corrections; they are applied under `EV-0007`. See `EV-0006`.

`[PENDING]` Whether Copilot discovers and automatically invokes the `genai-evidence` skill has not been verified. The skill files exist and their frontmatter is structurally valid, but discovery and invocation in a session have not been observed and are not claimed.

## 2. Task Management REST API — Assessment Exercise

The assessment requires generating a RESTful API for a simple task management system with:

- create, read, update, and delete tasks;
- a task with a title, description, status, and due date;
- tasks associated with a user, assuming a basic User model exists.

The assessment also requires the prompt used, the output code or a representative sample, and an explanation of how the output was validated, corrected, and how edge cases, authentication, and validation were handled.

`[PENDING]` This exercise has not started. No prompt exists for it, no code has been generated for it, and no build or test has been run against it. It will be executed through the Spec Kit workflow and recorded as its own evidence records, separate from `EV-0001`.

`[AI-PROPOSED]` Repository setup work is recorded separately from the API exercise so that preparatory documentation is never mistaken for the assessed generation task.

## 3. Actual Prompt

`[PENDING]` The prompt required by the assessment has not been written yet. When it exists, it will be recorded verbatim in an evidence record and reproduced in this section without editing.

For the record: the actual prompt that produced the documentation and configuration work in this repository is reproduced verbatim in `EV-0001`. That prompt covers repository standards and tooling; it is not the assessment's API generation prompt and must not be presented as such.

## 4. Original AI-Generated Code

`[PENDING]` No application code has been generated. `EV-0001` preserves representative excerpts of the generated documentation artifacts, including the skill frontmatter, before review.

`[AI-PROPOSED]` For code generation, a representative original excerpt or diff will be preserved in the evidence record before any correction is applied, and corrections will be recorded alongside the original rather than replacing it. The rationale is that replacing the original destroys the ability to evaluate what the model actually produced, which is the object of the assessment's review criteria.

## 5. Validation of AI Suggestions

`[AI-PROPOSED]` Every AI suggestion is evaluated against the following gates before acceptance, and the result is recorded in the evidence record:

| Gate | What is checked |
| --- | --- |
| Clean Architecture | Dependency direction, separation of concerns, independence of the business layer from data and API layers |
| SOLID | Single responsibility, open/closed, Liskov substitution, interface segregation, dependency inversion |
| TDD | A test exists for the behavior, it fails without the change, and it asserts meaningful outcomes |
| Security | Authentication and authorization placement, input validation, secret handling, no credential leakage |
| Functional requirements | Behavior matches the specification and the assessment requirements |

`[VERIFIED]` For the documentation work performed so far, AI output was validated by automated checks: YAML frontmatter parsing, skill name and directory consistency, description length limit, ASCII-only content, relative-link resolution, and confirmation that the assessment document was unmodified. One defect was found and corrected; see section 6.

`[PENDING]` Validation of API code, tests, and security controls.

## 6. Corrections and Improvements

`[VERIFIED]` Correction applied during this documentation work: four Markdown links in `.github/copilot-instructions.md` were generated as repository-root-relative paths and resolved to `.github/docs/...` and `.github/GENAI.md`. They were changed to file-relative paths (`../docs/...`, `../GENAI.md`). Technical justification: Markdown relative links resolve against the containing file's directory, so root-relative targets break editor and GitHub navigation. The defect was detected by an automated link-resolution scan, not by inspection.

`[VERIFIED]` Correction applied within the evidence record itself: a claimed wording change in the standard was removed because it never occurred, and the recorded command was restated in the exact form executed on this platform. Technical justification: an evidence record must not contain an event that did not happen, and command provenance must match what was actually run.

`[VERIFIED]` Correction applied to provenance labels: design choices in this document were initially marked `[DEVELOPER DECISION]` before any developer approval existed, and were relabeled `[AI-PROPOSED]`. Technical justification: the standard reserves `[DEVELOPER DECISION]` for choices the developer has made, so the initial labeling overstated the provenance of unreviewed AI output. The label transitions when approval is recorded in `EV-0001`.

`[VERIFIED]` Corrections applied on 2026-11-16 review (`EV-0007`): the foundational phase of the tasks template override was rebuilt RED-first so authentication, migrations, persistence, error handling, and seed behavior cannot be implemented before an observed RED; five residual optional-test instructions were removed from the effective `speckit-tasks` and `speckit-implement` workflows; `speckit-implement` gained a blocking precondition that `/speckit-analyze` has run with no unresolved blocking findings; the planned-versus-observed RED distinction was made explicit in analyze, implement, and the tasks template and skill; the plan test strategy gained unit tests for Infrastructure/data-access components and API controllers; the analyze read-only scope was clarified to permit report preservation through the evidence workflow; and stale constitution statuses were synchronized across four documents and `EV-0006`. Technical justification for each change is recorded in `EV-0007`.

`[VERIFIED]` Specification defects found by executed validation, not by inspection: a literal clarification marker token in explanatory prose, traceability tables citing `EC-01..EC-05` and `DO-01..DO-05` that were never defined, and Test-Driven Development mapped to an undefined identifier without ever being named. All three were corrected and iteration 2 re-validated clean.

`[PENDING]` Corrections to API implementation code, tests, or security controls.

## 7. Authentication, Authorization, Validation, and Edge Cases

`[PENDING]` This section cannot be answered until the API exercise is implemented. It will document, with references to code and tests:

- how users are created and how credentials are stored, and why that storage approach is safe;
- how login issues credentials and how authorized endpoints are protected, including what happens to missing, malformed, and expired credentials;
- which authorization rules decide whether a user may act on a task that belongs to another user;
- which input validation rules are enforced and where they live relative to the architecture;
- which edge cases are handled, such as duplicate user creation, invalid or missing fields, unknown identifiers, and concurrent updates;
- which edge cases are deliberately not handled, with the reason.

Recording intentions here would misrepresent them as results, so nothing is claimed at this stage.

## 8. Testing and Verification Results

### Documentation work performed so far

| Command or check | Observed result |
| --- | --- |
| `python -c "import yaml; print(...)"` | `pyyaml 6.0.3` available for frontmatter validation |
| Frontmatter parse of `SKILL.md` | Parsed keys: `name`, `description`, `compatibility`, `metadata` |
| Skill name vs directory | `genai-evidence` matches `.github/skills/genai-evidence`; pattern valid; description 696 of 1024 characters |
| ASCII-only scan of the created documents | No non-ASCII characters in the five workflow documents; the only non-ASCII character in `GENAI.md` is the em dash of the required section title |
| Relative-link resolution scan | Four broken links in `copilot-instructions.md` plus pending targets; all resolved after correction and after `GENAI.md` and `EV-0001` were created. Final prose-only scan: 40 links checked, 0 broken |
| `git diff -- docs/assessment/technical-exercise-v5.md` | Empty output; the assessment document is unchanged |

`[VERIFIED]` These are actual observed results from this session.

### Documentation work: refinement per developer review (`EV-0002`)

| Command or check | Observed result |
| --- | --- |
| Spec Kit state inspection | `init-options.json`: `speckit_version` `1.4.0`, `integration` `copilot`, `script` `ps`. `integration.json`: version `1.4.0`, `installed_integrations` `["copilot"]` |
| Workflow order check | Parsed headings: Phase 1 Constitution, Phase 2 Specify, Phase 3 Plan, Phase 4 Tasks, Phase 5 Analyze, Phase 6 Implement, Phase 7 Review, Phase 8 Conclude. `analyze before implement: True` |
| Skill frontmatter re-parse | Keys `compatibility`, `description`, `metadata`, `name`; `name==dir: True`; description 696 of 1024 characters |
| Relative-link resolution scan | 53 prose links checked, 0 broken |
| Non-ASCII prose scan | No non-ASCII in any document except the em dash of the required GENAI.md section title |
| Inaccurate-claim scan | All regex hits inspected; each is a prohibition or a description of the corrected defect, not a live claim. No absolute Windows paths; no claim that a commit must exist before it is created |
| `git status --porcelain -- .specify .github/skills/speckit-*` | Empty output: the Spec Kit installation and its bundled skills were not modified |
| `git diff --exit-code -- docs/assessment/technical-exercise-v5.md` | Exit code `0`; the assessment document is unchanged |

`[VERIFIED]` These are actual observed results from this session. `[PENDING]` Copilot skill discovery and automatic invocation, which have not been observed.

### Documentation work: engineering standards (`EV-0003`)

| Command or check | Observed result |
| --- | --- |
| Deadline verification | `2026-11-16` is a Monday; `08:00 -03:00`; approximately 13.8 hours remained when the documentation was written |
| Git history verification | `origin/main` contains `859b141`, `b3b6195`, `0439fbb`; `rev-list --left-right --count` returned `0 0`, so local and remote agree |
| Required sections in the nine standards | All nine contain purpose, mandatory rules, practical examples, prohibited practices, verification criteria, and references |
| Section numbering | Sequential in every standard after inserting the missing example sections |
| Internal links and anchors | 159 links checked, 0 broken; 1 internal anchor checked, valid |
| Cross-document rule consistency | 15 targeted assertions pass, including dependency direction, PostgreSQL integration tests, security test linkage, two-API requirement, and absence of extra infrastructure |
| Prohibited technologies | Entity Framework, EF Core, Dapper, MediatR, and Mediator appear only as prohibitions or assessment statements |
| Scope restrictions | No `.cs`, `.csproj`, `.sln`, `.ts`, `.tsx`, or `package.json` outside Spec Kit tooling; no tracked symbolic links; assessment and constitution unchanged (`git diff --exit-code` returned `0`) |
| Defects found and corrected | Two standards lacked the required practical-examples section; the index used a non-ASCII section sign with now-stale numbers |

`[VERIFIED]` These are actual observed results from this session. No application test was executed, because no application exists.

### Constitution ratification and Spec Kit integration (`EV-0006`)

| Command or check | Observed result |
| --- | --- |
| Extension hook check | `.specify/extensions.yml` absent; no hooks registered or executed |
| Constitution template resolution | `resolve-template.ps1 constitution-template -Json` succeeded and returned the unratified scaffold |
| Override resolution after creation | `plan-template override_active=True chars=6595`; `tasks-template override_active=True chars=7879`; `spec-template override_active=True chars=3836` |
| Constitution content | No placeholders remaining; six principles present; version 1.0.0, ratified 2026-11-16; no non-ASCII; no post-submission freeze in the body |
| Link and anchor scan | 242 links, 0 broken files, 0 broken anchors, after correcting 23 root-relative links |
| Skill enforcement markers | `speckit-tasks` contains "Tests are MANDATORY" and no longer contains "Tests are OPTIONAL"; analyze, implement, converge, and plan edits present |
| Installed templates | `git status --porcelain -- .specify/templates/*.md` empty; the overrides are additive |
| Scope | No application code, no project files, no tracked symbolic links |
| Defects found and corrected | 23 broken constitution links; one inline-code-wrapped markdown link; duplicate step numbering in the implement skill |

`[VERIFIED]` These are actual observed results from this session.

### Implementation plan and Phase 1 design (`EV-0008`)

| Command or check | Observed result |
| --- | --- |
| Template setup (`setup-plan.ps1 -Json`) | Copied plan template to `specs/001-task-manager/plan.md`; copied content identical to the project override |
| Toolchain observation | .NET SDKs 5.0.408, 8.0.428, 9.0.304, 10.0.404 (default 10.0.404); Node v25.9.0; npm 11.12.1; Docker 28.3.3; Compose v2.39.2 |
| Requirement traceability scan, first draft | 17 FRs and 6 NFRs declared, but 10 identifiers absent from the plan: FR-003, FR-004, FR-005, FR-008, FR-010, NFR-002, NFR-003, NFR-004, NFR-005, NFR-006 |
| Requirement traceability scan, after correction | 23 requirement IDs, 0 absent, 23 coverage rows |
| Artifact links | 17 links checked, 0 broken |
| Plan structure | All required sections present: Constitution Check with six principles, dependency matrix, SOLID table, seven test layers plus an architecture test, security and persistence design, GenAI capture points, complexity tracking, post-design re-check |
| Scope | No application code, solution, migration SQL, or test project created; constitution and original assessment unmodified |

`[VERIFIED]` These are actual observed results from this session. The first-draft traceability gap is
the notable one: the plan asserted complete requirement coverage that a scan disproved.

### Task generation (`EV-0009`)

| Command or check | Observed result |
| --- | --- |
| Template provenance (`setup-tasks.ps1 -Json`) | `TASKS_TEMPLATE` resolved to `.specify/templates/overrides/tasks-template.md`, 9,836 characters |
| First-list validation | 107 task lines, 33 missing a file path, duplicate identifier from an inserted `T005a`, 36 tasks with non-explicit RED dependencies |
| Final-list validation | 106 task lines, 0 format violations, 106 unique sequential identifiers, no gaps, no duplicates |
| Per-story counts | US1 13, US2 7, US3 13, US4 12, US5 11, US6 10, US7 6; 34 Setup/Foundational/Polish |
| RED lineage | 8 direct RED dependencies plus 24 inherited lineage statements; 0 implementation tasks without a documented RED checkpoint |
| Parallel markers | 34 `[P]` tasks, all test-authoring or independent verification; none crosses a test/implementation pair |

`[VERIFIED]` These are actual observed results from this session.

### API implementation

`[PENDING]` No build or test command has been executed for application code, because no solution, project, or test project exists. This section will record the actual `dotnet build` and `dotnet test` commands, their observed output, failures encountered, and how each failure was resolved.

### Governance corrections and feature specification (`EV-0007`)

| Command or check | Observed result |
| --- | --- |
| Extension hook check (`Test-Path .specify/extensions.yml`) | `False`; no hooks registered or executed |
| Effective tasks template after corrections | `Tests are OPTIONAL`: `False`; `(if requested)`: `False`; RED-first foundational rule present: `True` |
| Template resolver output versus override file | Identical; the override is the effective spec template |
| Residual conditional-wording scan | Remaining hits are historical verification statements in `GENAI.md` and `EV-0006`, and the superseded installed core template |
| Specification validation, iteration 1 | Found three defects: a literal clarification marker token in prose, undefined `EC`/`DO` identifiers, and TDD mapped without being named |
| Specification validation, iteration 2 | Placeholders absent; referenced-but-undefined IDs: none; `FR` 17, `NFR` 6, `SC` 8, `PDC` 8, `EC` 5, `DO` 5 all defined |
| Specification link targets | Both assessment links resolve |
| Link resolution across all changed documents | 109 live links checked with fenced code excluded, 0 broken; the two apparent hits in `EV-0006` are quoted defect evidence inside code fences |
| Final semantic review | One further defect found and corrected: `FR-003` was mapped to US7 only, although US1's first scenario asserts user persistence |
| Scope verification | 9 tracked files modified, 2 governance/spec paths added; the constitution and the original assessment are unmodified; no application code and no tracked symbolic links |

`[VERIFIED]` These are actual observed results from this session. No application test was executed, because no application exists.

## 9. Engineering Decisions and Lessons Learned

`[AI-PROPOSED]` Decisions embodied in this work, pending final developer approval recorded in the evidence records:

- Separate the AI proposal, the developer decision, and the verified result, using explicit labels in every record.
- Gate status transitions on evidence: a record advances from `Captured` to `Reviewed` to `Verified` only when the corresponding evidence exists.
- Keep one record per meaningful activity or related task group, so a single activity is not fragmented across files.
- Make the evidence workflow enforceable through repository instructions and a skill, rather than relying on convention.
- Treat the documentation work as its own record, distinct from the assessed API exercise.
- Scale evidence to the activity type and mark inapplicable requirements `Not Applicable`, so the workflow stays practical for a small assessment.
- Record commit references only after the commit exists, and track capture, review, validation, and committed state separately.

`[DEVELOPER DECISION]` The workflow order was corrected on review so that analysis of the specification, plan, tasks, and constitutional compliance precedes implementation, with post-implementation analysis kept as an optional repeat.

Observations from this work:

- `[VERIFIED]` Three defects were found by executed checks rather than by inspection: four broken relative links in the Copilot instructions, a provenance mislabeling where unreviewed AI choices were marked as developer decisions, and an evidence record that asserted a wording change that never occurred. All three are recorded with their corrections rather than removed.
- `[VERIFIED]` The first version of the standard placed analysis after implementation and understated the repository state by implying Spec Kit was not installed. Both were corrected on developer review; see `EV-0002`.
- `[VERIFIED]` Evidence discipline is easier to satisfy when validation is automated. Each defect above surfaced from a script or a review, not from reading the documents again.
- `[VERIFIED]` On 2026-11-16 the same pattern repeated at a larger scale: all three specification defects were found by scripted checks (placeholder scan, identifier definition check, keyword coverage scan), not by re-reading the generated document. Writing an identifier in a traceability table does not mean it exists.
- `[VERIFIED]` A governance rule expressed in a template can still leak through the skill that generates the artifact, because the skill is the instruction source. Correcting only the template would have left "tests (if requested)" in the effective workflow.
- `[VERIFIED]` The same traceability failure recurred at the plan phase: the plan claimed full requirement coverage while ten requirement identifiers were absent. The lesson is that a coverage claim is only credible when a mechanical scan can enumerate every requirement and point to where it is verified.
- `[PENDING]` That Copilot automatically selects the `genai-evidence` skill has not been observed and is not claimed. Per vendor documentation, skill selection is advisory; the repository instructions exist partly as a backstop, but this has not been tested here. Whether the directory name must equal the frontmatter `name` is likewise documented behavior, not a locally verified finding.

`[PENDING]` Lessons from implementing and testing the API, and from reviewing generated code.

## 10. Supporting Evidence

| Evidence | Location | Status |
| --- | --- | --- |
| `EV-0001` - repository GenAI standards and evidence workflow | [docs/evidence/genai/EV-0001-repository-genai-standards.md](docs/evidence/genai/EV-0001-repository-genai-standards.md) | `Verified` - committed in `b3b6195`, `0439fbb` |
| `EV-0002` - refinement per developer review | [docs/evidence/genai/EV-0002-standard-refinement.md](docs/evidence/genai/EV-0002-standard-refinement.md) | `Verified` - committed in `b3b6195`, `0439fbb` |
| `EV-0003` - engineering standards and submission requirements | [docs/evidence/genai/EV-0003-engineering-standards.md](docs/evidence/genai/EV-0003-engineering-standards.md) | `Verified` - committed in `9906b6a` |
| `EV-0004` - TDD enforcement lifecycle across Spec Kit phases | [docs/evidence/genai/EV-0004-tdd-enforcement-lifecycle.md](docs/evidence/genai/EV-0004-tdd-enforcement-lifecycle.md) | `Verified` - committed in `ba92121` |
| `EV-0005` - separation of engineering governance and submission process rules | [docs/evidence/genai/EV-0005-governance-separation.md](docs/evidence/genai/EV-0005-governance-separation.md) | `Verified` - committed in `f59bc6b` |
| `EV-0006` - constitution ratification and Spec Kit integration | [docs/evidence/genai/EV-0006-constitution-ratification.md](docs/evidence/genai/EV-0006-constitution-ratification.md) | `Reviewed` - committed in `ac604af`; seven review findings applied under `EV-0007` |
| `EV-0007` - targeted governance corrections and feature specification `001-task-manager` | [docs/evidence/genai/EV-0007-specification-001-task-manager.md](docs/evidence/genai/EV-0007-specification-001-task-manager.md) | `Captured` - committed in `3e8d0c9`; developer review `PENDING` |
| `EV-0008` - implementation plan, research, and Phase 1 design | [docs/evidence/genai/EV-0008-implementation-plan-001-task-manager.md](docs/evidence/genai/EV-0008-implementation-plan-001-task-manager.md) | `Captured` - developer review `PENDING` |
| `EV-0009` - task generation with RED-first ordering | [docs/evidence/genai/EV-0009-task-generation-001-task-manager.md](docs/evidence/genai/EV-0009-task-generation-001-task-manager.md) | `Captured` - developer review `PENDING` |
| Generative AI engineering standard | [docs/engineering/genai.md](docs/engineering/genai.md) | `Verified` - committed |
| Engineering standards index and nine standards | [docs/engineering/README.md](docs/engineering/README.md) | Established and accepted as the project foundation on 2026-11-16; targeted corrections recorded in `EV-0007` |
| Submission requirements | [docs/assessment/submission-requirements.md](docs/assessment/submission-requirements.md) | Recorded and accepted; constitution status synchronized 2026-11-16 |
| Evidence skill | [.github/skills/genai-evidence/SKILL.md](.github/skills/genai-evidence/SKILL.md) | `Verified` - committed; discovery and invocation unverified |
| Evidence index and template | [docs/evidence/genai/README.md](docs/evidence/genai/README.md), [template.md](docs/evidence/genai/template.md) | `Verified` - committed |
| Repository Copilot instructions | [.github/copilot-instructions.md](.github/copilot-instructions.md) | Updated with the standards index; review `PENDING` |
| Spec Kit installation | `.specify/init-options.json`, `.specify/integration.json` | Installed and configured, version 1.4.0, Copilot integration |
| Constitution | `.specify/memory/constitution.md` | **Ratified** - version 1.0.0, 2026-11-16; committed in `ac604af` |
| Feature specification `001-task-manager` | [specs/001-task-manager/spec.md](specs/001-task-manager/spec.md) | Draft - developer review `PENDING` |
| Assessment document | [docs/assessment/technical-exercise-v5.md](docs/assessment/technical-exercise-v5.md) | Unmodified |
| Git history | `859b141` foundation, `b3b6195` standards and workflow, `0439fbb` commit references, `9906b6a` standards, `ba92121` TDD lifecycle, `fd309c7` evidence reference, `f59bc6b` governance separation, `0018be0` evidence reference, `ac604af` constitution v1.0.0; all present on `origin/main` | `[VERIFIED]` |

### Coverage of the assessment's Generative AI questions

| Assessment question | Where it will be answered | Status |
| --- | --- | --- |
| Write the prompt used to generate the API scaffold or implementation | Section 3 and the corresponding evidence record | `PENDING` |
| Show the output code or a representative sample | Section 4 and the corresponding evidence record | `PENDING` |
| Describe how AI suggestions were validated | Section 5 | `PENDING` for the API exercise |
| Describe how output was corrected or improved | Section 6 | `PENDING` for the API exercise |
| Describe how edge cases, authentication, and validations were handled | Section 7 | `PENDING` |
| Demonstrate fluency with AI tools and prompt engineering | Sections 1, 5, 9 and the evidence records | `PENDING` |
