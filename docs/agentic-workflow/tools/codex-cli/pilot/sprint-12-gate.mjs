import { readFileSync } from 'node:fs';
import { pathToFileURL } from 'node:url';

const repository = 'bbubb/squadsync';
const issues = [217, 218];
const prerequisitePulls = [[215, 220], [216, 221]];
const workflows = ['.github/workflows/api-ci.yml', '.github/workflows/web-ci.yml'];
const stages = ['browser', 'api', 'postgresql'];
const sha = (value) => typeof value === 'string' && /^[0-9a-f]{40}$/.test(value);
const text = (value) => typeof value === 'string' && value.trim() !== '';
const list = (values, predicate) => Array.isArray(values) && values.length > 0 && values.every(predicate);
const instant = (value) => typeof value === 'string' &&
  /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}Z$/.test(value) &&
  Number.isFinite(Date.parse(value)) && new Date(value).toISOString() === value;

function webPath(path) {
  return typeof path === 'string' &&
    /^apps\/web\/(?:README\.md|src\/[a-zA-Z0-9_@.[\]/-]+)$/.test(path) &&
    path.split('/').every((part) => part && !part.startsWith('.') && !part.endsWith('.')) &&
    !/^apps\/web\/src\/lib\/api\/schema\.d\.ts$/i.test(path) &&
    !/(?:^|\/)(?:AGENTS\.md|package\.json|[^/]*lock\.[^/]+)$/i.test(path);
}

function pathsFit(files, allowedPaths) {
  if (!list(allowedPaths, (path) => webPath(path) ||
    (typeof path === 'string' && path.endsWith('/') && webPath(path + 'file')))) return false;
  const unique = new Set();
  return files.every((file) => {
    if (!['added', 'modified', 'removed', 'renamed'].includes(file?.status)) return false;
    if (file.status !== 'renamed' && file.previousPath !== undefined) return false;
    const paths = file.status === 'renamed' ? [file.path, file.previousPath] : [file.path];
    return paths.every((path) => {
      if (!webPath(path) || unique.has(path.toLowerCase())) return false;
      unique.add(path.toLowerCase());
      return allowedPaths.some((allowed) => allowed.endsWith('/') ? path.startsWith(allowed) : path === allowed);
    });
  });
}

export function evaluate(snapshot) {
  const { kind, testOnly, evaluatedAt, grant, github, review, integration } = snapshot ?? {};
  const { pr, main, issue, protection, checks, approvals, diff } = github ?? {};
  const subject = pr ?? snapshot?.historicalPr;
  const result = (reason, decision = 'reject') => ({
    mode: 'offline-dry-run',
    decision,
    reason,
    issue: Number.isSafeInteger(subject?.issue) ? subject.issue : null,
    pr: Number.isSafeInteger(subject?.number) ? subject.number : null,
    base: sha(subject?.baseSha) ? subject.baseSha : null,
    head: sha(subject?.headSha) ? subject.headSha : null,
    liveAuthority: false,
  });

  if (kind === 'historical' && testOnly === true) return result('historical-pr-is-ineligible');
  if (kind !== 'synthetic' || testOnly !== true) return result('test-only-snapshot-required');
  if (grant?.repository !== repository || grant?.tracker !== 223 || grant?.sprint !== 207 ||
      grant?.owner !== 'bbubb' || !text(grant.githubReader) || github?.producer !== grant.githubReader ||
      grant.status !== 'active' || !['merge', 'squash', 'rebase'].includes(grant.mergeMethod) ||
      ![evaluatedAt, grant.startsAt, grant.expiresAt].every(instant) ||
      evaluatedAt < grant.startsAt || evaluatedAt >= grant.expiresAt) return result('inactive-or-unverified-grant');

  const dispatch = grant.dispatch;
  if (!issues.includes(pr?.issue) || pr.issue !== dispatch?.issue ||
      !Number.isSafeInteger(pr.number) || pr.number <= 0 || pr.number !== dispatch.pr ||
      pr.repository !== repository || pr.headRepository !== repository || pr.baseBranch !== 'main' ||
      !text(dispatch.branch) || dispatch.branch === 'main' || pr.headBranch !== dispatch.branch ||
      pr.state !== 'open' || pr.draft !== false || pr.mergeable !== true ||
      !text(dispatch.publisher) || pr.author !== dispatch.publisher) return result('pr-outside-dispatch');

  const dependencies = pr.issue === 217 ? prerequisitePulls : [...prerequisitePulls, [217, grant.completed217Pr]];
  if (issue?.number !== pr.issue || issue.repository !== repository ||
      issue.state !== 'open' || issue.agentReady !== true ||
      !Array.isArray(issue.dependencies) || issue.dependencies.length !== dependencies.length ||
      (pr.issue === 217 && grant.completed217Pr !== undefined) ||
      !dependencies.every(([number, pull]) => Number.isSafeInteger(pull) && pull > 0 && pull !== pr.number &&
        issue.dependencies.some((dependency) =>
          dependency?.issue === number && dependency.pr === pull && dependency.merged === true))) {
    return result('issue-not-ready-or-dependencies-unmet');
  }

  const bound = (evidence) => evidence?.repository === repository && evidence.issue === pr.issue &&
    evidence.pr === pr.number && evidence.baseSha === pr.baseSha && evidence.headSha === pr.headSha;
  if (!sha(pr.baseSha) || !sha(pr.headSha) || pr.baseSha === pr.headSha ||
      main?.repository !== repository || main.branch !== 'main' ||
      main.sha !== pr.baseSha || pr.upToDate !== true ||
      (pr.testMerge !== undefined && (!bound(pr.testMerge) || !sha(pr.testMerge.sha) ||
        [pr.baseSha, pr.headSha].includes(pr.testMerge.sha)))) {
    return result('stale-or-unverified-pr-shas');
  }
  if (!bound(diff) || diff.complete !== true || !list(diff.files, (file) => file && typeof file === 'object') ||
      diff.total !== diff.files.length || !pathsFit(diff.files, dispatch.allowedPaths)) {
    return result('incomplete-or-disallowed-file-diff');
  }

  const checkKey = (check) => JSON.stringify([check.workflow, check.job, check.producer]);
  const checkIdentity = (check) => text(check?.workflow) && text(check.job) && text(check.producer);
  if (!bound(protection) || protection.verified !== true || protection.branch !== 'main' ||
      protection.noBypass !== true || protection.strict !== true ||
      !Array.isArray(protection.allowedMergeMethods) || !protection.allowedMergeMethods.includes(grant.mergeMethod) ||
      protection.queueRequired !== false || !Number.isSafeInteger(protection.approvalCount) ||
      protection.approvalCount < 1 || !list(protection.requiredChecks, checkIdentity)) {
    return result('branch-protection-unverified');
  }
  if (!list(grant.expectedChecks, checkIdentity) ||
      !workflows.every((workflow) => grant.expectedChecks.some((check) => check.workflow === workflow)) ||
      !list(checks, checkIdentity)) return result('required-ci-missing-or-invalid');
  for (const expected of [...grant.expectedChecks, ...protection.requiredChecks]) {
    const matching = checks.filter((check) => checkKey(check) === checkKey(expected));
    if (matching.length !== 1) return result('required-ci-missing-or-ambiguous');
    const [check] = matching;
    const expectedSha = check.ref === 'head' ? pr.headSha :
      check.ref === 'test-merge' ? pr.testMerge?.sha : undefined;
    if (!bound(check) || !expectedSha || check.sha !== expectedSha ||
        check.event !== 'pull_request' || check.latestEligible !== true || check.result !== 'success') {
      return result('required-ci-not-current-and-successful');
    }
  }

  if (!bound(review) || !text(grant.reviewer) || review.producer !== grant.reviewer ||
      review.producer.toLowerCase() === pr.author.toLowerCase() || review.verdict !== 'proceed' ||
      review.criteriaMet !== true || review.unresolvedFindings !== false ||
      !Array.isArray(review.residualRisks) || !review.residualRisks.every(text)) {
    return result('independent-review-missing-or-invalid');
  }
  if (!bound(approvals) || approvals.complete !== true || approvals.unresolvedChanges !== false ||
      !list(grant.approvers, text) || !Array.isArray(approvals.latest) ||
      !approvals.latest.every((entry) => text(entry?.actor) &&
        ['approved', 'dismissed', 'commented'].includes(entry.state)) ||
      new Set(approvals.latest.map(({ actor }) => actor.toLowerCase())).size !== approvals.latest.length ||
      approvals.latest.filter(({ actor, state, eligible, sha }) =>
        state === 'approved' && eligible === true && sha === pr.headSha &&
        actor.toLowerCase() !== pr.author.toLowerCase() && grant.approvers.includes(actor)
      ).length < protection.approvalCount) return result('formal-approval-missing-or-invalid');

  if (!bound(integration) || !text(grant.integrationObserver) ||
      integration.producer !== grant.integrationObserver ||
      integration.producer.toLowerCase() === pr.author.toLowerCase() ||
      integration.disposableData !== true || !Array.isArray(integration.stages) ||
      integration.stages.length !== stages.length ||
      !stages.every((stage) => integration.stages.some((entry) =>
        bound(entry) && entry.stage === stage && entry.result === 'success' &&
        text(entry.command) && text(entry.observation)))) return result('integration-missing-or-invalid');

  return result('synthetic-evidence-matched', 'would-proceed');
}

if (process.argv[1] && pathToFileURL(process.argv[1]).href === import.meta.url) {
  let result;
  try {
    if (process.argv.length !== 3) throw new Error('Expected one snapshot path');
    result = evaluate(JSON.parse(readFileSync(process.argv[2], 'utf8')));
  } catch {
    result = { mode: 'offline-dry-run', decision: 'reject', reason: 'snapshot-unreadable-or-invalid', liveAuthority: false };
  }
  process.stdout.write(JSON.stringify(result) + '\n');
  process.exitCode = result.decision === 'would-proceed' ? 0 : 1;
}
