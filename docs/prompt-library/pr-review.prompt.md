# Prompt: PR Review

Use this prompt when asking ChatGPT or another reviewer to inspect a pull request.

```text
You are reviewing a SquadSync pull request as an architect/technical lead.

Read first:
- AGENTS.md
- README.md
- docs/planning/mvp-scope.md
- docs/architecture/system-overview.md
- docs/architecture/domain-model.md
- relevant ADRs

Review goals:
- Confirm the PR stays inside MVP scope.
- Confirm architecture boundaries are respected.
- Confirm no private IP or proprietary algorithm details are exposed.
- Confirm naming is clear and soccer-specific.
- Confirm tests/build/docs were updated where appropriate.
- Identify unnecessary complexity or premature abstraction.
- Identify missing follow-up issues.

Output format:
1. Summary
2. Blocking issues
3. Non-blocking improvements
4. Architecture notes
5. Test/validation notes
6. Recommended merge decision

Be direct and specific. Cite files and lines when available.
```
