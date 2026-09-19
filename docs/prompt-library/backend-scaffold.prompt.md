# Prompt: Initial API Scaffold

Use this task-specific prompt only after the scaffold issue has been refreshed against the current repository. The active GitHub issue remains the executable task record; this file supplies stable API-scaffold guidance.

```text
You are working in the SquadSync repository.

Task issue:
[Exact GitHub issue URL]

Goal:
Create the initial ASP.NET Core API scaffold under apps/api/.

Target .NET version:
[Exact version approved by the issue]

Scope:
[Copy the exact authorized scaffold scope from the refreshed issue]

Issue-specific stop conditions:
[Copy the issue's stop conditions]

Required baseline context:
- AGENTS.md
- README.md
- CONTRIBUTING.md
- docs/planning/project-roadmap.md
- the active GitHub issue
- docs/agentic-workflow/tools/codex-cli/README.md
- docs/agentic-workflow/tools/codex-cli/operational-profile.md
- docs/agentic-workflow/tools/codex-cli/context-loading.md
- docs/agentic-workflow/tools/codex-cli/issue-intake.md
- docs/agentic-workflow/tools/codex-cli/validation.md
- docs/agentic-workflow/tools/codex-cli/pr-reporting.md

Task-specific source-of-truth documents:
- apps/api/AGENTS.md
- apps/api/README.md
- docs/planning/mvp-scope.md
- docs/architecture/system-overview.md
- docs/architecture/domain-model.md
- docs/adr/0001-initial-soccer-mvp-scope.md
- docs/adr/0002-use-aspnet-core.md
- docs/adr/0003-use-explicit-membership-model.md
- docs/agentic-workflow/workflow/coding-standards.md
- docs/agentic-workflow/workflow/testing-strategy.md
- docs/agentic-workflow/workflow/validation-gates.md
- .agents/skills/squadsync-api-task/SKILL.md
- docs/agentic-workflow/tools/codex-cli/skills/scaffold-backend.md

Required structure:
apps/api/
  src/
    SquadSync.Api/
    SquadSync.Application/
    SquadSync.Domain/
    SquadSync.Infrastructure/
  tests/
    SquadSync.UnitTests/
    SquadSync.IntegrationTests/

Requirements:
- Create the .NET solution and projects approved by the issue.
- Add project references with the documented dependency direction.
- Add an API health endpoint at /health.
- Add Swagger/OpenAPI for local development.
- Add Serilog console logging.
- Add basic appsettings files.
- Wire the unit and integration test projects.

Non-goals:
- Do not implement domain entities, authentication, frontend behavior, soccer-subber integration, or lineup suggestions.
- Do not add Docker Compose, CI, cloud infrastructure, or persistence unless the refreshed issue explicitly includes it.
- Do not create root-level /backend or /frontend directories.

Acceptance criteria:
[Copy the observable criteria from the refreshed issue]

Validation:
cd apps/api
dotnet restore
dotnet build
dotnet test

Instructions:
- Resolve every bracketed field before acting.
- Compare the issue path, scope, acceptance criteria, and validation with the canonical docs above.
- If the issue still targets /backend, lacks an approved .NET version, conflicts with the docs, or leaves validation unclear, stop and report that the issue is not agent-ready.
- Keep the change inside the refreshed issue scope and prepare PR notes that map each acceptance criterion to evidence.
```
