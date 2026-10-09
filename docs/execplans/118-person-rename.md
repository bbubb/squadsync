# Rename represented people while preserving PostgreSQL data

This ExecPlan follows PLANS.md and implements Issue #118 under accepted ADR 0006.

## Purpose / Big Picture
Use Person/PersonId/People throughout the current model while retaining every existing identifier, relationship, and domain rule.

## Progress
- [x] Verified agent-ready issue and merged architectural gate PR #123.
- [x] Created feature/118-person-rename from current clean main.
- [x] Renamed current C# and authored an explicit reversible rename-only migration.
- [x] Verified disposable migration upgrade/rollback/re-upgrade, all row values, object identities, and constraints.
- [x] Applied live upgrade after identical before/after data and object fingerprints; twice reran seed and repeated update without changes.
- [x] Validated baseline, PostgreSQL regressions, docs, and applied-history integrity.
- [ ] Open draft PR for human review.

## Surprises & Discoveries
- Command sandbox failed initialization; commands run through reviewed escalation.
- Local Compose service was stopped; started it using its existing persistent volume.
- PostgreSQL restrictive deletes return SQLSTATE 23001; invalid dependent references return 23503. The test checks each explicitly.
- Running all tests with the live connection environment configured caused two existing health tests to return OK instead of their expected unavailable response. The documented workflow runs database-category tests with opt-in, then clears database variables for baseline. Both prescribed runs passed; health configuration changes are outside #118.

## Decision Log
- Decision: Hand-author PostgreSQL rename operations and update the target model from the existing snapshot; do not scaffold an entity deletion/addition.
  Rationale: Entity vocabulary changes must preserve table, index, and constraint identity without destructive operations.
  Date/Author: 2026-10-09 / Codex.

## Outcomes & Retrospective
Person naming is implemented without changing domain meaning or live data. Baseline restore/build/test and the documented opt-in PostgreSQL category pass. Human review and merge remain outside agent authority.

## Context and Orientation
Domain entities live in apps/api/src/SquadSync.Domain. Infrastructure owns mappings and migrations. Existing migration files stay unchanged. ADR 0006 is accepted through PR #123; correct its stale status header in this PR.

## Plan of Work
Rename current types and identifiers, add a reversible schema rename and matching target model, then validate linked pre-existing data and constraints. Update canonical docs and relevant summaries in this same change.

## Concrete Steps
Run dotnet restore/build/test under apps/api. Review EF migration SQL and has-pending-model-changes. Run opt-in PostgreSQL tests with matching ignored local configuration. Exercise migration rollback only in an isolated test schema. Capture full normalized data and constraint evidence before and after the live upgrade.

## Validation and Acceptance
Require identical GUIDs, values, counts, and associations before/after upgrade and rollback. Require unique/FK/restrictive-delete behavior and existing Application/seeder tests. Confirm applied migrations unchanged and no schema reset or HTTP/auth changes.

## Idempotence and Recovery
EF history makes repeat upgrades safe. Dispose/rollback test transactions and isolate migration tests from public developer tables. Never roll back valuable developer records or remove the database volume. Stop on unexpected schema/history or preservation failure.

## Artifacts and Notes
Validation on 2026-10-09:

- dotnet restore succeeded; dotnet build succeeded with 0 warnings/errors.
- Baseline dotnet test: 89 unit + 7 integration passed, 6 opt-in tests skipped.
- Opt-in Category=Database: 6 passed, none skipped, including existing Application/seed regressions and the new isolated migration test.
- EF has-pending-model-changes reports no changes; repeated database update succeeds without applying another migration.
- All 8 changed documentation files passed local path and heading-anchor checks; git diff --check passed. The three applied migration pairs are byte-for-byte unchanged in Git.
- Generated upgrade SQL contains only table/column/index/constraint renames plus the EF history insertion inside a transaction. Down reverses only these renames and is exercised exclusively in the isolated schema.

Live preservation evidence (before upgrade, after upgrade, and after all tests plus two intentional seed reruns):

| Evidence | Before | After / final |
|---|---|---|
| People (formerly Users) | 4 | 4 |
| Teams | 1 | 1 |
| TeamMemberships | 4 | 4 |
| PlayerProfiles | 2 | 2 |
| RosterEntries | 2 | 2 |
| Normalized complete-row MD5 | f5cc33973a18c688ec3ee980c293459b | f5cc33973a18c688ec3ee980c293459b |
| Public relation + constraint OID MD5 | ac48ff50c87d7aa01ed455f185047ae4 | ac48ff50c87d7aa01ed455f185047ae4 |

The row fingerprint hashes a jsonb object of all five tables, each aggregated in Id order. Only Users/People table vocabulary and dependent UserId/PersonId JSON keys are normalized; every value and GUID is included. The object fingerprint hashes sorted public pg_class and pg_constraint OIDs; unchanged identities prove the tables, indexes, and constraints were not replaced. The isolated test in [PersonRenameMigrationTests.cs](../../apps/api/tests/SquadSync.IntegrationTests/PersonRenameMigrationTests.cs) also compares complete normalized row JSON and object OIDs directly across Up/Down/Up, without hashes. No credentials or personal row contents are committed.

## Interfaces and Dependencies
Existing EF Core/Npgsql 10, PostgreSQL Compose, immutable Domain entities, Application IRosterPersistence, DevelopmentDemoSeeder. No new packages or services.
