# Codex CLI Validation

## Purpose

This document defines how Codex CLI should report validation for SquadSync tasks.

## Required Behavior

Codex CLI should run the validation gates relevant to the task.

If validation cannot be run, Codex must document why.

## Docs-Only Tasks

For documentation and workflow changes, Codex should verify:

- new links and paths are correct
- terminology is consistent
- referenced files exist
- issue acceptance criteria are satisfied
- no application code was added unintentionally

## Backend Tasks

When backend code exists, Codex should run:

```bash
dotnet restore
dotnet build
dotnet test
```

If these commands are unavailable because the backend scaffold does not exist yet, document that limitation.

## Frontend Tasks

When frontend code exists, Codex should use the commands documented by the frontend scaffold.

Expected future commands may include:

```bash
npm install
npm run lint
npm run build
npm test
```

## Reporting Format

Codex should include validation notes in the PR using this format:

```markdown
## Validation

Validation performed:
- ...

Commands run:
- ...

Not run:
- ... because ...

Known limitations:
- ...
```

## Validation Honesty

Do not claim that validation passed unless it was actually run.

Do not hide validation limitations. Document them clearly so the human reviewer can decide what to do next.
