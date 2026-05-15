# Skill: Scaffold API

## Purpose

Use this skill for the Phase 1 API scaffold task.

## Expected Outcome

Create a minimal ASP.NET Core API foundation under `apps/api/` that supports future domain, application, infrastructure, API, and test work.

## Required Context

Before work begins, read:

- `AGENTS.md`
- `docs/planning/project-roadmap.md`
- `docs/planning/mvp-scope.md`
- `docs/architecture/system-overview.md`
- `docs/architecture/domain-model.md`
- `docs/agentic-workflow/workflow/testing-strategy.md`
- `docs/agentic-workflow/workflow/coding-standards.md`
- `apps/api/README.md`
- Codex rules under `rules/`

## Expected Shape

The API scaffold should follow the planned modular structure:

```text
apps/api/
  src/
    SquadSync.Api/
    SquadSync.Application/
    SquadSync.Domain/
    SquadSync.Infrastructure/
  tests/
    SquadSync.UnitTests/
    SquadSync.IntegrationTests/
```

## Validation

Expected validation after scaffold exists:

```bash
dotnet restore
dotnet build
dotnet test
```

## Stop Conditions

Stop if the issue does not define the exact scaffold scope, target .NET version, or validation expectations.
