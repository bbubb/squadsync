# Sprint 12 delegated pilot

## Status

Proposed operating setup for [#223](https://github.com/bbubb/squadsync/issues/223). The owner approved the design direction in [#224](https://github.com/bbubb/squadsync/issues/224). [#226](https://github.com/bbubb/squadsync/issues/226) supplies the offline prototype under supervised execution. Activation requires the owner's separate grant after the prerequisites below.

## Assignment

Under [Sprint 12 tracker #207](https://github.com/bbubb/squadsync/issues/207), run [#217](https://github.com/bbubb/squadsync/issues/217), then [#218](https://github.com/bbubb/squadsync/issues/218), then [#219](https://github.com/bbubb/squadsync/issues/219). Recheck readiness and merged dependencies before dispatch, starting with #217 against merged #216 and the accepted #215 contract. The grant may cover gate-operated merges for #217 and #218 only. For #219, assemble verification and closeout evidence after both merges. The owner accepts Sprint 12 and reviews and merges any #219 PR. Expiry follows [execution policy](../../policy/project-policy.md#execution-policy). Sprint 13 requires its own authorization.

## Runner recommendation

Use one owner-managed local controller invoking `codex exec` for implementation and a second, fresh read-only Codex session for review. Use a dedicated noninteractive host account and an owner-started background job that can run independently of the editor and be restarted. Keep controller code in an owner-controlled location outside the PR checkout. Before activation, prove that the selected host stays available when the editor is closed and that it can isolate workers and run the full validation stack. An interactive VS Code terminal alone provides no such guarantee. [OpenAI non-interactive mode](https://learn.chatgpt.com/docs/non-interactive-mode)

| Option | Feasibility and constraints |
| --- | --- |
| Local controller with Codex CLI | Reuses the documented local browser, API and database setup. Requires a host available for the pilot, separate worker isolation and a verified way to publish a branch and draft PR. OpenAI recommends API-key authentication for programmatic CLI use. It bills at API rates through the Platform account, with any applicable API credits. Enterprise Codex access tokens are another option when the workspace grants access. Interactive ChatGPT sign-in alone is evidence of interactive access. Verify the actual runner identity and approve its billing before use. GitHub authentication is separate. [OpenAI authentication](https://learn.chatgpt.com/docs/auth) · [OpenAI pricing](https://learn.chatgpt.com/docs/pricing) |
| Codex Cloud and GitHub Code Review | Cloud workspaces can continue independently of the local machine. Verify entitlement, usage limits, repository connection, setup and ability to run a disposable database and real browser against the loopback services. GitHub Code Review requires a connected repository and enabled review access. Feedback alone does not prove a positive verdict for the final SHA or a qualifying GitHub approval. An external controller/gate would still be needed. [Cloud environments](https://learn.chatgpt.com/docs/environments/cloud-environments) · [GitHub Code Review](https://learn.chatgpt.com/docs/third-party/github) |
| GitHub-hosted Actions controller | Fresh hosted jobs remove the local uptime dependency. Standard hosted runners are free for public repositories and private usage depends on plan and quotas. Model usage is separate. A hosted design needs a reviewed trusted control workflow, distinct credential-free PR workers, and a demonstrated browser/API/PostgreSQL setup. The existing CI workflows do not provide those pieces. [Actions billing](https://docs.github.com/en/actions/concepts/billing-and-usage) · [Actions security](https://docs.github.com/en/actions/reference/security/secure-use) |

### Repository evidence and owner checks

- The checked-in [API CI](../../../../.github/workflows/api-ci.yml) runs restore, build and test on Ubuntu for PRs and pushes to `main`. [Web CI](../../../../.github/workflows/web-ci.yml) runs formatting, lint, API type drift, type checking, tests and build on Ubuntu for PRs and `main` pushes affecting `apps/web/**` or that workflow file. Both declare read-only contents access. Neither provisions the live browser-to-PostgreSQL smoke. Workflow YAML establishes job behavior, while enforced checks and approvals are repository settings.
- GitHub issue reads are available through the connected tool. [#77](https://github.com/bbubb/squadsync/issues/77) records why the Windows sandbox cannot assume access to the owner's CLI keyring. [#74](https://github.com/bbubb/squadsync/issues/74) defines the supervised draft-PR handoff and closure check. Neither verifies unattended GitHub write access.
- Owner verification is needed for host availability and isolation, Codex entitlement/authentication and billing, Actions approval settings and quotas where applicable, branch rules and bypass lists, required check names and sources, reviewer eligibility, permitted merge method and availability of a distinct restricted identity. Keep supervised execution if any required capability is unavailable.

### Trust and validation

The controller starts each issue from current `main`, verifies the issue contract and scoped instructions, and reuses the one bound issue branch and PR if they already exist. It checks the patch paths before publishing through a constrained GitHub write capability outside the model process. It opens a draft, runs applicable validation and requests independent review. Once the PR meets draft handoff criteria, the publisher can mark it ready for formal review under the grant. Use one active controller and at most two correction/review cycles per PR before escalating. On restart, recover workflow state from GitHub and regenerate missing trusted evidence. If a response is lost after a write or merge, read the branch, PR and issue before retrying. Verify merge commit, PR merge status and issue closure before dispatching the next issue. Route missing automatic issue closure to the owner under the existing lifecycle.

Stop and escalate scope, architecture, backend contract or schema, dependency, toolchain, credential, security or workflow changes that need a new decision. Stop on conflicts, unresolved findings or unsafe or unexplained product behavior, including ambiguous Person matches, duplicate creation, partial writes, stale roster data or disclosure. Follow the repository [stop conditions](../../workflow/stop-conditions.md).

Run implementation and tests in an isolated disposable workspace with synthetic data, without host credential stores, GitHub write/merge tokens, protected secrets or access to the controller filesystem. Keep model authentication outside PR commands and verify that isolation on the selected host. If that separation cannot be demonstrated, stop. A fresh reviewer reads the final diff, governing sources and validation evidence, with no write, merge or credential access. The controller associates the verdict with the PR and the exact base and head it supplied. Require an explicit proceed verdict, findings and residual risks. Treat missing, inconclusive or negative verdicts as blockers. Never promote author-controlled comments or PR text into trusted review or test results.

Use the [web local preview](../../../../apps/web/README.md#quality-checks-and-local-preview) and [API PostgreSQL validation](../../../../apps/api/README.md#http-to-postgresql-validation) guidance. Use local Compose PostgreSQL, committed migrations and intentional fixtures. Run the browser against `http://127.0.0.1:3000` with the API on its documented Development loopback endpoint. For each issue, capture the exact checkout SHA, commands, scenarios, actual observations and outcomes through a trusted validation observer. Verify the target UI path, API response and persisted behavior where the issue requires it. Use a human observer if the runner cannot reliably observe browser behavior. Report any skipped or unrun checks as such. Required smoke or PostgreSQL evidence missing for the current SHA blocks the merge.

Keep GitHub publishing, formal review and merge capabilities distinct. Prefer a separate repo-scoped GitHub App/service identity for the controller's merge step if the owner approves. A GitHub App installation token can be narrowed to selected repositories and permissions and expires after one hour. The merge API requires Contents write, which is broader than a merge-only permission. Keep that credential within the trusted gate, rely on server-side protection for `main`, and exclude the identity from bypass lists. Confirm a distinct reviewer who qualifies under the actual rules. If the rules require a human review, wait for it. AI findings and verdicts remain independent evidence. GitHub PR authors cannot approve their own PRs. [Installation tokens](https://docs.github.com/en/apps/creating-github-apps/authenticating-with-a-github-app/generating-an-installation-access-token-for-a-github-app) · [Required reviews](https://docs.github.com/en/pull-requests/how-tos/review-pull-requests/approving-a-pull-request-with-required-reviews)

### GitHub event and merge checks

Publish through the owner-approved GitHub integration or separate publisher identity after verifying that opening and updating a test PR starts ordinary `pull_request` CI. For changes made with a workflow's `GITHUB_TOKEN`, PR opened, synchronized and reopened events can create runs requiring approval by a user with write access. Other events generally do not start another workflow, apart from dispatch exceptions. A manually dispatched workflow's job is not a substitute for an eligible required PR check. Keep validation on unprivileged `pull_request` workflows. Never check out or execute PR code in a privileged `pull_request_target` workflow or merge job. If the event path or settings leave CI missing, pending or approval-blocked, stop. [Workflow trigger rules](https://docs.github.com/en/actions/how-tos/write-workflows/choose-when-workflows-run/trigger-a-workflow) · [Actions security](https://docs.github.com/en/actions/reference/security/secure-use)

Immediately before a merge, the trusted controller reads live repository and PR state and requires all of the following:

1. The owner grant is active and unexpired. The repository, base branch, same-repository head branch, bound PR number and issue match the approved dispatch for #217 or #218. The issue is next in sequence and all prerequisites are met. The PR is open, ready for review and conflict-free.
2. The entire file diff, including both sides of renames, fits the per-issue path allowlist recorded by the owner at activation. Reject incomplete file lists and changes to workflow, permissions, dependencies, backend contracts or schema. Independent review also checks behavioral scope, issue criteria and unresolved findings.
3. The review verdict and trusted integration evidence apply to the current head. Verify current `main`, an up-to-date branch and the latest eligible PR checks from the expected workflows and producers. Require actual success for both API and Web CI on these web issues and for all enforced checks. Match results to the current head or the GitHub test merge commit for that head and base, as appropriate. Pending, failed, skipped or unverified results fail closed. Confirm the server enforces required checks, formal approvals and up-to-date merging without bypass. Web CI is path-filtered. A skipped workflow can leave a required check pending. Stop if the configured rules or merge queue cannot be satisfied by the existing triggers. [Required check behavior](https://docs.github.com/en/pull-requests/how-tos/merge-and-close-pull-requests/troubleshooting-required-status-checks) · [Protected branches](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/about-protected-branches)
4. Use the protected PR merge endpoint with the verified head in its `sha` parameter and the approved merge method. GitHub rejects a changed head with HTTP 409. Revalidate after head or base changes and defer to server-side branch protection at merge time. A denial, unknown result or unavailable protection blocks progression. [Merge API](https://docs.github.com/en/rest/pulls/pulls#merge-a-pull-request)

## No-merge dry run

Run the [Node.js evaluator](pilot/sprint-12-gate.mjs) from the repository root. It reads one local JSON snapshot and emits one JSON decision. Exit 0 means the synthetic case would proceed. Exit 1 means rejection. Both report `liveAuthority: false`.

```powershell
node docs/agentic-workflow/tools/codex-cli/pilot/sprint-12-gate.mjs docs/agentic-workflow/tools/codex-cli/pilot/proposed-217.test-only.json
```

The [synthetic #217 snapshot](pilot/proposed-217.test-only.json) shows the input format. The owner-side grant pins an issue, same-repo branch, PR, path allowance, publisher, readers and reviewers. The GitHub snapshot supplies readiness, merged prerequisites, current `main`, a complete changed-file list, protection, formal approvals and the latest eligible checks. The independent review and the integration observer supply their own records for the same repository, issue, PR, base and head. A path ending in `/` allows files beneath that directory. Any other path matches one file. Both names in a rename must qualify. The fixture's evaluation time is explicit so reruns make the same decision.

The [historical #221 snapshot](pilot/merged-221.test-only.json) contains sanitized, read-only GitHub metadata for a merged PR. It is expected to reject, even with successful historical CI. No historical observation establishes current enforcement or authority. The evaluator only accepts test-only inputs and has no credential handling, network client or mutation capability. Identity fields in fixtures are assertions for exercising the gate. They provide no authentication and must never feed a live adapter.

| Fixture | Expected decision |
| --- | --- |
| Matching #217 PR, active synthetic grant, correct base/head, green checks, positive independent verdict and verified smoke | Would proceed |
| #219, unrelated issue, wrong repository/base/branch, fork or unbound PR | Reject allowlist |
| API/Web check failing, absent, pending, skipped, approval-blocked or from the wrong producer | Reject CI evidence |
| Current head changes after review, base advances, or check applies to an obsolete merge ref | Reject stale evidence |
| Missing, negative or author-supplied verdict, or missing required GitHub approval | Reject review |
| Unexpected file, rename across the allowance, protected path or incomplete file list | Reject file scope |
| Browser or PostgreSQL evidence skipped, unverified or for another SHA | Reject integration evidence |
| Paused, revoked or expired grant, or unknown branch protection | Reject authority |

Run `node --test docs/agentic-workflow/tools/codex-cli/pilot/sprint-12-gate.test.mjs` for the matrix, including #218 sequencing and byte-identical CLI output from fresh processes. Record outcomes in the implementation PR linked to #223. Real event delivery still requires a separately authorized draft PR.

## Read-only host preflight

Run these commands from the repository root in PowerShell under the intended unattended runner identity. Use the installed `codex` executable on other shells. The status probe returns an exit code without recording account output. A successful status is only evidence of a local login. The owner must independently confirm that the selected programmatic authentication method, entitlement and billing are approved for that identity.

```powershell
node --version
codex.cmd --version
codex.cmd login status *> $null
"Codex login status exit: $LASTEXITCODE"
gh api --method GET repos/bbubb/squadsync/issues/226 --jq '{number,state}'
gh api --method GET repos/bbubb/squadsync/branches/main/protection --jq '{required_status_checks,required_pull_request_reviews,enforce_admins}'
gh api --method GET repos/bbubb/squadsync/rules/branches/main --jq 'map({type,parameters})'
dotnet --version
Get-Command pnpm.cmd -ErrorAction SilentlyContinue | Select-Object Name
docker compose version
docker info --format '{{.ServerVersion}}'
Test-Path 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'
Test-Path 'C:/Program Files/Google/Chrome/Application/chrome.exe'
```

- Read both protection responses and confirm required check job names and GitHub App IDs, strict updates, formal approval counts and qualifying reviewers. Have an authorized owner inspect applicable repository and organization rulesets, bypass lists and allowed merge methods if the read identity lacks visibility. A failed read or absent enforcement blocks activation. Compare historical Actions jobs and review metadata to their expected producers. Verify producer identity and eligibility again for the actual draft.
- Confirm the runner stays available with the editor closed. Verify separate worker directories and identities, no PR access to credential stores or controller files, and a fresh read-only reviewer. Local tool availability and status probes do not prove isolation or uptime.
- Locate the intended browser if it is installed elsewhere. Check the documented disposable PostgreSQL, Development API and browser setup on the selected host. Read-only probes do not establish that the stack can run together. Record browser launch, API responses and PostgreSQL observations after the owner authorizes the disposable smoke. Use the linked web and API instructions above.
- Record actual results, unavailable observations and any deviations in the setup PR. Keep the owner follow-up in #223 and the sprint outcome in #207.

## Next operational step

After owner review of #226, ask the owner to authorize one supervised, non-merging draft PR rehearsal on a named host under a distinct approved publisher. Confirm API and Web CI can both trigger on its permitted patch. Have that publisher open it and push one follow-up commit. Record both ordinary `pull_request` event deliveries, eligible check producers, a fresh read-only review of the final revision, formal approval eligibility, and the disposable browser/API/PostgreSQL smoke. Keep model authentication, publishing credentials and any future merge credential outside the worker and PR checkout. Receive owner approval before model billing, protection changes, credential creation or any new persistent service.

For activation, use owner-controlled code outside the checkout to read grant state from #223 and live GitHub state through read-only access. Collect complete paginated files, effective rules and latest eligible Actions jobs and formal reviews using authenticated GitHub reads. Receive independent review and integration records over owner-controlled channels bound to the exact PR, base and head. Before enabling a future merge adapter, demonstrate credential isolation and server enforcement with no bypass, including rejection when evidence is missing or stale. The separate merge identity would use the protected endpoint with the final verified head and the owner-approved method, as specified above. It must derive authority from trusted live evidence and the active grant. If this boundary requires a persistent GitHub App or broker, propose an ADR under #77 before implementation.

## Owner activation and shutdown

1. Review and merge the #226 prototype PR through supervised execution. Approve the concrete host, reviewer, credential boundary and any billing or repository settings needed for the supervised rehearsal and subsequent operational implementation.
2. Review the resulting dry-run evidence and confirm real CI event delivery, reviewer eligibility, branch protection without bypass, and real browser/API/PostgreSQL validation on the chosen host. Verify #217 readiness. If any prerequisite fails, continue supervised.
3. Record an explicit active grant in #223 with the start and deadline, controller and reviewer identities, merge method, per-issue allowed paths, authorized #217/#218 merge eligibility and #219 owner acceptance. Bind each actual PR to its issue on dispatch. The controller rechecks owner-authored grant state before dispatch and before attempting a merge.
4. To pause, mark the grant paused in #223, stop the controller job and halt queued or active workers. Reconcile any in-flight GitHub request by reading its result. Resume only after the owner records resumption within the unexpired grant and rechecks head, CI and readiness. To revoke or expire, disable the job, remove the gate's credential access, revoke outstanding tokens or suspend the dedicated installation as applicable, and return to supervised execution. The owner handles any already-merged change through normal reviewed follow-up.

Use existing PRs and #207 closeout to capture change, decision, deviation, deferral, evidence and revisit trigger. Include actual pilot results, escalations and any rollback. The owner closes #223 after pilot completion.
