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

## Context Routing

For future web work, start with the active issue, root `AGENTS.md`, `CONTRIBUTING.md`, and this file. Load product, architecture, coding, and validation guidance when the issue or implementation needs that source; for example, consult UX or brand notes for a user-facing design question and the system overview for an API boundary question. Do not preload these documents solely because a task is under `apps/web/`.

Follow references or investigate further when a dependency, ambiguity, conflict, or architecture concern emerges. Read `PLANS.md` when the task is complex, cross-cutting, risky, or multi-step. Preserve the stop conditions above.
