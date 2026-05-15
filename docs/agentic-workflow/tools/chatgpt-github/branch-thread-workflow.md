# Branch Thread Workflow

## Purpose

Branch threads are focused ChatGPT discussions for a specific sprint, issue, PR, debugging task, or implementation review.

Prefer one branch thread per sprint. Use issue-level branch threads only when a task becomes complex, blocked, or requires isolated review/debugging.

A branch thread should execute scoped work, not redefine the roadmap. If the branch thread discovers broader work, it should surface that work as a suggested follow-up and return it to the main planning thread for prioritization.

## Workflow Mental Model

```text
Main planning thread = project control room
Sprint branch thread = sprint execution manager
Issue/PR subthread = optional escalation path
ChatGPT GitHub = planning, docs, issues, PR setup, review support
Codex CLI = implementation worker for scoped agent-ready tasks
GitHub Issues = task records
GitHub Project = visual tracking board
Docs/ADRs = durable project memory
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

```text
We are working on SquadSync inside the ChatGPT Project “SquadSync.”

Use the GitHub connector for repository context and repository changes when needed.

Repository:
https://github.com/bbubb/squadsync

Current task:
[Issue/PR/Sprint/Phase name and number]

Goal:
[One clear goal]

Relevant docs:
- README.md
- AGENTS.md
- docs/planning/project-roadmap.md
- docs/planning/mvp-scope.md
- docs/architecture/system-overview.md
- docs/architecture/domain-model.md
- docs/agentic-workflow/README.md
- docs/agentic-workflow/policy/project-policy.md
- docs/agentic-workflow/workflow/lifecycle.md
- docs/agentic-workflow/workflow/validation-gates.md
- docs/agentic-workflow/workflow/stop-conditions.md
- docs/agentic-workflow/specs/agent-task-spec.md
- docs/agentic-workflow/specs/pull-request-spec.md
- docs/agentic-workflow/tools/chatgpt-github/README.md
- docs/agentic-workflow/tools/codex-cli/README.md
- docs/adr/
- .github/ISSUE_TEMPLATE/
- .github/PULL_REQUEST_TEMPLATE.md

Additional relevant docs for this task:
[List task-specific docs/prompts]

Non-goals:
[List what should not be changed]

Instructions:
Review the relevant issue/PR/docs first. Confirm scope, non-goals, acceptance criteria, and validation steps before making or recommending changes. Keep the work focused on the current task. Surface suggested follow-ups separately instead of expanding scope.
```

## Branch Thread Operating Loop

```text
1. Inspect the referenced sprint, issue, PR, and docs.
2. Confirm scope, non-goals, acceptance criteria, and validation.
3. Execute or guide the scoped work.
4. Create/update an issue, branch, or PR when appropriate.
5. Review results against acceptance criteria.
6. Capture suggested follow-ups without expanding scope.
7. Move to the next sprint issue when appropriate.
8. Produce a closeout summary for the main planning thread when the sprint or issue is complete.
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
## Sprint/Issue Completed

Name/number:

## What Changed

- 

## PRs / Branches

- 

## Validation

- 

## Docs / ADRs Updated

- 

## Follow-up Issues Created or Suggested

- 

## Questions for Main Planning

- 
```

## Scope Rule

If a branch thread uncovers work outside the current sprint or issue, do not silently implement it. Record it under suggested follow-ups and return it to the main planning thread.
