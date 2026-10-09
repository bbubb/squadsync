# ADR 0006: Represented People, Identity Boundary, and Initial HTTP Contract

## Status

Proposed for Phase 3 / Sprint 7 ([Issue #117](https://github.com/bbubb/squadsync/issues/117)). Requires human PR review and acceptance before implementation. The deployed Phase 2 model still uses `User` and `UserId`; the rename is separately scoped to #118.

## Context

A SquadSync roster includes people who may never sign in, notably youth players. Phase 2 models these people as `User`, while account identity and authentication are deferred. Exposing a public resource named `users` now would make it harder to distinguish represented people from future login identities. Sprint 7 also needs an explicit HTTP contract and safe boundary for unauthenticated development endpoints.

The accepted `TeamMembership` model (ADR 0003) and Infrastructure-owned persistence (ADR 0005) remain in force.

## Decision

### Soccer-domain identity

- Rename the represented Domain `User` entity to `Person`; use `PersonId` in `TeamMembership` and `PlayerProfile`. Name the EF Core collection `People` and the corresponding HTTP resource `/api/people`.
- Preserve one `TeamMembership` per person/team pair, one constrained `TeamRole` per membership, and player-only roster details attached to player memberships. This is a vocabulary change, not a new relationship or authorization model.
- A `Person` does not need an account or the ability to sign in. `Account`, `IdentityUser`, or an identity-provider principal is a *future* identity/access concept. No login schema, identity provider, account-person cardinality, or delegation model is chosen by this ADR.
- The future authorization flow must establish the authenticated principal, verify its authority to act for a particular `Person`, then evaluate team/resource-specific permissions. `TeamRole` is soccer-domain participation data, not authentication and not automatically a global security permission. Client-supplied IDs or role values never prove caller authority.
- Future identity/access responsibilities may remain a bounded module of the modular monolith; a separate authentication service is not required.

### Initial Phase 3 HTTP contract

Use thin ASP.NET Core controllers backed by Application use cases and Application-owned persistence ports (ADR 0005). The minimal Sprint 7 resources are:

| Method | Route | Success |
|---|---|---|
| POST | `/api/people` | `201 Created`, response body and resource `Location` |
| GET | `/api/people/{id}` | `200 OK` or `404 Not Found` |
| POST | `/api/teams` | `201 Created`, response body and resource `Location` |
| GET | `/api/teams/{id}` | `200 OK` or `404 Not Found` |

- The Application layer generates entity GUIDs. Clients do not assign creation IDs. Initial contracts contain person first/last name or team name, respectively, and return normalized values and ID.
- Invalid input returns standard HTTP `400` validation details. Missing resources return `404` ProblemDetails. Unexpected failures must not expose credentials, database details, or stack traces. Use ASP.NET Core's built-in validation and ProblemDetails; defer FluentValidation until complex validation actually warrants it. Do not add a custom envelope, API versioning, MediatR, or generic repositories.
- Person and Team creation are independent in Sprint 7. Creating a Team does **not** prove caller ownership or automatically create an Owner `TeamMembership`. Membership/role assignment, roster endpoints, updates, and authentication are later work.

### Interim access boundary

Without authentication, management endpoints are **Development-only** and must not be mapped in Staging or Production. Development use assumes a trusted local environment and no public exposure. Environment gating is a temporary safety restriction, **not** authentication or authorization. Preserve existing environment-independent health/readiness endpoints. Public deployment of management routes requires a separate, reviewed access-control design.

## Implementation boundary and migration safety

Issue #117 changes documentation only. Issue #118 owns the code/schema transition:

- Rename Domain `User` and dependent `UserId` identifiers in current application code, tests, EF configurations and development seed to `Person`/`PersonId`.
- Add a **new**, reviewed, data-preserving EF Core migration from `Users` to `People` and from dependent `UserId` columns to `PersonId`, including necessary foreign-key/index/constraint names. Keep all existing GUIDs, row data, one-to-one/unique constraints, and restrictive delete behavior intact.
- Do not rewrite historical, already-applied Phase 2 migrations or drop/recreate populated tables. Validate upgrade on existing related records, including demo seed behavior, and review/test rollback on disposable data.
- Until #118 is merged, documentation referencing the *implemented* Phase 2 `User` entity, `Users` table, and `UserId` fields remains factually correct. Historical sprint contracts and accepted decisions retain their original terminology with this ADR as the forward-looking naming decision.

Issue #119 owns HTTP safety/validation wiring; #120–#122 own API operations and tests. No runtime, schema, authentication, or RBAC code is included here.

## Alternatives considered

- **Keep `User` throughout the soccer domain and HTTP:** valid technically, but misleading for rostered people who never use the app and ambiguous when account identity arrives.
- **Use `Person` for HTTP but retain `User` internally:** a permissible boundary translation, but unnecessary vocabulary friction while the codebase is small.
- **Create an `Account` entity and permissions framework now:** rejected as premature; no authenticated use case exists yet.

## Consequences and review triggers

The rename requires a carefully reviewed schema migration and updates to existing dependent code/tests; its cost is justified before HTTP clients depend on resource names. Revisit identity/account linking, delegated guardianship, authorization policies, and role-to-permission rules when a real authenticated workflow is scoped. Do not infer those future structures from this ADR.

## References

- [Domain model](../architecture/domain-model.md)
- [System overview](../architecture/system-overview.md)
- [MVP scope](../planning/mvp-scope.md)
- [ADR 0003](0003-use-explicit-membership-model.md)
- [ADR 0005](0005-ef-core-npgsql-persistence.md)
- [Sprint 7 tracker #116](https://github.com/bbubb/squadsync/issues/116)
