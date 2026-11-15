# EV-0002 - Refinement of the GenAI standard per developer review

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0002` |
| Date | `2026-11-14` |
| Development phase | Documentation and configuration, pre-specification. Constitution remains the unratified template; no specification, plan, or task list exists. |
| Activity type | Documentation and specification (review-driven correction) |
| Related specification | `PENDING` - no feature specification exists. Input was the developer review of `EV-0001`. |
| Related task identifiers | `PENDING` - no Spec Kit task list exists. The work is identified by the six numbered items in the prompt below. |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session. |
| Evidence status | `Captured` |
| Commit references | `b3b6195` - "docs: establish GenAI engineering standard and evidence workflow". Recorded after the commit was created. |

## Actual prompt

Verbatim developer prompt for this activity:

```text
Review and refine the GenAI engineering standard and evidence workflow based on the following developer feedback.

The initial structure is approved as a proposal. Apply the following corrections before we approve and commit it.

## 1. Correct the Spec Kit lifecycle
The first Spec Kit analysis must occur BEFORE implementation.

Define the standard workflow as:

1. Constitution and engineering standards.
2. Specify and clarify.
3. Plan.
4. Tasks.
5. Analyze specification, plan, tasks, and constitutional compliance.
6. Implement using TDD and GenAI evidence checkpoints.
7. Review, validate, and converge.
8. Conclude and update documentation.
The analyze step may be repeated after implementation when useful, but the pre-implementation analysis is mandatory.

## 2. Correct the repository status
GitHub Spec Kit 1.4.0 is already installed and configured in this repository.

Inspect .specify/init-options.json and .specify/integration.json.

The existing constitution is still the original template and has not been ratified.

Do not claim that Spec Kit is not installed. Correct any inaccurate statements in the engineering standard, GENAI.md, evidence record, or related documentation.

Do not modify the existing Spec Kit skills or templates yet.

## 3. Keep evidence collection proportional
The GenAI evidence workflow must remain practical for a small technical assessment.

Record evidence per meaningful development activity or related group of tasks, not per individual command or trivial edit.

Capture relevant prompts, generated artifacts, important validation commands and observed results, review findings, corrections, and engineering decisions.

Do not require exhaustive logs of routine file reads or trivial shell operations.

Differentiate evidence requirements by activity type:

- Documentation and specification: prompts, generated documents, review findings, and relevant consistency checks.
- Code generation: original generated code, TDD evidence, review, corrections, and validation.
- Testing: actual commands, observed results, and failures.
- Review: concrete findings, decisions, and verified corrections.
Requirements that do not apply to a particular activity should be marked Not Applicable with a brief justification.

## 4. Avoid circular completion requirements
Evidence must be captured and reviewed before the associated task is completed.

However, a commit hash cannot be mandatory before the commit containing the evidence exists.

Allow commit references to be recorded after the relevant commit has been created.

Keep the evidence status accurate and distinguish evidence capture, developer review, observed validation, and committed artifacts.

Do not fabricate commit references or developer approval.

## 5. Protect public repository evidence
Review the evidence files for secrets, tokens, private information, machine-specific absolute paths, and unrelated conversation.

Preserve the actual relevant prompts and generated output. If sensitive information requires redaction, explicitly indicate that redaction rather than presenting the modified content as an exact verbatim record.

Use repository-relative paths for published references.

## 6. Validate the final documentation
Ensure that the engineering standard, skill, Copilot instructions, evidence template, first evidence record, and root GENAI.md are consistent.

Keep the mandatory REST API GenAI exercise explicitly pending.

Do not claim that the skill was successfully discovered or invoked unless this was actually verified.

Preserve the original assessment document and existing Spec Kit installation.

Do not create the constitution, application code, or additional engineering standards yet.

Report the exact corrections made, actual validation results, and anything that still requires developer approval.

Stop after completing this refinement.
```

## Relevant context

- Requirement or assessment section: [technical-exercise-v5.md](../../assessment/technical-exercise-v5.md), "Generative AI tools".
- Engineering standard applied: [docs/engineering/genai.md](../../engineering/genai.md).
- Predecessor record under review: [EV-0001](./EV-0001-repository-genai-standards.md).
- Files read as context: `.specify/init-options.json`, `.specify/integration.json`, `.specify/integrations/speckit.manifest.json`, `.specify/integrations/copilot.manifest.json`, `.specify/workflows/workflow-registry.json`, `.specify/memory/constitution.md`.

### Observed repository state (Spec Kit)

Commands run and output observed:

```text
Get-Content .specify/init-options.json
```

```json
{
  "ai": "copilot",
  "ai_skills": true,
  "allow_symlinks": false,
  "feature_numbering": "sequential",
  "here": true,
  "integration": "copilot",
  "script": "ps",
  "speckit_version": "1.4.0"
}
```

```text
Get-Content .specify/integration.json
```

```json
{
  "version": "1.4.0",
  "integration_state_schema": 1,
  "installed_integrations": ["copilot"],
  "integration_settings": {
    "copilot": { "script": "ps", "invoke_separator": "-" }
  },
  "integration": "copilot",
  "default_integration": "copilot"
}
```

The Speckit and Copilot manifests both record version `1.4.0` with `installed_at` `2026-11-14T17:31:55`. `.specify/memory/constitution.md` begins with `# [PROJECT_NAME] Constitution` and placeholder markup, confirming it is the unratified template.

## Original AI-generated output

Representative excerpt of the standard before correction - the workflow ordering defect reported by the developer:

```markdown
### Phase 4 - Implement

- Use `/speckit-implement` and GitHub Copilot for code and tests.
- Follow Test-Driven Development: write the failing test, confirm the failure, then implement.
- Preserve a representative original AI-generated excerpt or diff **before** any correction.

**Evidence:** original generated code, actual commands run, observed output including failures, TDD evidence.

### Phase 5 - Analyze

- Use `/speckit-analyze` and `/speckit-converge` to check artifact consistency and close gaps.
```

Representative excerpt of the inaccurate repository status reported by the developer:

```markdown
This standard is adopted. No application code, specification, plan, or task list exists yet; the project constitution has not been initialized.
```

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | `Not Applicable` - no application code was produced. | `Not Applicable` |
| SOLID | `Not Applicable` - no application code was produced. | `Not Applicable` |
| TDD | `Not Applicable` - no test project exists. | `Not Applicable` |
| Security | Evidence documents scanned for secrets, tokens, and machine-specific absolute paths; published references checked for repository-relative form. | `[VERIFIED]` - no secrets or absolute paths found |
| Functional requirements | Each of the six feedback items checked against the corrected files; cross-document consistency and link validity re-checked by script. | `[VERIFIED]` - see command results below |

## Review findings

Developer findings from the feedback, with resolution:

| # | Finding | Resolution |
| --- | --- | --- |
| 1 | Analysis was placed after implementation, but the first analysis must precede implementation | Workflow reordered to the eight phases the developer specified; analyze is Phase 5 and mandatory pre-implementation, repeatable afterwards |
| 2 | The repository status falsely implied Spec Kit was not installed | Standard, GENAI.md, and `EV-0001` corrected using observed `.specify` metadata; Spec Kit 1.4.0 recorded as installed with the Copilot integration and the constitution as an unratified template |
| 3 | Evidence collection risked becoming disproportionate | Added a proportionality section and an activity-type evidence table to the standard, skill, README, and template; non-applicable requirements marked `Not Applicable` with justification |
| 4 | Completion requirements were circular: a commit hash was demanded before the commit existed | Completion gate, lifecycle, skill, template, and README now record commit references only after the commit exists, and separate capture, review, validation, and committed state |
| 5 | Evidence files needed review for secrets, personal data, absolute paths, and unrelated conversation | Scanned; no secrets or machine-specific paths found; redaction and repository-relative-path rules added to the standard and README |
| 6 | Final documentation had to be consistent, the API exercise kept pending, and no unverified claim about skill discovery | Cross-document checks run; GENAI.md keeps the API exercise `PENDING`; no claim of skill discovery or invocation is made anywhere |

`[AI-PROPOSED]` Additional finding not raised by the developer: GENAI.md described the lifecycle as if status advanced in a single line, which obscured the distinction between evidence capture and committed artifacts. Corrected alongside item 4.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Adopt the eight-phase workflow with mandatory pre-implementation analysis | Developer direction: analysis must catch inconsistencies before code exists | `Approved` (directive in feedback item 1) |
| Record Spec Kit as installed (1.4.0) and the constitution as unratified | Matches observed `.specify` metadata | `Approved` (directive in feedback item 2) |
| Scale evidence to the activity type, marking inapplicable requirements `Not Applicable` | Developer direction: keep the workflow practical for a small assessment | `Approved` (directive in feedback item 3) |
| Record commit references only after the commit exists | Developer direction: remove the circular requirement | `Approved` (directive in feedback item 4) |
| Add redaction disclosure and repository-relative path rules | Developer direction: protect a public repository | `Approved` (directive in feedback item 5) |
| Final approval of the corrected artifacts | Required before commit | `Pending` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Standard: workflow rewritten from 8 phases (`Phase 0`-`Phase 7`, analyze after implement) to the developer's 8 phases with analyze as Phase 5 | Feedback item 1 | Pre-implementation analysis lets defects be caught while they are still cheap to fix; post-implementation analysis is kept as an optional repeat |
| Standard: `## 2. Tooling` now records Spec Kit 1.4.0, the Copilot integration, and the unratified constitution | Feedback item 2 | The prior text claimed Spec Kit was not used, contradicting `.specify/init-options.json` and `.specify/integration.json` |
| Standard: added `### Proportionality`, `### Evidence by activity type`, `### Fidelity, redaction, and paths`, `### Commit references` | Feedback items 3, 4, 5 | Encodes scope, applicability, redaction disclosure, and non-circular commit rules in one authoritative place |
| Standard: completion gate item 6 split into task-identifier traceability plus a separate commit-reference item | Feedback item 4 | Removes the circular dependency between completing a task and committing the evidence about it |
| Standard: `## 9. Current status` rewritten with observed state | Feedback item 2 | Replaces a claim with verifiable facts |
| Skill: non-negotiable rules, all three checkpoints, and status transitions updated | Feedback items 1, 3, 4, 6 | Keeps the enforced procedure aligned with the standard; adds the prohibition on claiming skill discovery that did not occur |
| Evidence README: added proportionality table, redaction and absolute-path rules, commit-reference guidance | Feedback items 3, 4, 5 | Keeps the index's guidance consistent with the standard |
| Template: added `Activity type` and `Commit references` fields; applicability note on validation; validation-command guidance; repository-relative path note | Feedback items 3, 4, 5 | Makes activity-type expectations and non-circular commit recording explicit at the point of use |
| `EV-0001`: corrected development-phase wording, added activity type and commit field, recorded developer review, status `Captured` -> `Reviewed` | Feedback items 2, 4 | The old status text claimed no developer review had occurred, which the feedback contradicts |
| `EV-0001`: developer review recorded as review-only, with approval still `Pending` | Feedback item 4 | The developer approved the structure as a proposal and requested corrections; recording that as final approval would fabricate consent |

`Not Applicable` items: Clean Architecture, SOLID, and TDD validation do not apply because no application code or test project exists. Brief justification recorded in the validation table above.

## Commands and observed results

### Re-verification on 2026-11-15

The same six feedback items were re-issued on `2026-11-15`. The corrected documents were re-checked against each item instead of being rewritten. No additional correction was required.

```text
@'
<verification script: phase ordering, Spec Kit status claims, proportionality,
 commit rules, path/secret scan, link resolution, frontmatter>
'@ | python -
```

```text
=== 1. lifecycle order ===
   Phase 1 - Constitution and engineering standards
   Phase 2 - Specify and clarify
   Phase 3 - Plan
   Phase 4 - Tasks
   Phase 5 - Analyze (mandatory, before implementation)
   Phase 6 - Implement using TDD and GenAI evidence checkpoints
   Phase 7 - Review, validate, and converge
   Phase 8 - Conclude and update documentation
  count: 8 | analyze before implement: True
  analyze mandatory pre-implementation: True
  repeat-after-allowed: True

=== 2. Spec Kit status ===
  standard says 1.4.0 installed: True
  constitution unratified in standard: True
  GENAI says 1.4.0: True | unratified: True
  EV-0001 records 1.4.0: True
  live inaccurate claims: none

=== 3. proportionality ===
  Proportionality heading: True
  activity-type table: True (all four rows present)
  Not Applicable rule: True
  excludes trivial ops: True
  skill has Not Applicable: True
  template has activity type: True

=== 4. circular completion ===
  commit-after-exists section: True
  gate separates commit item: True
  lifecycle distinguishes states: True
  skill commit rule: True
  template commit field: True

=== 5. public repo hygiene ===
  redaction rule: True
  repo-relative paths rule: True
  machine-specific paths: 2 hits, both `https:` URLs (false positives of the drive-letter pattern)
  secrets: none

=== 6. consistency ===
  prose links: 53 | broken: 0
  GENAI sections: 10
  API pending: True
  skill discovery unverified: True
  frontmatter: ['compatibility', 'description', 'metadata', 'name'] | name==dir: True | desc: 696
```

Status cross-check across the three publishing points:

```text
EV-0001 record: `Reviewed`   | README index: `Reviewed`   | GENAI.md: `Reviewed`
EV-0002 record: `Captured`   | README index: `Captured`   | GENAI.md: `Captured`
```

### Initial validation

Structural and consistency validation after the corrections:

```text
@'
<validation script: frontmatter YAML parse, name/dir match, description length,
 prose-only relative-link resolution, non-ASCII detection, lifecycle term presence>
'@ | python -
```

Observed results after all corrections were applied:

```text
=== 1. skill frontmatter ===
parsed keys: ['compatibility', 'description', 'metadata', 'name']
name==dir: True | pattern: True | desc len: 696 <=1024: True

=== 2. prose links ===
checked: 53 | broken: 0

=== 3. non-ascii in prose ===
GENAI.md                                                       ['0x2014']
.github/copilot-instructions.md                                none
.github/skills/genai-evidence/SKILL.md                         none
docs/engineering/genai.md                                      none
docs/evidence/genai/README.md                                  none
docs/evidence/genai/template.md                                none
docs/evidence/genai/EV-0001-repository-genai-standards.md      none
docs/evidence/genai/EV-0002-standard-refinement.md             none

=== 4. forbidden/inaccurate claims ===
spec kit not installed                   [('GENAI.md', 129), ('EV-0001...', 60), ('EV-0002...', 65)]
not been initialized (constitution)      none
skill verified/invoked                   [('.github/copilot-instructions.md', 24), ('.github/skills/genai-evidence/SKILL.md', 62)]
absolute windows path                    none
commit hash as requirement pre-commit    none

=== 5. workflow order (analyze before implement) ===
   Phase 1 - Constitution and engineering standards
   Phase 2 - Specify and clarify
   Phase 3 - Plan
   Phase 4 - Tasks
   Phase 5 - Analyze (mandatory, before implementation)
   Phase 6 - Implement using TDD and GenAI evidence checkpoints
   Phase 7 - Review, validate, and converge
   Phase 8 - Conclude and update documentation
analyze before implement: True

=== 6. lifecycle vocabulary present in all ===
(all eight documents contain Pending / Captured / Reviewed / Verified)
```

Interpretation of category 4 hits: each was inspected and is a prohibition or a historical description of the corrected defect, not a live inaccurate claim. The inspection output was:

```text
GENAI.md:129 => - `[VERIFIED]` The first version of the standard placed analysis after implementation and understated the repository state by implying Spec Kit was not installed. Both were corrected on developer review; see `EV-0002`.
EV-0001:239 => - `[VERIFIED]` Developer review finding: this record understated the repository state by implying Spec Kit was not installed. ...
EV-0002:93 => Do not claim that the skill was successfully discovered or invoked unless this was actually verified.
EV-0002:192 => | 2 | The repository status falsely implied Spec Kit was not installed | ...
copilot-instructions.md:24 => - Never claim that the skill was discovered, loaded, or invoked unless that was observed.
SKILL.md:62 => 7. Do not claim that this skill was discovered, loaded, or invoked unless that was actually observed in the session.
```

The single non-ASCII character is an em dash (U+2014) in the GENAI.md heading that reproduces the required section title verbatim. It is typographic, not non-English content.

```text
git diff --exit-code -- docs/assessment/technical-exercise-v5.md
```

Exit code `0`: the assessment document is unchanged.

```text
git status --short
```

The Spec Kit installation under `.specify/` and the skills under `.github/skills/speckit-*` show no modifications, confirming no Spec Kit skill or template was altered.

Redaction and path review: no secrets, tokens, credentials, personal data, unrelated conversation, or machine-specific absolute paths were found in the evidence files. One absolute `https://` documentation URL is referenced as an external source; it contains no machine-specific information and is not a local path.

## Final references

- Standard: [docs/engineering/genai.md](../../engineering/genai.md)
- Skill: [.github/skills/genai-evidence/SKILL.md](../../../.github/skills/genai-evidence/SKILL.md)
- Copilot instructions: [.github/copilot-instructions.md](../../../.github/copilot-instructions.md)
- Evidence index: [docs/evidence/genai/README.md](./README.md)
- Template: [docs/evidence/genai/template.md](./template.md)
- Predecessor record: [EV-0001](./EV-0001-repository-genai-standards.md)
- Assessment response: [GENAI.md](../../../GENAI.md)
- Implementation files: `Not Applicable` - documentation only
- Test files: `Not Applicable` - no test project exists
- Task identifiers: `PENDING` - no Spec Kit task list exists
- Commits: `b3b6195`

## Evidence status rationale

Status is `Captured`: the prompt, the observed repository state, the original defective excerpts, the six findings, the corrections, and the executed validation results are all recorded. A second review pass on `2026-11-15` re-verified the same six items and required no further correction.

It is not `Reviewed` or `Verified` because the developer has not yet reviewed these corrected artifacts or granted final approval, and no commit exists for this activity. Final approval is the outstanding item; nothing in this record is presented as approved.
