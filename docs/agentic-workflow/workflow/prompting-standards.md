# Prompting Standards

## Purpose

This document defines how SquadSync prompts should be structured for ChatGPT GitHub work, Codex CLI implementation, and PR review support.

## Default Prompt Shape

Use this structure for most implementation or workflow prompts:

```text
Goal:
[What should change?]

Context:
[Issue, PR, docs, files, errors, or examples that matter]

Constraints:
[Architecture, scope, validation, safety, or workflow boundaries]

Done when:
[Observable completion criteria]
```

This mirrors the Codex best-practice pattern of giving goal, context, constraints, and done criteria.

## Prompting Principles

- Be specific about the current task.
- Name repository paths explicitly.
- Include non-goals to prevent scope drift.
- Reference source-of-truth docs instead of relying on memory.
- Ask for a plan before implementation when the task is complex.
- Use `PLANS.md` for complex or multi-step work.
- Ask the agent to stop when scope, architecture, or validation is unclear.
- Require validation results or documented limitations before PR review.

## ChatGPT GitHub Prompts

Use ChatGPT GitHub for:

- sprint planning;
- issue creation;
- documentation edits;
- PR setup;
- PR review support;
- closeout summaries.

A ChatGPT GitHub prompt should include:

- current phase or sprint;
- issue or PR number when available;
- relevant docs;
- desired GitHub action;
- explicit non-goals;
- whether to create issues/branches/PRs or only advise.

## Codex CLI Prompts

Use Codex CLI for scoped implementation work.

A Codex prompt should include:

- issue number and objective;
- relevant skill name from `.agents/skills/` when applicable;
- expected working area, such as `apps/api/`;
- required local `AGENTS.md` file;
- acceptance criteria;
- validation commands;
- stop conditions.

For the first API scaffold task, Codex should use:

- `AGENTS.md`
- `apps/api/AGENTS.md`
- `PLANS.md` if the task is treated as an ExecPlan
- `.agents/skills/squadsync-api-task/SKILL.md`
- `docs/agentic-workflow/tools/codex-cli/skills/scaffold-backend.md`

## Review Prompts

A review prompt should ask the agent to check:

- issue scope;
- acceptance criteria;
- architecture boundaries;
- validation gates;
- documentation updates;
- follow-up work;
- whether the PR is too broad.

## Anti-Patterns

Avoid prompts that:

- ask for broad implementation without issue scope;
- omit validation expectations;
- ask the agent to decide product scope silently;
- mix planning, implementation, and review in one unclear instruction;
- rely on previous chat context that is not present in the repo.
