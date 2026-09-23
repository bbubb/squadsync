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

- active GitHub issue
- `AGENTS.md`
- `CONTRIBUTING.md`
- `apps/api/AGENTS.md`
- `apps/api/README.md`
- `docs/architecture/system-overview.md`
- `docs/architecture/domain-model.md`
- `docs/agentic-workflow/workflow/coding-standards.md`
- `docs/agentic-workflow/workflow/testing-strategy.md`
- `.agents/skills/squadsync-api-task/SKILL.md`
- `docs/agentic-workflow/tools/codex-cli/rules/README.md` and task-applicable rule files

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
cd apps/api
dotnet restore
dotnet build
dotnet test
```

Document unavailable validation clearly.

## Status

Planned role profile only. No active subagent automation is configured.
