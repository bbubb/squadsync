# GitHub Copilot / GHCP Tool Profile

## Purpose

This profile defines the current role of GitHub Copilot / GHCP in SquadSync.

Copilot may help with local editor assistance, cleanup, refactoring, boilerplate, and review support. It is not a required formal workflow stage.

## Current Role

Copilot can be used for:

- local implementation suggestions
- boilerplate assistance
- test-writing assistance
- small refactors
- review support
- explanation of code changes

## Boundaries

Copilot should not be treated as:

- the primary implementation agent
- the canonical source of workflow state
- an autonomous reviewer with merge authority
- a replacement for Codex CLI validation
- a required step in every issue

## Relationship to Codex CLI

Codex CLI is the primary implementation agent for scoped `agent-ready` issues.

Copilot may assist the human owner locally, but the repo workflow should not depend on Copilot being available.

## Future Review

This profile may be expanded if Copilot becomes a formal part of the workflow later.

Any expansion should be captured through a workflow issue and, if architecture-relevant, an ADR.
