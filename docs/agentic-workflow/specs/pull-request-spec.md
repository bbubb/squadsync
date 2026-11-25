# Pull Request Specification

## Purpose

This specification defines what every SquadSync pull request should communicate.

A PR should make agent-generated work easy for a human reviewer to evaluate.

## Required Sections

### Summary

What changed and why.

### Related Issue(s)

The GitHub issue or issues addressed by the PR.

### Scope

The main files, folders, behavior, or docs changed.

### Non-Goals

Anything intentionally excluded from the PR.

### Acceptance Criteria Status

A brief checklist showing whether each issue acceptance criterion was met.

### Validation

Commands or checks run, including documentation-only validation when applicable.

### Documentation Impact

Whether README, architecture docs, ADRs, workflow docs, or prompt templates changed.

### Architecture Notes

Any architecture-relevant trade-offs, boundaries, or ADR needs.

### Known Limitations

Checks not run, incomplete criteria, temporary constraints, or review risks that remain.

### Suggested Follow-ups

New issues or workflow improvements discovered during implementation.

For delegated work, use these existing PR sections to record the material change, decision, deviation, deferral, evidence and revisit trigger, as applicable. Link the tracker grant and identify the reviewed head and validation evidence before the trusted gate considers the PR. Summarize material outcomes in the existing tracker closeout. See [execution policy](../policy/project-policy.md#execution-policy).

## PR Quality Standard

A PR is reviewable when:

- it is linked to a clear issue
- it stays inside scope
- validation is reported honestly
- known limitations are explicit
- docs or ADRs are updated when needed
- follow-up work is captured
- implementation decisions are understandable without private chat context
