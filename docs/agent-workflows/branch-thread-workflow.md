# Branch Thread Workflow

## Purpose

Branch threads are focused ChatGPT discussions for a specific sprint, issue, PR, debugging task, or implementation review.

A branch thread should execute scoped work, not redefine the roadmap. If the branch thread discovers broader work, it should surface that work as a suggested follow-up and return it to the main thread for prioritization.

## Use Branch Threads For

- implementing a specific GitHub Issue
- reviewing a specific Pull Request
- debugging a focused problem
- generating a Codex prompt for one task
- validating a sprint deliverable
- producing a closeout summary

## Standard Branch Thread Prompt

```text
We are working on SquadSync inside the ChatGPT Project “SqudSync.”

Use the GitHub connector.

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
- docs/agent-workflows/codex-task-lifecycle.md
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
1. Inspect the referenced issue, PR, and docs.
2. Confirm scope, non-goals, acceptance criteria, and validation.
3. Execute or guide the scoped work.
4. Create/update branch or PR when appropriate.
5. Review results against acceptance criteria.
6. Capture suggested follow-ups without expanding scope.
7. Produce a closeout summary for the main thread.
```

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

If a branch thread uncovers work outside the current issue, do not silently implement it. Record it under suggested follow-ups and return it to the main thread.
