# Subagent Profile: Web Implementation

## Purpose

Planned role for scoped web/frontend implementation work under `apps/web/`.

## When to Use

Use this role for future issues that modify:

- Next.js app structure
- coach dashboard UI
- team/roster/match/lineup screens
- frontend service modules
- frontend tests
- UX behavior tied to MVP workflows

## Required Context

- `AGENTS.md`
- `apps/web/README.md`
- `docs/planning/mvp-scope.md`
- `docs/product/product-brief.md`
- `docs/product/ux-notes.md`
- `docs/product/brand-notes.md`
- `docs/agentic-workflow/workflow/coding-standards.md`

## Allowed Changes

- web app code under `apps/web/`
- related frontend tests
- docs updates directly related to changed UX behavior

## Stop Conditions

Stop if the issue requires:

- selecting a full design system without approval
- changing product scope
- adding unrelated frontend framework complexity
- introducing global state without a documented need
- implementing backend behavior in the frontend

## Validation Expectations

When web scaffold exists, expected validation will be defined by the web app package scripts.

Potential future commands:

```bash
npm run lint
npm run build
npm test
```

Document unavailable validation clearly.

## Status

Planned role profile only. No active subagent automation is configured.
