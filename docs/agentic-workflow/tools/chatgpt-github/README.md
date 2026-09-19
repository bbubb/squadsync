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

- [README.md](../../../../README.md)
- [AGENTS.md](../../../../AGENTS.md)
- [CONTRIBUTING.md](../../../../CONTRIBUTING.md)
- [Project roadmap](../../../planning/project-roadmap.md)
- [Agentic workflow index](../../README.md)
- [ChatGPT GitHub navigation](navigation.md)
- the active issue or pull request when work is bounded

Then load context by task:

- complex or multi-step work: [PLANS.md](../../../../PLANS.md)
- product or architecture decisions: [MVP scope](../../../planning/mvp-scope.md), [system overview](../../../architecture/system-overview.md), [domain model](../../../architecture/domain-model.md), and the exact governing [ADRs](../../../adr/)
- documentation changes: [docs/AGENTS.md](../../../AGENTS.md), [documentation standards](../../workflow/documentation-standards.md), [root-summary sync](../../workflow/root-summary-sync.md), and [spec consistency](../../workflow/spec-consistency.md)
- issue creation or refinement: [issue orchestration](../../workflow/issue-orchestration.md), the applicable issue template, and exact source documents; use the [agent-task specification](../../specs/agent-task-spec.md) only for executable tasks and apply `agent-ready` after review
- PR review: [PR review prompt](../../../prompt-library/pr-review.prompt.md), [pull-request specification](../../specs/pull-request-spec.md), [validation gates](../../workflow/validation-gates.md), the related issue, and changed-area guidance
- scoped area work: the nearest local `AGENTS.md` and relevant [repo-native skill](../../../../.agents/README.md)
- Phase 0 historical orientation: [Phase 0 closeout guide](../../../planning/phase-0-closeout-guide.md)

Do not load every referenced document by default. Use `navigation.md`, the active task, and local guidance to select the required set.

## Related Docs

- [Navigation](navigation.md) — how ChatGPT should route common project interactions.
- [Main thread workflow](main-thread-workflow.md) — project-control-room behavior and the canonical main-thread startup prompt.
- [Branch thread workflow](branch-thread-workflow.md) — bounded-work behavior and the canonical branch-thread startup prompt.
- [Context management](../../workflow/context-management.md) — how to preserve durable project context.
- [Prompting standards](../../workflow/prompting-standards.md) — how to structure prompts for ChatGPT and Codex work.
