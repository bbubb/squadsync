# Implement atomic Add Player

This ExecPlan follows `PLANS.md`.

## Purpose / Big Picture
Implement owner-approved #215: Development-only POST /api/teams/{teamId}/roster/players creates or reuses a Person and Player membership, and creates a roster entry atomically.

## Progress
- [x] Retrieve issue and dependencies; confirm readiness and governing contracts.
- [x] Inspect existing Application, EF, controller and PostgreSQL fixture patterns; create issue branch.
- [x] Implement focused transaction port, Application orchestration, EF adapter and HTTP contract.
- [x] Add unit, HTTP, Swagger and real PostgreSQL rollback/conflict tests.
- [x] Run restore/build/test and PostgreSQL validation; prepare draft PR.

## Surprises & Discoveries
- Git metadata writes require elevation in this workspace.
- Description attributes are not emitted by the current Swagger configuration; a focused request schema filter documents the variant and jersey rules. HTTP assertions verify the emitted descriptions.
- The existing Swagger path inventory sorts braces before literal segments under the current culture; its expected route order was updated.
- Existing HTTP database tests enlist per-request contexts in a test-owned transaction. The adapter must use a savepoint in that case; ordinary requests own a real transaction.

## Decision Log
- Decision: Application owns membership reuse/eligibility; a dedicated Add Player transaction port owns reads, writes and commit/rollback.
  Rationale: Preserve inward dependencies and existing standalone operations without a generic transaction framework.
  Date/Author: 2026-11-13 / Codex.

## Outcomes & Retrospective
Implemented the additive operation within apps/api with no schema, dependency or web changes. Both variants and membership reuse return the existing typed roster representation. Final PostgreSQL-enabled suite: 567 passed, zero failed/skipped. Human review and merge remain owner actions.

## Context and Orientation
Existing roster POST requires a membership. This additive route accepts exactly one newPerson or personId, returns the existing RosterEntryResponse and GET Location, and creates no PlayerProfile. Program maps controllers only in Development.

## Plan of Work
Validate intrinsic inputs before persistence, then begin a transaction, resolve references, reuse/create Player membership and persist roster. Translate only known constraints, rollback on all failures and propagate cancellation/unrelated errors. Test HTTP contracts and real database preservation.

## Concrete Steps
From apps/api:
- dotnet restore: passed, all projects up to date.
- dotnet build: passed, zero warnings/errors.
- dotnet test --no-build --logger 'console;verbosity=minimal' without opt-in: 210 unit + 330 integration passed; 27 database cases skipped as designed; zero failures.
- Final dotnet test --no-build --logger 'console;verbosity=minimal' with local ConnectionStrings__SquadSync and SQUADSYNC_RUN_DATABASE_TESTS=1: 210 unit + 357 integration passed, zero failures/skips. This includes all 27 Category=Database cases.
- Targeted Swagger/HTTP suite: 37 passed after correcting the route order and explicitly emitting request descriptions.
- git diff --check: passed.

Tests inspect Development Swagger through the in-process HTTP host. New real PostgreSQL tests verify earlier rows reached the database before an actual roster PK collision, complete rollback/preservation, and subsequent successful HTTP requests. Owned transaction rollback/empty commit and externally enlisted savepoints are both exercised. Constraint filters also use injected known/unknown provider failures and cancellation. Deterministic stale-read interleaving exercises a real membership unique constraint; no concurrent-load stress test was run. Database tests leave no durable writes.

Root README checked: it still correctly describes Add Player as a pending connected web workflow. No milestone, architecture or workflow transition requiring a root-summary update. Targeted API README updated; all changed paths stay under apps/api.

## Validation and Acceptance
Cover both variants, reuse, all roles, duplicate and absent references, invalid models, leading zeros, null jerseys, Development/CORS, typed Swagger, no profiles, rollback after staged writes and known concurrent uniqueness constraints.

## Idempotence and Recovery
No automatic retry. Failed operations roll back; ambiguous committed responses require caller reconciliation. Tests roll back external transactions and verify full-row fingerprints, without migrations or durable writes.

## Artifacts and Notes
Branch feature/215-atomic-add-player. PR must contain Closes #215. Human owner reviews and merges.

## Interfaces and Dependencies
Application AddPlayer and dedicated transaction interfaces; EF/Npgsql adapter; existing roster controller/response and DI. No schema or dependency changes.
