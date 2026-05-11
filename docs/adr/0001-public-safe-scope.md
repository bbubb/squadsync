# ADR 0001: Define the Initial Soccer MVP Scope

## Status

Accepted for Sprint 0 foundation.

## Context

SquadSync needs a clear first product slice. The project should be narrow enough to build quickly while still demonstrating meaningful architecture, full-stack implementation, service boundaries, and cloud-readiness.

The initial scope centers on a coach managing a soccer team, roster, matches, and lineups.

## Decision

SquadSync will begin as a soccer-specific team management and match-planning MVP.

The MVP includes:

- Teams
- Rosters
- Player profiles
- Coach/team roles
- Matches
- Manual lineup planning
- Lineup suggestion integration contracts
- Event/notification readiness

New product areas should be introduced through future issues and, when architectural, ADRs.

## Consequences

### Benefits

- The project remains understandable and shippable.
- The MVP has a clear product story.
- Engineering depth can be demonstrated through architecture, boundaries, tests, and documentation.
- Future service integration has a concrete platform to connect to.

### Trade-offs

- Some broader product directions are deferred.
- The first version will prioritize a focused coach workflow over generalized configurability.
- Future expansion may require new ADRs and refactoring.

## Alternatives Considered

### Build a Generic Team-Sports Platform Immediately

Rejected for now. That would increase modeling complexity before the core soccer workflow is proven.

### Build Only a Simple Soccer CRUD App

Rejected. That would be faster, but it would not demonstrate enough architecture, service boundaries, or cloud-readiness.

## Review Trigger

Revisit this ADR if the MVP needs to support additional sports, broader organization structures, or more advanced role systems.
