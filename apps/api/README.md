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

## Run the API

From `apps/api`, start the API in Development on a fixed local port. In PowerShell:

```bash
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src/SquadSync.Api --urls http://localhost:5050
```

In a POSIX-compatible shell, use `ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/SquadSync.Api --urls http://localhost:5050` instead. ASP.NET Core defaults to Production when no environment is set.

When running in Development, use:

- Health check: <http://localhost:5050/health>
- Swagger UI: <http://localhost:5050/swagger>

Swagger is enabled only in the Development environment. Serilog writes structured logs to the console only.

## Not included yet

This scaffold intentionally does not include PostgreSQL, Docker Compose, CI, authentication, a frontend, or soccer-subber integration. Those capabilities remain planned for later work.

## References

- [Project Roadmap](../../docs/planning/project-roadmap.md)
- [System Overview](../../docs/architecture/system-overview.md)
- [Domain Model](../../docs/architecture/domain-model.md)
- [Testing Strategy](../../docs/agentic-workflow/workflow/testing-strategy.md)
- [Coding Standards](../../docs/agentic-workflow/workflow/coding-standards.md)
