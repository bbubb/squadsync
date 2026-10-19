# Create and retrieve Person PlayerProfiles

This ExecPlan follows `PLANS.md` from the repository root.

## Purpose / Big Picture

Implement Issue #148: explicit POST and singleton GET for an existing Person's optional PlayerProfile, using the accepted HTTP contract and existing schema.

## Progress

- [x] Read issue, scoped instructions, contract and existing membership patterns; start branch from latest main.
- [x] Add Application use cases, EF adapter and Development-only controller.
- [x] Verify unit, real-controller HTTP, provider failure and projection tests.
- [x] Run restore/build/test; prepare one PR for human review.

## Surprises & Discoveries

- The merged contract retains a Proposed status; Issue #148 authorizes correcting only that status.
- Existing mapping and migrations define IX_PlayerProfiles_PersonId and FK_PlayerProfiles_People_PersonId.

## Decision Log

- Decision: Reuse IPersonPersistence and existing MVC/ProblemDetails patterns; restrict database exception translation to the established constraint names and SQL states.
  Rationale: Preserve architecture and avoid hiding unrelated errors or cancellation.
  Date/Author: 2026-10-19 / Codex.

## Outcomes & Retrospective

Implemented POST/GET, narrow persistence and known provider mappings without schema or Domain changes. Restore/build/test pass: 163 unit tests and 212 integration tests passed; 14 existing opt-in database tests skipped. Build has zero warnings/errors. Corrected an async assertion and double response-stream read in the new test harness during validation. Full PostgreSQL HTTP workflow remains the later Sprint 9 slice.

## Context and Orientation

Domain PlayerProfile already guards identity, defined DominantFoot and positive nullable measurements. Membership Application, Infrastructure and API files establish the patterns for this slice. Program maps all controllers only in Development.

## Plan of Work

Add tests alongside narrow Application create/get operations, typed details and persistence port. Implement an EF adapter with no-tracking projection and known constraint mappings. Add a strict request DTO, typed response, controller and DI. Update Swagger assertions and the contract status.

## Concrete Steps

Work under apps/api/src and apps/api/tests. From apps/api run `dotnet restore`, `dotnet build`, `dotnet test`. Review the diff, commit and open a PR linked to #148 without merging.

## Validation and Acceptance

Unit tests cover nullable/canonical values, validation, missing Person, duplicate, token propagation and failures. In-process HTTP tests cover 201/Location/GET, 400/404/409, safe failures, aborted requests and Production/Staging route absence. Adapter tests cover existing constraints, actual Npgsql projection and narrow provider error translation without PostgreSQL.

## Idempotence and Recovery

No migration or database cleanup is required. Tests use fakes and intercepted provider operations. Re-run commands safely; human review owns merge.

## Artifacts and Notes

Issue: https://github.com/bbubb/squadsync/issues/148

Reviewed root summaries: no phase/sprint transition or architecture change. Updated the API README for the new routes and only the accepted contract's status paragraph. No database writes or new dependencies.

## Interfaces and Dependencies

Application depends on Domain and IPersonPersistence. Infrastructure implements IPlayerProfilePersistence with existing EF Core/Npgsql dependencies. API owns wire DTOs and status mapping. No new packages or Domain mutations.
