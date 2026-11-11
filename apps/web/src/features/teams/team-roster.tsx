"use client";

import type { components } from "@/lib/api/schema";
import styles from "./team-roster.module.css";
import { useTeamRoster } from "./use-team-roster";

type RosterEntry = components["schemas"]["RosterEntryResponse"];

export function RosterList({ entries }: { entries: readonly RosterEntry[] }) {
  return (
    <ul className={styles.list} aria-label="Roster entries">
      {entries.map((entry) => (
        <li key={entry.id} className={styles.entry}>
          <h3 className="min-w-0 font-semibold wrap-anywhere">
            {entry.person.firstName} {entry.person.lastName}
          </h3>
          <dl className={styles.details}>
            <div>
              <dt>Jersey number</dt>
              <dd>{entry.jerseyNumber ?? "Not assigned"}</dd>
            </div>
            <div>
              <dt>Roster status</dt>
              <dd>{entry.rosterStatus}</dd>
            </div>
          </dl>
        </li>
      ))}
    </ul>
  );
}

export function TeamRoster({ teamId }: { teamId: string }) {
  const query = useTeamRoster(teamId);
  if (query.isPending) return <p role="status">Loading roster…</p>;
  if (query.isError) {
    return (
      <div>
        <p role="alert">
          Roster could not be loaded. Check that the local Development API is running, then retry.
        </p>
        <button
          className="team-action"
          onClick={() => void query.refetch()}
          disabled={query.isFetching}
        >
          {query.isFetching ? "Retrying…" : "Retry roster"}
        </button>
      </div>
    );
  }
  if (query.data.length === 0) {
    return <p role="status">No roster entries yet for this Team.</p>;
  }
  return <RosterList entries={query.data} />;
}
