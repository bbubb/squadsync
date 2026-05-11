# ADR 0004: Keep Soccer-Subber as a Separate Service

## Status

Accepted for Sprint 0 foundation.

## Context

SquadSync needs a path to demonstrate microservice integration, cloud/serverless readiness, and future lineup optimization behavior. The owner also wants to keep proprietary optimization details outside the public platform repository.

A separate `soccer-subber` service can provide this boundary. SquadSync can define a public-safe request/response contract while the actual algorithm remains outside the core platform.

## Decision

The public SquadSync monorepo will not contain the proprietary lineup optimization implementation.

Instead:

- SquadSync will define a public-safe service boundary for lineup suggestions.
- The separate `soccer-subber` service will eventually implement the optimization behavior.
- Early SquadSync development may use a mock lineup suggestion adapter.
- Future integration can use HTTP, events, or AWS service patterns depending on the deployment phase.

## Consequences

### Benefits

- Protects private algorithm/IP details.
- Demonstrates service-boundary thinking.
- Creates a natural AWS/serverless learning path.
- Keeps the core platform simpler.
- Allows the optimization service to evolve independently.

### Trade-offs

- Adds integration complexity later.
- Requires contract versioning discipline.
- Local development may need mocks or service emulation.
- More moving parts when deployed.

## Public Contract Direction

The public contract may include concepts such as:

- Team identifier
- Match identifier
- Formation
- Available players
- Player preferred positions
- Simple planning constraints
- Suggested lineup assignments
- Public-safe summary text

The public contract should not include proprietary scoring methods, ranking formulas, or detailed optimization heuristics.

## Alternatives Considered

### Embed Optimization in SquadSync

Rejected. This would simplify local development but would expose private logic and reduce the architecture value of a separate service boundary.

### Keep Optimization Entirely Undocumented

Rejected. The public platform should still show that it is designed for integration and cloud-native expansion.

### Build Soccer-Subber First

Rejected for now. The core platform needs enough roster/match/lineup data before the optimization service can be meaningfully integrated.

## Review Trigger

Revisit this ADR when the first integration contract is implemented or when the service deployment strategy is selected.
