# Expose player roster creation and individual retrieval

This ExecPlan follows `PLANS.md` from the repository root.

## Purpose / Big Picture

Implement issue #135 against the accepted membership/roster HTTP contract: Development-only POST roster and individual GET with a retrievable Location.

## Progress

- [x] Retrieve agent-ready issue and governing sources; create issue branch from clean main.
- [x] Implement Application team boundary and EF projection/error mapping.
- [x] Implement HTTP contracts and behavior tests.
- [x] Validate and review the issue-scoped change.
- [x] Prepare the issue-linked PR for publication and human review.

## Surprises & Discoveries

Git metadata writes and network access require elevated execution in this environment.

## Decision Log

- Decision: Add a team-scoped entry point sharing AddPlayerToRoster's eligibility/write core; preserve the original non-HTTP signature.
  Rationale: Enforce route ownership before writes without duplicating Player eligibility. Use the existing membership projection to obtain Person details before insertion.
  Date/Author: 2026-10-18 / Codex.
- Decision: Add a narrow roster query port implemented by the existing EF adapter.
  Rationale: Avoid changing existing write-only fakes/callers or introducing collection queries from #136.
  Date/Author: 2026-10-18 / Codex.

## Outcomes & Retrospective

Implemented POST and individual GET with Application-owned team scoping, shared Player eligibility, safe errors and retrievable Location. Restore/build pass with zero warnings/errors; 142 unit and 150 integration tests pass. Eleven opt-in PostgreSQL tests are skipped, including the new rolled-back projection test. No database tests were enabled or migrations changed.

## Context and Orientation

Application Rosters owns AddPlayerToRoster and its persistence port. EF owns uniqueness/FKs. Program maps controllers only in Development. Membership HTTP conventions define string enums and safe ProblemDetails.

## Plan of Work

Share eligibility/write behavior, validate Application inputs, project Person via membership, map only known database constraints, and expose typed HTTP create/read. Cover these using fake ports, real in-process controllers and EF save interceptors.

## Concrete Steps

Work under apps/api. Run dotnet restore, dotnet build, dotnet test from apps/api and git diff --check. Publish issue-linked PR without merging.

## Validation and Acceptance

Prove 201 Location to 200, all three statuses, nullable/leading-zero jerseys, 400/404/409, cross-team isolation before writes, safe 500, cancellation, Staging/Production absence, and targeted adapter failure mapping without PostgreSQL.

## Idempotence and Recovery

No schema/migration or durable database writes. Ordinary tests repeat safely; opt-in database suites stay disabled. Keep one branch and human-reviewed PR.

## Artifacts and Notes

From apps/api: dotnet restore; dotnet build --no-restore; dotnet test --no-build --no-restore all pass. git diff --check passes. Constraint names match the committed migration. Swagger includes POST and individual GET without collection GET. Root README and system overview remain consistent with the Sprint 8 direction. API README documents this slice.

## Interfaces and Dependencies

Existing MVC, EF Core/Npgsql and xUnit. No packages added. Existing membership projection plus new IRosterQueries; existing IRosterPersistence method signatures remain unchanged.
