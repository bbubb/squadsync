# ADR 0003: Use Team Membership as the Core Team-User Relationship

## Status

Accepted for Sprint 0 foundation.

## Context

SquadSync needs to represent how people participate in teams. A person may be a coach, player, manager, or viewer depending on the team context.

The MVP needs a model that is easy to understand, test, authorize, and represent in the frontend.

## Decision

SquadSync will model team participation through a `TeamMembership` relationship.

Core relationship:

```text
User -> TeamMembership -> Team
TeamMembership -> Role
```

A user may have multiple memberships across teams. Each membership describes the user's relationship and role within that team.

## Consequences

### Benefits

- Easy to understand and explain.
- Fits the coach/team/roster workflow.
- Supports team-contextual authorization.
- Keeps frontend workflows straightforward.
- Allows one person to participate in different teams in different ways.

### Trade-offs

- Broader organization modeling is deferred.
- Future expansion may add additional relationships around clubs, leagues, or organizations.
- Authorization should remain simple until product behavior requires more detail.

## Alternatives Considered

### Put Roles Directly on Users

Rejected. User-level roles do not represent team-specific context well. A user may be a coach on one team and a player or viewer on another.

### Use Only Player and Coach Tables Without Membership

Rejected. That would be simple initially but would make multi-team participation and authorization harder later.

## Review Trigger

Revisit this ADR if SquadSync needs to model clubs, leagues, organizations, or nested team structures.
