# Validation Gates

## Purpose

Validation gates define how SquadSync work proves it is complete enough for review.

A validation gate may be automated, manual, or documentation-only depending on project phase.

## Gate Types

### Docs-Only Gate

Used for documentation, planning, ADR, and workflow changes.

Expected checks:

- referenced paths exist
- links are accurate
- terminology is consistent
- roadmap/status language is aligned
- no application code is added unintentionally
- scope and non-goals are clear

### Backend Gate

Used once backend application code exists.

Expected checks:

```bash
dotnet restore
dotnet build
dotnet test
```

If a command is unavailable because the scaffold does not exist yet, document that limitation in the PR.

### Frontend Gate

Used once frontend application code exists.

Expected checks will likely include:

```bash
npm install
npm run lint
npm run build
npm test
```

Exact commands should be confirmed when the frontend scaffold is created.

### Integration Gate

Used later when local services, Docker Compose, database, soccer-subber, or AWS-facing paths exist.

Expected checks may include:

- Docker Compose startup
- API health checks
- database migration checks
- contract tests
- local integration smoke tests

## PR Reporting Format

Every PR should include a validation section with these elements:

- validation performed
- commands run
- checks not run, with reasons
- known limitations

Example:

````markdown
## Validation

Validation performed:
- Docs-only validation completed.

Commands run:
```bash
# command output summary
```

Not run:
- Backend tests because the backend scaffold does not exist yet.

Known limitations:
- None.
````

Use only the relevant lines for the task.

## When Validation Cannot Be Run

If validation cannot be run, the implementation agent must document:

- which validation was expected
- why it could not be run
- whether the limitation is temporary
- what a human reviewer should do next

Do not mark validation as complete when it was not performed.

## Validation Ownership

The active implementation tool should run available validation gates before opening or finalizing a PR.

The current active implementation profile is `docs/agentic-workflow/tools/codex-cli/`.

The human owner decides whether the validation evidence is sufficient to merge.
