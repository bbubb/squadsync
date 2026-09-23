# Phase 0 Closeout Guide

## Status

Accepted historical orientation. Current phase and sprint state belong in `docs/planning/project-roadmap.md` and GitHub issues.

## Purpose

This guide explains what Phase 0 created, why it matters, and how the foundation should be used in later phases.

It is written for Brandon, future ChatGPT project threads, Codex CLI sessions, and reviewers who need a fast orientation to the project foundation.

## What Phase 0 Accomplished

Phase 0 turned SquadSync from a repo shell into an agent-ready MVP workspace.

It established:

- product scope and MVP boundaries;
- architecture direction;
- repo structure;
- planning and roadmap flow;
- documentation governance;
- issue and PR standards;
- Codex-native plans and skills;
- ChatGPT GitHub workflow guidance;
- planned Codex subagent roles;
- validation and stop-condition rules.

## Mental Model

```text
GitHub repo = source of truth
README.md = public landing page
AGENTS.md = root agent operating map
PLANS.md = complex-task planning standard
GitHub Issues = executable task records
Pull Requests = review and validation gates
docs/agentic-workflow = human-readable process manual
.agents/skills = native Codex skill entry points
.codex = future native Codex configuration area
ChatGPT GitHub = planning, docs, issues, PR setup, review support
Codex CLI = implementation worker
Brandon = architect, reviewer, merge authority
```

## How Humans Use the Foundation

Humans should start with:

1. `README.md` for project overview.
2. `docs/planning/project-roadmap.md` for phase direction.
3. `docs/planning/mvp-scope.md` for scope boundaries.
4. `docs/architecture/system-overview.md` for system structure.
5. `CONTRIBUTING.md` for issue, branch, PR, and validation flow.

Use this guide when the amount of documentation feels hard to navigate.

## How ChatGPT Uses the Foundation

ChatGPT GitHub should use:

- `AGENTS.md` for root operating guidance;
- `docs/agentic-workflow/tools/chatgpt-github/navigation.md` for routing behavior;
- `docs/agentic-workflow/tools/chatgpt-github/main-thread-workflow.md` for roadmap and phase-level discussion;
- `docs/agentic-workflow/tools/chatgpt-github/branch-thread-workflow.md` for focused sprint, issue, or PR discussions.

ChatGPT should create durable records in GitHub issues, PRs, ADRs, or docs when decisions need to survive the current chat.

## How Codex Uses the Foundation

Codex CLI should use:

- `AGENTS.md` for root repo guidance;
- local `AGENTS.md` files for active work areas;
- `.agents/skills/` for native skill entry points;
- `PLANS.md` for complex multi-step tasks;
- `docs/agentic-workflow/tools/codex-cli/` for expanded human-readable playbooks.

For Phase 1 API work, Codex should begin with:

- `apps/api/AGENTS.md`;
- `.agents/skills/squadsync-api-task/SKILL.md`;
- `docs/agentic-workflow/tools/codex-cli/skills/scaffold-backend.md`.

## Status at Phase 0 Closeout

Established artifacts at closeout:

- `README.md`
- `AGENTS.md`
- `CONTRIBUTING.md`
- `PLANS.md`
- `docs/planning/`
- `docs/architecture/`
- `docs/product/`
- `docs/agentic-workflow/`
- `.agents/skills/`
- `apps/api/AGENTS.md`
- `docs/AGENTS.md`

Reserved or planned at closeout:

- `apps/web/`
- `infra/`
- `.codex/rules/`
- `.codex/hooks/`
- active Codex subagent automation
- MCP configuration
- CI/CD workflows
- AWS deployment assets

## How Later Phases Build on Phase 0

Phase 1 uses the foundation to scaffold the API under `apps/api/` with validation and clean architecture boundaries.

Phase 2 uses the domain docs and testing strategy to implement core domain and persistence.

Phase 3 uses issue orchestration and validation gates to build roster management APIs.

Phase 4 activates `apps/web/` and should add web-specific validation and possibly strengthen `apps/web/AGENTS.md`.

Phase 5 adds match and lineup planning workflows while preserving MVP boundaries.

Phase 6 integrates soccer-subber through service boundaries instead of embedding optimization logic directly in the core platform.

Phase 7 introduces event and notification readiness.

Phase 8 evaluates cloud deployment and scale-to-zero infrastructure choices.

Phase 9 revisits AI-assisted planning summaries once core workflows are stable.

## Closeout Standard

Phase 0 is complete when:

- the repo has a clear landing page;
- canonical planning and architecture docs exist;
- the agentic workflow is discoverable;
- ChatGPT and Codex have clear tool profiles;
- native Codex plans and skills exist;
- active vs placeholder areas are labeled;
- Phase 1 can begin without relying on hidden chat context.

After Phase 0 closes, new workflow improvements should come from real implementation friction, not speculative expansion.
