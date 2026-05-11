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
- Do not reintroduce `IRoleBearer` or similar generalized role-bearing abstractions in the public MVP.
- Keep soccer terminology clear and concrete.
- Avoid generic competition-platform modeling unless an ADR approves it.

## Implementation Defaults

- Use EF Core for persistence.
- Use PostgreSQL as the public target database.
- Use DTOs at API boundaries.
- Use FluentValidation for request validation.
- Use structured logs; avoid noisy debug logs in normal paths.
- Prefer application services/use cases over fat controllers.
- Keep controllers/endpoints thin.

## Testing Expectations

- Add unit tests for domain rules and application behavior.
- Add integration tests for persistence/API behavior when infrastructure exists.
- Avoid tests that only verify framework behavior.

## Safety

Do not expose proprietary soccer-subber optimization internals. The core platform may define an integration contract, but not the private algorithm.
