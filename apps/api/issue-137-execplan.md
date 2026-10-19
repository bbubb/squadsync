# Verify membership and roster HTTP persistence safely

This ExecPlan follows `PLANS.md` from the repository root.

## Purpose / Big Picture

Implement [#137](https://github.com/bbubb/squadsync/issues/137): prove the Development HTTP membership/roster workflow against real PostgreSQL without leaving changes to developer/demo data, and document a reproducible local demonstration.

## Progress

- [x] Retrieve issue, confirm readiness and sources, fetch main and create issue branch.
- [x] Add transactional HTTP workflow and injected-failure tests.
- [x] Update local demonstration and minimally correct stale roadmap gates.
- [x] Run restore/build/test and the complete opt-in database suite; review results.
- [x] Prepare the issue-linked draft PR for publication and human review without merging or closing Sprint 8.

## Surprises & Discoveries

Git metadata writes and network access require elevated execution. Current main includes #136 through PR #142. Existing environment tests cover Staging/Production absence; Swagger already describes membership and roster routes and responses. The existing local Compose PostgreSQL instance was healthy, allowing the full database suite to run without infrastructure changes. Tracker #129 already reflects merged features and the remaining #137 gate; only roadmap wording needed correction.

## Decision Log

- Decision: Reuse TransactionalHttpDatabase unchanged except for verifying the additional real adapter registrations.
  Rationale: Its preflight, table locks, full-row fingerprints, observer connection and finally rollback already supply the required isolation and preservation guarantees.
  Date/Author: 2026-10-18 / Codex.

## Outcomes & Retrospective

Implemented two opt-in real HTTP/PostgreSQL tests covering the complete workflow, rejected operations and injected assertion failure. Restore and build pass with zero warnings/errors. Ordinary tests: 149 unit and 158 integration passed, with 14 database tests skipped. Full opted-in Category=Database suite: 14 passed, zero failed/skipped. No product code, schema, dependencies or routes changed. Phase 3/Sprint 8 remain active pending human review and merge.

## Context and Orientation

Tests live under apps/api/tests/SquadSync.IntegrationTests. PeopleTeamsHttpPersistenceTests supplies the existing success/failure pattern. Governing sources are apps/api/AGENTS.md, the API task skill, docs/architecture/membership-roster-http-contract.md, ADRs 0003/0005/0006, docs/planning/mvp-scope.md and the testing strategy. apps/api/README.md owns local setup; docs/planning/project-roadmap.md owns phase direction.

## Plan of Work

Create People, Teams, Player and staff memberships and roster entries through separate HTTP requests. Verify Locations, lists, ordering, persisted values, team isolation and unchanged snapshots after each rejected operation. Reuse the same workflow before an intentional assertion failure. Document trusted localhost usage and correct only stale Sprint 8 gate text.

## Concrete Steps

From apps/api run dotnet restore, dotnet build and dotnet test without database opt-in. Load ignored local connection settings without printing secrets; enable SQUADSYNC_RUN_DATABASE_TESTS=1 and run dotnet test tests/SquadSync.IntegrationTests --filter Category=Database. Run git diff --check and publish the issue branch with a draft PR linking Closes #137.

## Validation and Acceptance

Require real ports/adapters, distinct request EF contexts, 201/200 resource equality, ordered player-only team roster, unrostered/staff membership visibility, leading-zero and nullable jerseys, all statuses, 400/404/409 safe errors, unchanged snapshots after every failure, observer invisibility and complete rollback after success/assertion failure. Reuse ordinary environment/Swagger tests. Record precise blockers and counts.

## Idempotence and Recovery

Never run migrations, seed commands, reset schema, delete rows or remove volumes. Use existing opted-in rolled-back transactions against the idle local database. Stop on unexpected schema/history, failed locks, unavailable database or inability to prove noninterference. Preserve the original harness and full-suite isolation settings. No new dependencies, endpoints or product behavior.

## Artifacts and Notes

- dotnet restore: passed (all projects up to date).
- dotnet build: passed, zero warnings/errors, SDK 10.0.401.
- dotnet test: 149 unit + 158 ordinary integration passed; 14 database tests skipped.
- SQUADSYNC_RUN_DATABASE_TESTS=1 with ignored local configuration, dotnet test tests/SquadSync.IntegrationTests --filter Category=Database: 14 passed, zero failed/skipped, including both new tests and the existing HTTP success/failure tests.
- HTTP harness preflight, complete-row snapshots, observer invisibility and post-rollback preservation assertions all passed. No migrations, seed commands or schema changes were invoked outside the existing isolated suite behavior.
- Reviewed existing Swagger and Staging/Production tests; no additional assertions needed. README local links and roadmap references checked; root summary updates beyond the roadmap are unnecessary because scope, architecture and phase direction are unchanged.

## Interfaces and Dependencies

Reuse ApiTestFactory, DatabaseFact, TransactionalHttpDatabase, MembershipResponse, RosterEntryResponse and real Application/EF registrations. No interface or schema changes.
