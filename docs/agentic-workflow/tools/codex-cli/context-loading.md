# Codex CLI Context Loading

## Purpose

This document defines how Codex CLI should load enough repository context to execute an issue safely without flooding every task with unrelated documents.

## Required Baseline Context

Before modifying files, Codex CLI should read:

1. `AGENTS.md`
2. `README.md`
3. `CONTRIBUTING.md`
4. the current GitHub issue
5. `docs/planning/project-roadmap.md`
6. `docs/agentic-workflow/README.md`
7. `docs/agentic-workflow/tools/codex-cli/README.md`
8. `docs/agentic-workflow/tools/codex-cli/operational-profile.md`
9. `docs/agentic-workflow/tools/codex-cli/issue-intake.md`
10. `docs/agentic-workflow/tools/codex-cli/validation.md`
11. `docs/agentic-workflow/tools/codex-cli/pr-reporting.md`

Read the issue early because its scope, affected paths, acceptance criteria, and validation determine which additional documents are relevant.

## Task-Specific Context Resolution

Then load the exact paths required for the task:

- the nearest scoped `AGENTS.md` for each affected area;
- the relevant repo-native skill under `.agents/skills/`;
- planning, product, architecture, domain, integration, and ADR documents named by the issue or required by the affected area;
- applicable policy, workflow, and specification documents selected through `AGENTS.md` and `docs/agentic-workflow/workflow/issue-orchestration.md`;
- `PLANS.md` and the issue's ExecPlan, if the task meets the ExecPlan threshold;
- changed-area setup or validation documentation.

For API work, this must include `apps/api/AGENTS.md` and `.agents/skills/squadsync-api-task/SKILL.md`. Other areas must use their nearest local `AGENTS.md` and applicable skill or playbook.

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
