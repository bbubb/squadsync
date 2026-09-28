# Codex CLI Context Loading

## Purpose

This document defines how Codex CLI resolves task context without loading unrelated repository documents before it knows the active issue.

## Startup and Issue Retrieval

Use this order for a prompt such as `Work Issue #59.`:

1. Read the already-discovered root `AGENTS.md` as the repository landing page and routing map.
2. Retrieve the active GitHub issue immediately, before loading broad project or workflow context.
3. Confirm readiness using `issue-intake.md`, including objective, scope, non-goals, acceptance criteria, validation, governing sources, and stop conditions.
4. Resolve and read the task-specific sources and scoped instructions described below.

Prefer authenticated, repository-aware GitHub tooling available in the Codex environment. If using the GitHub CLI, identify the repository explicitly so retrieval does not depend on local Git repository discovery:

```powershell
gh issue view <number> --repo bbubb/squadsync
```

Public web search is not the normal fallback for canonical issue state. If authenticated retrieval and the explicit-repository CLI command are unavailable, report the retrieval blocker rather than treating search results as the issue record.

## Minimal Startup Baseline

The normal startup baseline is only:

- root `AGENTS.md`;
- the active issue.

Read `README.md`, `CONTRIBUTING.md`, and the Codex profile entry points only when needed to understand repository purpose, workflow obligations, or the applicable task route. Do not preload `PLANS.md`, roadmap/MVP/product/architecture documents, generic workflow documents, or unrelated skills for every issue. Load them when the issue, affected path, or routing guidance makes them relevant.

## Task-Specific Context Resolution

After readiness is confirmed, load the exact paths required for the task:

- the nearest scoped `AGENTS.md` for each affected area;
- the relevant repo-native skill under `.agents/skills/` and its linked playbook, if needed;
- planning, product, architecture, domain, integration, and ADR documents named by the issue or required by the affected area;
- applicable policy, workflow, and specification documents selected through `AGENTS.md` and `docs/agentic-workflow/workflow/issue-orchestration.md`;
- `PLANS.md` and the issue's ExecPlan only when the task meets the ExecPlan threshold;
- changed-area setup or validation documentation;
- Codex validation and PR-reporting guidance when preparing those task outputs.

For API work, include `apps/api/AGENTS.md` and `.agents/skills/squadsync-api-task/SKILL.md`. Other areas use their nearest local `AGENTS.md` and applicable skill or playbook.

## Context Resolution Gate

Before modifying files, Codex should be able to state:

- the exact issue and goal;
- the affected repository paths;
- the exact task-specific source documents;
- scope and non-goals;
- acceptance criteria;
- validation commands or checks;
- stop conditions;
- whether an ExecPlan or ADR is required.

Do not proceed with unresolved placeholders or generic references such as “relevant docs.” If the issue does not provide enough information to resolve the required context, report that it is not agent-ready.

## Context Conflicts

If docs conflict, Codex should stop and report the conflict instead of choosing silently.

If the issue conflicts with documented architecture, pathing, or MVP scope, implementation must pause until the issue or canonical documentation is corrected through the approved workflow.

## Context Minimization

Load enough context to act correctly, but do not read unrelated future-phase or tool documents merely because they are linked from an index. Follow references when they govern the active task.
