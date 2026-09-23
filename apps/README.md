# Apps

This directory contains deployable application surfaces for SquadSync.

## Planned Structure

```text
apps/
  api/  ASP.NET Core API and modular monolith backend
  web/  Next.js frontend application
```

## Current Status

No application source code has been scaffolded yet.

The API foundation under `apps/api/` is the first planned application surface. Phase and sprint sequencing belong in the [Project Roadmap](../docs/planning/project-roadmap.md) and current GitHub task records.

## Rules

- Keep application code under the appropriate app boundary.
- Do not place infrastructure or planning documents here.
- Use issue-backed PRs for app scaffolding and implementation.
