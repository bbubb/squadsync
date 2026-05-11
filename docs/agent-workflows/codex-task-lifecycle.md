# Codex Task Lifecycle

## Status

Draft for Sprint 0 review.

## Purpose

This document defines how AI-assisted implementation work should flow in SquadSync. The goal is to make Codex, Copilot, and ChatGPT useful without allowing them to create architectural drift or expose private IP.

## Workflow Summary

```text
Architecture docs define direction.
GitHub issues define work.
Codex implements small tasks.
Human review validates the result.
Docs and ADRs preserve decisions.
```

## Roles

### Human Owner

The human owner approves scope, reviews code, runs the application locally, and decides what is merged.

### ChatGPT Architect / Technical Lead

ChatGPT helps produce:

- architecture docs
- ADRs
- issue breakdowns
- Codex prompts
- PR review checklists
- debugging strategies
- implementation sequencing

### Codex

Codex should implement narrow, issue-scoped changes against the repository.

### GitHub Copilot

Copilot should assist inside the editor with local implementation details, boilerplate, tests, and refactors.

## Task Lifecycle

### 1. Define the Work

Every agent-ready task should include:

- clear objective
- files/areas likely affected
- explicit non-goals
- acceptance criteria
- testing expectations
- relevant docs/ADRs

### 2. Confirm Scope

Before implementation, verify:

- task fits the MVP scope
- task does not expose private IP
- architecture is already documented or an ADR exists
- task is small enough for one PR

### 3. Generate or Assign the Task

Create a GitHub issue or task prompt. Add `agent-ready` only when the issue is clear enough for implementation without additional architecture decisions.

### 4. Implement in a Branch

Codex should work in a branch named for the task, such as:

```text
feature/backend-scaffold
feature/team-membership-model
fix/lineup-slot-validation
```

### 5. Validate Locally

A task is not complete until the relevant checks are run or the limitation is documented.

Expected validation later:

- `dotnet build`
- `dotnet test`
- frontend build/lint
- formatting checks
- Docker Compose startup where relevant

### 6. Open a Pull Request

The PR should include:

- summary
- changed areas
- acceptance criteria status
- tests run
- known limitations
- docs/ADR updates

### 7. Human Review

Review for:

- scope fit
- architecture fit
- readability
- maintainability
- test coverage
- public-safety/IP risk
- hiring-facing clarity

### 8. Merge or Iterate

Merge only after the human owner is satisfied. If the change reveals a missing decision, pause and create or update an ADR before continuing.

## Agent-Ready Issue Template

```markdown
## Objective

## Context

## Scope

## Non-Goals

## Relevant Docs/ADRs

## Acceptance Criteria

## Validation

## Notes for Agent
```

## Prompt Pattern for Codex

Use this shape:

```text
You are working in the SquadSync repository.

Read:
- AGENTS.md
- docs/planning/mvp-scope.md
- docs/architecture/system-overview.md
- docs/architecture/domain-model.md
- relevant ADRs

Task:
[clear objective]

Constraints:
[scope and non-goals]

Acceptance criteria:
[checklist]

Do not:
[explicit boundaries]
```

## Common Failure Modes

### Too Much Scope

Symptom: Codex implements multiple product areas at once.

Prevention: split issues aggressively.

### Architecture Drift

Symptom: new folders, libraries, or patterns appear without explanation.

Prevention: require ADRs for architecture changes.

### Public IP Leakage

Symptom: docs or code describe hidden platform strategy or optimization details.

Prevention: keep soccer-specific public MVP language and review docs carefully.

### Framework-Centered Code

Symptom: generated code follows framework examples but not project architecture.

Prevention: point agents to repository docs first.

### Test-Free Behavior

Symptom: behavior is added without validation.

Prevention: acceptance criteria must include testing expectations.

## Merge Standard

A PR is mergeable when it is:

- inside scope
- architecturally consistent
- reasonably tested
- readable
- documented where needed
- free of private IP leakage
- understandable to a future reviewer
