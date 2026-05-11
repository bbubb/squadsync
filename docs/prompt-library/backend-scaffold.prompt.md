# Prompt: Backend Scaffold

Use this prompt when asking an AI coding agent to create the initial backend skeleton.

```text
You are working in the SquadSync repository.

Read first:
- AGENTS.md
- README.md
- docs/planning/mvp-scope.md
- docs/architecture/system-overview.md
- docs/architecture/domain-model.md
- docs/adr/0001-initial-soccer-mvp-scope.md
- docs/adr/0002-use-aspnet-core.md
- docs/adr/0003-use-explicit-membership-model.md
- .github/instructions/backend.instructions.md

Task:
Create the initial ASP.NET Core backend scaffold under /backend.

Required structure:
/backend
  /src
    SquadSync.Api
    SquadSync.Application
    SquadSync.Domain
    SquadSync.Infrastructure
  /tests
    SquadSync.UnitTests
    SquadSync.IntegrationTests

Requirements:
- Create a .NET solution.
- Add project references with proper dependency direction.
- Add an API health endpoint at /health.
- Add Swagger/OpenAPI for local development.
- Add Serilog console logging.
- Add basic appsettings files.
- Add Docker Compose support for PostgreSQL if appropriate for the scaffold task.
- Add a basic GitHub Actions workflow for restore/build/test if requested by the issue.

Non-goals:
- Do not implement domain entities yet.
- Do not implement authentication yet.
- Do not implement soccer-subber integration yet.
- Do not implement lineup suggestion behavior yet.
- Do not introduce broad product areas outside the MVP scope.

Acceptance criteria:
- Solution builds.
- API starts locally.
- /health returns healthy response.
- Swagger is available in development.
- Project dependency direction matches docs.
- README or setup docs are updated if commands are added.
```
