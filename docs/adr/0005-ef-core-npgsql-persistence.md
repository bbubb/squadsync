# ADR 0005: Use EF Core with Npgsql for PostgreSQL Persistence

## Status

Accepted for Phase 2 / Sprint 3.

## Context

Phase 2 introduces the first application persistence. The API foundation already uses PostgreSQL for local development and readiness checks, but it does not yet persist domain data. The project needs a clear ORM/provider choice and ownership boundary before implementation begins.

Persistence abstractions should support concrete application use cases without imposing generic infrastructure that the MVP does not need.

## Decision

SquadSync will use Entity Framework Core with the Npgsql provider and PostgreSQL for application persistence.

`SquadSync.Infrastructure` owns:

- the EF Core `DbContext`;
- EF Core entity configurations and provider packages;
- PostgreSQL migrations and their lifecycle.

`SquadSync.Domain` remains independent of EF Core, Npgsql, PostgreSQL, and other infrastructure packages. Infrastructure maps domain concepts to persistence without making the Domain project depend on persistence technology.

Introduce repository or query abstractions in `SquadSync.Application` only when a concrete use case needs a persistence capability. Define the smallest useful contract for that use case; do not require one repository per entity or add a generic repository by default.

## Consequences

### Benefits

- Uses the PostgreSQL database already established for local development.
- Keeps ORM and provider dependencies within Infrastructure.
- Keeps migrations and schema lifecycle in one owning project.
- Lets application contracts follow actual use cases rather than a repository-per-entity convention.

### Trade-offs

- EF Core mapping and provider configuration require Infrastructure-level tests and maintenance.
- Use cases may need explicit query/write abstractions as requirements emerge.
- Migrations must be reviewed as schema changes and kept aligned with domain decisions.

## Alternatives Considered

### Keep persistence provider-agnostic before implementation

Rejected for the initial persistence work. PostgreSQL is already the selected database, and provider abstraction would add complexity without a current portability requirement.

### Put EF Core references in Domain

Rejected. It would couple domain rules and entities to infrastructure persistence technology.

### Mandate repository-per-entity

Rejected. That would create abstractions before use cases establish which persistence capabilities are needed.

## Review Trigger

Revisit this ADR if PostgreSQL or EF Core no longer fits the product's concrete persistence requirements, or if a new architecture decision changes layer ownership.
