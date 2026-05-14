# Stop Conditions

## Purpose

Stop conditions define when an agent should pause instead of guessing, expanding scope, or making unapproved architecture decisions.

Stopping is not failure. It is a guardrail that protects project scope and architecture quality.

## Required Stop Conditions

An agent must stop when:

- the issue lacks a clear objective
- acceptance criteria are missing or contradictory
- the requested change is broader than one focused PR
- the task requires an architecture decision not covered by an ADR
- relevant docs conflict with each other
- validation expectations are missing or impossible to determine
- the task would add product scope outside the documented MVP
- the task would introduce new infrastructure, cloud services, or dependencies without approval
- the task would implement soccer-subber optimization logic inside the SquadSync core platform
- the task would change tool workflow, agent behavior, or harness structure without a workflow issue

## Recommended Agent Response

When a stop condition is hit, the agent should report:

```markdown
## Stop Condition Hit

Reason:

Relevant docs/issues:

Decision needed:

Recommended next action:
```

## Follow-up Paths

Depending on the blocker, the next action may be:

- clarify the issue
- split the issue
- create an ADR
- update the roadmap
- update workflow docs
- create a harness friction note
- request human owner review

## Human Review

The human owner decides whether to unblock the task, change scope, create follow-up work, or close the task as not ready.
