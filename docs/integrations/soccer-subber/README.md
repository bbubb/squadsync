# Soccer-Subber Integration

This directory documents the planned integration boundary between SquadSync and the separate `soccer-subber` service.

## Current Status

`soccer-subber` remains a separate service/repository.

SquadSync should not embed the substitution optimization algorithm inside the core platform. Instead, SquadSync should define a clean integration boundary.

## Planned SquadSync Responsibilities

SquadSync may contain:

- lineup suggestion request/response contracts
- application ports/interfaces
- mock adapters for local/demo behavior
- HTTP or event adapter later
- failure handling and fallback behavior
- documentation for integration assumptions

## Non-Goals

- Do not duplicate `soccer-subber` source code in this repository.
- Do not implement optimization logic in the SquadSync core platform.
- Do not require the external service for the early manual lineup MVP.

## References

- [System Overview](../../architecture/system-overview.md)
- [MVP Scope](../../planning/mvp-scope.md)
- [Project Roadmap](../../planning/project-roadmap.md)
