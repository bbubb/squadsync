import assert from 'node:assert/strict';
import { spawnSync } from 'node:child_process';
import { readFileSync } from 'node:fs';
import test from 'node:test';
import { fileURLToPath } from 'node:url';
import { evaluate } from './sprint-12-gate.mjs';

const fixture = (name) => new URL(name, import.meta.url);
const example = fixture('proposed-217.test-only.json');
const historical = fixture('merged-221.test-only.json');
const sample = JSON.parse(readFileSync(example, 'utf8'));
const gate = fileURLToPath(fixture('sprint-12-gate.mjs'));
const run = (url) => spawnSync(process.execPath, [gate, fileURLToPath(url)], { encoding: 'utf8', env: {} });
const changedSha = 'c'.repeat(40);
const path = sample.github.diff.files[0].path;

test('fresh processes make the same offline decision without credentials', () => {
  const first = run(example);
  const restarted = run(example);
  assert.equal(first.status, 0);
  assert.equal(restarted.status, 0);
  assert.equal(first.stderr + restarted.stderr, '');
  assert.equal(first.stdout, restarted.stdout);
  assert.deepEqual(JSON.parse(first.stdout), {
    mode: 'offline-dry-run', decision: 'would-proceed', reason: 'synthetic-evidence-matched',
    issue: 217, pr: 9000, base: sample.github.pr.baseSha, head: sample.github.pr.headSha, liveAuthority: false,
  });
});

test('merged historical evidence is repeatably ineligible despite successful CI', () => {
  const first = run(historical);
  const restarted = run(historical);
  assert.equal(first.status, 1);
  assert.equal(restarted.status, 1);
  assert.equal(first.stdout, restarted.stdout);
  assert.equal(JSON.parse(first.stdout).reason, 'historical-pr-is-ineligible');
});

test('the CLI fails closed on an unreadable or malformed snapshot', () => {
  for (const url of [fixture('missing.json'), fixture('sprint-12-gate.mjs')]) {
    const runResult = run(url);
    assert.equal(runResult.status, 1);
    assert.equal(JSON.parse(runResult.stdout).reason, 'snapshot-unreadable-or-invalid');
  }
});

test('accepts a rename within both allowed paths and CI on the bound test merge', () => {
  const snapshot = structuredClone(sample);
  const { github } = snapshot;
  github.diff.files = [{ path, previousPath: path.replace('form', 'dialog'), status: 'renamed' }];
  github.pr.testMerge = { ...snapshot.review, sha: changedSha };
  Object.assign(github.checks[0], { ref: 'test-merge', sha: changedSha });
  assert.equal(evaluate(snapshot).decision, 'would-proceed');
  github.pr.testMerge.sha = 'd'.repeat(40);
  assert.equal(evaluate(snapshot).reason, 'required-ci-not-current-and-successful');
});

test('requires the bound #217 merge before #218', () => {
  const snapshot = structuredClone(sample);
  const { grant, github, review, integration } = snapshot;
  for (const evidence of [grant.dispatch, github.pr, github.diff, github.protection, ...github.checks,
    github.approvals, review, integration, ...integration.stages]) evidence.issue = 218;
  github.issue.number = 218;
  github.issue.dependencies.push({ issue: 217, merged: true, pr: 8999 });
  assert.equal(evaluate(snapshot).reason, 'issue-not-ready-or-dependencies-unmet');
  grant.completed217Pr = 8999;
  assert.equal(evaluate(snapshot).decision, 'would-proceed');
  github.issue.dependencies[2].merged = false;
  assert.equal(evaluate(snapshot).reason, 'issue-not-ready-or-dependencies-unmet');
});

const cases = {
  'test-only-snapshot-required': {
    'unknown snapshot': (s) => { s.kind = 'live'; },
    'unmarked synthetic': (s) => { delete s.testOnly; },
  },
  'inactive-or-unverified-grant': {
    'missing grant': (s) => { delete s.grant; },
    'wrong owner': (s) => { s.grant.owner = 'author'; },
    'unknown reader': (s) => { s.github.producer = 'author'; },
    'paused grant': (s) => { s.grant.status = 'paused'; },
    'revoked grant': (s) => { s.grant.status = 'revoked'; },
    'expired grant': (s) => { s.evaluatedAt = s.grant.expiresAt; },
    'future grant': (s) => { s.evaluatedAt = '2026-11-25T11:00:00.000Z'; },
  },
  'pr-outside-dispatch': {
    '#219 closeout': (s) => { s.github.pr.issue = s.grant.dispatch.issue = 219; },
    'unrelated issue': (s) => { s.github.pr.issue = 226; },
    'wrong repo': (s) => { s.github.pr.repository = 'someone/else'; },
    'fork': (s) => { s.github.pr.headRepository = 'someone/squadsync'; },
    'wrong base': (s) => { s.github.pr.baseBranch = 'next'; },
    'wrong branch': (s) => { s.github.pr.headBranch = 'feature/other'; },
    'unbound PR': (s) => { s.github.pr.number++; },
    'draft': (s) => { s.github.pr.draft = true; },
    'closed PR': (s) => { s.github.pr.state = 'closed'; },
    'merge conflict': (s) => { s.github.pr.mergeable = false; },
  },
  'issue-not-ready-or-dependencies-unmet': {
    'planned issue': (s) => { s.github.issue.agentReady = false; },
    'missing dependency': (s) => { s.github.issue.dependencies.pop(); },
    'unmerged dependency': (s) => { s.github.issue.dependencies[1].merged = false; },
    'wrong dependency PR': (s) => { s.github.issue.dependencies[1].pr = 225; },
    'replayed completed issue': (s) => { s.grant.completed217Pr = 8999; },
  },
  'stale-or-unverified-pr-shas': {
    'advanced main': (s) => { s.github.main.sha = changedSha; },
    'branch behind main': (s) => { s.github.pr.upToDate = false; },
  },
  'incomplete-or-disallowed-file-diff': {
    'unexpected file': (s) => { s.github.diff.files[0].path = 'apps/web/src/app/layout.tsx'; },
    'rename crosses allowance': (s) => {
      Object.assign(s.github.diff.files[0], { status: 'renamed', previousPath: 'apps/web/src/app/layout.tsx' });
    },
    'rename without old path': (s) => { s.github.diff.files[0].status = 'renamed'; },
    'truncated diff': (s) => { s.github.diff.complete = false; },
    'inconsistent count': (s) => { s.github.diff.total++; },
    'stale diff': (s) => { s.github.diff.headSha = changedSha; },
    'protected workflow even if listed': (s) => {
      s.github.diff.files[0].path = s.grant.dispatch.allowedPaths[0] = '.github/workflows/api-ci.yml';
    },
    'schema even if listed': (s) => {
      s.github.diff.files[0].path = s.grant.dispatch.allowedPaths[0] = 'apps/web/src/lib/api/schema.d.ts';
    },
    'traversal': (s) => { s.github.diff.files[0].path = path.replace('teams/', 'teams/../'); },
  },
  'branch-protection-unverified': {
    'unknown protection': (s) => { delete s.github.protection; },
    'bypass': (s) => { s.github.protection.noBypass = false; },
    'queue required': (s) => { s.github.protection.queueRequired = true; },
    'merge method unavailable': (s) => { s.github.protection.allowedMergeMethods = ['rebase']; },
    'no formal reviews required': (s) => { s.github.protection.approvalCount = 0; },
  },
  'required-ci-missing-or-ambiguous': {
    'missing API CI': (s) => { s.github.checks.shift(); },
    'missing Web CI': (s) => { s.github.checks.pop(); },
    'wrong producer': (s) => { s.github.checks[0].producer = 'pr-author'; },
    'duplicated eligible run': (s) => { s.github.checks.push(s.github.checks[0]); },
    'additional enforced check missing': (s) => { s.github.protection.requiredChecks[0].job = 'other'; },
  },
  'required-ci-not-current-and-successful': {
    'failed check': (s) => { s.github.checks[0].result = 'failure'; },
    'pending check': (s) => { s.github.checks[0].result = 'pending'; },
    'skipped check': (s) => { s.github.checks[1].result = 'skipped'; },
    'approval blocked': (s) => { s.github.checks[1].result = 'approval-blocked'; },
    'manual dispatch': (s) => { s.github.checks[1].event = 'workflow_dispatch'; },
    'superseded run': (s) => { s.github.checks[0].latestEligible = false; },
    'stale check SHA': (s) => { s.github.checks[0].sha = changedSha; },
  },
  'independent-review-missing-or-invalid': {
    'review missing': (s) => { delete s.review; },
    'review rejected': (s) => { s.review.verdict = 'reject'; },
    'review inconclusive': (s) => { s.review.verdict = 'unknown'; },
    'review by author': (s) => { s.review.producer = s.grant.reviewer = s.github.pr.author.toUpperCase(); },
    'review on different PR': (s) => { s.review.pr++; },
    'head advanced after review': (s) => { s.review.headSha = changedSha; },
    'base advanced after review': (s) => { s.review.baseSha = changedSha; },
    'unresolved findings': (s) => { s.review.unresolvedFindings = true; },
  },
  'formal-approval-missing-or-invalid': {
    'missing approval': (s) => { s.github.approvals.latest = []; },
    'approval not eligible': (s) => { s.github.approvals.latest[0].eligible = false; },
    'stale approval': (s) => { s.github.approvals.latest[0].sha = changedSha; },
    'reviewer not allowed': (s) => { s.github.approvals.latest[0].actor = 'other'; },
    'unresolved change request': (s) => { s.github.approvals.unresolvedChanges = true; },
    'change request contradicts summary': (s) => {
      s.github.approvals.latest.push({ actor: 'other', state: 'changes_requested' });
    },
  },
  'integration-missing-or-invalid': {
    'missing smoke': (s) => { delete s.integration; },
    'skipped browser': (s) => { s.integration.stages[0].result = 'skipped'; },
    'unknown API outcome': (s) => { s.integration.stages[1].result = 'unknown'; },
    'unverified PostgreSQL': (s) => { delete s.integration.stages[2].observation; },
    'PostgreSQL on old SHA': (s) => { s.integration.stages[2].headSha = changedSha; },
    'author claims validation': (s) => { s.integration.producer = s.github.pr.author; },
    'non-disposable data': (s) => { s.integration.disposableData = false; },
  },
};

for (const [reason, scenarios] of Object.entries(cases)) {
  for (const [name, change] of Object.entries(scenarios)) {
    test(name, () => {
      const snapshot = structuredClone(sample);
      change(snapshot);
      const result = evaluate(snapshot);
      assert.equal(result.decision, 'reject');
      assert.equal(result.reason, reason);
      assert.equal(result.liveAuthority, false);
    });
  }
}
