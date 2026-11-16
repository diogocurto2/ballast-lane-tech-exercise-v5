# EV-0005 - Separation of engineering governance and submission process rules

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0005` |
| Date | `2026-11-16` |
| Development phase | Phase 1 - Constitution and engineering standards (governance boundary correction). |
| Activity type | Documentation and specification |
| Related specification | `PENDING` - no feature specification exists. |
| Related task identifiers | `PENDING` - no Spec Kit task list exists. Work is identified by the developer instruction below. |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session. |
| Evidence status | `Verified` |
| Commit references | `f59bc6b` - "docs: separate engineering governance from submission process rules". Recorded after the commit was created, per the standard. |

## Actual prompt

Verbatim developer instruction for this activity:

```text
## Separation of Engineering Governance and Submission Rules
The constitution MUST govern software engineering principles, architecture, testing, quality, security, and AI-assisted development.

The recruiter's instruction to stop working on the project after submission is an external assessment-process requirement, not an engineering principle.

- Preserve this requirement in docs/assessment/submission-requirements.md for reference.
- Do not include a post-submission code freeze in the constitution.
- Remove post-submission development restrictions from docs/engineering/delivery-and-ci.md and repository-wide Copilot instructions, if present.
- Do not implement automated mechanisms, hooks, gates, or agent restrictions to prevent code changes after submission.
- The developer is responsible for submission timing and compliance with the recruiter's instructions.
Keep all other mandatory assessment deliverables and engineering principles unchanged.
```

## Relevant context

- Files inspected for the restriction: all `docs/engineering/*.md`, `.github/copilot-instructions.md`, `GENAI.md`, and `docs/evidence/genai/*.md`.
- Governance authority for this change: [genai.md](../../engineering/genai.md) Phase 1, which establishes engineering standards and defers the constitution.
- The constitution at `.specify/memory/constitution.md` remains the unratified template and was not modified.
- The submission requirement itself is retained in [submission-requirements.md](../../assessment/submission-requirements.md) as SUB-08.

## Original AI-generated output

Representative excerpt of the content removed from `delivery-and-ci.md`:

```markdown
### 2.9 Freeze after submission

- After the submission link is sent, the repository MUST NOT be modified until the interviewer authorizes changes or the interview completes.
- Preparation notes outside the repository MAY continue.
```

```markdown
- Tagging a release or editing the repository after submission.
```

Original wording in `.github/copilot-instructions.md`:

```markdown
- Read [docs/assessment/submission-requirements.md](../docs/assessment/submission-requirements.md) for delivery constraints, including the submission deadline and the post-submission freeze.
```

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | `Not Applicable` - documentation change only. | `Not Applicable` |
| SOLID | `Not Applicable` - documentation change only. | `Not Applicable` |
| TDD | `Not Applicable` - documentation only; no behavior changed. Applicable validation performed instead. | `Not Applicable` |
| Security | No secrets or machine paths introduced. | `[VERIFIED]` |
| Functional requirements | Each of the four directives checked against the final documents; no enforcement mechanisms added. | `[VERIFIED]` with the findings below |

## Review findings

- `[VERIFIED]` The restriction existed in two places in `delivery-and-ci.md` (section 2.9 and a prohibited-practices bullet) and once in `.github/copilot-instructions.md`. All three were removed.
- `[VERIFIED]` Section numbering in `delivery-and-ci.md` remains sequential (`2.1`-`2.8`) after deleting section 2.9.
- `[VERIFIED]` SUB-08 is preserved in `submission-requirements.md` and is now explicitly labeled a process obligation owned by the developer, recorded for reference only.
- `[VERIFIED]` No hooks, gates, workflow conditions, or agent restrictions were added. No `.git/hooks` files, no CI enforcement of the freeze, and no instruction in the Copilot instructions restricts changes after submission.
- `[VERIFIED]` Links and anchors validate: 208 links, 0 broken files, 0 broken anchors.
- `[VERIFIED]` No non-ASCII characters in any modified document.
- `[AI-PROPOSED]` Added a "Separation of engineering governance and submission process" subsection to the standards index, plus a verification criterion, so the boundary is documented once and remains checkable rather than being rediscovered.
- `[AI-PROPOSED]` Kept proposed principle VII "Reproducibility and Delivery Readiness" citing `submission-requirements.md`, because its subject is engineering deliverables such as tests and documentation, not the post-submission freeze. The new subsection states explicitly that process obligations are excluded from principles.
- `[PENDING]` Developer review and commit of this change.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Remove the freeze from delivery and Copilot instructions | Developer instruction: process requirement, not an engineering rule | `Approved` - explicit instruction |
| Keep SUB-08 in `submission-requirements.md` as reference | Developer instruction: preserve for reference | `Approved` - explicit instruction |
| Add a written separation note to the standards index | Prevents the process rule from re-entering the constitution later | `Approved` - committed in `f59bc6b` at developer instruction |
| Add a verification criterion for the separation | Keeps the boundary objectively checkable | `Approved` - committed in `f59bc6b` at developer instruction |
| No automated enforcement of the freeze | Developer instruction: no hooks, gates, or agent restrictions | `Approved` - explicit instruction |
| Commit this change | Developer directed the commit on `2026-11-16` | `Approved` - published in `f59bc6b` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Deleted `### 2.9 Freeze after submission` from `delivery-and-ci.md` | Engineering standards must not impose assessment-process obligations | A code freeze is a submission-process constraint tied to one delivery event, not a durable engineering principle; encoding it as a standard forces every future change to be evaluated against an expired process rule |
| Deleted the prohibited-practice bullet "Tagging a release or editing the repository after submission" | Same reason | The bullet duplicated the removed section and was the only prohibited practice in that list that was not an engineering anti-pattern |
| Reworded the Copilot instruction to reference "assessment deliverables and delivery constraints, including the submission deadline" | Same reason, while retaining the deadline as context | The deadline is useful planning context; the freeze was an instruction to the developer, not a rule for the agent |
| Rewrote SUB-08 in `submission-requirements.md` as a process obligation owned by the developer | The requirement must be preserved for reference without becoming governance | States provenance and ownership, and explicitly bars it from the constitution, the standards, and tooling |
| Added a "Separation of engineering governance and submission process" subsection and a verification criterion to the index | Prevent recurrence at the constitution phase | The boundary is most likely to be crossed when writing the constitution, so the exclusion is stated where the mapping to constitutional principles lives |

## Commands and observed results

Search for the restriction across engineering documents and instructions:

```text
Select-String -Path docs/engineering/*.md, .github/copilot-instructions.md -Pattern 'freeze|after submission|post-submission|MUST NOT be modified until'
```

Before the change, this returned:

```text
delivery-and-ci.md:88: ### 2.9 Freeze after submission
delivery-and-ci.md:90: - After the submission link is sent, the repository MUST NOT be modified until the interviewer authorizes changes or the interview completes.
delivery-and-ci.md:154: - Tagging a release or editing the repository after submission.
README.md:129: | Submission requirements | ... no symbolic links, deadline, post-submission freeze |
copilot-instructions.md:8: - Read ... for delivery constraints, including the submission deadline and the post-submission freeze.
```

After the change, only the deliberate separation statements remain in `README.md`, which prohibit the rule from entering the constitution rather than imposing a development restriction.

Structure, link, and encoding validation:

```text
links 208 | broken files 0 | broken anchors 0

subsections in delivery-and-ci.md:
  [1, 2, 3, 4, 5, 6, 7, 8] sequential: True
  main sections: [1, 2, 3, 4, 5, 6, 7]

non-ascii: all modified files clean
```

Preservation check:

```text
Select-String -Path docs/assessment/submission-requirements.md -Pattern 'SUB-08'
24: | SUB-08 | After submission, the project MUST NOT be modified until further notice. | Process | Repository freeze after the submission link is sent. |
34: - **SUB-08** - a **process obligation**, not an engineering rule. ...
58: 6. `SUB-08` - after sending the link, stop changing the repository.
```

Scope check: `.specify/` unchanged, so no hook, gate, template, or constitution edit was made.

## Final references

- Submission requirements (preserved requirement): [submission-requirements.md](../../assessment/submission-requirements.md)
- Delivery standard (restriction removed): [delivery-and-ci.md](../../engineering/delivery-and-ci.md)
- Standards index (separation documented): [docs/engineering/README.md](../../engineering/README.md)
- Copilot instructions (restriction removed): [.github/copilot-instructions.md](../../../.github/copilot-instructions.md)
- Implementation files: `Not Applicable` - documentation only
- Test files: `Not Applicable` - no test project exists
- Task identifiers: `PENDING` - no Spec Kit task list exists
- Commits: `f59bc6b`

## Evidence status rationale

Status is `Verified`: the instruction, the removed content, the corrections with justification, and the executed validation results are recorded; the developer directed the commit on `2026-11-16` and the change is published in `f59bc6b`.

Approval covers the removal and the separation documentation. No enforcement mechanism exists, by design, and none was added.
