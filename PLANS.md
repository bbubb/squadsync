# SquadSync Execution Plans

## Purpose

This document defines SquadSync's standard for execution plans, called ExecPlans.

An ExecPlan is a living implementation plan for complex, multi-step work. It should be self-contained enough that a human or coding agent can restart the task from the plan without relying on hidden chat history.

## When to Use an ExecPlan

Use an ExecPlan for:

- multi-hour implementation work
- major feature slices
- significant refactors
- cross-cutting changes across `apps/api`, `apps/web`, `infra`, and `docs`
- risky migrations or architecture changes
- work that needs milestones, decisions, validation evidence, and recovery notes

Do not require an ExecPlan for:

- small documentation edits
- simple one-issue code changes
- routine PR review
- typo/link fixes
- small scoped follow-up issues

## Relationship to Issues and PRs

GitHub issues remain the executable work record.

ExecPlans are used when an issue needs more design depth before implementation.

A PR that implements an ExecPlan should link both:

- the GitHub issue
- the ExecPlan file, if checked into the repository

## Required Sections

Each ExecPlan should include:

```text
# <Action-oriented title>

## Purpose / Big Picture
## Progress
## Surprises & Discoveries
## Decision Log
## Outcomes & Retrospective
## Context and Orientation
## Plan of Work
## Concrete Steps
## Validation and Acceptance
## Idempotence and Recovery
## Artifacts and Notes
## Interfaces and Dependencies
```

## Living Document Requirements

The `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` sections must be updated as work proceeds.

Each major decision should include:

```text
Decision:
Rationale:
Date/Author:
```

## Validation Standard

Every ExecPlan must explain how success is proven.

For future API work, expected baseline commands are:

```bash
dotnet restore
dotnet build
dotnet test
```

For docs-only work, validation should cover:

- links and paths
- source-of-truth consistency
- stale references
- scope/non-goals

## Safety and Recovery

ExecPlans should prefer additive, reviewable changes. If a step can fail or be repeated, describe how to retry safely.

Do not include destructive operations without an explicit recovery path.

## Minimal Skeleton

```md
# <Action-oriented title>

This ExecPlan follows `PLANS.md` from the repository root.

## Purpose / Big Picture

Explain what someone gains after this change and how to see it working.

## Progress

- [ ] Initial plan drafted.

## Surprises & Discoveries

- None yet.

## Decision Log

- Decision:
  Rationale:
  Date/Author:

## Outcomes & Retrospective

- Not started.

## Context and Orientation

Describe relevant files, paths, terms, and current state.

## Plan of Work

Describe the implementation sequence in prose.

## Concrete Steps

List exact commands and repository paths.

## Validation and Acceptance

Describe tests/checks and expected outcomes.

## Idempotence and Recovery

Explain safe retry/rollback behavior.

## Artifacts and Notes

Capture concise outputs, diffs, or notes that prove progress.

## Interfaces and Dependencies

Name the files, APIs, libraries, and interfaces involved.
```
