# Codex Workflow Rules

## Purpose

These rules define how Codex CLI should move work through issues, branches, validation, and PRs.

## Issue Rules

Codex should work from `agent-ready` issues.

Before implementation, Codex should verify:

- objective is clear
- scope is narrow
- non-goals are explicit
- acceptance criteria are reviewable
- validation expectations are defined
- relevant docs/ADRs are known

## Branch Rules

Codex should use short-lived branches from `main`.

Recommended pattern:

```text
feature/<short-topic>
fix/<short-topic>
docs/<short-topic>
chore/<short-topic>
```

## PR Rules

Codex PRs should include:

- linked issue
- summary
- scope
- non-goals
- acceptance criteria status
- validation results
- documentation/ADR impact
- follow-up work

## Validation Rules

Codex should run available validation gates before reporting completion.

If validation cannot be run, Codex must document:

- expected validation
- reason not run
- risk or limitation
- suggested human follow-up

## Stop Rules

Codex should stop when:

- scope is unclear
- validation is unclear
- docs conflict
- task requires an unapproved ADR
- task becomes broader than one PR
- implementation would violate architecture or MVP boundaries
