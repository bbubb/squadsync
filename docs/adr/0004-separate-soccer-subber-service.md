# ADR 0004: Keep Soccer-Subber as a Separate Service

## Status

Accepted for Sprint 0 foundation.

## Context

SquadSync needs a clear service boundary for future lineup assistance. The core platform should own team, roster, match, availability, and lineup data. A separate service can later evaluate that data and return lineup suggestions.

Keeping lineup assistance behind a service boundary also creates a useful learning path for API integration, cloud deployment, serverless patterns, and failure handling.

## Decision

SquadSync will treat `soccer-subber` as a separate service.

The core platform will:

- Define request/response contracts for lineup suggestions
- Provide a mock or adapter boundary during early development
- Store team, roster, match, availability, and lineup data
- Handle service failures gracefully
- Keep manual lineup planning useful even when the external service is unavailable

The `soccer-subber` service will eventually own lineup suggestion behavior.

## Consequences

### Benefits

- Demonstrates service-boundary thinking.
- Creates a natural AWS/serverless learning path.
- Keeps the core platform simpler.
- Allows the lineup assistance service to evolve independently.
- Supports graceful degradation when the service is unavailable.

### Trade-offs

- Adds integration complexity later.
- Requires contract versioning discipline.
- Local development may need mocks or service emulation.
- More moving parts when deployed.

## Contract Direction

The contract may include concepts such as:

- Team identifier
- Match identifier
- Formation
- Available players
- Player preferred positions
- Planning constraints
- Suggested lineup assignments
- Planning summary text

## Alternatives Considered

### Embed Lineup Assistance in SquadSync

Rejected for the initial architecture. That would simplify local development but reduce the value of demonstrating a separate service boundary.

### Build Soccer-Subber First

Rejected for now. The core platform needs enough roster, match, and lineup data before the service can be meaningfully integrated.

## Review Trigger

Revisit this ADR when the first integration contract is implemented or when the service deployment strategy is selected.
