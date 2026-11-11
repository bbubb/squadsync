"use client";

import type { UseQueryResult } from "@tanstack/react-query";
import styles from "./team-overview.module.css";
import { useTeamMemberships } from "./use-team-memberships";
import { useTeamRoster } from "./use-team-roster";

function Summary({
  title,
  empty,
  description,
  query,
}: {
  title: string;
  empty: string;
  description: string;
  query: UseQueryResult<readonly unknown[], Error>;
}) {
  return (
    <section className={styles.summary} aria-label={title}>
      <h3>{title}</h3>
      {query.isPending ? (
        <p role="status">Loading {title.toLowerCase()}…</p>
      ) : query.isError ? (
        <>
          <p role="alert">
            {title} could not be loaded. Check that the local Development API is running.
          </p>
          <button
            className="team-action"
            onClick={() => void query.refetch()}
            disabled={query.isFetching}
          >
            {query.isFetching ? "Retrying…" : `Retry ${title.toLowerCase()}`}
          </button>
        </>
      ) : (
        <>
          <output className={styles.count} aria-label={`${title}: ${query.data.length}`}>
            {query.data.length}
          </output>
          <p>{query.data.length === 0 ? empty : description}</p>
        </>
      )}
    </section>
  );
}

export function TeamOverview({ teamId }: { teamId: string }) {
  const memberships = useTeamMemberships(teamId);
  const roster = useTeamRoster(teamId);
  return (
    <div className={styles.grid}>
      <Summary
        title="Roster entries"
        empty="No roster entries yet."
        description="Total roster entries across all statuses."
        query={roster}
      />
      <Summary
        title="Memberships"
        empty="No memberships yet."
        description="Total Team memberships across all roles."
        query={memberships}
      />
    </div>
  );
}
