# Subagent Profile: API Implementation

## Purpose

Planned role for scoped API/backend implementation work under `apps/api/`.

## When to Use

Use this role for future issues that modify:

- ASP.NET Core API endpoints
- application use cases
- domain model behavior
- infrastructure adapters
- persistence wiring
- backend tests

## Required Context

- `AGENTS.md`
- `apps/api/README.md`
- `docs/architecture/system-overview.md`
- `docs/architecture/domain-model.md`
- `docs/agentic-workflow/workflow/coding-standards.md`
- `docs/agentic-workflow/workflow/testing-strategy.md`
- Codex rules under `../rules/`

## Allowed Changes

- API app code under `apps/api/`
- related API tests
- docs updates directly related to changed behavior

## Stop Conditions

Stop if the issue requires:

- changing MVP scope
- changing service boundaries
- adding unapproved infrastructure dependencies
- implementing soccer-subber optimization logic inside SquadSync core
- changing architecture without an ADR

## Validation Expectations

When API scaffold exists, expected validation is:

```bash
dotnet restore
dotnet build
dotnet test
```

Document unavailable validation clearly.

## Status

Planned role profile only. No active subagent automation is configured.
