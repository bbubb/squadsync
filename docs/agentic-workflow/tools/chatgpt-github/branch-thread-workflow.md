# Branch Thread Workflow

## Purpose

Branch threads are bounded ChatGPT working contexts for a specific sprint, issue, PR, debugging task, or implementation review.

Prefer one branch thread per sprint. Use issue-level branch threads only when a task becomes complex, blocked, or requires isolated review/debugging.

A branch thread keeps work within the confirmed scope rather than redefining roadmap direction. ChatGPT GitHub may perform supported planning, documentation, issue, branch/PR, review, and closeout actions; Codex CLI remains the primary implementation profile for scoped application-code work. If broader work appears, surface it as a suggested follow-up and return it to the main planning thread for prioritization.

## Workflow Mental Model

```text
Main planning thread = project control room
Sprint branch thread = bounded sprint working context
Issue/PR subthread = optional escalation path
ChatGPT GitHub = planning, docs, issues, PR setup, review support
Codex CLI = implementation worker for scoped agent-ready tasks
GitHub Issues = task records
GitHub Project = visual tracking board
Docs/ADRs = durable project memory
PLANS.md = complex-task execution plan standard
.agents/skills = native Codex skill entry points
```

## Use Branch Threads For

- managing execution across a confirmed sprint
- working through sprint issues in sequence
- implementing a specific GitHub issue when isolated focus is needed
- reviewing a specific pull request
- debugging a focused problem
- generating a Codex CLI prompt for one task
- validating a sprint deliverable
- producing a closeout summary

## Standard Branch Thread Prompt

Use this canonical prompt only after the main planning thread has confirmed the sprint or task scope:

```text
We are working on SquadSync inside the ChatGPT Project “SquadSync.”

Use the GitHub connector for repository context and authorized GitHub actions when needed.

Repository:
https://github.com/bbubb/squadsync

Thread role:
Bounded sprint / issue / PR / debugging / closeout context

Current phase:
[Phase from docs/planning/project-roadmap.md]

Current sprint:
[Confirmed sprint, or “not applicable”]

Current task:
[Issue, PR, debugging task, or closeout name and number]

Goal:
[One clear goal]

Scope:
[List the exact files, areas, behavior, or GitHub records allowed to change]

Intended GitHub action:
[Advise only / refine issue / create docs branch and PR / review PR / prepare Codex handoff / other approved action]

Required baseline context:
- README.md
- AGENTS.md
- CONTRIBUTING.md
- docs/planning/project-roadmap.md
- the active GitHub issue or pull request
- docs/agentic-workflow/README.md
- docs/agentic-workflow/tools/chatgpt-github/README.md
- docs/agentic-workflow/tools/chatgpt-github/navigation.md

Task-specific source-of-truth documents:
- [List the exact MVP, architecture, domain, ADR, workflow, local AGENTS.md, skill, or other paths selected for this task]
- [Include PLANS.md only when the task meets its complexity threshold]

Non-goals:
[List what must not change]

Acceptance criteria:
[List the observable completion conditions]

Validation:
[List the required checks or explain why a check is not applicable]

Instructions:
Resolve every bracketed field before acting, and name task-specific sources by exact repository path rather than a generic label such as “relevant docs.” If the roadmap, active task record, and navigation cannot resolve a required value, stop and ask for clarification.

Review the baseline context, active task record, and task-specific documents first. Confirm scope, non-goals, acceptance criteria, intended state change, and validation before repository changes. Keep work within the confirmed task. Perform only the authorized GitHub action. Route scoped application-code implementation through the Codex CLI profile. Surface broader discoveries as follow-up work, and persist decisions that must survive this thread in GitHub.
```

This file owns the branch-thread startup prompt. The roadmap and main-thread workflow should link here instead of copying it.

For application-code handoff, populate the [Codex CLI Task Prompt](../codex-cli/task-prompt.md) from the confirmed issue and task-specific sources.

## Branch Thread Operating Loop

```text
1. Inspect the referenced sprint, issue, PR, and docs.
2. Confirm scope, non-goals, acceptance criteria, and validation.
3. Decide whether the task needs a normal issue workflow or an ExecPlan.
4. Perform supported ChatGPT GitHub work or prepare the scoped Codex CLI handoff for application-code implementation.
5. Create/update an issue, branch, or PR when appropriate.
6. Review results against acceptance criteria and validation gates.
7. Capture suggested follow-ups without expanding scope.
8. Move to the next sprint issue when appropriate.
9. Produce a closeout summary for the main planning thread when execution is ready for a human closeout decision.
```

## GitHub Change Rule

When ChatGPT GitHub creates repository changes, the work should be issue-backed.

Default order:

```text
Issue -> branch -> changes -> PR -> human review
```

If a PR is created before an issue by mistake, create and link the issue before requesting review.

## Closeout Summary Template

```markdown
## Sprint/Issue Closeout Candidate

Name/number:

## GitHub State

- Issue:
- Pull request:
- Checks/reviews:

## What Changed

-

## Scope / Non-Goal Status

-

## PRs / Branches

-

## Validation

-

## Acceptance Criteria Status

-

## Known Limitations / Risks

-

## Docs / ADRs Updated

-

## Follow-up Issues Created or Suggested

-

## Questions for Main Planning

-

## Human Decision Needed

- Close, iterate, create follow-up work, or defer:
```

## Scope Rule

If a branch thread uncovers work outside the current sprint or issue, do not silently implement it. Record it under suggested follow-ups and return it to the main planning thread.

## Context Rule

Do not rely on hidden chat memory for project decisions. If a decision, issue, constraint, or follow-up must survive the current thread, capture it in GitHub or a canonical repo document.
