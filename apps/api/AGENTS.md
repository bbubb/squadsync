# API Agent Instructions

## Status

Active for API planning and implementation.

This file provides local guidance for agents working under `apps/api/`.

## Purpose

`apps/api/` contains the SquadSync ASP.NET Core API and backend application layers.

Scaffold the API foundation here before domain or feature work expands.

## Expected Structure

When scaffolded, the API should follow a modular Clean Architecture shape:

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

## Layer Rules

- Domain contains entities, value objects, and domain rules.
- Application contains use cases, interfaces, DTOs, and validation orchestration.
- Infrastructure contains persistence, external clients, logging implementations, and integration adapters.
- API contains endpoints/controllers, request/response contracts, dependency injection, and composition root.
- Dependencies should point inward. Domain must not depend on Application, Infrastructure, or API.

## Testing Expectations

Use a TDD-oriented flow for behavior work:

1. Define expected behavior and validation before implementation.
2. Add or update tests for behavior changes.
3. Run the relevant validation commands.
4. Document any validation limitations in the PR.

Initial expected validation after scaffold exists:

```bash
cd apps/api
dotnet restore
dotnet build
dotnet test
```

## Agent Rules

- Keep changes issue-scoped and PR-sized.
- Do not add frontend, cloud, or soccer-subber implementation here unless the issue explicitly requires it.
- Do not place soccer-subber optimization logic inside the core API.
- Use interfaces/ports for external service boundaries.
- Stop if the issue requires an architectural decision that is not covered by an ADR.

## Context Routing

Use the active issue, root `AGENTS.md`, and `CONTRIBUTING.md` as the implementation baseline. For API work, also follow this file and `.agents/skills/squadsync-api-task/SKILL.md`.

Load deeper sources when the issue or implementation needs them: use `docs/architecture/system-overview.md` for service boundaries, `docs/architecture/domain-model.md` for domain concepts, roadmap or MVP documents for scope questions, coding/testing standards for implementation and behavior changes, and validation guidance for the relevant checks. Check applicable ADRs when a design choice may already be decided. `apps/api/README.md` supplies setup details when needed. Do not preload these documents solely because a task is under `apps/api/`.

Read `PLANS.md` and create an ExecPlan only when the task is complex, cross-cutting, risky, or multi-step. Follow references or investigate further when a dependency, ambiguity, conflict, or architecture concern emerges. Stop when the issue conflicts with canonical guidance or needs an unapproved architectural decision.
