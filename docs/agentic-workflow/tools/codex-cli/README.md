# Codex CLI Tool Profile

## Purpose

This directory defines the Codex CLI operational profile for SquadSync.

Codex CLI is the first supported implementation tool for the agentic workflow architecture.

## Role

Codex CLI should implement scoped, `agent-ready` GitHub issues by reading the repository-owned policy, workflow, specs, and relevant architecture docs.

Codex CLI should not depend on private ChatGPT session context to understand what to do.

## Profile Files

- `operational-profile.md` — Codex-specific operating rules and boundaries.
- `context-loading.md` — Required context loading order before work begins.
- `issue-intake.md` — How Codex should interpret and validate issue readiness.
- `task-prompt.md` — Reusable task prompt pattern for Codex CLI.
- `validation.md` — Codex-specific validation reporting expectations.
- `pr-reporting.md` — Pull request summary and review expectations.
- `future-hooks.md` — Candidate scripts/hooks to consider after real Codex usage reveals friction.

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
