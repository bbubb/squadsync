# Domain Model

## Status

Active. This document is the current domain-model baseline.

## Purpose

This document defines the initial SquadSync domain model. The model is intentionally soccer-specific so the first implementation remains understandable, testable, and useful for the coach workflow.

## Modeling Strategy

The MVP uses explicit domain relationships:

```text
A represented User participates in a Team through a TeamMembership.
The TeamMembership carries one constrained TeamRole.
```

`User` represents a person in SquadSync's domain. It is distinct from a future authentication account or external login identity. Authentication/account identity remains a future concern. The future access model may allow one account to act for more than one represented user, for example a parent or guardian acting for a youth player. This possibility does not define or implement account linking or delegation behavior in the MVP.

## Core Concepts

### User

A represented person in the domain, such as a coach, assistant coach, team manager, or player. A person's participation in each team is modeled by a separate `TeamMembership`.

`User` is not an authentication account. Account identity and authentication are deferred; an account may eventually be associated with or act for multiple represented users.

### Team

A soccer team managed in SquadSync. A team has memberships; player memberships may carry roster entries. A team also owns matches and lineups.

### TeamMembership

The relationship between a `User` and a `Team`. It records the person's team participation and has exactly one `TeamRole` in the MVP. A person may have memberships on multiple teams and may have different roles on each.

### TeamRole

A constrained MVP value on `TeamMembership`. Its allowed values are exactly `Owner`, `Coach`, `AssistantCoach`, `Manager`, `Player`, and `Viewer`; undefined values are invalid. It is not a persisted, dynamically configurable Role/Permission entity model. Keep authorization simple; introduce more granular permissions only when concrete product behavior requires them and an approved design defines them.

### PlayerProfile

Person-level soccer attributes associated with a `User`: optional dominant foot, height in total inches, and weight in pounds. Preferred positions are deferred until the soccer-position/formation/lineup vocabulary is defined. It does not own team-context data such as jersey number or roster status, or match-specific availability.

### RosterEntry

The planned player-only team-context record attached to a player's `TeamMembership`. The membership remains the canonical relationship between the represented `User` and `Team`; `RosterEntry` adds roster details such as jersey number and roster status to that player membership and does not create a second `User`-to-`Team` relationship. It is distinct from person-level `PlayerProfile` attributes.

### CoachProfile

Deferred. A dedicated coach profile is not needed for the initial model; add it only if a concrete MVP use case requires coaching-specific person attributes.

### Statistics

Deferred to later dedicated models. Do not add statistics fields to `PlayerProfile` or `RosterEntry` as a shortcut.

### Match

A scheduled soccer match for a team. Potential fields include opponent name, scheduled date/time, location, status, and notes.

### Formation

A named soccer formation used for lineup planning, such as 4-3-3, 4-4-2, or 3-5-2. The first MVP may store formations as controlled values before introducing a configurable formation model.

### Lineup

A lineup plan for a match. A lineup belongs to a match and contains lineup slots.

### LineupSlot

A specific assignment within a lineup, including position and assigned player. Period/segment and notes may be added when needed.

### PlayerAvailability

A player's availability for a match, with values such as Available, Unavailable, Injured, Late, or Unknown.

## Initial Relationship Diagram

```mermaid
erDiagram
    USER ||--o{ TEAM_MEMBERSHIP : has
    TEAM ||--o{ TEAM_MEMBERSHIP : has
    TEAM_MEMBERSHIP {
        string team_role
    }
    USER ||--o| PLAYER_PROFILE : may_have
    TEAM_MEMBERSHIP ||--o| ROSTER_ENTRY : may_have_player_roster_details
    TEAM ||--o{ MATCH : schedules
    MATCH ||--o{ PLAYER_AVAILABILITY : tracks
    USER ||--o{ PLAYER_AVAILABILITY : has
    MATCH ||--o{ LINEUP : has
    LINEUP ||--o{ LINEUP_SLOT : contains
    USER ||--o{ LINEUP_SLOT : assigned
```

The diagram shows domain relationships, not authentication-account relationships. `TeamMembership` is the only `User`-to-`Team` relationship. `RosterEntry` is optional player-specific detail attached to that membership. `TeamRole` is a constrained value carried by membership, not a separately managed role catalog.

## Suggested Initial Entities and Values

```text
User
Team
TeamMembership (with one TeamRole value)
PlayerProfile
RosterEntry
Match
PlayerAvailability
Formation
Lineup
LineupSlot
```

`CoachProfile`, statistics models, and authentication/account models are deferred. `TeamRole` is a constrained value and does not imply a persisted `Role` entity.

## Soccer Position Modeling

Preferred positions are excluded from the Sprint 5 `PlayerProfile` contract. Define the soccer-position/formation/lineup vocabulary in a later slice before adding preferred positions; do not use JSON or string position shortcuts in the interim.

The MVP can begin with a simple position enum or value object.

Examples:

- GK
- CB
- LB
- RB
- CDM
- CM
- CAM
- LW
- RW
- ST

Do not overbuild the position model early. Formation-specific slot modeling can evolve after manual lineup building works.

## Membership, Roster, and Profile

Use these distinctions:

- `User`: represented person, separate from future authentication/account identity
- `TeamMembership`: the person's team relationship and one constrained `TeamRole`
- `PlayerProfile`: soccer attributes about the person
- `RosterEntry`: player-only team-context data attached to a `TeamMembership`, including jersey number and roster status

A person may be a player on one team and an assistant coach on another. Represent that with one `User` and separate memberships. A player's roster entry extends the player membership with team-specific data; it does not create another `User`-to-`Team` relationship or turn `PlayerProfile` into team data.

## Sprint 4 Membership Implementation Contract

The minimal Sprint 4 shape is:

```text
TeamMembership
- Id: Guid
- UserId: Guid
- TeamId: Guid
- TeamRole: TeamRole
```

### Domain Invariants

- A represented `User` may belong to many `Teams`; a `Team` may have many memberships.
- A `User` may have at most one `TeamMembership` for the same `Team`.
- Each membership has exactly one `TeamRole`, from the six allowed values above.
- `Id`, `UserId`, and `TeamId` are application-assigned and must not be `Guid.Empty`.
- Undefined `TeamRole` values are invalid.
- Domain represents the relationship using `UserId` and `TeamId` identifiers and remains EF-independent; it does not require EF navigation properties or persistence annotations.
- `TeamMembership` owns no roster-only or authentication/account fields.

### Infrastructure Mapping Direction

Under [ADR 0003](../adr/0003-use-explicit-membership-model.md) and [ADR 0005](../adr/0005-ef-core-npgsql-persistence.md), Infrastructure owns the EF Core mapping and migration:

- Foreign keys from `TeamMembership.UserId` to `User` and from `TeamMembership.TeamId` to `Team` enforce endpoint existence.
- Both relationships use restrictive/no-cascade deletion semantics: deleting a referenced `User` or `Team` must not implicitly delete memberships.
- A unique constraint or unique index on `(UserId, TeamId)` enforces one membership per pair across persisted records.
- `TeamRole` is persisted as a readable string value; there is no separate `Role` table.
- No repository abstraction is introduced in Phase 2 merely because `TeamMembership` exists. Application persistence contracts wait for concrete use cases, as required by ADR 0005.

### Deferred Concerns

Sprint 4 excludes `PlayerProfile` and `RosterEntry` implementation, roster fields or status rules, authentication/account identity and delegation, membership status, joined/left timestamps, granular permissions, and a dynamic `Role` entity. API endpoints and Application use cases, generic repository/base entity work, auditing, and soft deletion also remain deferred. The future `RosterEntry` extends a player membership; it does not add another `User`-to-`Team` relationship.

## Sprint 5 Player Profile and Roster Implementation Contract

The accepted contract under [Sprint 5 tracker #102](https://github.com/bbubb/squadsync/issues/102) and [architecture gate #103](https://github.com/bbubb/squadsync/issues/103) separates person-level soccer attributes, team-context roster data, and match availability.

### PlayerProfile

The minimal persisted shape is exactly:

```text
PlayerProfile
- Id: Guid
- UserId: Guid
- DominantFoot: DominantFoot?
- HeightInches: int?
- WeightPounds: decimal?
```

- A `User` may have at most one optional `PlayerProfile`.
- `Id` and `UserId` are application-assigned and must not be `Guid.Empty`.
- `DominantFoot`, when supplied, must be exactly `Left`, `Right`, or `Both`; undefined values are invalid. This represents foot dominance, not a player's preferred field side or flank.
- Optional height and weight values, when supplied, must be greater than zero.
- `HeightInches` stores total inches as an integer (for example, 5 ft 10 in = 70); `WeightPounds` stores pounds as a decimal.
- These are the canonical MVP storage units for the initial US-first product context. Do not persist duplicate imperial and metric copies of the same measurement or introduce ambiguous `Height` or `Weight` fields.
- Later measurement-system support should convert at the API/UI boundary so users can enter/view feet+inches/pounds or centimeters/kilograms without creating two competing sources of truth. Implementing conversion is outside this slice.
- Preferred positions remain deferred until the soccer-position/formation/lineup vocabulary is defined; do not store JSON or string position shortcuts.
- `PlayerProfile` owns no jersey number, roster status, match availability, team assignment, statistics, or authentication fields.

### RosterEntry

The minimal persisted shape is exactly:

```text
RosterEntry
- Id: Guid
- TeamMembershipId: Guid
- JerseyNumber: string?
- RosterStatus: RosterStatus
```

- A `TeamMembership` may have at most one optional `RosterEntry`.
- `Id` and `TeamMembershipId` are application-assigned and must not be `Guid.Empty`.
- Do not duplicate `UserId` or `TeamId`; `TeamMembership` remains the only `User`-to-`Team` relationship.
- `JerseyNumber` is an optional label containing 1–3 ASCII digits when supplied. Preserve legitimate formatting such as `00`. Whitespace is not meaningful and must not be persisted.
- `RosterStatus` must be exactly `Active`, `Reserved`, or `Suspended`; undefined values are invalid.
- Career/person-level states such as `Retired` do not belong in `RosterStatus`.
- Match-specific states such as `Available`, `Unavailable`, `Injured`, and `Late` belong to match availability in Match context, not `RosterEntry`.

### Persistence and Enforcement Boundary

Following [ADR 0003](../adr/0003-use-explicit-membership-model.md) and [ADR 0005](../adr/0005-ef-core-npgsql-persistence.md), Domain remains EF-independent and Infrastructure owns explicit one-to-one mappings and migrations:

- `PlayerProfile.UserId` is a foreign key to `User` with a unique constraint or unique index, enforcing one optional profile per user.
- `RosterEntry.TeamMembershipId` is a foreign key to `TeamMembership` with a unique constraint or unique index, enforcing one optional roster entry per membership.
- Both relationships use restrictive/no-cascade deletion: deleting a referenced `User` or `TeamMembership` must not implicitly delete its profile or roster entry.
- PostgreSQL enforces these structural foreign-key and uniqueness rules.

Player-role eligibility is a business/use-case invariant: only a membership whose `TeamRole` is `Player` may receive a `RosterEntry`. The future Application use case must load the membership and validate its role before creating the roster entry. Do not create a cross-table database constraint for role eligibility. Phase 2 persistence tests may prove structural persistence independently; Phase 3 use-case tests must prove Player-role eligibility.

### Deferred Concerns

Statistics, `CoachProfile`, authentication/account work, match availability, and formation/lineup implementation remain deferred. This slice introduces no API CRUD or Application use cases, and no repository abstraction merely because these entities exist. The architecture gate records this contract only; C# implementation and EF migrations belong to subsequent scoped issues.

## Integration Boundary Concepts

### LineupSuggestionRequest

A contract that packages enough match, roster, formation, and constraint data for an external service to suggest a lineup.

### LineupSuggestionResponse

A response containing suggested player-slot assignments and a high-level planning summary.

The request and response belong to the platform integration boundary. The algorithm/service implementation belongs to the separate lineup assistance service.

## Design Principle

The model should be understandable to a soccer coach, a software engineer, and a hiring reviewer. If a concept requires a long explanation, it probably does not belong in the first MVP.
