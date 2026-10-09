# ADR 0006: Represent People Separately from Authentication Identity

## Status

Proposed for Phase 3 / Sprint 7.

## Context

SquadSync represents coaches, players, and managers who may never sign in. Phase 2 calls these people `User`, which can be confused with a future login identity. Before adding HTTP contracts, we need consistent business vocabulary and a safe access boundary while authentication remains deferred.

## Decision

- **Soccer identity:** The represented domain entity will be `Person`, its collection `People`, and its ID `PersonId`. `TeamMembership` continues to associate a person with a team and carries one `TeamRole`. `PlayerProfile` remains person-level; `RosterEntry` remains team-contextual.
- **Authentication identity:** A future `Account` or identity-provider principal will describe who can sign in, not every person represented in SquadSync. Account-to-person linking, guardian delegation, provider selection, and login storage are deferred. Neither a supplied `PersonId` nor a `TeamRole` proves caller authority. Future authorization must combine authenticated identity, verified authority over a person, and team/resource-specific policy.
- **HTTP boundary:** The first API resources use `/api/people` and `/api/teams`, each with `POST` and `GET /{id}`. Creation uses Application-generated GUIDs and returns `201 Created` with a `Location` header; retrieval returns `200` or `404`. Use ASP.NET Core controllers, Application use cases, built-in validation, and standard ProblemDetails for invalid and failed requests. No generic repository or FluentValidation dependency is required yet.
- **Interim security:** Until authentication exists, these management endpoints are available **only in Development** and only on a trusted local environment. They must not be mapped in Staging or Production. Environment restrictions are not a substitute for authentication or authorization.
- **Team creation:** Creating a person or team does not automatically establish an authenticated owner or create a team membership. Those workflows require later explicit design.

## Consequences

- The `User` → `Person` change requires a new **data-preserving** EF Core migration, including the `Users` → `People` table and dependent `UserId` → `PersonId` columns. Existing rows, identifiers, relationships, unique constraints, and restrictive deletion semantics must remain intact. Do not rewrite applied migrations.
- Until that change is implemented, the current `User` code and documentation remain accurate. The rename issue must update active domain documentation alongside the implementation. Historical migrations, completed issues, and prior ADR decisions remain traceable.
- This decision does not introduce an `Account` entity, authentication, account delegation, or RBAC implementation.

## Alternatives considered

- **Keep `User` for represented people:** Technically valid, but ambiguous for rostered people who never access the software.
- **Expose `/api/people` while retaining `User` internally:** Possible, but adds unnecessary vocabulary translation before the API is established.
- **Implement accounts and permissions now:** Adds complexity without a current authenticated workflow.

## References

- [Explicit membership model (ADR 0003)](0003-use-explicit-membership-model.md)
- [EF Core persistence boundary (ADR 0005)](0005-ef-core-npgsql-persistence.md)
- [Current domain model](../architecture/domain-model.md)
- [Phase 3 roadmap](../planning/project-roadmap.md)
