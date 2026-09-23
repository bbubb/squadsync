# Skill: Scaffold API

## Purpose

Use this skill for the initial API scaffold task.

## Expected Outcome

Create a minimal ASP.NET Core API foundation under `apps/api/` that supports future domain, application, infrastructure, API, and test work.

## Required Context

Before work begins, read:

- the active GitHub issue
- `AGENTS.md`
- `CONTRIBUTING.md`
- `apps/api/AGENTS.md`
- `apps/api/README.md`
- `docs/planning/project-roadmap.md`
- `docs/planning/mvp-scope.md`
- `docs/architecture/system-overview.md`
- `docs/architecture/domain-model.md`
- `docs/agentic-workflow/workflow/testing-strategy.md`
- `docs/agentic-workflow/workflow/coding-standards.md`
- `docs/agentic-workflow/tools/codex-cli/context-loading.md`
- `docs/agentic-workflow/tools/codex-cli/issue-intake.md`
- `.agents/skills/squadsync-api-task/SKILL.md`
- `docs/agentic-workflow/tools/codex-cli/rules/README.md` and the task-applicable rules it routes to

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
cd apps/api
dotnet restore
dotnet build
dotnet test
```

## Stop Conditions

Stop if the issue does not define the exact scaffold scope, target .NET version, or validation expectations, or if it names a path other than the canonical `apps/api/` location.
