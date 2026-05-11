# ADR 0002: Use ASP.NET Core for the Public Backend

## Status

Accepted for Sprint 0 foundation.

## Context

The archived SquadSync backend was previously built with C# and ASP.NET Core concepts, including API controllers, DTOs, validation, logging, Entity Framework Core, repositories, and MySQL persistence. The public rebuild should reuse the strongest parts of that technical direction while cleaning up scope and implementation.

The project also needs to support the owner's career goals: backend/cloud architecture, integration engineering, AWS learning, and enterprise-readable software design.

## Decision

The public SquadSync backend will use C# and ASP.NET Core Web API.

The backend will be organized as a modular monolith with separate projects for:

- `SquadSync.Api`
- `SquadSync.Application`
- `SquadSync.Domain`
- `SquadSync.Infrastructure`
- test projects

The initial persistence target will be PostgreSQL through Entity Framework Core.

## Consequences

### Benefits

- Builds on prior SquadSync experience.
- Provides strong enterprise/backend hiring signal.
- Supports clear layering and testability.
- Works well with Docker, CI/CD, and cloud deployment paths.
- Aligns with structured logging, validation, and API documentation practices.

### Trade-offs

- ASP.NET Core may add more ceremony than a smaller framework.
- The frontend and backend will use different languages.
- PostgreSQL introduces a database the owner has not used deeply before.

## Alternatives Considered

### Spring Boot

Considered because there was a prior Spring Boot SquadSync repo and Java is valuable in enterprise environments. Rejected for the public rebuild because the C# version appears to contain the deeper architectural investment and aligns well with the intended backend portfolio story.

### Node/TypeScript Backend

Considered because it would match the frontend language. Rejected because ASP.NET Core provides stronger continuity with the archived work and better enterprise backend signal for the intended career path.

### Python Backend

Rejected for the core platform. Python remains a good fit for the separate `soccer-subber` service, especially if it evolves toward optimization or AI-assisted behavior.

## Review Trigger

Revisit this ADR if the project target changes from backend/cloud portfolio signal to rapid product prototyping or if deployment constraints strongly favor another stack.
