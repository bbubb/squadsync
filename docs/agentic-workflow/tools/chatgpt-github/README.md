# ChatGPT GitHub Tool Profile

## Purpose

This profile defines how ChatGPT should operate when used with the GitHub Connector inside the SquadSync project.

ChatGPT may support planning, documentation, issue creation, branch/PR setup, review, and closeout work. Because it can modify repository artifacts through tools, it must follow the same repo-owned workflow discipline as other project tools.

## Role

ChatGPT GitHub is a planning and workflow tool, not the primary implementation worker.

Primary uses:

- explain project context from repository docs
- support ad hoc project discussions
- plan phases, sprints, and issues
- create or update GitHub issues
- create documentation branches and PRs when requested
- review PRs against acceptance criteria
- produce sprint or issue closeout summaries
- identify follow-up work without silently expanding scope

## Required Operating Rules

ChatGPT GitHub must:

- treat GitHub docs, issues, PRs, and ADRs as the source of truth
- use `navigation.md` to route orientation, planning, issue/PR, review, closeout, and ad hoc requests
- create or reference an issue before opening a PR, unless the human explicitly asks for an emergency direct fix
- keep changes scoped to the issue or prompt
- avoid merging PRs
- surface scope changes as follow-up issues or recommendations
- document validation honestly
- stop when architecture, scope, or validation is unclear

## Tool Boundaries

ChatGPT GitHub may create planning and documentation artifacts.

Codex CLI remains the primary implementation profile for scoped code-generation tasks.

Symphony remains a future/reference orchestrator profile.

## Canonical Thread Prompts

- [Standard Main Thread Prompt](main-thread-workflow.md#standard-main-thread-prompt) — start or re-orient the project control room.
- [Standard Branch Thread Prompt](branch-thread-workflow.md#standard-branch-thread-prompt) — begin bounded sprint, issue, PR, debugging, or closeout work after scope is confirmed.

These files own the exact prompt wording. The roadmap and other workflow documents should link to them rather than maintain duplicate prompt copies.

## Required Context

Always begin with:

- `README.md`
- `AGENTS.md`
- `CONTRIBUTING.md`
- `docs/planning/project-roadmap.md`
- `docs/agentic-workflow/README.md`
- `docs/agentic-workflow/tools/chatgpt-github/navigation.md`
- the active issue or pull request when work is bounded

Then load context by task:

- complex or multi-step work: `PLANS.md`
- product or architecture decisions: MVP scope, system overview, domain model, and relevant ADRs
- documentation changes: `docs/AGENTS.md`, documentation standards, root-summary sync, and spec consistency
- issue creation or refinement: agent-task specification and relevant source documents
- PR review: pull-request specification, validation gates, the related issue, and changed-area guidance
- scoped area work: the nearest local `AGENTS.md` and relevant repo-native skill
- Phase 0 history or closeout questions: the Phase 0 closeout guide

Do not load every referenced document by default. Use `navigation.md`, the active task, and local guidance to select the required set.

## Related Docs

- `navigation.md` — how ChatGPT should route common project interactions.
- `main-thread-workflow.md` — project-control-room behavior and the canonical main-thread startup prompt.
- `branch-thread-workflow.md` — bounded-work behavior and the canonical branch-thread startup prompt.
- `../../workflow/context-management.md` — how to preserve durable project context.
- `../../workflow/prompting-standards.md` — how to structure prompts for ChatGPT and Codex work.
