# Backend Instructions

Apply these instructions to backend changes under `backend/`.

## Target Architecture

Use a modular ASP.NET Core architecture:

```text
backend/
  src/
    SquadSync.Api/
    SquadSync.Application/
    SquadSync.Domain/
    SquadSync.Infrastructure/
  tests/
    SquadSync.UnitTests/
    SquadSync.IntegrationTests/
```

## Dependency Direction

Preferred dependency flow:

```text
Api -> Application -> Domain
Infrastructure -> Application -> Domain
```

Domain should not depend on API, Infrastructure, EF Core, ASP.NET Core, or external services.

## Domain Modeling Rules

- Use explicit entities and relationships.
- Model team participation through `TeamMembership`.
- Keep soccer terminology clear and concrete.
- Avoid broad product modeling unless an ADR approves it.

## Implementation Defaults

- Use EF Core for persistence.
- Use PostgreSQL as the target database.
- Use DTOs at API boundaries.
- Use FluentValidation for request validation.
- Use structured logs; avoid noisy debug logs in normal paths.
- Prefer application services/use cases over fat controllers.
- Keep controllers/endpoints thin.

## Testing Expectations

- Add unit tests for domain rules and application behavior.
- Add integration tests for persistence/API behavior when infrastructure exists.
- Avoid tests that only verify framework behavior.

## Service Boundaries

The core backend may define interfaces and contracts for external lineup assistance, but it should not implement the lineup suggestion algorithm unless a future approved issue requires that work.
