"use client";

import Link from "next/link";
import { useEffect, useRef, useState } from "react";
import type { components } from "@/lib/api/schema";
import { AddPlayerForm } from "./add-player-form";
import { RosterEntryDetails } from "./roster-entry-details";
import styles from "./team-roster.module.css";
import { useTeamRoster } from "./use-team-roster";

type RosterEntry = components["schemas"]["RosterEntryResponse"];

export function RosterList({
  teamId,
  entries,
}: {
  teamId: string;
  entries: readonly RosterEntry[];
}) {
  return (
    <ul className={styles.list} aria-label="Roster entries">
      {entries.map((entry) => (
        <li key={entry.id} className={styles.entry}>
          <h3 className="min-w-0 font-semibold wrap-anywhere">
            <Link
              className="inline-block min-h-6 underline underline-offset-4"
              href={`/teams/${teamId}/roster/${entry.id}`}
            >
              {entry.person.firstName} {entry.person.lastName}
            </Link>
          </h3>
          <RosterEntryDetails entry={entry} />
        </li>
      ))}
    </ul>
  );
}

function RosterResults({
  teamId,
  query,
}: {
  teamId: string;
  query: ReturnType<typeof useTeamRoster>;
}) {
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
  return <RosterList teamId={teamId} entries={query.data} />;
}

export function TeamRoster({ teamId }: { teamId: string }) {
  const query = useTeamRoster(teamId);
  const [showForm, setShowForm] = useState(false);
  const [announcement, setAnnouncement] = useState("");
  const opener = useRef<HTMLButtonElement>(null);
  const restoreFocus = useRef(false);

  useEffect(() => {
    if (!showForm && restoreFocus.current) {
      opener.current?.focus();
      restoreFocus.current = false;
    }
  }, [showForm]);

  function closeForm() {
    restoreFocus.current = true;
    setShowForm(false);
  }

  return (
    <>
      {showForm ? (
        <AddPlayerForm
          teamId={teamId}
          onAdded={({ person }) => {
            setAnnouncement(`${person.firstName} ${person.lastName} was added to the roster.`);
            closeForm();
          }}
          onCancel={closeForm}
          onRefreshRoster={async () => (await query.refetch()).isSuccess}
        />
      ) : (
        <button
          ref={opener}
          type="button"
          className="team-action mb-4"
          onClick={() => {
            setAnnouncement("");
            setShowForm(true);
          }}
        >
          Add Player
        </button>
      )}
      <p className="wrap-anywhere empty:sr-only" role="status">
        {announcement}
      </p>
      <RosterResults teamId={teamId} query={query} />
    </>
  );
}
