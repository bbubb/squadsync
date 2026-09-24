# Codex CLI Task Prompt

## Purpose

This canonical template starts Codex CLI work on an agent-ready issue while keeping dynamic task state in GitHub.

## Template

```text
You are working in the SquadSync repository.

Task issue:
[Exact GitHub issue URL or number]

Goal:
[One clear outcome copied from or reconciled with the issue]

Expected working area:
[Exact repository paths]

Nearest local instructions:
[Exact local AGENTS.md path for each affected area]

Repo-native skill:
[Exact .agents/skills/.../SKILL.md path, or “none” with reason]

Task-specific source-of-truth documents:
- [Exact planning, architecture, ADR, workflow, or integration paths required for this task]
- [Include PLANS.md and an ExecPlan only when the task meets the complexity threshold]

Scope:
[Files, areas, and behavior allowed to change]

Non-goals:
[What must not change]

Acceptance criteria:
[Observable checklist from the issue]

Validation:
[Exact commands and checks]

Stop conditions:
[Known reasons to pause]

Required baseline context:
- AGENTS.md
- README.md
- CONTRIBUTING.md
- the active GitHub issue
- docs/planning/project-roadmap.md
- docs/agentic-workflow/README.md
- docs/agentic-workflow/tools/codex-cli/README.md
- docs/agentic-workflow/tools/codex-cli/operational-profile.md
- docs/agentic-workflow/tools/codex-cli/context-loading.md
- docs/agentic-workflow/tools/codex-cli/issue-intake.md
- docs/agentic-workflow/tools/codex-cli/validation.md
- docs/agentic-workflow/tools/codex-cli/pr-reporting.md

Instructions:
- Resolve every bracketed field and name task-specific sources by exact repository path before changing files.
- Confirm issue readiness, scope, non-goals, acceptance criteria, validation, and stop conditions.
- If the issue conflicts with current pathing, architecture, MVP scope, or validation guidance, stop and report the discrepancy.
- Keep the change scoped to the issue and affected paths.
- Do not introduce architecture changes without an approved ADR.
- Update docs if behavior, architecture, workflow, or operating commands change.
- Run available validation gates or document why they cannot be run.
- Create an issue-scoped branch from `main`, push it, and open a draft PR after scoped implementation and available validation unless a documented stop condition prevents it.
- Include PR notes that map changes and evidence to each acceptance criterion; document any validation limitation in the draft PR.
- Do not merge the PR or close the issue.
- Suggest follow-up issues instead of expanding scope.
```

## Usage

ChatGPT or the human owner may populate this template after the issue is confirmed. A task-specific prompt may add stable guidance for a known work type, but it must not override the issue or canonical docs.

The issue remains the canonical executable task record. Do not use this template to rehabilitate a stale or incomplete issue silently.
