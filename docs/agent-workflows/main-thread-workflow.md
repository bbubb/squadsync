# Main Thread Workflow

## Purpose

The main ChatGPT thread acts as the project control room for SquadSync. It should maintain overall project coherence, inspect the repository when needed, plan upcoming work, and decide when to branch into focused sprint or issue discussions.

The main thread is not where detailed implementation work should happen. Implementation, debugging, and PR-specific review should usually happen in scoped branch threads.

## Responsibilities

Use the main thread for:

- roadmap and phase decisions
- sprint planning
- deciding what issue should be worked next
- creating or refining GitHub Issues
- reviewing sprint closeout summaries
- deciding whether follow-up issues or ADRs are needed
- checking whether a phase is complete enough to proceed
- keeping GitHub Issues, Projects, docs, and PRs aligned

## Standard Main Thread Startup

When starting a new main planning discussion, use a durable prompt that references the repository and stable docs, but does not hard-code current sprint state.

The main thread should inspect repository docs and GitHub Issues/PRs to determine current state.

## Main Thread Operating Loop

```text
1. Inspect repo/docs/issues/PRs as needed.
2. Summarize current project state.
3. Identify the next recommended phase/sprint/issue.
4. Ask for human confirmation when the next step changes scope or starts a new sprint.
5. Generate or update GitHub Issues as needed.
6. Provide a focused branch-thread prompt for execution.
7. After branch work completes, review the closeout summary.
8. Update roadmap/issues/docs if needed.
9. Repeat.
```

## Startup Response Pattern

When the main thread starts or is asked to re-orient, respond with:

```markdown
## Current Project State

- Current phase:
- Current sprint:
- Relevant open issues:
- Relevant open PRs:
- Recently completed work:

## Recommended Next Step

Recommended task:
Reason:
Relevant docs:
Expected output:

## Confirmation Needed

Confirm whether to proceed with this next step. If confirmed, I will generate the sprint/issue plan and branch-thread prompt.
```

## Sprint Planning in the Main Thread

Sprint planning belongs in the main thread.

For each sprint, the main thread should define:

- phase
- sprint name
- sprint goal
- included issues
- non-goals
- relevant docs/ADRs
- expected branch/thread prompts
- acceptance criteria
- validation expectations
- project board field suggestions

The main thread may create GitHub Issues directly when the scope is clear.

## Branch Thread Handoff

When a sprint or issue is ready for execution, the main thread should produce a branch-thread prompt that includes:

- repository
- current issue/PR/sprint
- goal
- relevant docs
- non-goals
- acceptance criteria
- validation steps
- instructions to inspect the issue/docs first

## Sprint Closeout

When a branch thread completes a sprint or issue, bring a closeout summary back to the main thread. The main thread should then decide whether to:

- mark the issue complete
- create follow-up issues
- update docs or ADRs
- adjust the roadmap
- proceed to the next sprint

## Human Checkpoints

The human owner should be asked for confirmation before:

- starting a new sprint
- changing phase direction
- adding a new architectural pattern
- creating a large batch of issues
- changing scope
- merging work that has not been validated

The human owner should not need to manually manage every implementation detail once the sprint and issue scope are approved.
