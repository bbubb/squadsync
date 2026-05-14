# Agentic Workflow Lifecycle

## Purpose

This lifecycle defines how work moves through SquadSync from planning to review.

The workflow is sequential by default but role-separated so it can evolve toward multi-agent execution later.

## Lifecycle

```text
1. Plan
2. Specify
3. Confirm readiness
4. Implement
5. Validate
6. Open PR
7. Review
8. Merge or iterate
9. Capture follow-up work
```

## 1. Plan

ChatGPT and the human owner define phase, sprint, issue, and architecture direction.

Planning output should be captured in repository docs, GitHub issues, ADRs, or PRs. Important decisions should not live only in chat history.

## 2. Specify

Work is converted into GitHub issues with:

- objective
- context
- scope
- non-goals
- relevant docs/ADRs
- acceptance criteria
- validation expectations
- stop conditions

## 3. Confirm Readiness

An issue is `agent-ready` only when an implementation agent can begin without making new architecture decisions.

If scope, validation, or architecture is unclear, the agent should stop and request clarification or create a follow-up planning issue.

## 4. Implement

Codex CLI is the primary implementation agent for scoped issues.

Implementation should happen in a task branch and remain inside the issue scope.

## 5. Validate

The implementation agent runs the relevant validation gates or clearly documents why validation could not be run.

Validation expectations are defined in `docs/agentic-workflow/workflow/validation-gates.md`.

## 6. Open PR

The pull request should summarize:

- changed areas
- issue acceptance criteria status
- validation performed
- docs/ADR impact
- known limitations
- follow-up work

## 7. Review

The human owner reviews for:

- scope fit
- architecture fit
- readability
- validation quality
- service-boundary clarity
- portfolio/hiring readability

## 8. Merge or Iterate

Merge only after the human owner approves.

If implementation reveals a missing decision, pause and create or update an ADR before continuing.

## 9. Capture Follow-up Work

Follow-ups should become GitHub issues, roadmap updates, ADRs, or harness friction notes.

Do not leave important discoveries only in chat or PR comments.
