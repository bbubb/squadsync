# Codex Task Lifecycle

## Status

Superseded by the layered agentic workflow architecture created in Phase 0 / Sprint 0.1.

## Purpose

This document is retained as a compatibility pointer for existing references.

The canonical workflow now lives under:

- `docs/agentic-workflow/README.md`
- `docs/agentic-workflow/policy/project-policy.md`
- `docs/agentic-workflow/workflow/lifecycle.md`
- `docs/agentic-workflow/workflow/validation-gates.md`
- `docs/agentic-workflow/workflow/stop-conditions.md`
- `docs/agentic-workflow/specs/agent-task-spec.md`
- `docs/agentic-workflow/specs/pull-request-spec.md`
- `docs/agentic-workflow/tools/codex-cli/README.md`

## Current Workflow Summary

```text
Architecture docs define direction.
GitHub issues define executable work.
Codex CLI implements scoped tasks.
Validation gates prove readiness for review.
Human review decides merge.
Docs and ADRs preserve decisions.
```

## Current Tool Roles

- Human owner: owns architecture, scope, review, and merge decisions.
- ChatGPT: supports architecture, planning, issue creation, ADR drafting, and PR review support.
- GitHub: acts as the canonical source of truth for docs, issues, PRs, and workflow state.
- Codex CLI: primary implementation agent for scoped `agent-ready` issues.
- GitHub Copilot / GHCP: optional local/editor assistance only.

## Migration Note

Future workflow updates should be made under `docs/agentic-workflow/` rather than expanding this legacy file.
