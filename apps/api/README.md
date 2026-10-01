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

`dotnet test` runs both the unit-test and in-process health integration-test projects. No database, Docker service, environment variables, secrets, or connection strings are required for the current scaffold.

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

Infrastructure uses EF Core with Npgsql and explicitly maps the current `User` and `Team` domain entities. The API can check PostgreSQL connectivity, but migrations and persisted application workflows are not included yet. PostgreSQL and Compose provide a local development dependency only. Authentication, a frontend, and soccer-subber integration are not included yet.

## References

- [Project Roadmap](../../docs/planning/project-roadmap.md)
- [System Overview](../../docs/architecture/system-overview.md)
- [Domain Model](../../docs/architecture/domain-model.md)
- [Testing Strategy](../../docs/agentic-workflow/workflow/testing-strategy.md)
- [Coding Standards](../../docs/agentic-workflow/workflow/coding-standards.md)
