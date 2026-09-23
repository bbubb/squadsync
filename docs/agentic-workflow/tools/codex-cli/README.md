# Codex CLI Tool Profile

## Purpose

This directory defines the Codex CLI operational profile for SquadSync.

Codex CLI is the first supported implementation tool for the agentic workflow architecture.

## Role

Codex CLI should implement scoped, `agent-ready` GitHub issues after revalidating the issue body against current repository guidance; the label alone is not proof of readiness.

Codex CLI should not depend on private ChatGPT session context to understand what to do.

## Profile Files

- [Operational profile](operational-profile.md) — how Codex CLI fits into the SquadSync workflow.
- [Context loading](context-loading.md) — required context loading order before work begins.
- [Issue intake](issue-intake.md) — how Codex should interpret and validate issue readiness.
- [Task prompt](task-prompt.md) — reusable task prompt pattern for Codex CLI.
- [Validation](validation.md) — Codex-specific validation reporting expectations.
- [PR reporting](pr-reporting.md) — pull request summary and review expectations.

## Codex-Native Structure

- [Rules](rules/README.md) — persistent Codex behavior constraints.
- [Skills](skills/README.md) — reusable Codex task playbooks.
- [Hooks](hooks/README.md) — lifecycle automation points and planned validation hooks.
- [Subagents](subagents/README.md) — planned specialized roles for future orchestration.

## Task-Specific Prompts

- [Initial API Scaffold](../../../prompt-library/backend-scaffold.prompt.md) — copy-ready scaffold prompt that must be populated from a refreshed agent-ready issue.
- [Pull Request Review](../../../prompt-library/pr-review.prompt.md) — bounded review prompt aligned with the PR specification and human merge authority.

## Relationship to Generic Workflow

Generic concepts live in:

- `docs/agentic-workflow/policy/`
- `docs/agentic-workflow/workflow/`
- `docs/agentic-workflow/specs/`

This profile maps those concepts into Codex CLI usage.

## Non-Goals

This profile does not:

- implement application code
- define Claude/Cursor behavior
- create a universal adapter layer
- automate merges
- replace human architectural review
