# Implement bounded Person candidate reads

This ExecPlan follows `PLANS.md` from the repository root.

## Purpose / Big Picture

Implement issue #212: trusted-local Development-only name search with minimal candidate pages, SQL filtering and keyset continuation. No schema, web, identity or write changes.

## Progress

- [x] Read issue #212, accepted #199 and tracker #207; confirm agent readiness and ADR alignment.
- [x] Inspect existing controller, Application query ports and PostgreSQL test patterns.
- [x] Implement query port, validated use case/cursor and EF query; wire HTTP metadata.
- [x] Add unit, HTTP, SQL translation and opt-in PostgreSQL coverage.
- [x] Run restore/build/test, inspect Swagger and record database validation.

## Surprises & Discoveries

- Git metadata requires elevated filesystem access; issue branch created successfully.
- Docker is installed but its pipe is inaccessible to the sandbox; retry database checks with elevation.
- Elevated Compose check confirmed the existing PostgreSQL container is healthy. Test connections work without elevation.
- Person names have no persisted length limit. A cursor containing full normalized keys can exceed the input bound. Long-name cursors therefore reference the Person ID, resolving normalized keys in the same SQL statement; no additional row is materialized.
- Empty query-string cursor values normally bind to null; explicitly retain them so Application rejects malformed empty tokens.
- The existing Swagger route inventory needed the new literal route, and API response annotations must mark required properties and nullable nextCursor accurately.

## Decision Log

- Decision: Use PostgreSQL lower(name) with explicit C collation for both ordering and continuation; carry database-produced normalized keys through the query projection.
  Rationale: Avoid discrepancies between CLR normalization/collation and database comparisons. Person IDs break name ties.
  Date/Author: 2026-11-12 / Codex.
- Decision: Keep a separate read port and base64url versioned cursor bound to the trimmed query.
  Rationale: Preserve existing Person persistence contracts; cursor is a continuation aid, not a credential.
  Date/Author: 2026-11-12 / Codex.

## Outcomes & Retrospective

Implemented the focused Development-only read slice without schema, dependencies or frontend changes. Full suite with PostgreSQL opt-in: 510 passed, 0 failed, 0 skipped (191 unit + 319 integration). Duplicate names remain separate and continuation is deterministic for unchanged data. CORS and non-Development boundaries are preserved.

## Context and Orientation

PeopleController currently exposes POST/by-ID GET. Program maps management controllers only in Development. Application owns validation and ports; Infrastructure owns EF/Npgsql. Existing DatabaseFact tests opt in via SQUADSYNC_RUN_DATABASE_TESTS and roll back their writes.

## Plan of Work

Add focused candidate DTO/position/query contracts and validation, then EF filtering/projection/limit and controller routing. Test malformed inputs before database access, cursor round trips, minimal responses and environment isolation; prove SQL translation and real PostgreSQL paging.

## Concrete Steps

From apps/api run dotnet restore, dotnet build, dotnet test. Inspect /swagger/v1/swagger.json through the in-process HTTP host. Run Category=Database with documented local PostgreSQL configuration when available.

Completed commands from `apps/api`:

- `dotnet restore`: successful, all projects up to date.
- `dotnet build`: successful, 0 warnings/errors.
- `dotnet test --no-build --logger 'console;verbosity=minimal'` without database opt-in: 191 unit + 298 integration passed; 21 database cases skipped as designed; 0 failures.
- `dotnet test tests/SquadSync.IntegrationTests --no-build --filter 'Category=Database' --logger 'console;verbosity=minimal'` with documented local configuration and opt-in: 21 passed, 0 skipped/failed.
- Final `dotnet test --no-build --logger 'console;verbosity=minimal'` with local configuration and database opt-in: 191 unit + 319 integration passed; 0 skipped/failed. This includes the final explicit name-order assertion and query parameter metadata changes.
- Swagger inspected through real in-process Development HTTP tests: lowercase q/limit/cursor, required minimal candidate/page properties, nullable cursor and typed 200/400 responses.
- `git diff --check`: passed. All changed paths are under `apps/api/`. Root README checked; no milestone, architecture or workflow transition requiring root-summary edits.

## Validation and Acceptance

Prove first/last/full-name case-insensitive literal matching, deterministic duplicate-name paging, safe 400s, empty pages, cancellation, limit+1 SQL fetch and no non-Development route. Record actual results here.

## Idempotence and Recovery

No migrations or production changes. Database tests use rollback transactions. Repeated validation is safe; do not remove database data or volumes.

## Artifacts and Notes

Branch: feature/212-person-candidates. Canonical issue: https://github.com/bbubb/squadsync/issues/212.

Review focus: identical SQL collation/normalization in ORDER BY and keyset comparisons; no OFFSET/count/client name filtering. Long-name references intentionally end continuation if their anchor is removed; changing data has no snapshot guarantee. Human review/merge and issue closure remain owner actions.

## Interfaces and Dependencies

Application candidate query port/use case; Infrastructure scoped EF adapter; API request/response and controller. Existing dependencies only.
