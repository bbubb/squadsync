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

## Required Startup Behavior

On startup or re-orientation, the main thread should not jump directly into a branch-thread prompt unless an active sprint and issue have already been confirmed.

The main thread should first determine:

- current phase
- current sprint, if one is already active
- open issues and PRs
- completed setup or workflow tasks
- whether the next action is sprint planning, issue execution, PR review, or closeout

If the next phase/sprint has not been explicitly confirmed, the recommended next action should be to generate or confirm the sprint plan.

## Main Thread Operating Loop

```text
1. Inspect repo/docs/issues/PRs as needed.
2. Summarize current project state.
3. Identify whether the next step is phase planning, sprint planning, issue execution, PR review, or closeout.
4. If a new sprint is next, generate a sprint plan and ask for confirmation before branch execution.
5. If the sprint is already confirmed, identify the next issue or PR to execute/review.
6. Generate or update GitHub Issues as needed after human approval.
7. Provide a focused branch-thread prompt for execution only after sprint/issue scope is confirmed.
8. After branch work completes, review the closeout summary.
9. Update roadmap/issues/docs if needed.
10. Repeat.
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

Recommended next planning action:
Reason:
Relevant docs:
Expected output:

## Sprint Planning Gate

If a new sprint is next, summarize the proposed sprint scope before generating any branch-thread prompt:

- Sprint name:
- Sprint goal:
- Included issue(s):
- Non-goals:
- Validation expectations:
- Project board field suggestions:

## Confirmation Needed

Confirm whether to proceed with this sprint plan. If confirmed, I will create/refine needed issues and generate the branch-thread prompt for the first issue.
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

The main thread may create GitHub Issues directly when the scope is clear and the human owner has approved the sprint plan or issue creation.

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

A branch-thread handoff should happen only after the sprint/issue scope is confirmed.

By default, generate a sprint-level branch-thread prompt. Generate an issue-level branch-thread prompt only when the task needs isolated execution, review, or debugging.

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
