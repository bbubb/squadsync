# Codex Workflow Rules

## Purpose

These rules define how Codex CLI should move work through issues, branches, validation, and PRs.

## Issue Rules

Codex should work from `agent-ready` issues.

The label is a routing signal. Codex must still perform the readiness check in `docs/agentic-workflow/tools/codex-cli/issue-intake.md` before implementation.

Before implementation, Codex should verify:

- objective is clear
- scope is narrow
- non-goals are explicit
- acceptance criteria are reviewable
- validation expectations are defined
- governing docs/ADRs are named by exact path or resolved through the repository routing docs

## Branch Rules

Codex must use a short-lived, issue-scoped branch from `main`.

Before editing any file, Codex must establish the repository state, verify that the working tree is clean or that any existing changes are understood and preserved, and create or switch to the issue branch. Verify the active branch and expected working tree before edits. If repository state cannot be established or branch creation fails, stop before modifying files and report the blocker.

Recommended pattern:

```text
feature/<short-topic>
fix/<short-topic>
docs/<short-topic>
chore/<short-topic>
```

## PR Rules

Commit and push the issue-scoped branch and open a draft PR after scoped implementation and available validation. These steps are part of normal task completion. Validation limitations must be reported in the draft PR; they should not prevent draft-PR creation by default. If a documented blocker prevents commit, push, or PR creation, report the handoff as incomplete, identify the failed step, and do not claim the task is complete.

Codex must not merge its own PR or close the related issue.

Codex draft PRs must include:

- linked issue
- summary
- scope
- non-goals
- acceptance criteria status
- validation results
- documentation/ADR impact
- known limitations and remaining review risks
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
