# Prove User and Team persistence against PostgreSQL

This ExecPlan follows `PLANS.md` from the repository root and implements Issue #86.

## Purpose / Big Picture

Generate the first User/Team migration and prove EF materializes the existing immutable domain entities against the local Compose database.

## Progress

- [x] Retrieve agent-ready issue, confirm prerequisites are merged, and create issue branch from current main.
- [x] Add pinned CLI tooling, private Design dependency, and opt-in transactional round-trip test.
- [x] Generate and inspect migration.
- [x] Inspect existing schema, apply migration, and run database validation.
- [x] Document commands, run baseline gates, and prepare draft PR handoff.

## Surprises & Discoveries

- Git needs a process-scoped safe.directory setting under the sandbox user; no persistent configuration changes made.
- Docker Desktop was not running at intake; start it before the database gate.
- The first schema inspection ran before PostgreSQL finished starting. Retry after Compose reports healthy; pass SQL through standard input to preserve quoting on Windows.
- The first EF update logged a missing migration-history SELECT before creating history and applying successfully; history was verified afterward.

## Decision Log

- Decision: Use a small FactAttribute subclass that sets Skip during discovery unless opt-in equals 1.
  Rationale: Existing xUnit supports this without another dependency; ordinary tests visibly skip the database test before any connection.
  Date/Author: 2026-10-02 / Codex.

## Outcomes & Retrospective

Implementation and all validation gates passed. No materialization or tooling architecture blocker occurred. The PostgreSQL container is stopped after validation; its existing named volume and applied schema remain intact. Commit, push, and draft PR are the remaining handoff steps. Manual Docker startup and environment setup remain evidence relevant to deferred Issue #72; no automation was added outside this issue.

## Context and Orientation

Infrastructure owns SquadSyncDbContext and explicit User/Team mappings. API startup already reads ConnectionStrings:SquadSync. Domain entities use validated constructors and getter-only properties. Issues #83–#85 and #91 are merged.

## Plan of Work

Restore pinned tooling, generate InitialUserTeam using API startup, and inspect generated artifacts for only the approved model. Start the existing Compose service and inspect application tables before applying. Run opt-in persistence validation with a rolled-back transaction, then ordinary tests with no database configuration. Update the API README and prepare a draft PR.

## Concrete Steps

From apps/api run dotnet restore, dotnet tool restore, and dotnet ef --version. Set ConnectionStrings__SquadSync privately from ignored infra/docker/.env. Run:

```powershell
dotnet ef migrations add InitialUserTeam --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext --output-dir Persistence/Migrations
dotnet ef database update --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext
$env:SQUADSYNC_RUN_DATABASE_TESTS = "1"
dotnet test tests/SquadSync.IntegrationTests --filter "Category=Database"
Remove-Item Env:SQUADSYNC_RUN_DATABASE_TESTS
Remove-Item Env:ConnectionStrings__SquadSync
dotnet build
dotnet test
```

## Validation and Acceptance

Migration must create only Teams (Id/Name) and Users (Id/FirstName/LastName), with primary keys and required columns. Database test must save, clear tracking, read fresh instances, assert persisted values, roll back, and verify its IDs no longer exist. Ordinary test discovery must skip without connecting. Existing health tests remain green.

## Idempotence and Recovery

Database update uses EF migration history for repeat runs. Test uses fresh IDs and an explicit transaction disposed without commit even if assertions fail. Inspect existing schema before update; stop for conflicting application tables. Never delete databases or named volumes. Stop and report if API-startup tooling or immutable materialization fails.

## Artifacts and Notes

Migration, designer, and snapshot belong in Infrastructure/Persistence/Migrations. Record actual validation outcomes here and in the draft PR, without connection strings or credentials.

- Generated migration: `20261002142123_InitialUserTeam`; reviewed only the two approved entities, UUID keys, required text columns, and primary keys.
- `dotnet tool restore` and `dotnet ef --version`: passed, version 10.0.0.
- PostgreSQL healthy; pre-migration non-system table inventory: zero tables.
- `dotnet ef database update`: passed; repeated update passed. History records migration version 10.0.0.
- `dotnet ef migrations has-pending-model-changes` with the same project/startup/context arguments: no pending changes.
- Opt-in `dotnet test tests/SquadSync.IntegrationTests --filter "Category=Database"`: 1 passed, 0 skipped. Both immutable entities materialized and rollback absence assertions passed.
- Independent SQL counts after the test: Users=0, Teams=0.
- `dotnet restore`: passed. `dotnet build`: passed, zero warnings/errors.
- `dotnet test` with PostgreSQL stopped and both database environment variables absent: 14 unit tests and 5 integration tests passed, 1 database test skipped.
- Root summaries checked; root README updated to remove stale migration status. Roadmap phase and architecture boundaries remain accurate and unchanged.

## Interfaces and Dependencies

Local dotnet-ef 10.0.0, private API Microsoft.EntityFrameworkCore.Design 10.0.0, existing EF/Npgsql Infrastructure dependencies, existing xUnit IntegrationTests, and infra/docker Compose PostgreSQL. No new product interface or architecture pattern.
