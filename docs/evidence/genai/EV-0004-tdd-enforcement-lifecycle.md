# EV-0004 - TDD enforcement lifecycle across Spec Kit phases

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0004` |
| Date | `2026-11-16` |
| Development phase | Phase 1 - Constitution and engineering standards (refinement of an existing standard). |
| Activity type | Documentation and specification |
| Related specification | `PENDING` - no feature specification exists; that belongs to Phase 2. |
| Related task identifiers | `PENDING` - no Spec Kit task list exists. Work is identified by the developer instruction below. |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | `PENDING` - the model identifier was not exposed in the session. |
| Evidence status | `Verified` |
| Commit references | `ba92121` - "docs: enforce TDD across all Spec Kit phases". Recorded after the commit was created, per the standard. |

## Actual prompt

Verbatim developer instruction for this activity:

```text
Add the following mandatory section to docs/engineering/testing-and-tdd.md:

## TDD Enforcement Across Spec Kit Phases
TDD is not limited to implementation. It MUST govern specification, planning, task generation, analysis, implementation, and review.

### Specification
Every functional requirement and business behavior MUST have testable acceptance criteria, including relevant failure scenarios and edge cases.

### Planning
The implementation plan MUST include:

- A test strategy covering Domain, Application, Infrastructure, API endpoints, and frontend behavior.
- Unit and integration test approaches.
- Real PostgreSQL integration testing where persistence behavior is involved.
- Test scenarios mapped to acceptance criteria.
- Required test tools, environments, fixtures, and dependencies.
- A test-first implementation strategy.
Planning MUST establish how each behavior will be tested before implementation.

### Task generation
The tasks.md artifact MUST explicitly represent TDD execution.

For each independently testable behavior or coherent group of related behaviors, generate dependency-ordered tasks for:

1. Writing the relevant tests.
2. Executing the tests and observing the expected RED result.
3. Implementing the minimum required behavior.
4. Executing the tests and observing GREEN.
5. Refactoring when necessary and rerunning the tests.
6. Recording actual TDD and GenAI evidence.
Implementation tasks MUST depend on the corresponding RED checkpoint.

A test task and its corresponding implementation task MUST NOT be marked as parallelizable.

Do not defer all testing to a final project-wide testing phase.

Each user story MUST include its own relevant test-first tasks.

### Analysis
Before implementation, verify that:

- Every functional requirement has corresponding test coverage planned.
- Test-writing tasks precede their corresponding implementation tasks.
- RED checkpoints are explicit.
- No task dependencies allow implementation before the expected failing tests have been observed.
- All critical security and authorization behaviors have tests planned.
Missing TDD tasks or incorrect task dependencies constitute constitutional violations that MUST be resolved before implementation begins.

### Implementation
Execute RED, GREEN, and REFACTOR in the defined order.

Do not mark a behavior complete without actual test execution and observed results.

An unrelated build failure, configuration failure, or unavailable dependency is not valid evidence of an expected behavioral RED.

Preserve test evidence through the existing genai-evidence workflow.

### Review and convergence
Verify that the implemented behavior satisfies the specification and that the recorded evidence demonstrates genuine test-first development.

Tests written only after implementation MUST NOT be presented as evidence of TDD.

Documentation-only and non-behavioral infrastructure tasks MAY be marked Not Applicable with justification and appropriate alternative validation.

### Spec Kit integration
When the project constitution is ratified, these requirements MUST be reflected in the active Spec Kit planning and task-generation templates and in the pre-implementation Constitution Check.

Do not modify the Spec Kit templates or constitution during this documentation task. Those changes will be performed during the constitution phase.

Update the engineering documentation index to reference this TDD enforcement lifecycle.
```

The `commit` instruction that preceded this activity concluded the previous work; it is recorded in `EV-0003`.

## Relevant context

- Target document: [testing-and-tdd.md](../../engineering/testing-and-tdd.md), previously sections 1-6.
- Index document: [docs/engineering/README.md](../../engineering/README.md), phases table, notes, and proposed constitutional mapping.
- Existing related content reused rather than duplicated: section 2.2 (behavior failure versus build failure), section 2.8 (applicability), and the `genai-evidence` workflow in [genai.md](../../engineering/genai.md).
- Constraint observed: Spec Kit templates and `.specify/memory/constitution.md` were read only and not modified.

## Original AI-generated output

The section body was dictated by the developer and is reproduced verbatim as section 3. The AI-generated part is the integration: section numbering, placement, cross-references, and index updates.

Representative generated index row before correction - the applicability table still treated TDD as merely supporting in the Specify, Tasks, and Analyze phases:

```markdown
| testing-and-tdd.md | S | S | P | P | S | P | P | S |
```

Originally generated numbering attempt, which produced duplicate and non-sequential headings:

```text
## 1. Purpose and scope
## 2. Mandatory rules
## 4. TDD Enforcement Across Spec Kit Phases
## 5. Practical examples
## 4. Prohibited practices and anti-patterns
```

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | `Not Applicable` - documentation change only, no code produced. | `Not Applicable` |
| SOLID | `Not Applicable` - documentation change only. | `Not Applicable` |
| TDD | `Not Applicable` to this activity's own execution - documentation only. The activity defines TDD enforcement for later phases. Applicable validation (structure, anchors, consistency) was performed. | `Not Applicable` |
| Security | No secrets, credentials, or machine-specific paths introduced. | `[VERIFIED]` |
| Functional requirements | All eight subsections of the dictated content present; index updated to reference the lifecycle; templates and constitution untouched. | `[VERIFIED]` with two defects found and corrected |

## Review findings

- `[VERIFIED]` All dictated subsections were inserted verbatim and are present: `## 3. TDD Enforcement Across Spec Kit Phases` plus `### 3.1` through `### 3.7`.
- `[VERIFIED]` Defect: the first renumbering pass produced duplicate and non-sequential headings (`4, 5, 4`). Detected by the sequential-numbering check; corrected to `1-7`.
- `[VERIFIED]` Defect: the applicability table still marked `testing-and-tdd.md` as supporting (`S`) in the Specify, Tasks, and Analyze phases, contradicting the new requirement that TDD governs those phases. Corrected to primary (`P`) for phases 2-7.
- `[VERIFIED]` Document links and anchors validate: 197 links checked, 0 broken files, 0 broken anchors, including the new `#3-tdd-enforcement-across-spec-kit-phases` anchor.
- `[VERIFIED]` No non-ASCII characters in either modified document.
- `[VERIFIED]` Spec Kit templates and the constitution were not modified; `.specify/` shows no changes.
- `[DEVELOPER DECISION]` The dictated content requires the constraint that templates and the constitution are changed only in the constitution phase. Implemented as a deferral note in section 3.7 and in the index, with no template edits.
- `[AI-PROPOSED]` Placement as section 3, immediately after mandatory rules, so enforcement sits with mandatory content rather than after examples.
- `[AI-PROPOSED]` Added two verification rows (phase enforcement, task dependencies) so the new requirement is checkable against the existing verification table.
- `[PENDING]` Developer review and commit of this change.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Dictated section content | Developer instruction | `Approved` - content specified verbatim by the developer |
| Number the new section 3 and renumber the remainder | Keeps mandatory content contiguous and numbering sequential | `Approved` - committed in `ba92121` at developer instruction |
| Mark `testing-and-tdd.md` primary for phases 2-7 in the applicability table | The new section makes TDD governing, not supporting, in those phases | `Approved` - committed in `ba92121` at developer instruction |
| Add phase-enforcement and task-dependency verification rows | Makes the new rule objectively checkable | `Approved` - committed in `ba92121` at developer instruction |
| Defer template and constitution changes to the constitution phase | Developer instruction; also avoids modifying Spec Kit artifacts | `Approved` - explicit instruction |
| Commit this change | Developer directed the commit on `2026-11-16` | `Approved` - published in `ba92121` |

Approval basis, stated precisely: the developer dictated the content, directed the commit, and the change is published in `ba92121`. That covers the dictated rules and their publication. The formatting and placement choices were not separately debated but were accepted by publishing them.

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Renumbered headings from `1, 2, 4, 5, 4` to `1-7` | First pass produced duplicate section numbers | Duplicate anchors make cross-references ambiguous and break deterministic anchor generation |
| Applicability table row changed to `S P P P P P P S` | Contradicted the new enforcement rule | An index that marks TDD as supporting in the phases where it now governs would misdirect readers and misstate the standard |
| Added `Phase enforcement` and `Task dependencies` verification rows | New mandatory rules need objective checks | The standard requires objective verification criteria for its own rules |
| Principle II mapping reworded to "Test-First Development across all phases" | Was "Test-First Development" | Reflects that enforcement now spans specification through review, not implementation alone |

## Commands and observed results

Structure and numbering validation after insertion:

```text
@'
<validation script: heading extraction, numbering sequence, anchor resolution>
'@ | python -
```

```text
headings:
  ## 1. Purpose and scope
  ## 2. Mandatory rules
  ## 4. TDD Enforcement Across Spec Kit Phases
  ## 5. Practical examples
  ## 4. Prohibited practices and anti-patterns
  ## 6. Verification and review criteria
  ## 7. References
```

That output is the defect. After correction:

```text
headings:
  ## 1. Purpose and scope
  ## 2. Mandatory rules
  ## 3. TDD Enforcement Across Spec Kit Phases
  ## 4. Practical examples
  ## 5. Prohibited practices and anti-patterns
  ## 6. Verification and review criteria
  ## 7. References
sequential: True [1, 2, 3, 4, 5, 6, 7]
```

Link, anchor, encoding, and content validation:

```text
=== links ===
  links: 197 | broken files: 0 | broken anchors: 0

=== numbering (all standards) ===
  all standards sequential=True

=== non-ascii ===
  docs/engineering/testing-and-tdd.md: none
  docs/engineering/README.md: none

=== required content present in new section ===
  ## 3. TDD Enforcement Across Spec Kit Phases     OK
  ### 3.1 Specification                            OK
  ### 3.2 Planning                                 OK
  ### 3.3 Task generation                          OK
  ### 3.4 Analysis                                 OK
  ### 3.5 Implementation                           OK
  ### 3.6 Review and convergence                   OK
  ### 3.7 Spec Kit integration                     OK

  index references enforcement: True
  index table row updated: True
  index defers template changes: True
```

Scope verification:

```text
git status --porcelain -- .specify   -> empty (Spec Kit templates and constitution untouched)
```

## Final references

- Standard modified: [testing-and-tdd.md](../../engineering/testing-and-tdd.md) (section 3)
- Index modified: [docs/engineering/README.md](../../engineering/README.md)
- Predecessor record for the standards: [EV-0003](./EV-0003-engineering-standards.md)
- Implementation files: `Not Applicable` - documentation only
- Test files: `Not Applicable` - no test project exists
- Task identifiers: `PENDING` - no Spec Kit task list exists
- Commits: `ba92121`

## Evidence status rationale

Status is `Verified`: the instruction, the dictated content, the generated integration, the two detected defects with their corrections, and the executed validation results are recorded; the developer directed the commit on `2026-11-16` and the change is published in `ba92121`.

Approval covers the dictated rules and their publication in the repository. The Spec Kit template and Constitution Check changes remain deferred to the constitution phase, as the instruction requires.
