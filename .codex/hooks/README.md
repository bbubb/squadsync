# Codex Hooks Placeholder

This directory is reserved for future native Codex lifecycle hook configuration.

## Current Status

Placeholder only. No executable hooks are active yet.

## Important Distinction

Human-readable hook planning lives under:

```text
docs/agentic-workflow/tools/codex-cli/hooks/
```

Future native hook configuration may live here after it is tested in Codex and approved through an issue-backed PR.

## Adoption Rule

Add executable hooks only when:

- the manual workflow has repeated enough to justify automation
- the hook behavior is deterministic
- the hook can be tested locally
- the hook reduces review burden
- the human owner approves the change
