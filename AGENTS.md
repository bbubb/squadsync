# Agent Instructions

This repository is developed with AI assistance, but the architecture is human-owned. Agents should implement small, reviewable tasks that follow the documented project scope, architectural decisions, and agentic workflow rules.

## Project Mission

SquadSync is a soccer-focused team management and match-planning platform. The repository demonstrates full-stack architecture, service boundaries, cloud-readiness, and disciplined engineering practice through a focused MVP.

## Source of Truth

Before making changes, read the relevant documents:

- `README.md` for repository purpose, current state, and navigation
- `CONTRIBUTING.md` for issue, branch, PR, and validation rules
- `docs/planning/project-roadmap.md` for phase and sprint direction
- `docs/planning/mvp-scope.md` for MVP boundaries
- `docs/product/product-brief.md` for product direction
- `docs/product/ux-notes.md` for early UX direction
- `docs/product/brand-notes.md` for lightweight voice/brand direction
- `docs/architecture/system-overview.md` for system structure
- `docs/architecture/domain-model.md` for domain concepts
- `docs/adr/` for accepted architectural decisions
- `docs/agentic-workflow/README.md` for the layered agentic workflow architecture
- `docs/agentic-workflow/policy/project-policy.md` for workflow policy
- `docs/agentic-workflow/workflow/lifecycle.md` for task lifecycle
- `docs/agentic-workflow/workflow/branching-strategy.md` for branch/PR standards
- `docs/agentic-workflow/workflow/testing-strategy.md` for TDD-oriented validation expectations
- `docs/agentic-workflow/workflow/coding-standards.md` for Clean Architecture, SOLID, and implementation standards
- `docs/agentic-workflow/workflow/documentation-standards.md` for document headers, status, and versioning
- `docs/agentic-workflow/workflow/root-summary-sync.md` for root summary consistency
- `docs/agentic-workflow/workflow/spec-consistency.md` for source-of-truth discrepancy handling
- `docs/agentic-workflow/workflow/issue-orchestration.md` for route-by-task/path guidance
- `docs/agentic-workflow/workflow/validation-gates.md` for validation expectations
- `docs/agentic-workflow/workflow/stop-conditions.md` for when agents should pause
- `docs/agentic-workflow/specs/agent-task-spec.md` for agent-ready issue structure
- `docs/agentic-workflow/specs/pull-request-spec.md` for PR expectations
- `docs/agentic-workflow/tools/chatgpt-github/README.md` for ChatGPT GitHub Connector workflow
- `docs/agentic-workflow/tools/chatgpt-github/navigation.md` for ChatGPT routing behavior
- `docs/agentic-workflow/tools/codex-cli/README.md` for Codex CLI implementation workflow
- `docs/agentic-workflow/tools/codex-cli/rules/README.md` for Codex-specific rules
- `docs/agentic-workflow/tools/codex-cli/skills/README.md` for Codex task playbooks
- `docs/agentic-workflow/tools/codex-cli/hooks/README.md` for Codex hook structure
- `docs/agentic-workflow/tools/codex-cli/subagents/README.md` for planned Codex subagent roles

## Working Boundaries

- Keep changes aligned with the documented soccer MVP.
- Do not add unrequested product areas or broad framework changes.
- Do not implement lineup optimization logic inside the core platform unless a future issue and ADR explicitly approve that direction.
- Do not add new architectural patterns without an ADR.
- Keep service responsibilities clear and documented.
- Do not begin Phase 1 application code before Phase 0 operational harness hardening is merged.

## Tool Roles

- ChatGPT GitHub: planning, issue generation, documentation edits, PR setup, PR review support, and closeout summaries.
- GitHub: canonical source of truth for docs, issues, PRs, ADRs, and workflow state.
- Codex CLI: primary implementation agent for scoped `agent-ready` issues.

Local editor assistants may be used by a human contributor, but they are not part of the canonical workflow and must not replace issue scope, validation gates, or human review.

## Preferred Engineering Style

Use explicit, readable, production-minded code. Prefer simple domain language and straightforward architecture over clever abstractions.

API changes should preserve a modular ASP.NET Core structure under `apps/api/`:

- Domain: entities, value objects, domain rules
- Application: use cases, interfaces, DTOs, validation orchestration
- Infrastructure: persistence, external clients, logging implementations
- API: controllers/endpoints, request/response contracts, composition root

Web changes should preserve feature-oriented organization and type-safe API access under `apps/web/`.

## Task Rules

- One issue or prompt should produce one focused change.
- Use short-lived branches from `main`.
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
- The change follows branching, testing, coding, documentation, and validation standards.
- Root summaries and derivative docs were checked when relevant.
- The change does not introduce unapproved service responsibilities or algorithm behavior.
- Validation gates were run or limitations are documented.
- Tests were added or the absence of tests is justified.
- Naming is clear to a future reviewer or hiring manager.
