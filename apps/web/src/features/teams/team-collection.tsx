"use client";

import Link from "next/link";
import { useTeams } from "./use-teams";

export function TeamCollection() {
  const query = useTeams();

  if (query.isPending) return <p role="status">Loading Teams…</p>;
  if (query.isError) {
    return (
      <div>
        <p role="alert">
          Teams could not be loaded. Check that the local Development API is running.
        </p>
        <button
          className="team-action"
          onClick={() => void query.refetch()}
          disabled={query.isFetching}
        >
          {query.isFetching ? "Retrying…" : "Try again"}
        </button>
      </div>
    );
  }

  if (query.data.length === 0) {
    return <p role="status">No Teams yet. Choose Create Team to start your workspace.</p>;
  }

  return (
    <ul className="team-grid">
      {query.data.map((team) => (
        <li key={team.id}>
          <Link className="team-card" href={`/teams/${team.id}`}>
            <span className="team-name">{team.name}</span>
            <span className="team-identity">{team.id}</span>
            <span className="team-enter">
              Open Team <span aria-hidden="true">↗</span>
            </span>
          </Link>
        </li>
      ))}
    </ul>
  );
}
