# Agent Instructions

This repository is developed with AI assistance, but the architecture is human-owned. Agents should implement small, reviewable tasks that follow the documented project scope, architectural decisions, and agentic workflow rules.

## Project Mission

SquadSync is a soccer-focused team management and match-planning platform. The repository demonstrates full-stack architecture, service boundaries, cloud-readiness, and disciplined engineering practice through a focused MVP.

## Source of Truth

Before making changes, read the relevant documents:

- `README.md` for repository purpose and scope
- `docs/planning/project-roadmap.md` for phase and sprint direction
- `docs/planning/mvp-scope.md` for MVP boundaries
- `docs/architecture/system-overview.md` for system structure
- `docs/architecture/domain-model.md` for domain concepts
- `docs/adr/` for accepted architectural decisions
- `docs/agentic-workflow/README.md` for the layered agentic workflow architecture
- `docs/agentic-workflow/policy/project-policy.md` for workflow policy
- `docs/agentic-workflow/workflow/lifecycle.md` for task lifecycle
- `docs/agentic-workflow/workflow/validation-gates.md` for validation expectations
- `docs/agentic-workflow/workflow/stop-conditions.md` for when agents should pause
- `docs/agentic-workflow/specs/agent-task-spec.md` for agent-ready issue structure
- `docs/agentic-workflow/specs/pull-request-spec.md` for PR expectations
- `docs/agentic-workflow/tools/codex-cli/README.md` for Codex CLI implementation workflow

## Working Boundaries

- Keep changes aligned with the documented soccer MVP.
- Do not add unrequested product areas or broad framework changes.
- Do not implement lineup optimization logic inside the core platform unless a future issue and ADR explicitly approve that direction.
- Do not add new architectural patterns without an ADR.
- Keep service responsibilities clear and documented.
- Do not begin Phase 1 application code before Phase 0 / Sprint 0.1 workflow hardening is merged.

## Tool Roles

- ChatGPT + human owner: architecture, planning, issue generation, ADR drafting, and PR review support.
- GitHub: canonical source of truth for docs, issues, PRs, ADRs, and workflow state.
- Codex CLI: primary implementation agent for scoped `agent-ready` issues.
- GitHub Copilot / GHCP: optional local/editor assistance only; not required for the formal workflow.

## Preferred Engineering Style

Use explicit, readable, production-minded code. Prefer simple domain language and straightforward architecture over clever abstractions.

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
- Stop instead of guessing when scope, architecture, or validation is unclear.

## Review Checklist

Before finishing a task, verify:

- The change stays inside MVP scope.
- The change follows existing docs and ADRs.
- The change follows the agentic workflow docs when applicable.
- The change does not introduce unapproved service responsibilities or algorithm behavior.
- Validation gates were run or limitations are documented.
- Tests were added or the absence of tests is justified.
- Naming is clear to a future reviewer or hiring manager.
