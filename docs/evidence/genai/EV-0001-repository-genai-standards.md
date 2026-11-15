# EV-0001 - Repository GenAI engineering standard and evidence workflow

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0001` |
| Date | `2026-11-14` |
| Development phase | Documentation and configuration, pre-specification. Spec Kit 1.4.0 is installed and configured, but no Spec Kit command was run for this activity: no feature specification, plan, or task list exists, and the constitution remains the unratified template. |
| Activity type | Documentation and specification |
| Related specification | `PENDING` - no feature specification exists. Input was [docs/assessment/technical-exercise-v5.md](../../assessment/technical-exercise-v5.md), Generative AI tools section. |
| Related task identifiers | `PENDING` - no Spec Kit task list exists. The work is identified by the six numbered items in the prompt below. |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session and is not recorded rather than guessed. |
| Evidence status | `Reviewed` |
| Commit references | `PENDING` - this work was not committed at the time of review; the reference is recorded after the commit exists. |

## Actual prompt

Verbatim developer prompt for this activity:

```text
We are establishing the first engineering standard for the Ballast Lane Technical Interview Exercise V5.

Repository:
[https://github.com/diogocurto2/ballast-lane-tech-exercise-v5](https://github.com/diogocurto2/ballast-lane-tech-exercise-v5)

Read the original assessment at:
docs/assessment/technical-exercise-v5.md

Pay special attention to the Generative AI tools section and its mandatory requirements.

We will use GitHub Copilot and GitHub Spec Kit throughout the development of this assessment. The project must demonstrate evidence-based AI-assisted software engineering.

Perform the following documentation and configuration tasks only.

## 1. Create the Generative AI engineering standard
Create:

docs/engineering/genai.md

Define the mandatory workflow for AI-assisted development, including:

- Specification-driven development using GitHub Spec Kit.
- GitHub Copilot as the primary AI coding tool.
- Capture of actual prompts used during relevant development activities.
- References to specifications, requirements, and engineering standards supplied as context.
- Preservation of representative original AI-generated code before review and corrections.
- Critical evaluation of AI suggestions by the developer.
- Validation against Clean Architecture, SOLID, TDD, security, and functional requirements.
- Actual build and test commands, observed results, and failures.
- Corrections and improvements when necessary, with technical justification.
- Traceability to source files, test files, task identifiers, and Git commits.
- Clear distinction between AI suggestions, developer decisions, and verified results.
Do not invent prompts, generated output, defects, corrections, test executions, approvals, or results.

Record concise technical rationales and observed evidence, not private model reasoning.

## 2. Create the GitHub Copilot skill
Create:

.github/skills/genai-evidence/SKILL.md

Follow the official GitHub Copilot agent skill format.

Use the skill name `genai-evidence` and a description that makes it relevant to specification, planning, implementation, testing, review, and documentation activities.

The skill must define three checkpoints:

### Before execution

- Identify the current Spec Kit phase, task, or development activity.
- Record the actual user-provided prompt and relevant context.
- Identify applicable requirements and engineering standards.
- Create or open the corresponding evidence record.

### During execution

- Preserve meaningful AI-generated artifacts.
- For code generation, capture a representative original code excerpt or diff before corrections.
- Record actual commands and observed results.
- Preserve TDD evidence when applicable.
- Record significant design alternatives and decisions without inventing information.

### Before task completion

- Record actual review findings and developer decisions.
- Document corrections, improvements, or justified acceptance.
- Reference the final implementation and relevant tests.
- Update the root GENAI.md with relevant verified evidence.
- Mark unavailable or incomplete evidence as pending.
- Report missing evidence as a completion blocker when required.
Do not allow retrospective fabrication of original generated code or prompts.

If the exact original prompt is unavailable, request it from the developer or explicitly record the evidence as unavailable.

## 3. Create the evidence storage structure
Create:

docs/evidence/genai/README.md
docs/evidence/genai/template.md

The evidence template must support:

- Evidence identifier.
- Date and development phase.
- Related specification and task identifiers.
- AI tool and model, if known.
- Actual prompt.
- Relevant context.
- Original generated output.
- Validation performed.
- Review findings.
- Developer decisions and approval status.
- Corrections and improvements.
- Actual test commands and observed results.
- Final code and commit references.
- Evidence status: Pending, Captured, Reviewed, or Verified.
Use one evidence record per meaningful development activity or related group of tasks.

Do not record secrets, credentials, private information, or irrelevant chat transcripts.

## 4. Create the root GENAI.md
Create an initial GENAI.md in English at the repository root.

It must be a living assessment response, not a generic AI policy or a complete chat transcript.

Use the following structure:

1. AI Tool and Development Approach
2. Task Management REST API — Assessment Exercise
3. Actual Prompt
4. Original AI-Generated Code
5. Validation of AI Suggestions
6. Corrections and Improvements
7. Authentication, Authorization, Validation, and Edge Cases
8. Testing and Verification Results
9. Engineering Decisions and Lessons Learned
10. Supporting Evidence
Include an explicit statement that the API implementation exercise and its evidence are pending at this stage.

Do not present planned work as completed work.

The document must eventually answer every Generative AI question from the original technical assessment.

## 5. Configure repository-wide Copilot instructions
Create or update:

.github/copilot-instructions.md

Keep the instructions concise.

Require Copilot to:

- Read the original assessment before making relevant project decisions.
- Follow docs/engineering/genai.md.
- Use the genai-evidence skill for every meaningful specification, planning, coding, testing, review, or documentation task.
- Capture original AI-generated artifacts before subsequent corrections.
- Update the relevant evidence record and GENAI.md before concluding the task.
- Never fabricate evidence.
- Stop and report missing mandatory evidence instead of falsely marking a task complete.
Record actual observed evidence from this documentation task as the first project evidence record, but clearly distinguish it from the future mandatory REST API generation exercise.

## 6. Verify the configuration
Verify that:

- All files use English.
- The skill has valid YAML frontmatter and a matching directory name.
- The evidence workflow is consistent across all documents.
- GENAI.md exists at the repository root.
- No evidence is presented as completed unless it was actually observed.
- The original assessment remains unchanged.
Do not install Spec Kit, create the constitution, generate application code, or define application architecture yet.

Return:

- Files created or modified.
- A summary of the GenAI evidence lifecycle.
- How Copilot discovers and invokes the skill.
- Any limitations of automatic skill execution.
- The first actual evidence captured.
- Remaining steps for integrating this workflow into the future constitution and Spec Kit templates.
Stop after completing this documentation task.
```

## Relevant context

- Requirement or assessment section: [technical-exercise-v5.md](../../assessment/technical-exercise-v5.md), "Generative AI tools", "Presentation and Code Review".
- Engineering standard applied: [docs/engineering/genai.md](../../engineering/genai.md), created in this activity.
- Files supplied or read as context: `docs/assessment/technical-exercise-v5.md`; existing skill format in `.github/skills/speckit-plan/SKILL.md`; `.specify/memory/constitution.md` (read to confirm it is still the unmodified template); repository state via `git status` and `git log`.
- External reference consulted: VS Code "Agent Skills" documentation (`https://code.visualstudio.com/docs/agent-customization/agent-skills`) for the `SKILL.md` frontmatter contract.
- Scope constraints from the prompt: no Spec Kit installation, no constitution, no application code, no architecture, no version decisions.

## Original AI-generated output

Representative excerpt of the generated skill header, before review:

```yaml
---
name: genai-evidence
description: "Capture and maintain evidence of AI-assisted engineering for the Ballast Lane Technical Interview Exercise V5. Use for every meaningful specification, planning, implementation, testing, review, or documentation task: record the actual prompt and supplied context, preserve original AI-generated artifacts before corrections, log actual commands and observed results, document review findings and developer decisions, and update the matching evidence record plus GENAI.md. Use when starting or finishing Spec Kit phases or tasks, generating or correcting code, running tests, reviewing AI output, or writing project documentation. Never fabricate prompts, output, defects, corrections, or results."
compatibility: "Requires docs/engineering/genai.md, docs/evidence/genai/template.md, and GENAI.md at the repository root"
metadata:
  version: "1.0.0"
  source: "docs/engineering/genai.md"
---
```

Representative excerpt of the generated `copilot-instructions.md` section that was later corrected:

```markdown
## Required reading

- Read [docs/assessment/technical-exercise-v5.md](docs/assessment/technical-exercise-v5.md) before making relevant project decisions.
- Read the project constitution and the active specification, plan, and tasks in `specs/` before implementation work.
```

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | No application code was produced, so no dependency direction or layering to check. | `Not Applicable` |
| SOLID | No application code was produced, so no class or interface design to check. | `Not Applicable` |
| TDD | No test project exists; this activity produced documentation only. | `Not Applicable` |
| Security | Documents reviewed for secrets, credentials, and personal data. None present. Evidence rules prohibit storing them. | `[VERIFIED]` |
| Functional requirements | Every item in the six numbered prompt tasks checked against the created files; YAML frontmatter parsed; relative links resolved; files confirmed ASCII-only (English). | `[VERIFIED]` with one defect found and fixed |

## Review findings

- `[VERIFIED]` Skill frontmatter parses as YAML with keys `name`, `description`, `compatibility`, `metadata`. `name` is `genai-evidence`, which matches the directory `.github/skills/genai-evidence`, and satisfies the lowercase/hyphen/64-character rule.
- `[VERIFIED]` Skill description length is 696 characters, within the 1024-character maximum, and states both capability and use cases.
- `[VERIFIED]` All five created files contain no non-ASCII characters.
- `[VERIFIED]` Defect: in `.github/copilot-instructions.md`, four Markdown links were written repository-root-relative (`docs/...`, `GENAI.md`) instead of file-relative, so they resolved to `.github/docs/...` and `.github/GENAI.md`. Detected by automated relative-link resolution.
- `[VERIFIED]` Links to `GENAI.md` and `EV-0001` were unresolvable during the first scan because those files did not exist yet; re-validated after creation.
- `[VERIFIED]` `git status` and `git diff` confirmed the assessment document was not modified.
- `[AI-PROPOSED]` The skill is scoped to meaningful activities only, so trivial read-only lookups do not generate noise records.
- `[AI-PROPOSED]` Design choices in GENAI.md and the standard are labeled `[AI-PROPOSED]` rather than `[DEVELOPER DECISION]`, because no developer approval had been recorded when the label was written.
- `[VERIFIED]` Developer review received on `2026-11-14`, returned as six numbered correction items. Its scope covers the lifecycle order, the Spec Kit status, evidence proportionality, circular commit requirements, public-repository hygiene, and final consistency. The findings and their resolutions are recorded in [EV-0002](./EV-0002-standard-refinement.md).
- `[VERIFIED]` Developer review finding: this record understated the repository state by implying Spec Kit was not installed. `.specify/init-options.json` records `speckit_version` `1.4.0` and `.specify/integration.json` records version `1.4.0` with the `copilot` integration installed. Corrected in this record and in the standard.
- `[PENDING]` Developer approval. The developer approved the initial structure as a proposal and requested corrections; approval of the corrected artifacts has not been granted and is not implied by the review above.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Adopt provenance labels `[AI-PROPOSED]`, `[DEVELOPER DECISION]`, `[VERIFIED]`, `[PENDING]` | Keeps AI proposals, human decisions, and executed checks distinguishable in every record and in GENAI.md | `Pending` |
| Lifecycle `Pending` -> `Captured` -> `Reviewed` -> `Verified` | Requires evidence to exist before status advances; prevents unverified claims | `Pending` |
| One record per meaningful activity or related task group | Matches the template requirement and avoids fragmentation of a single activity | `Pending` |
| Store the skill in `.github/skills/genai-evidence/` | Copilot project skill discovery path; directory name must equal the skill `name` | `Pending` |
| Record the documentation task as `EV-0001`, separate from the future API exercise | The prompt requires distinguishing existing evidence from the mandatory API generation exercise | `Pending` |

`[DEVELOPER DECISION]` The developer reviewed the initial structure and approved it as a proposal, subject to the six corrections tracked in `EV-0002`. This is a review outcome, not final approval.

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Four links in `.github/copilot-instructions.md` changed from root-relative to file-relative (`../docs/...`, `../GENAI.md`) | Link scan reported broken targets | Markdown relative links resolve against the containing file's directory, not the repository root; editor and GitHub navigation require correct targets |
| Constitution reference reworded to "when present, the active specification, plan, and tasks under `specs/`" | No specification or task list exists yet | Avoids implying artifacts that do not exist, consistent with the no-fabrication rule |
| Design choices in GENAI.md and the review findings were relabeled from `[DEVELOPER DECISION]` to `[AI-PROPOSED]` | No developer approval existed when the labels were written | The standard defines `[DEVELOPER DECISION]` as a choice made by the developer; labeling unreviewed AI choices that way would overstate their provenance |
| "Spec Kit was not used and the project constitution does not exist yet" replaced with the installed version and the unratified-template state | The statement was factually wrong | `init-options.json` and `integration.json` record Spec Kit 1.4.0 with the Copilot integration; the constitution file exists as the original template |

## Commands and observed results

```text
python -c "import yaml, sys; print('pyyaml', yaml.__version__)"
```

```text
pyyaml 6.0.3
```

```text
@'
<validation script: YAML frontmatter parse, ASCII check, relative-link resolution>
'@ | python -
```

```text
frontmatter block found: True
yaml parsed keys: ['compatibility', 'description', 'metadata', 'name']
name: genai-evidence | matches dir: True
name pattern ok: True
description length: 696 (max 1024): True

-- ASCII / English check --
.github/skills/genai-evidence/SKILL.md exists | bytes: 4839 | non-ascii: none
.github/copilot-instructions.md exists | bytes: 1587 | non-ascii: none
docs/engineering/genai.md exists | bytes: 8849 | non-ascii: none
docs/evidence/genai/README.md exists | bytes: 2699 | non-ascii: none
docs/evidence/genai/template.md exists | bytes: 1931 | non-ascii: none

-- relative link targets --
BROKEN .github/skills/genai-evidence/SKILL.md -> ../../../GENAI.md
BROKEN .github/copilot-instructions.md -> docs/assessment/technical-exercise-v5.md
BROKEN .github/copilot-instructions.md -> docs/engineering/genai.md
BROKEN .github/copilot-instructions.md -> GENAI.md
BROKEN .github/copilot-instructions.md -> GENAI.md
BROKEN docs/engineering/genai.md -> ../../GENAI.md
BROKEN docs/evidence/genai/README.md -> ../../../GENAI.md
BROKEN docs/evidence/genai/README.md -> ./EV-0001-repository-genai-standards.md
link scan done
```

```text
git status --short
git diff -- docs/assessment/technical-exercise-v5.md
```

```text
?? .github/copilot-instructions.md
?? .github/skills/genai-evidence/
?? docs/engineering/
?? docs/evidence/

(no output from git diff: assessment unchanged)
```

Final re-validation after all corrections, with fenced code blocks excluded from the link scan:

```text
@'
<validation script: prose-only relative-link scan, frontmatter, non-ASCII check>
'@ | python -
```

```text
prose links checked: 40 | broken: 0
frontmatter ok: True | desc chars: 696
GENAI.md                                                          prose non-ascii=['0x2014']
.github/copilot-instructions.md                                   prose non-ascii=none
.github/skills/genai-evidence/SKILL.md                            prose non-ascii=none
docs/engineering/genai.md                                         prose non-ascii=none
docs/evidence/genai/README.md                                     prose non-ascii=none
docs/evidence/genai/template.md                                   prose non-ascii=none
docs/evidence/genai/EV-0001-repository-genai-standards.md         prose non-ascii=none
```

The single non-ASCII character is an em dash (U+2014) in the GENAI.md heading that reproduces the section title requested in the prompt. It is typographic, not non-English text, and is retained so the required section name matches the request exactly. The earlier report of one broken link was a false positive: the target was inside a fenced block that deliberately preserves the original defective excerpt.

## Final references

- Standard: [docs/engineering/genai.md](../../engineering/genai.md)
- Skill: [.github/skills/genai-evidence/SKILL.md](../../../.github/skills/genai-evidence/SKILL.md)
- Evidence index: [docs/evidence/genai/README.md](./README.md)
- Template: [docs/evidence/genai/template.md](./template.md)
- Copilot instructions: [.github/copilot-instructions.md](../../../.github/copilot-instructions.md)
- Assessment response: [GENAI.md](../../../GENAI.md)
- Test files: `Not Applicable` - no test project exists
- Task identifiers: `PENDING` - no Spec Kit task list exists
- Commits: foundation commit `859b141` (pre-existing); commit for this activity `PENDING` - recorded after the commit exists

## Evidence status rationale

Status is `Reviewed`: the prompt, context, generated artifacts, validation results, and confirmed defects with their corrections are recorded, and on `2026-11-14` the developer reviewed the artifacts and returned six specific correction items, which are tracked in `EV-0002`.

It is not `Verified`: developer approval of the corrected artifacts has not been granted, and no commit exists for this activity. Both states are tracked separately rather than collapsed into one status.
