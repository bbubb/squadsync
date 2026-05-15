# SquadSync API

The API app will contain the ASP.NET Core backend for SquadSync.

## Planned Role

`apps/api/` will host the modular monolith backend with clear boundaries:

```text
src/
  SquadSync.Api/
  SquadSync.Application/
  SquadSync.Domain/
  SquadSync.Infrastructure/
tests/
  SquadSync.UnitTests/
  SquadSync.IntegrationTests/
```

## Current Status

Not scaffolded yet.

Phase 1 will create the initial .NET solution, projects, health endpoint, Swagger/OpenAPI setup, logging baseline, and test projects.

## Validation Direction

Once scaffolded, expected validation is:

```bash
dotnet restore
dotnet build
dotnet test
```

## References

- [System Overview](../../docs/architecture/system-overview.md)
- [Domain Model](../../docs/architecture/domain-model.md)
- [Testing Strategy](../../docs/agentic-workflow/workflow/testing-strategy.md)
- [Coding Standards](../../docs/agentic-workflow/workflow/coding-standards.md)
