# Skill: Update Docs

## Purpose

Use this skill when Codex is asked to update documentation, workflow policy, ADRs, or templates.

## Flow

1. Identify the canonical doc location before editing.
2. Check whether the change belongs in generic policy/workflow/specs or a tool-specific profile.
3. Keep the update narrow and issue-backed.
4. Remove stale references when moving or renaming docs.
5. Validate links, paths, and terminology.
6. Report documentation impact in the PR.

## Placement Rules

Use generic layers for reusable project rules:

```text
docs/agentic-workflow/policy/
docs/agentic-workflow/workflow/
docs/agentic-workflow/specs/
```

Use tool profiles for tool-specific behavior:

```text
docs/agentic-workflow/tools/<tool-name>/
```

## Stop Conditions

Stop if a documentation change would alter product scope, application architecture, or workflow governance without an issue or ADR.
