# Implement membership creation and individual retrieval

This ExecPlan follows `PLANS.md` from the repository root.

## Purpose / Big Picture

Implement [issue #132](https://github.com/bbubb/squadsync/issues/132) against the accepted [Sprint 8 contract](../../docs/architecture/membership-roster-http-contract.md): Development-only membership creation and a retrievable Location with Person summary.

## Progress

- [x] Retrieved agent-ready issue, confirmed current main and created issue branch.
- [x] Implemented narrow Application, EF and HTTP slice.
- [x] Add focused tests and run restore/build/test.
- [x] Review diff and prepare PR for publication without merging.

## Surprises & Discoveries

- Git operations require process-local safe.directory and elevated filesystem access in this environment.
- Initial HTTP success tests read a disposed response stream twice; reading the body once fixed the test harness.

## Decision Log

- Decision: Reuse existing Person/Team ports for reference validation; add only membership write and team-scoped projection capabilities.
  Rationale: Follow existing use cases and ADR 0005 without a generic repository or list capability.
  Date/Author: 2026-10-11 / Codex.
- Decision: Accept a validated string role in HTTP and serialize the response role as a string.
  Rationale: Enforce canonical names without changing global enum behavior.
  Date/Author: 2026-10-11 / Codex.

## Outcomes & Retrospective

Implementation complete: create and individual GET satisfy the accepted contract. Restore and build pass (zero warnings/errors); the ordinary suite passes with 123 unit tests and 93 integration tests. Nine opt-in database tests are skipped, including the added real membership adapter round trip. No database writes, migrations or owner-authored #133 files were involved.

## Context and Orientation

Domain TeamMembership validates IDs and TeamRole. Infrastructure already maps unique PersonId/TeamId and restrictive FKs. Program maps controllers only in Development. People/Teams establish controller and use-case conventions. #133 list work is excluded.

## Plan of Work

Implement create/get projection with explicit validation and reference/conflict exceptions. Add EF join projection and constrained PostgreSQL exception translation. Add typed controller with local role validation. Verify fake ports, in-process HTTP, and EF model/exception handling without a database.

## Concrete Steps

Work under apps/api/src and apps/api/tests. Run `dotnet restore`, `dotnet build`, `dotnet test` from apps/api. Review `git diff --check` and complete PR with issue and contract links.

## Validation and Acceptance

Prove generated IDs, all roles, Person summary, 201 Location to 200, 400/404/409, cross-team 404, safe 500, cancellation, known constraint translation and unrelated failure propagation, unique EF mapping, and Staging/Production route absence. Ordinary suite requires no PostgreSQL.

## Idempotence and Recovery

No migrations, schema changes, seed execution or durable test writes. Keep one issue branch and human-reviewed PR. Tests can repeat safely. No automatic merge.

## Artifacts and Notes

`dotnet restore`, `dotnet build --no-restore`, and `dotnet test --no-restore` pass. Checked PostgreSQL constraint names against the committed migration and verified documentation links, Swagger operations, and diff whitespace. Root README and system overview remain consistent with the active Sprint 8 direction; API README now explains this slice. PR publication is the final workflow action.

## Interfaces and Dependencies

Application membership use cases and IMembershipPersistence; existing IPersonPersistence/ITeamPersistence; EF Core/Npgsql; MVC and xUnit. No new packages.
