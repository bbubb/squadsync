# Codex Architecture Rules

## Purpose

These rules define architecture boundaries Codex CLI must preserve.

## Backend Rules

Codex must preserve the intended dependency direction:

```text
API -> Application -> Domain
Infrastructure -> Application
Infrastructure -> Domain
```

Codex must keep:

- API layer thin
- Application layer responsible for use-case orchestration
- Domain layer independent of infrastructure and frameworks
- Infrastructure layer responsible for persistence and external adapters

## Domain Rules

Codex must:

- use soccer-specific domain language
- keep the first MVP explicit and understandable
- avoid premature generalization into a broad competition platform
- preserve `User`, `Team`, `TeamMembership`, `Role`, `PlayerProfile`, `Match`, `Lineup`, and related concepts unless an ADR changes the model

## Integration Boundary Rules

Codex must not implement soccer-subber optimization logic inside the SquadSync core platform.

SquadSync may define:

- request/response contracts
- application ports/interfaces
- mock adapters
- failure handling rules

The actual optimization algorithm belongs outside the core platform unless a future ADR explicitly changes that direction.

## Architecture Decision Rules

Codex must stop and request human/ADR review when work requires:

- a new service boundary
- a new architectural pattern
- a new infrastructure dependency
- a new persistence strategy
- a major domain model change
- a change to MVP scope
