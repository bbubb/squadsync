# ADR 0001: Keep the Public MVP Soccer-Specific and Portfolio-Safe

## Status

Accepted for Sprint 0 foundation.

## Context

SquadSync has a broader long-term vision involving competition management, flexible roles, and potentially reusable platform concepts. The public repository is intended to demonstrate engineering ability without revealing private IP, strategic product direction, or proprietary algorithmic details.

A public MVP must be useful and credible, but it should not act as a roadmap for the full private concept.

## Decision

The public SquadSync repository will be scoped to a soccer-specific team management and match-planning MVP.

The public MVP may include:

- Teams
- Rosters
- Player profiles
- Coach/team roles
- Matches
- Manual lineup planning
- Public-safe integration contracts
- Event/notification readiness

The public MVP must not include:

- Generalized competition-platform abstractions
- Universal multi-sport configuration
- Proprietary substitution algorithm internals
- Private platform strategy
- Advanced generalized role frameworks

## Consequences

### Benefits

- The project remains understandable and shippable.
- The public repository becomes safer to share with hiring reviewers.
- The MVP has a clear product story.
- Engineering depth can still be demonstrated through architecture, boundaries, tests, and documentation.

### Trade-offs

- Some long-term extensibility ideas are intentionally hidden or deferred.
- Public code may look narrower than the full private vision.
- Future expansion may require new ADRs and refactoring.

## Alternatives Considered

### Expose the Broader Platform Direction

Rejected. This would risk revealing too much private product strategy and would likely overcomplicate the public MVP.

### Build a Generic Team-Sports Platform

Rejected for now. It would improve apparent extensibility but would also push the project back toward the hidden platform concept.

### Build Only a Simple Soccer CRUD App

Rejected. That would be safer but would not demonstrate enough architecture, service boundaries, or cloud-readiness.

## Review Trigger

Revisit this ADR if the public MVP needs to support additional sports, broader competition structures, or more advanced role systems.
