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

`dotnet test` runs both the unit-test and in-process health integration-test projects. The database persistence test is skipped unless `SQUADSYNC_RUN_DATABASE_TESTS=1`, so ordinary validation requires no database, Docker service, environment variables, secrets, or connection strings.

The in-process health tests verify liveness and the unavailable-database readiness response using a test server. They do not require a running PostgreSQL instance. The separate manual local smoke check below verifies connectivity to the Compose database.

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

The `InitialUserTeam` and `AddTeamMembership` migrations and model snapshot are committed in Infrastructure. For reference, these are the generation commands used from `apps/api` (do not rerun them on a checkout that already contains the migrations):

```powershell
dotnet ef migrations add InitialUserTeam --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext --output-dir Persistence/Migrations
dotnet ef migrations add AddTeamMembership --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext --output-dir Persistence/Migrations
```

Before the first update, inspect the local database for pre-existing application tables. Stop if they conflict with `Users`, `Teams`, `TeamMemberships`, or the migration history; do not drop data or delete the named volume to resolve a conflict. When upgrading an existing Sprint 3 database, confirm its history contains `InitialUserTeam` and no conflicting membership schema. Review new migrations before applying them. Apply the committed migrations:

```powershell
dotnet ef database update --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext
dotnet ef migrations has-pending-model-changes --project src/SquadSync.Infrastructure --startup-project src/SquadSync.Api --context SquadSyncDbContext
```

`InitialUserTeam` creates `Users` and `Teams` with their required columns and primary keys, plus EF migration history. `AddTeamMembership` adds only `TeamMemberships` with an application-assigned primary key, required User/Team foreign keys with restrictive deletion, required readable-string `TeamRole`, and a unique `(UserId, TeamId)` index. Repeating `database update` is safe once the migrations are recorded.

Run the real PostgreSQL persistence test explicitly:

```powershell
$env:SQUADSYNC_RUN_DATABASE_TESTS = "1"
dotnet test tests/SquadSync.IntegrationTests --filter "Category=Database"
Remove-Item Env:SQUADSYNC_RUN_DATABASE_TESTS
Remove-Item Env:ConnectionStrings__SquadSync
dotnet test
```

With opt-in enabled, missing `ConnectionStrings__SquadSync` fails the test with a configuration message. The test does not apply migrations: run the update first. It writes a valid User, Team, and TeamMembership inside one explicit transaction, clears tracking, queries fresh entities through EF, and checks their persisted state, including the raw readable-string role. It attempts a second membership with a different Id for the same User/Team pair and requires a `DbUpdateException` caused by PostgreSQL's unique index. After that failure, its next database operation is rollback. It then clears tracking and confirms all validation rows are absent. Transaction disposal also rolls back if an assertion fails. No test rows are committed. Without opt-in, xUnit reports the test as skipped before any database connection is attempted.

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

## Not included yet

Infrastructure uses EF Core with Npgsql, explicitly maps the current `User`, `Team`, and `TeamMembership` domain entities, and owns their migrations. An opt-in integration test proves persistence and membership uniqueness against local PostgreSQL. Application CRUD workflows and API endpoints are not included yet. PostgreSQL and Compose provide a local development dependency only. Authentication, a frontend, and soccer-subber integration are not included yet.

## References

- [Project Roadmap](../../docs/planning/project-roadmap.md)
- [System Overview](../../docs/architecture/system-overview.md)
- [Domain Model](../../docs/architecture/domain-model.md)
- [Testing Strategy](../../docs/agentic-workflow/workflow/testing-strategy.md)
- [Coding Standards](../../docs/agentic-workflow/workflow/coding-standards.md)
