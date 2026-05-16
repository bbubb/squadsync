# Spec Consistency

## Purpose

Spec consistency rules prevent agents from implementing against stale, partial, or conflicting documentation.

The goal is to make the repository a reliable source of truth before implementation work begins.

## Canonical Order

When documents overlap, prefer this order unless a newer ADR says otherwise:

1. Accepted ADRs
2. `docs/planning/mvp-scope.md`
3. `docs/architecture/system-overview.md`
4. `docs/architecture/domain-model.md`
5. `docs/planning/project-roadmap.md`
6. `docs/agentic-workflow/`
7. Area README files
8. Chat summaries or comments

## Derivative Docs

A derivative doc is any document that summarizes, applies, or operationalizes another canonical doc.

Examples:

- README summarizes roadmap and architecture.
- Codex skills operationalize workflow docs.
- Area READMEs summarize system overview pathing.
- Product notes summarize MVP scope.

When editing a derivative doc, check the upstream source first.

## Discrepancy Rule

If two docs conflict, do not silently choose one unless the source of truth is clear.

Report:

```markdown
## Spec Discrepancy

Conflicting docs:

Conflict:

Likely source of truth:

Recommended fix:
```

## Agent Guidance

Agents should stop when:

- an implementation target path conflicts across docs
- MVP scope conflicts with roadmap or architecture docs
- a tool profile conflicts with generic workflow rules
- an ADR conflicts with a non-ADR doc
- a derivative doc appears newer but contradicts a canonical doc

## Resolution

Resolve discrepancies through one of these paths:

- update the stale derivative doc
- create a follow-up docs issue
- create or update an ADR when architecture is affected
- ask the human owner to confirm source of truth
