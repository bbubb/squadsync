# ADR 0003: Use Team Membership as the Core Team-User Relationship

## Status

Accepted for Sprint 0 foundation.

Sprint 4 implementation clarification preserves the accepted membership relationship.

Historical vocabulary: [ADR 0006](0006-represented-person-identity-api-contract.md) renames represented `User` to `Person` and `UserId` to `PersonId`; the relationship and role decision below remains unchanged.

## Context

SquadSync needs to represent how people participate in teams. A person may be a coach, player, manager, or viewer depending on the team context.

The MVP needs a model that is easy to understand, test, authorize, and represent in the frontend.

## Decision

SquadSync will model team participation through a `TeamMembership` relationship.

Core relationship:

```
User -> TeamMembership -> Team
TeamMembership -> one constrained TeamRole value
```

A user may have multiple memberships across teams. Each membership describes the user's relationship and has one constrained `TeamRole` value for the MVP. This does not require a persisted, dynamically configurable Role/Permission entity model.

### Sprint 4 Implementation Clarification

A represented `User` may have at most one membership per `Team`, and a `Team` may have many memberships. `TeamRole` is limited to `Owner`, `Coach`, `AssistantCoach`, `Manager`, `Player`, and `Viewer`. The exact fields, identifier invariants, and deferred concerns are defined in the [domain model's current membership contract](../architecture/domain-model.md#current-membership-implementation-contract).

Domain represents membership endpoints by identifiers and remains EF-independent. Following [ADR 0005](0005-ef-core-npgsql-persistence.md), Infrastructure owns foreign keys to `User` and `Team`, restrictive/no-cascade deletion for both relationships, uniqueness on `(UserId, TeamId)`, and readable string persistence for `TeamRole`. There is no separate `Role` table and no Phase 2 repository abstraction merely because membership exists.

## Consequences

### Benefits

- Easy to understand and explain.
- Fits the coach/team/roster workflow.
- Supports team-contextual authorization.
- Keeps frontend workflows straightforward.
- Allows one person to participate in different teams in different ways.

### Trade-offs

- Broader organization modeling is deferred.
- Future expansion may add additional relationships around clubs.
- Authorization should remain simple until product behavior requires more detail.

## Alternatives Considered

### Put Roles Directly on Users

Rejected. User-level roles do not represent team-specific context well. A user may be a coach on one team and a player or viewer on another.

### Use Only Player and Coach Tables Without Membership

Rejected. That would be simple initially but would make multi-team participation and authorization harder later.

## Review Trigger

Revisit this ADR if SquadSync needs to model clubs structures.
