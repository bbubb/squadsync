# Native Agent Assets

This directory contains repo-native agent assets intended for tools that discover project instructions from conventional paths.

## Structure

```text
.agents/
  skills/
    squadsync-api-task/
    squadsync-docs-maintenance/
    squadsync-pr-review/
    squadsync-spec-verification/
```

## Purpose

The files under `.agents/skills/` are concise Codex-native skills.

## Current Skills

- [API task](skills/squadsync-api-task/SKILL.md)
- [Documentation maintenance](skills/squadsync-docs-maintenance/SKILL.md)
- [Pull request review](skills/squadsync-pr-review/SKILL.md)
- [Specification verification](skills/squadsync-spec-verification/SKILL.md)

Expanded human-readable playbooks remain under:

```text
docs/agentic-workflow/tools/codex-cli/skills/
```

## Rule

Keep native skills short and operational. Put deeper explanation in the documentation playbooks and link to them from each skill.
