# Agent Instructions

This repository is developed with AI assistance, but the architecture is human-owned. Agents should implement small, reviewable tasks that follow the documented project scope and architectural decisions.

## Project Mission

SquadSync is a soccer-focused team management and match-planning platform. The public repository is a portfolio-safe vertical slice that demonstrates full-stack architecture, service boundaries, cloud-readiness, and disciplined engineering practice.

## Source of Truth

Before making changes, read the relevant documents:

- `README.md` for repository purpose and scope
- `docs/planning/mvp-scope.md` for MVP boundaries
- `docs/architecture/system-overview.md` for system structure
- `docs/architecture/domain-model.md` for domain concepts
- `docs/adr/` for accepted architectural decisions
- `docs/agent-workflows/codex-task-lifecycle.md` for AI task workflow

## Hard Boundaries

Do not add or expose:

- Proprietary substitution algorithm internals
- Generalized competition-platform abstractions beyond the approved soccer MVP
- Private business strategy or unreleased IP
- Large unrequested framework changes
- New architectural patterns without an ADR

## Preferred Engineering Style

Use explicit, readable, production-minded code. Prefer simple domain language and boring architecture over clever abstractions.

Backend changes should preserve a modular ASP.NET Core structure:

- Domain: entities, value objects, domain rules
- Application: use cases, interfaces, DTOs, validation orchestration
- Infrastructure: persistence, external clients, logging implementations
- API: controllers/endpoints, request/response contracts, composition root

Frontend changes should preserve feature-oriented organization and type-safe API access.

## Task Rules

- One issue or prompt should produce one focused change.
- Do not implement broad product areas in one pass.
- Update docs when behavior, architecture, or scope changes.
- Add or update tests when adding behavior.
- Keep commits and PRs reviewable.
- Explain trade-offs in PR descriptions when applicable.

## Review Checklist

Before finishing a task, verify:

- The change stays inside MVP scope.
- The change follows existing docs and ADRs.
- No proprietary algorithm or private platform concept was exposed.
- The code builds locally or the limitation is documented.
- Tests were added or the absence of tests is justified.
- Naming is clear to a future reviewer or hiring manager.
