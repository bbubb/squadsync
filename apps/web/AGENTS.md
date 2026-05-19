# Web Agent Instructions

## Status

Placeholder for a planned implementation area.

This file reserves local guidance for future agents working under `apps/web/`. It is intentionally concise until the frontend phase begins.

## Purpose

`apps/web/` will contain the SquadSync frontend application.

The expected direction is a type-safe, feature-oriented frontend that consumes the SquadSync API through explicit service boundaries.

## Planned Rules

When frontend work begins:

- Follow the frontend technology decisions accepted by ADR.
- Keep feature code organized by user workflow, not by generic technical buckets alone.
- Use typed API access and avoid duplicating backend domain rules in UI state.
- Preserve accessibility, responsive layout, and clear coach-facing workflows.
- Add validation commands to this file when the frontend scaffold exists.

## Stop Conditions

Agents should stop before adding frontend code if:

- the frontend stack has not been confirmed by ADR or roadmap update;
- the work depends on API contracts that do not exist yet;
- the task would introduce product scope beyond the MVP.

## Required Context

Before future web work, review:

- `AGENTS.md`
- `CONTRIBUTING.md`
- `docs/product/ux-notes.md`
- `docs/product/brand-notes.md`
- `docs/architecture/system-overview.md`
- `docs/agentic-workflow/workflow/coding-standards.md`
- `docs/agentic-workflow/workflow/validation-gates.md`
