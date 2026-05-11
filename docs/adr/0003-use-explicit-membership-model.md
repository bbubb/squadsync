# ADR 0003: Use Explicit Team Membership Instead of Generalized Role Bearers

## Status

Accepted for Sprint 0 foundation.

## Context

The archived SquadSync system experimented with a generalized role-bearing model. That direction allowed multiple entity types, such as users and organizational units, to participate in role assignment patterns.

That abstraction has long-term theoretical value, but it increases complexity for the public MVP. It also begins to reveal broader platform ideas that are intentionally outside the public scope.

The MVP needs a clear, soccer-specific model that supports roster management, team roles, and future authorization without premature generalization.

## Decision

The public MVP will model team participation through an explicit `TeamMembership` relationship.

Core relationship:

```text
User -> TeamMembership -> Team
TeamMembership -> Role
```

A user may have multiple memberships across teams. Each membership describes the user's relationship and role within that team.

The public MVP will not use `IRoleBearer`, generic role-bearing interfaces, or broad organization-unit inheritance patterns.

## Consequences

### Benefits

- Easier to understand and explain.
- Better fit for the soccer-focused MVP.
- Easier to authorize because permissions are team-contextual.
- Avoids premature abstraction.
- Keeps broader private platform design hidden.
- Makes frontend workflows simpler.

### Trade-offs

- Less flexible than a generalized role-bearing model.
- Future expansion to organizations, clubs, leagues, or competitions may require additional modeling.
- Some archived code concepts will not be ported directly.

## Alternatives Considered

### Keep `IRoleBearer`

Rejected for the public MVP. It is powerful but too abstract for the first rebuild and exposes too much of the broader platform direction.

### Put Roles Directly on Users

Rejected. User-level roles do not represent team-specific context well. A user may be a coach on one team and a player or viewer on another.

### Use Only Player and Coach Tables Without Membership

Rejected. That would be simple initially but would make multi-team participation and authorization harder later.

## Review Trigger

Revisit this ADR if SquadSync needs to model clubs, leagues, organizations, or nested team structures in the public repo.
