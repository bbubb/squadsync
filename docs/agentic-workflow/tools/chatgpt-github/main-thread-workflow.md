# Main Thread Workflow

## Purpose

The main ChatGPT thread acts as the project control room for SquadSync. It should maintain overall project coherence, inspect the repository when needed, plan upcoming work, and decide when to branch into focused sprint or issue discussions.

The main thread is not where detailed implementation work should happen. Implementation, debugging, and PR-specific review should usually happen in scoped branch threads.

## Responsibilities

Use the main thread for:

- roadmap and phase decisions
- guided reorientation and understanding checks at phase boundaries
- sprint planning
- deciding what issue should be worked next
- creating or refining GitHub issues
- reviewing sprint closeout summaries
- deciding whether follow-up issues or ADRs are needed
- checking whether a phase is complete enough to proceed
- keeping GitHub issues, GitHub Projects, docs, and PRs aligned

## Standard Main Thread Prompt

Use this canonical prompt when starting or re-orienting the main SquadSync planning discussion:

```text
We are working on SquadSync inside the ChatGPT Project “SquadSync.”

Use the GitHub connector for repository context and authorized GitHub actions when needed.

Repository:
https://github.com/bbubb/squadsync

Thread role:
Main planning thread / project control room

Goal:
Reconstruct the current project state from GitHub, explain the next planning decision, and preserve human ownership before bounded execution begins.

Required starting context:
- README.md
- AGENTS.md
- CONTRIBUTING.md
- docs/planning/project-roadmap.md
- docs/agentic-workflow/README.md
- docs/agentic-workflow/tools/chatgpt-github/README.md
- docs/agentic-workflow/tools/chatgpt-github/navigation.md
- relevant open GitHub issues, pull requests, and Project state
- additional source-of-truth documents selected through navigation.md for the current question

Instructions:
- Derive the current phase, sprint, and task state from GitHub; do not rely on prompt text or chat memory.
- Identify stale issues, conflicting documents, missing decisions, or unvalidated work before recommending execution.
- Explain the recommendation, why it is next, its risk, and the proposed next action.
- When the user requests reorientation or the project is at a phase boundary, provide a curated reading order, explain the relevant product, architecture, and workflow concepts in plain language, and ask focused questions that can expose misunderstandings before closeout or execution.
- Do not begin bounded sprint/issue execution or make unapproved repository changes until the intended state and action are confirmed.
```

This file owns the main-thread startup prompt. Other documents should link here instead of maintaining duplicate copies.

## Required Startup Behavior

On startup or re-orientation, the main thread should not jump directly into a branch-thread prompt unless an active sprint and issue have already been confirmed.

The main thread should first determine:

- current phase
- current sprint, if one is already active
- open issues and PRs
- completed setup or workflow tasks
- whether the next action is sprint planning, issue execution, PR review, or closeout

If the next phase/sprint has not been explicitly confirmed, the recommended next action should be to generate or confirm the sprint plan.

When reorientation is requested or a phase transition is being considered, the main thread should also identify what the human owner needs to review and understand. This is a deliberate checkpoint, not a requirement to turn every routine planning response into a tutorial.

## Main Thread Operating Loop

```text
1. Inspect repo/docs/issues/PRs as needed.
2. Summarize current project state.
3. Identify whether the next step is phase planning, sprint planning, issue execution, PR review, or closeout.
4. If a new sprint is next, generate a sprint plan and ask for confirmation before branch execution.
5. If the sprint is already confirmed, identify the next issue or PR to execute/review.
6. Generate or update GitHub issues as needed after human approval.
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

## Guided Reorientation

Include this section when requested or at a phase boundary:

- Curated reading order:
- Product/MVP concepts to understand:
- Architecture concepts to understand:
- Workflow and tool-role concepts to understand:
- Decisions or assumptions that need human confirmation:

## Understanding Check

Ask a small set of focused questions that test the concepts needed for the next decision. Correct misunderstandings before recommending phase closeout or bounded execution.

## Sprint Planning Gate

If a new sprint is next, summarize the proposed sprint scope before generating any branch-thread prompt:

- Sprint name:
- Sprint goal:
- Scope:
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
- scope
- included issues
- non-goals
- exact source-of-truth document and ADR paths
- expected branch/thread prompts
- acceptance criteria
- validation expectations
- project board field suggestions

The main thread may create GitHub issues directly when the scope is clear and the human owner has approved the sprint plan or issue creation.

Creating an agent-task issue is a drafting step. Before adding or retaining `agent-ready`, compare the completed issue with the current repository paths, source documents, architecture decisions, acceptance criteria, validation, and stop conditions. If an existing label is stale, surface it for correction before handoff.

## Branch Thread Handoff

When a sprint or issue is ready for execution, the main thread should use the [Standard Branch Thread Prompt](branch-thread-workflow.md#standard-branch-thread-prompt) and populate:

- repository
- current issue/PR/sprint
- goal
- scope
- exact task-specific source paths
- non-goals
- acceptance criteria
- validation steps
- instructions to inspect the issue/docs first

A branch-thread handoff should happen only after the sprint/issue scope is confirmed.

By default, generate a sprint-level branch-thread prompt. Generate an issue-level branch-thread prompt only when the task needs isolated execution, review, or debugging.

## Sprint Closeout

When a branch thread finishes its scoped execution, bring a closeout candidate summary back to the main thread. The main thread should then recommend whether to:

- mark the issue complete
- create follow-up issues
- update docs or ADRs
- adjust the roadmap
- proceed to the next sprint

Present closeout evidence, limitations, unresolved questions, and any required reorientation to the human owner. Do not declare a sprint or phase closed solely from an agent summary; closure requires the applicable issue/PR state and human confirmation.

## Human Checkpoints

The human owner should be asked for confirmation before:

- starting a new sprint
- closing a sprint or phase, or declaring a phase transition ready
- changing phase direction
- adding a new architectural pattern
- creating a large batch of issues
- changing scope
- merging work that has not been validated

The human owner should not need to manually manage every implementation detail once the sprint and issue scope are approved.
