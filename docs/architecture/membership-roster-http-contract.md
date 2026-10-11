# Membership and Roster HTTP Contract — Sprint 8

## Status

**Acceptance gate: [Issue #131](https://github.com/bbubb/squadsync/issues/131) and its documentation PR.** While that PR is unmerged, this is a proposal. After human approval and merge, it is the accepted Sprint 8 contract until explicitly superseded. Implementation issues require separate readiness reviews.

## Purpose and boundary

Extend the Development-only Person/Team create-read conventions from [ADR 0006](../adr/0006-represented-person-identity-api-contract.md). A `TeamMembership` connects a represented Person to a Team **and requires exactly one** `TeamRole` describing how they participate. The relationship identifies the Person+Team association; the role is its required attribute. `RosterEntry` is optional player-only team-context data attached to a Player membership. `TeamRole` does **not** establish the identity or authorization of an API caller. No account/permission system, public anonymous access, or automatic Owner membership is added.

These paths are available only in **Development on a trusted localhost environment**, never Staging/Production. Environment gating is **not** a security control or a substitute for future authorization.

## Resource routes

All routes are relative to `/api`. `{teamId}`, `{membershipId}`, and `{rosterEntryId}` are GUIDs.

| Method | Route | Purpose | Issue |
| --- | --- | --- | --- |
| POST | `/teams/{teamId}/memberships` | Link an existing Person to an existing Team with one role | #132 |
| GET | `/teams/{teamId}/memberships/{membershipId}` | Read the created membership; POST Location points here | #132 |
| GET | `/teams/{teamId}/memberships` | Read **all** team memberships (including staff and players) | #133 (owner-authored) |
| POST | `/teams/{teamId}/roster` | Create a RosterEntry for an existing Player membership | #135 |
| GET | `/teams/{teamId}/roster/{rosterEntryId}` | Read the created roster entry; POST Location points here | #135 |
| GET | `/teams/{teamId}/roster` | Read the team's **rostered players** | #136 |

The two individual GETs are essential to the `201 Created` + retrievable `Location` convention: a creation PR must not return a Location to an endpoint that has not been implemented.

### Membership creation and reads

`POST /api/teams/{teamId}/memberships` accepts:

```json
{ "personId": "<person-guid>", "teamRole": "Player" }
```

The route supplies TeamId. Application generates MembershipId and validates the Team/Person exist; the Domain validates identifiers and `TeamRole`. No caller ID is accepted as proof of authority. The request and response use **role names as JSON strings**, not numeric enum values: exactly `Owner`, `Coach`, `AssistantCoach`, `Manager`, `Player`, or `Viewer` (canonical spelling/case). Avoid changing JSON enum serialization globally for other endpoints.

`201 Created` includes a typed membership and `Location: /api/teams/{teamId}/memberships/{membershipId}`. The same representation is returned by individual GET and as an element of the list:

```json
{
  "id": "<membership-guid>",
  "teamId": "<team-guid>",
  "person": { "id": "<person-guid>", "firstName": "Alex", "lastName": "Player" },
  "teamRole": "Player"
}
```

The list GET returns `200 OK` with a JSON array of memberships **of all roles**, including Player memberships with no roster entry. Existing Team with no memberships returns `200 []`; missing Team returns `404`. Sort consistently by person's last name, first name, then membership Id.

### Player roster creation and reads

`POST /api/teams/{teamId}/roster` accepts:

```json
{ "teamMembershipId": "<membership-guid>", "jerseyNumber": "007", "rosterStatus": "Active" }
```

The membership **must belong to the route Team** and must carry `TeamRole.Player`. Reuse Application's `AddPlayerToRoster` behavior and existing one-to-one uniqueness; enforce team scoping in Application, not solely in controller code. Application generates RosterEntryId for this API. `rosterStatus` accepts only canonical string names `Active`, `Reserved`, `Suspended` (not numeric enums); `jerseyNumber` is nullable, otherwise 1–3 ASCII digits, with leading zeros preserved.

`201 Created` includes `Location: /api/teams/{teamId}/roster/{rosterEntryId}`. Individual GET and the list share this response item:

```json
{
  "id": "<roster-entry-guid>",
  "teamMembershipId": "<membership-guid>",
  "person": { "id": "<person-guid>", "firstName": "Alex", "lastName": "Player" },
  "jerseyNumber": "007",
  "rosterStatus": "Active"
}
```

The roster list includes **only Player memberships that have a persisted RosterEntry**, including Reserved and Suspended statuses; it does not invent a RosterEntry for an unrostered Player membership. Such people remain visible through `GET /memberships` and can be rostered later. Existing Team with no roster entries returns `200 []`; missing Team returns `404`. Sort by person's last name, first name, then roster entry Id.

**Status boundary:** `RosterStatus` describes team-level roster standing, not participation in an individual match. `Reserved` is not synonymous with sitting on the bench. Injury/availability concerns are outside the Sprint 8 roster API and do not change the accepted `RosterStatus` enum.

These are **read projections/HTTP response DTOs**; no duplicate PersonId, TeamId or Person-name columns are added to persisted RosterEntry.

## HTTP errors and safe behavior

| Condition | Status | Representation |
| --- | --- | --- |
| Malformed JSON; malformed or empty GUID; missing required field; noncanonical/unknown or numeric role/status; invalid jersey number | `400` | `ValidationProblemDetails` |
| Team does not exist; referenced Person does not exist; nested membership/roster entry not found **or belongs to another Team** | `404` | `ProblemDetails` |
| Duplicate `(PersonId, TeamId)` membership; second RosterEntry for one membership; RosterEntry ID collision | `409` | `ProblemDetails` |
| Attempt to add a roster entry for an existing non-Player membership | `409` | `ProblemDetails` describing the business-state conflict without sensitive information |
| Successful POST | `201` | Typed resource response + valid retrievable `Location` |
| Successful individual/list GET | `200` | Typed resource / array |

Malformed and empty GUIDs are `400`, not an invitation to query or persist `Guid.Empty`. A well-formed GUID that does not identify an existing resource returns `404`. Missing nested resources or wrong-Team relationships return `404` and must not reveal cross-team data. Return standard, safe error bodies without exposing EF/Npgsql messages. Catch known database uniqueness/FK races at the Infrastructure/Application boundary as appropriate; do not mask unrelated persistence failures. Cancellation remains cancellation, not an artificial 500.

All responses follow the existing ASP.NET Core controller, built-in validation and ProblemDetails conventions. No generic error framework or new validation package.

## Implementation dependencies and owner-authored pilot

- **#132** owns membership POST **and individual GET** (including typed 201/Location) in one coherent PR.
- **#133** is the owner's full vertical slice for **team membership list GET**: examine the existing Domain model, write Application query/port, EF projection, HTTP endpoint and unit/in-process HTTP tests. Do not recreate TeamMembership or change schema. This is intentionally a complete read feature rather than an artificially new entity.
- **#134** independently compares the owner's PR against an isolated Codex reference solution and repository standards **before merge**. Codex's output is evidence, not a required replacement.
- **#135** owns roster-entry POST **and individual GET** with valid Location, team-scoped validation and error mapping.
- **#136** owns the roster **list GET** according to the inclusion rule above.
- **#137** verifies HTTP→Application→real EF/PostgreSQL integration without leaving durable test writes, and documents Swagger/local usage.

Existing migration and entity relationships remain unchanged. Normal unit/HTTP tests stay Docker-independent, with opt-in real PostgreSQL tests following Sprint 7's rolled-back data-preservation design.

## Deferred

This contract preserves the currently accepted MVP model and does not introduce authentication, permission administration, additional membership roles, or roster editing. Changes to those concerns require separate, human-approved scope and design decisions. Anonymous management endpoints remain restricted to trusted Development environments.

## References

- [Sprint 8 tracker](https://github.com/bbubb/squadsync/issues/129)
- [Domain model](domain-model.md)
- [Membership relationship (ADR 0003)](../adr/0003-use-explicit-membership-model.md)
- [Persistence boundary (ADR 0005)](../adr/0005-ef-core-npgsql-persistence.md)
- [Person/Account distinction and Development-only API (ADR 0006)](../adr/0006-represented-person-identity-api-contract.md)
- [API developer guide](../../apps/api/README.md)
