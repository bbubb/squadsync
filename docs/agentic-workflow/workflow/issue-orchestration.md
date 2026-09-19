# Issue Orchestration

## Purpose

This document defines how SquadSync routes work by issue type, task intent, and repository path.

The guidance is tool-agnostic. Tool-specific profiles may adapt this routing under `docs/agentic-workflow/tools/`.

## Routing by Task Type

| Task type | Primary owner/tool profile | Expected output |
|---|---|---|
| Phase/sprint planning | ChatGPT GitHub + human owner | sprint plan, issues, roadmap updates |
| Issue creation/refinement | ChatGPT GitHub + human owner | issue using the applicable template; `agent-ready` only after readiness review |
| API implementation | Codex CLI implementation profile | app code, tests, PR |
| Web implementation | Codex CLI implementation profile later | app code, tests, PR |
| Infrastructure docs | ChatGPT GitHub or Codex CLI | scoped infrastructure docs, placeholder updates, or ADR recommendations |
| Documentation update | ChatGPT GitHub or Codex CLI docs skill | docs PR |
| PR review support | ChatGPT GitHub or Codex review skill | review notes, risks, follow-ups |
| Spec verification | Codex CLI using `squadsync-spec-verification` or human/ChatGPT review | consistency findings |
| QA/test review | planned QA role | validation and coverage findings |

## Routing by Path

| Path | Work type | Notes |
|---|---|---|
| `apps/api/` | API/backend implementation | Initial backend implementation starts here |
| `apps/web/` | frontend/web implementation | Frontend implementation area |
| `infra/` | local/cloud infrastructure docs/artifacts | Deployment choices require ADR when material |
| `.github/` | templates, CI, repo automation | Add CI when executable validation exists |
| `docs/architecture/` | system/domain design | Architecture changes may require ADR |
| `docs/planning/` | roadmap, MVP, implementation planning | Planning state lives here |
| `docs/agentic-workflow/` | agent/workflow governance | Workflow changes require issue-backed review |
| `docs/product/` | product, UX, brand direction | Keep lightweight until product/UI work justifies expansion |
| `docs/integrations/` | external service boundaries | soccer-subber integration docs live here |

## Issue Granularity

Prefer issues that are:

- focused enough for one PR
- clear about scope and non-goals
- explicit about validation
- connected to phase/sprint context
- executable without hidden chat context

Avoid issues that combine unrelated app, infra, docs, and workflow changes.

## Dispatch Rule

Before executing an issue, identify:

- primary path affected
- task type
- relevant source-of-truth docs
- expected validation
- likely stop conditions
- whether an ADR may be needed

Use the agent-task specification for work intended for autonomous implementation. Planning, architecture-decision, and exploratory issues should use their applicable contracts and should not receive `agent-ready` until they are converted into an executable task.

## Human Ownership

The human owner approves:

- sprint scope
- architecture changes
- ADRs
- PR merges
- phase transitions
- toolchain changes

Agents may recommend, draft, implement, validate, and summarize, but should not silently change project direction.
