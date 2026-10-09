# SquadSync API

The initial SquadSync API foundation is an ASP.NET Core modular-monolith solution at `apps/api/SquadSync.sln`. It contains API, Application, Domain, Infrastructure, unit-test, and integration-test projects.

## Prerequisite

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0). Confirm it is available:

```bash
dotnet --version
```

## Local setup and validation

From the repository root, run:

```bash
cd apps/api
dotnet restore
dotnet build
dotnet test
```

`dotnet test` runs both the unit-test and in-process HTTP integration-test projects. The database persistence test is skipped unless `SQUADSYNC_RUN_DATABASE_TESTS=1`, so ordinary validation requires no database, Docker service, environment variables, secrets, or connection strings.

The in-process HTTP tests verify liveness, unavailable-database readiness, Development-only controller mapping, built-in request validation, and safe errors using a test server. The shared HTTP factory fixes database and demo-seed configuration before service registration, so machine-level connection strings do not affect these tests. Probe controllers live only in the test assembly and are explicitly registered by probe tests; they never ship in the API. These tests do not require a running PostgreSQL instance. The separate manual local smoke check below verifies connectivity to the Compose database.

## Local PostgreSQL smoke check

Start PostgreSQL using [`infra/docker/README.md`](../../infra/docker/README.md). With the example `.env` values, configure the API connection string as follows.

In PowerShell, from `apps/api`:

```powershell
$env:ConnectionStrings__SquadSync = "Host=127.0.0.1;Port=5432;Database=squadsync;Username=squadsync;Password=local_dev_password"
```

In a POSIX-compatible shell:

```bash
export ConnectionStrings__SquadSync='Host=127.0.0.1;Port=5432;Database=squadsync;Username=squadsync;Password=local_dev_password'
```

These are example local credentials from `.env.example`, not production credentials. If you changed the local database settings, use the same values here. ASP.NET Core maps the double underscore to the `ConnectionStrings:SquadSync` configuration key.

## EF migrations and database validation

From `apps/api`, restore the repository-local EF CLI tool (pinned to `10.0.0`):

```powershell
dotnet tool restore
dotnet ef --version
```

Start PostgreSQL using the existing [Compose workflow](../../infra/docker/README.md) and confirm `docker compose ps` reports healthy. Set `ConnectionStrings__SquadSync` to the local connection string matching the ignored `infra/docker/.env` values, as described above. Use this same configuration key for both EF commands and database tests; do not commit credentials.

The historical `InitialUserTeam`, `AddTeamMembership`, and `AddPlayerProfileAndRosterEntry` migrations, the additive `RenameUserToPerson` migration, and current model snapshot are committed in Infrastructure. For reference, these are the generation commands used from `apps/api` (do not rerun them on a checkout that already contains the migrations):

```powershell
dotnet ef migrations add InitialUserTeam --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext --output-dir Persistence/Migrations
dotnet ef migrations add AddTeamMembership --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext --output-dir Persistence/Migrations

dotnet ef migrations add AddPlayerProfileAndRosterEntry --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext --output-dir Persistence/Migrations
```

Before the first update, inspect the local database for pre-existing application tables. Stop if they conflict with `Users`, `Teams`, `TeamMemberships`, or the migration history; do not drop data or delete the named volume to resolve a conflict. When upgrading an existing Sprint 3 database, confirm its history contains `InitialUserTeam` and no conflicting membership schema. Review new migrations before applying them. Apply the committed migrations:

```powershell
dotnet ef database update --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext
dotnet ef migrations has-pending-model-changes --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext
```

`InitialUserTeam` creates `Users` and `Teams` with their required columns and primary keys, plus EF migration history. `AddTeamMembership` adds only `TeamMemberships` with an application-assigned primary key, required User/Team foreign keys with restrictive deletion, required readable-string `TeamRole`, and a unique `(UserId, TeamId)` index. Repeating `database update` is safe once the migrations are recorded.

`AddPlayerProfileAndRosterEntry` adds only `PlayerProfiles` and `RosterEntries`. Both use application-assigned Guid keys. Required foreign keys to `Users` and `TeamMemberships` have unique indexes and restrictive deletion, enforcing one optional dependent per principal. `DominantFoot` is an optional readable string and `RosterStatus` is a required readable string. Optional measurements use PostgreSQL `integer` for total height inches and unconstrained `numeric` for weight pounds, preserving the Domain's decimal precision without imposing a rounding rule. Optional jersey numbers use `character varying(3)` to preserve labels such as `007`. When upgrading the Sprint 4 baseline, confirm both earlier migrations are recorded and no conflicting profile or roster tables exist before applying the additive migration.

`RenameUserToPerson` renames `Users` to `People`, dependent `UserId` columns to `PersonId`, and the corresponding indexes and constraints. It preserves existing GUIDs, rows, relationships, uniqueness, and restrictive deletion. Earlier migration files retain their original terminology and are not rewritten. Inspect migration history before upgrading an existing database; the expected baseline is all three earlier migrations, with no competing `People` table. Rollback only on disposable data. The opt-in `PersonRenameMigrationTests` creates an isolated schema in a rolled-back transaction, exercises upgrade/rollback/re-upgrade with pre-existing linked rows, compares all values and PostgreSQL object identities, and verifies uniqueness, foreign keys, and restrictive deletion.

Run the real PostgreSQL persistence tests explicitly:

```powershell
$env:SQUADSYNC_RUN_DATABASE_TESTS = "1"
dotnet test tests/SquadSync.IntegrationTests --filter "Category=Database"
Remove-Item Env:SQUADSYNC_RUN_DATABASE_TESTS
Remove-Item Env:ConnectionStrings__SquadSync
dotnet test
```

With opt-in enabled, missing `ConnectionStrings__SquadSync` fails the tests with a configuration message. The round-trip tests do not apply migrations: run the update first. The rename test applies scripts only inside its disposable schema. Each round-trip test uses an explicit transaction, persists its principals, clears tracking, and queries fresh entities through EF. The original test verifies Person, Team, and TeamMembership state, including the raw readable-string role. The profile test verifies nullable attributes, the raw dominant-foot string, and decimal measurement precision. The roster test verifies jersey formatting and the raw roster-status string. Each attempts a duplicate dependent with a different Id and requires a `DbUpdateException` caused by the expected PostgreSQL unique index. After that failure, its next database operation is rollback. It then clears tracking and confirms all its validation rows are absent. Transaction disposal also rolls back if an assertion fails. No test rows are committed. These are structural persistence tests; Player-role eligibility is enforced by the Application use case described below. Without opt-in, xUnit reports the database tests as skipped before any database connection is attempted.

## AddPlayerToRoster application use case

`SquadSync.Application.Rosters.AddPlayerToRoster.ExecuteAsync` accepts a new roster-entry Id, existing membership Id, optional jersey number, roster status, and cancellation token. It loads the membership through Application's `IRosterPersistence`, rejects missing memberships with `RosterMembershipNotFoundException` and non-Player roles with `RosterMembershipNotPlayerException`, then constructs and persists a Domain `RosterEntry`. Domain constructor exceptions propagate unchanged; Application does not duplicate intrinsic validation. Successful execution returns the persisted entry.

Infrastructure's `EfRosterPersistence` uses the same scoped `SquadSyncDbContext` for the lookup and atomic `SaveChangesAsync` write. `AddSquadSyncPersistence` registers the adapter as scoped. Memberships are read without tracking to avoid one-to-one relationship fixup changing an existing roster entry. The accepted membership contract is immutable; database foreign keys and unique constraints protect the insertion. Both duplicate membership and duplicate roster-entry Id constraints become Application's `RosterEntryConflictException`; unrelated database errors and cancellation propagate. Successful and conflicting inserts are detached so repeated attempts use database constraints rather than EF identity tracking. When a caller supplies an explicit transaction, it owns rollback/disposal after a persistence failure.

Unit tests exercise the workflow without EF or HTTP. The opt-in `RosterUseCasePersistenceTests` verifies the registered adapter, round trip, and both conflict translations against PostgreSQL in rolled-back transactions. No HTTP endpoint or schema change is introduced.

## Explicit Development demo seed

After starting local PostgreSQL, configuring `ConnectionStrings__SquadSync`, and applying the committed migrations above, intentionally seed the database from `apps/api`:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src/SquadSync.Api -- --seed-demo=true
```

In a POSIX-compatible shell, use `ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/SquadSync.Api -- --seed-demo=true`.

The command persists the scenario and exits without starting the HTTP server. It rejects Production and Staging before database access. Ordinary API startup, migration commands, and ordinary tests do not seed. Missing database configuration or migrations cause failure; the command does not apply migrations, reset the database, delete rows, or update existing data.

The scenario is **SquadSync Demo FC**, with four represented people (Casey Coach Demo, Morgan Manager Demo, Alex Player Demo, and Sam Player Demo), Coach/Manager/Player memberships, two PlayerProfiles, and two player-only RosterEntries: jersey `007` / Active and `12` / Reserved. Infrastructure constructs Domain entities for the principals and profiles; roster creation invokes Application's `AddPlayerToRoster` eligibility check.

All seed identifiers use the reserved `11100000-0000-0000-0000-` prefix: Team suffix `000000000001`, People `000000000011`–`000000000014`, memberships `000000000021`–`000000000024`, profiles `000000000031`–`000000000032`, and roster entries `000000000041`–`000000000042`. Inspect these rows using your PostgreSQL client. Running the same command again leaves row counts unchanged. Missing demo rows are added; conflicting existing seed identifiers or unique relationships fail and roll back the command's transaction, preserving existing and unrelated data. Concurrent invocations may encounter a database uniqueness conflict; rerun after the other invocation completes.

The opt-in `DevelopmentDemoSeedTests` verifies first-run contents, rerun idempotence, Player-only roster membership, unrelated-row preservation, conflict rejection, and ordinary startup without seeding. Like the other database tests, it rolls back its transaction and leaves no demo rows behind; only an intentional successful seed command commits the demo scenario. Non-Development rejection is also tested without a database.

## Run the API

From `apps/api`, start the API in Development on a fixed local port. In PowerShell:

```bash
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src/SquadSync.Api --urls http://localhost:5050
```

In a POSIX-compatible shell, use `ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/SquadSync.Api --urls http://localhost:5050` instead. ASP.NET Core defaults to Production when no environment is set.

When running in Development, use:

- Liveness check: <http://localhost:5050/health> (HTTP 200 while the API is running, even when PostgreSQL is unavailable)
- Readiness check: <http://localhost:5050/health/ready> (HTTP 200 when PostgreSQL accepts connections; HTTP 503 when it is unavailable)
- Swagger UI: <http://localhost:5050/swagger>

Swagger is enabled only in the Development environment. Serilog writes structured logs to the console only.

After confirming both health endpoints return HTTP 200, stop PostgreSQL with `docker compose down` in `infra/docker/` and check again: `/health` remains HTTP 200 while `/health/ready` returns HTTP 503. Start PostgreSQL again and readiness should return HTTP 200. This is a manual, database-backed smoke check; it is separate from `dotnet test`.

## HTTP conventions and local access boundary

MVC controllers are mapped only inside the Development environment gate in `Program.cs`. This applies to the upcoming Person/Team management controllers; no product CRUD endpoints are implemented yet. Staging and Production expose no controller routes. `/health` and `/health/ready` remain mapped in every environment, and Swagger remains Development-only.

Management endpoints are anonymous until an authentication and authorization design is implemented. Run them only in a trusted local environment, bind to localhost, and do not expose them on public networks. The environment gate is an interim route restriction, not a production security mechanism. A `PersonId` or `TeamRole` does not establish caller authority; see [ADR 0006](../../docs/adr/0006-represented-person-identity-api-contract.md).

Future controllers should use `[ApiController]` and request validation attributes. MVC automatically returns HTTP 400 `ValidationProblemDetails` for malformed JSON and invalid models. Return `NotFound()` when the Application use case indicates an absent resource; MVC supplies standard HTTP 404 `ProblemDetails`. Unexpected exceptions use the built-in exception handler and `AddProblemDetails` in every environment, including Development, returning a generic HTTP 500 response without exception details, stack traces, or credentials. Aborted requests follow ASP.NET Core's cancellation handling rather than being converted into HTTP 500 errors. Health responses retain their existing format.

## Not included yet

Infrastructure uses EF Core with Npgsql, explicitly maps the current `Person`, `Team`, `TeamMembership`, `PlayerProfile`, and `RosterEntry` domain entities, and owns their migrations. Opt-in integration tests prove persistence and structural uniqueness against local PostgreSQL. Application includes the AddPlayerToRoster use case described above; HTTP endpoints and other CRUD workflows remain future work. PostgreSQL and Compose provide a local development dependency only. Authentication, a frontend, and soccer-subber integration are not included yet.

## References

- [Project Roadmap](../../docs/planning/project-roadmap.md)
- [System Overview](../../docs/architecture/system-overview.md)
- [Domain Model](../../docs/architecture/domain-model.md)
- [Testing Strategy](../../docs/agentic-workflow/workflow/testing-strategy.md)
- [Coding Standards](../../docs/agentic-workflow/workflow/coding-standards.md)
