# PlayerProfile HTTP Contract — Sprint 9

## Status

**Proposed for owner review in [Issue #146](https://github.com/bbubb/squadsync/issues/146).** After human approval and merge, this will be the accepted Phase 3/Sprint 9 PlayerProfile HTTP contract. Implementation issues require separate readiness reviews.

## Purpose and ownership

Expose a small, functional **Person-level** PlayerProfile API using the existing Domain `PlayerProfile` and EF Core/Npgsql mapping; do not add schema. The [accepted domain contract](domain-model.md#current-player-profile-and-roster-implementation-contract) permits **zero or one** profile per Person with optional dominant foot, height and weight.

The profile is **not created automatically** when a Person, Player TeamMembership or RosterEntry is created. A caller explicitly creates the profile on first save. No TeamMembership, TeamRole.Player or RosterEntry is required to create or read a Person's profile. Multiple TeamMemberships refer to the same Person and therefore the same optional profile. Jersey and roster status remain exclusively team-specific `RosterEntry` data.

## Routes and lifecycle

All routes are relative to `/api`; `{personId}` is an existing Person GUID. One profile per Person allows a **singleton** route without exposing a second profile identifier in the URL.

| Method | Route | Behavior |
| --- | --- | --- |
| POST | `/people/{personId}/player-profile` | Explicitly create a profile; `201 Created` with a retrievable `Location` to this singleton resource |
| GET | `/people/{personId}/player-profile` | Retrieve existing profile (`200`); if the Person or profile does not exist, `404` |
| PUT | `/people/{personId}/player-profile` | Fully replace editable fields **on an existing profile** (`200` with updated representation); `404` if Person or profile does not exist; **never create on PUT** |

- POST generates `PlayerProfile.Id` in Application. It must not accept a client-supplied profile ID or PersonId in the body.
- PUT preserves the original `Id` and `PersonId`, updates only the three attributes below, and must not insert a profile as a side effect.
- If someone has no profile, `GET` and `PUT` return `404`; the caller must use explicit POST to create one.
- The `Location` on POST is `/api/people/{personId}/player-profile`, which GET can retrieve. An additional by-profile-ID GET is unnecessary.
- A Person can gain or lose TeamMemberships without automatically creating, changing or deleting their profile. Roster API representations remain unchanged; the existing `person.id` is sufficient to request the Person-scoped profile separately.

## Requests and response

POST body (all three attributes are optional; omitted fields default to `null`):

```json
{ "dominantFoot": "Right", "heightInches": 70, "weightPounds": 152.5 }
```

PUT body (**all three JSON property names must be present**; each value may be `null` to explicitly clear the existing attribute):

```json
{ "dominantFoot": null, "heightInches": 70, "weightPounds": null }
```

POST (201), GET (200) and successful PUT (200) return the same typed representation:

```json
{
  "id": "<profile-guid>",
  "personId": "<person-guid>",
  "dominantFoot": "Right",
  "heightInches": 70,
  "weightPounds": 152.5
}
```

- `dominantFoot`: JSON `null` or **exact canonical** string `Left`, `Right`, `Both`; reject numeric enums, undefined strings and incorrect spelling/case. This is foot dominance, not preferred side/field position.
- `heightInches`: `null` or a positive whole-number integer in **total inches**. No feet/inches or metric conversion in persistence.
- `weightPounds`: `null` or a positive decimal JSON number in pounds; preserve existing Domain decimal precision. Do not use a string or impose a new rounding rule.
- No fields for position, statistics, jersey number, team assignment, roster status, identity/account, or match availability.
- POST may accept all-null or omitted fields because the accepted Domain has no minimum-population invariant. Creating a blank profile is an **explicit** action, never an automatic consequence of creating a Person/membership. A future UI may choose not to POST until actual profile data is entered.

## Errors and security

| Condition | HTTP result |
| --- | --- |
| Malformed JSON, malformed or empty `personId`, wrong field type, undefined/noncanonical/numeric `dominantFoot`, nonpositive height/weight, or missing required PUT property name | `400 ValidationProblemDetails` |
| Person does not exist; profile does not exist for GET/PUT; known Person-reference FK race | `404 ProblemDetails` |
| Explicit POST when Person already has a PlayerProfile; known PostgreSQL one-profile-per-Person uniqueness race | `409 ProblemDetails` |
| Successful POST | `201 Created` + typed profile + working `Location` |
| Successful GET or PUT | `200 OK` + typed profile |

Map only known database constraint failures; do not leak raw EF Core/Npgsql exception, connection string, or stack trace details. Preserve request cancellation rather than translating it into an artificial 500. Use existing built-in MVC validation/ProblemDetails conventions; no new validation framework.

**Interim access boundary:** these anonymous management endpoints must be mapped **only in Development on trusted localhost**, never Staging/Production. Environment gating is not authentication and does not authorize access. A GUID is a resource identifier, **not** an authorization token. Future publicly accessible APIs must verify the authenticated caller's authority over the requested Person or Team on *every* relevant action (BOLA/IDOR protection). Do not introduce public access or a new permissions system in Sprint 9.

## Architecture and validation constraints

- API owns routes, DTOs and status mapping. Application owns explicit create/get/update use cases and narrow persistence/query contracts. Domain owns `PlayerProfile` invariants; Infrastructure owns EF/Npgsql queries, unique Person FK and update persistence.
- The existing `PlayerProfile` uses guarded getter-only properties. Implementing PUT may require an explicit Domain mutation method or an equivalent invariant-preserving approach, but **must not** expose unrestricted public setters, silently change `Id`/`PersonId`, or add new columns/migrations.
- Unit and real-controller HTTP tests should cover success, absent Person/profile, duplicate/race, invalid values, PUT full replacement including clearing values, stable identity, failure sanitization, cancellation and Staging/Production route absence without Docker.
- Opt-in, rollback-safe real PostgreSQL HTTP tests should prove POST → GET → PUT → GET across separate request contexts, persisted values/identity, known constraints and preexisting/demo-data preservation. Reuse existing `TransactionalHttpDatabase` guardrails rather than create a new test environment.
- No automatic PlayerProfile creation in `CreatePerson`, `CreateMembership`, or `CreateRosterEntry`.

## Deferred

Public authentication/authorization, accounts/guardian delegation, roster editing, preferred positions, statistics, match availability, frontend screens, cloud infrastructure, Soccer-Subber and a new generic repository are outside this contract. Any decision to auto-create a profile or add new profile attributes requires a separate approved scope change.

## References

- [Sprint 9 tracker #145](https://github.com/bbubb/squadsync/issues/145)
- [PlayerProfile contract and Person relationship](domain-model.md#current-player-profile-and-roster-implementation-contract)
- [Sprint 8 membership/roster HTTP conventions](membership-roster-http-contract.md)
- [Persistence boundary: ADR 0005](../adr/0005-ef-core-npgsql-persistence.md)
- [Person/account and Development-only API: ADR 0006](../adr/0006-represented-person-identity-api-contract.md)
- [MVP Scope](../planning/mvp-scope.md)
- [OWASP API1:2023 Broken Object Level Authorization](https://api-security.owasp.org/editions/2023/en/0xa1-broken-object-level-authorization/)
