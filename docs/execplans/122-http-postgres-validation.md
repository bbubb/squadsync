# Validate People and Teams HTTP flows without durable writes

This ExecPlan follows `PLANS.md` and implements [Issue #122](https://github.com/bbubb/squadsync/issues/122).

## Purpose / Big Picture

Prove the Development controllers reach the real Application and EF adapters through separate HTTP requests against migrated PostgreSQL, while preserving developer and demo data.

## Progress

- [x] Verified issue readiness and clean current main; created feature/122-http-postgres-validation.
- [x] Inspected scoped instructions, ADRs, existing HTTP contracts, and database conventions.
- [x] Implement shared-transaction HTTP tests and failure-path preservation checks.
- [x] Verify Swagger and document local requests and opt-in safety.
- [x] Run baseline and live PostgreSQL validation.
- [x] Review code and documentation; prepare the issue branch for draft PR submission.

## Surprises & Discoveries

- Default shell sandbox initialization fails. Reviewed escalation works for shell commands.
- Existing Compose PostgreSQL is healthy; connection environment is absent. Use ignored local configuration only in the validation process.
- First live run passed seven tests but the deliberate-failure test expected xUnit's base exception instead of its exact FailException. Cleanup fingerprints passed even in that failed run. Corrected the exception assertion; all eight database tests then passed.

## Decision Log

- Decision: Replace only the test host's scoped DbContext construction, retaining production persistence adapter registrations. Enlist every fresh context in one externally owned Npgsql connection/transaction.
  Rationale: A transaction on an unrelated context cannot cover HTTP writes. Sequential requests on one connection provide explicit rollback ownership without schema creation or committed cleanup.
  Date/Author: 2026-10-09 / Codex.
- Decision: Require exact committed migration history and mapped public schema before writes; compare complete table fingerprints before/after, including on injected failure.
  Rationale: Test isolation must preserve existing data and relationships, rather than assume empty tables.
  Date/Author: 2026-10-09 / Codex.

## Outcomes & Retrospective

The scoped HTTP round trips and preservation checks pass against the existing migrated PostgreSQL database. Ordinary tests and Swagger verification pass without database configuration. Human review and merge remain outside agent authority.

## Context and Orientation

`ApiTestFactory` defaults to an empty connection string. The new database host supplies explicit configuration and retains `EfPersonPersistence`/`EfTeamPersistence`. Ordinary endpoint tests already cover exhaustive HTTP validation with fake ports; new tests add database evidence for representative invalid requests.

## Plan of Work

Build a test-owned transaction runner with preflight, fresh enlisted contexts, observer connection, unconditional rollback, and full-row fingerprints. Exercise both POST/GET flows and forced failure after successful writes. Add a Docker-independent Swagger contract test and concise API README examples.

## Concrete Steps

From `apps/api`, run `dotnet restore`, `dotnet build`, and `dotnet test`. With ignored local credentials supplied only to the child process, run `dotnet test tests/SquadSync.IntegrationTests --filter "Category=Database"` with `SQUADSYNC_RUN_DATABASE_TESTS=1`. Do not apply migrations or seed as part of these tests.

## Validation and Acceptance

Require real adapter types, distinct request context IDs, successful independent reads of both created resources, database-visible normalized values inside the shared transaction, and invisibility outside it. Require standard 400/404 ProblemDetails and unchanged database fingerprints after invalid calls, success, and deliberate failure. Verify Swagger methods/statuses and baseline Docker independence. Report unavailable checks as blockers to full acceptance.

## Idempotence and Recovery

The runner never commits. Dispose the HTTP host, roll back in `finally`, then compare all five application tables and migration history through a separate connection. Serialize this test collection; briefly lock application tables to prevent concurrent writes during preservation checks. Stop on unexpected history/schema or inability to establish isolation. No deletes, truncation, schema rewriting, or volume removal.

## Artifacts and Notes

Validation on 2026-10-09:

- `dotnet restore` passed; `dotnet build` passed with zero warnings/errors.
- Ordinary `dotnet test`: 106 unit and 49 integration tests passed; all eight database tests skipped.
- Opt-in `Category=Database`: eight passed, none skipped, including existing seed, readiness/startup, migration, roster, and structural persistence regressions.
- Both new HTTP tests verified exact migration history and mapped schema, fresh enlisted request contexts, real adapter registrations, normalized database rows, and unchanged complete-row fingerprints after rollback. The success test also verified uncommitted rows are invisible to another connection; the failure test preserved the exact injected assertion through cleanup.
- Swagger's four routes, methods, and documented status codes passed in an ordinary test.
- No live migrations, seed commands, schema resets, data deletion, or volume operations were performed. Credentials remained process-local and are absent from tracked changes.
- Final staged diff review and whitespace checks passed. All local links and heading anchors in the changed documentation passed. Root README and system overview remain accurate; no architecture or phase direction changed.
- Draft PR submission follows this validated branch; GitHub owns subsequent review status.

## Interfaces and Dependencies

Existing xUnit DatabaseFact/Category conventions, WebApplicationFactory, EF Core 10, Npgsql, production Application ports and EF adapters. No new packages, product behavior, migrations, or ADRs.
