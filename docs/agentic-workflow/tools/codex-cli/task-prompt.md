# Codex CLI Task Prompt

## Purpose

This template provides a reusable prompt pattern for starting Codex CLI work on an agent-ready issue.

## Template

```text
You are working in the SquadSync repository.

First, read the required project context:
- AGENTS.md
- README.md
- docs/planning/project-roadmap.md
- docs/planning/mvp-scope.md
- docs/architecture/system-overview.md
- docs/architecture/domain-model.md
- relevant ADRs in docs/adr/
- docs/agentic-workflow/README.md
- docs/agentic-workflow/policy/project-policy.md
- docs/agentic-workflow/workflow/lifecycle.md
- docs/agentic-workflow/workflow/validation-gates.md
- docs/agentic-workflow/workflow/stop-conditions.md
- docs/agentic-workflow/specs/agent-task-spec.md
- docs/agentic-workflow/specs/pull-request-spec.md
- docs/agentic-workflow/tools/codex-cli/README.md
- docs/agentic-workflow/tools/codex-cli/operational-profile.md
- docs/agentic-workflow/tools/codex-cli/context-loading.md
- docs/agentic-workflow/tools/codex-cli/issue-intake.md
- docs/agentic-workflow/tools/codex-cli/validation.md
- docs/agentic-workflow/tools/codex-cli/pr-reporting.md

Task issue:
[GitHub issue link or number]

Objective:
[clear objective]

Scope:
[files/areas likely affected]

Non-goals:
[what must not be changed]

Acceptance criteria:
[checklist]

Validation:
[commands/checks expected]

Stop conditions:
[known reasons to pause]

Instructions:
- Keep the change scoped to the issue.
- Do not add unrelated product scope.
- Do not introduce architecture changes without an ADR.
- Update docs if behavior, architecture, or workflow changes.
- Run available validation gates or document why they cannot be run.
- Prepare PR notes that map changes back to acceptance criteria.
- Stop and report if scope, architecture, or validation is unclear.
```

## Usage

ChatGPT or the human owner may adapt this prompt for a specific GitHub issue before starting Codex CLI.

The issue itself remains the canonical task record.
