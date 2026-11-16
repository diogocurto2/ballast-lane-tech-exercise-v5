# Generative AI Engineering Standard

Mandatory engineering standard for AI-assisted development of the Ballast Lane Technical Interview Exercise V5.

| Reference | Location |
| --- | --- |
| Assessment requirements | [docs/assessment/technical-exercise-v5.md](../assessment/technical-exercise-v5.md) |
| Assessment response (living document) | [GENAI.md](../../GENAI.md) |
| Evidence records | [docs/evidence/genai/](../evidence/genai/README.md) |
| Evidence template | [docs/evidence/genai/template.md](../evidence/genai/template.md) |
| Copilot evidence skill | [.github/skills/genai-evidence/SKILL.md](../../.github/skills/genai-evidence/SKILL.md) |
| Repository-wide Copilot instructions | [.github/copilot-instructions.md](../../.github/copilot-instructions.md) |

## 1. Purpose and scope

This standard defines how AI assistance is used and evidenced in this repository. It applies to every specification, plan, task breakdown, code change, test, review, and documentation update produced with AI assistance.

The assessment requires the candidate to show, for a REST API generation exercise:

1. the prompt written to generate the scaffold or full implementation;
2. the output code, or a representative sample of it;
3. how AI suggestions were validated;
4. how the output was corrected or improved, if necessary;
5. how edge cases, authentication, and validations were handled;
6. fluency with AI tools and prompt engineering, including critical thinking when evaluating AI-generated code.

This standard turns those requirements into a repeatable workflow with auditable evidence. Evidence must be observed, never reconstructed from memory. Do not invent prompts, generated output, defects, corrections, test executions, approvals, or results.

## 2. Tooling

- **GitHub Spec Kit 1.4.0** is installed and configured in this repository with the Copilot integration, PowerShell scripts, and sequential feature numbering. The configuration is recorded in `.specify/init-options.json` and `.specify/integration.json`; the installed skills and templates are listed in `.specify/integrations/`.
- **GitHub Copilot** is the primary AI coding tool. It operates under the repository-wide instructions in `.github/copilot-instructions.md` and the `genai-evidence` skill.
- **Evidence records** are stored as Markdown under `docs/evidence/genai/`, one record per meaningful activity or related group of tasks.
- **GENAI.md** at the repository root is the consolidated assessment response. It summarizes verified evidence from the records.

**Current state of the Spec Kit artifacts:** the constitution at `.specify/memory/constitution.md` is **ratified** at version 1.0.0 (2026-11-16) and committed in `ac604af`. The plan, tasks, and spec templates are overridden in `.specify/templates/overrides/` to enforce it. The first feature specification, `001-task-manager`, is drafted under `specs/001-task-manager/`; no implementation plan, task list, or application code exists yet.

## 3. Mandatory workflow

The order below is mandatory. Analysis of the specification, plan, tasks, and constitutional compliance happens **before** implementation. The analysis may be repeated after implementation when it is useful, but the pre-implementation analysis cannot be skipped.

Each phase produces artifacts and evidence. A phase is complete only when its evidence exists and is consistent with the artifacts.

### Phase 1 - Constitution and engineering standards

- Ratify the project constitution and maintain the engineering standards that govern the work, including this document.
- Treat the constitution as the source of non-negotiable principles for every later phase.

**Evidence:** prompt, ratified text or standards change, review findings. The constitution is ratified; a record of a standards or governance change must distinguish reviewer findings from the developer's approval rather than presenting review as approval.

### Phase 2 - Specify and clarify

- Use Spec Kit (`/speckit-specify`, with `/speckit-clarify` and `/speckit-checklist` as needed) to produce or refine the feature specification.
- Keep requirements testable and traceable to assessment requirements.

**Evidence:** prompts used, specification changes accepted or rejected, developer decisions.

### Phase 3 - Plan

- Use `/speckit-plan` to produce design artifacts consistent with Clean Architecture and SOLID.
- Record significant alternatives considered and the rationale for the chosen approach.

**Evidence:** prompts, design alternatives, decisions, rejected options with technical reasons.

### Phase 4 - Tasks

- Use `/speckit-tasks` to produce a dependency-ordered task list.
- Assign identifiers so code, tests, and commits can reference tasks.

**Evidence:** task identifiers created or changed, mapping to requirements.

### Phase 5 - Analyze (mandatory, before implementation)

- Run `/speckit-analyze` across the specification, plan, task list, and compliance with the constitution.
- Resolve inconsistencies, gaps, and coverage problems before writing implementation code, or record an explicit deferral with its reason.
- Repeat this analysis after implementation when it adds value; the repetition is optional, the pre-implementation analysis is not.

**Evidence:** findings, resolutions, and justified deferrals.

### Phase 6 - Implement using TDD and GenAI evidence checkpoints

- Use `/speckit-implement` and GitHub Copilot for code and tests.
- Follow Test-Driven Development: write the failing test, confirm the failure, then implement.
- Apply the three `genai-evidence` checkpoints: before execution, during execution, and before completion.
- Preserve a representative original AI-generated excerpt or diff **before** any correction.

**Evidence:** original generated code, TDD evidence, actual commands and observed results including failures, review findings, corrections.

### Phase 7 - Review, validate, and converge

- Use `/speckit-converge`, and `/speckit-analyze` again when useful, to close remaining gaps.
- Review AI suggestions against:

- **Clean Architecture:** dependency direction, separation of concerns, layer independence.
- **SOLID:** single responsibility, open/closed, Liskov substitution, interface segregation, dependency inversion.
- **TDD:** tests exist for the behavior, tests fail without the change, tests are meaningful.
- **Security:** authentication, authorization, input validation, secret handling, and no credential leakage.
- **Functional requirements:** behavior matches the specification and the assessment.

**Evidence:** review findings, developer decisions, corrections with technical justification.

### Phase 8 - Conclude and update documentation

- Record actual build and test commands with observed results, including failures and the resolution.
- Reference final implementation files, test files, and task identifiers.
- Record commit references once the relevant commits exist; a commit cannot be required before it is created.
- Update the matching evidence record and the relevant GENAI.md sections.
- Mark unavailable or incomplete evidence as `PENDING` and report it as a completion blocker when the requirement is mandatory.

## 4. Evidence capture rules

### Proportionality

Evidence must remain practical for a small technical assessment.

- Record evidence **per meaningful development activity**, or per related group of tasks. Do not create a record per command, per file read, or per trivial edit.
- Capture the **relevant prompts**, the **generated artifacts**, the **important validation commands and their observed results**, the **review findings**, the **corrections**, and the **engineering decisions**.
- Do not require exhaustive logs of routine file reads, exploratory lookups, or trivial shell operations.

### Evidence by activity type

| Activity type | Required evidence |
| --- | --- |
| Documentation and specification | Actual prompt, generated document or a representative excerpt, review findings, relevant consistency checks |
| Code generation | Original generated code before correction, TDD evidence (failing test, failure output, passing result), review findings, corrections, validation commands and results |
| Testing | Actual commands, observed results, failures and how they were resolved |
| Review | Concrete findings, decisions, and verified corrections |

Requirements that do not apply to a given activity are marked `Not Applicable` with a one-line justification. A missing requirement is never silently omitted.

### Fidelity, redaction, and paths

- Record the **actual prompt** as provided by the developer. If the exact prompt is unavailable, request it or record the evidence as unavailable; never paraphrase a prompt into something that appears verbatim.
- Preserve **original AI-generated artifacts** before review. Corrections are recorded alongside, not in place of, the original.
- If sensitive information must be removed, state explicitly that the content was redacted and what kind of information was removed. Redacted content is never presented as a verbatim record.
- Never record secrets, credentials, tokens, connection strings, personal data, or unrelated conversation.
- Use repository-relative paths in published references. Machine-specific absolute paths are not recorded.
- Keep records **concise and technical**. Record rationales and observed evidence, not private model reasoning or full chat transcripts.

### Commit references

- Evidence is captured and reviewed **before** the associated task is completed.
- A commit reference is recorded **after** the commit that contains the evidence exists. The absence of a commit hash before that point is not a completion blocker.
- Keep these states distinct and accurate: evidence captured, developer review, observed validation, committed artifacts. Never record a commit reference or a developer approval that does not exist.

## 5. Provenance labels

Evidence and GENAI.md use these labels so AI output, human judgment, and observed facts are never conflated:

| Label | Meaning |
| --- | --- |
| `[AI-PROPOSED]` | Produced by the AI tool and not yet accepted, verified, or corrected. |
| `[DEVELOPER DECISION]` | A choice made by the developer, with its technical rationale. |
| `[VERIFIED]` | Confirmed by an executed command, test run, or direct observation. |
| `[PENDING]` | Required evidence that does not exist yet. |

An item cannot be labeled `[VERIFIED]` without a corresponding executed check.

## 6. Evidence lifecycle

| Status | Meaning |
| --- | --- |
| `Pending` | Record identified but not yet populated. |
| `Captured` | Prompt, context, and artifacts recorded; validation performed where applicable; developer review not yet complete. |
| `Reviewed` | Developer has evaluated findings and decisions; corrections documented. |
| `Verified` | Final implementation, tests, and observed validation confirmed; commit references recorded once the commits exist. |

The status tracks the record, not the repository. Capture, developer review, observed validation, and committed artifacts are separate facts: a record can be `Reviewed` while its commit reference is still absent, and a commit reference can only be recorded after the commit exists. Only `Verified` entries may be summarized in GENAI.md as completed outcomes.

## 7. Prohibited practices

- Fabricating or back-filling prompts, generated output, defects, corrections, test runs, approvals, or metrics.
- Recording a commit reference or a developer approval that does not exist.
- Presenting planned, drafted, or pending work as completed.
- Copying AI output into the repository without review and test evidence.
- Marking a task complete while mandatory evidence is missing.
- Modifying the original assessment document.
- Storing secrets, credentials, or personal information in evidence records.

## 8. Completion gate

A task is complete only when all of the following hold:

1. The evidence record exists and its status is accurate.
2. Original AI-generated artifacts are preserved where code was generated.
3. Actual commands and observed results are recorded, or marked `Not Applicable` with a justification.
4. Review findings and developer decisions are documented.
5. Corrections are justified, or acceptance of AI output is justified.
6. Final code, tests, and task identifiers are traceable.
7. Commit references are recorded once the relevant commits exist; their absence before that point is not a blocker.
8. GENAI.md reflects only verified outcomes.

If any mandatory item is missing, report it as a blocker instead of completing the task.

## 9. Current status

This standard is adopted as the working standard for this repository and remains subject to developer approval and commit.

- GitHub Spec Kit 1.4.0 is installed and configured with the Copilot integration.
- The constitution exists at `.specify/memory/constitution.md` as the original template and has not been ratified.
- No feature specification, implementation plan, or task list exists yet.
- No application code or architecture has been generated.
- The first evidence record, `EV-0001`, documents the creation of this standard and is at status `Reviewed`. `EV-0002` documents this refinement, required by developer review of `EV-0001`, and is at status `Captured`.
