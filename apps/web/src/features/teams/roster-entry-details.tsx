import type { components } from "@/lib/api/schema";
import styles from "./team-roster.module.css";

export function RosterEntryDetails({
  entry,
}: {
  entry: components["schemas"]["RosterEntryResponse"];
}) {
  return (
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
  );
}
