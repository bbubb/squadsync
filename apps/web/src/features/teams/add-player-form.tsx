import { useEffect, useRef, useState, type FormEvent } from "react";
import { PersonCandidatePicker } from "./person-candidate-picker";
import { useAddPlayer, type AddedPlayer, type AddPlayerError } from "./use-add-player";

const rosterStatuses = ["Active", "Reserved", "Suspended"];
const inputStyle = "min-h-12 w-full min-w-0 rounded-md border bg-white px-3";

function failureMessage(error: AddPlayerError) {
  switch (error.failure.kind) {
    case "validation":
      return "The API rejected the player details. Check the person, jersey number and roster status.";
    case "not-found":
      return "The Team or selected person could not be found. Search again or return to All Teams to check the Team.";
    case "conflict":
      return "This person may already be on the roster or have a different Team role. Check the roster and Members before trying again.";
    case "unknown-outcome":
      return "The API did not confirm whether the player was added. Refresh and check the roster before submitting again.";
  }
}

export function AddPlayerForm({
  teamId,
  onAdded,
  onCancel,
  onRefreshRoster,
}: {
  teamId: string;
  onAdded: (player: AddedPlayer) => void;
  onCancel: () => void;
  onRefreshRoster: () => Promise<boolean>;
}) {
  const mutation = useAddPlayer(teamId);
  const [mode, setMode] = useState<"new" | "existing">("new");
  const [personId, setPersonId] = useState("");
  const [selectionError, setSelectionError] = useState(false);
  const [rosterCheck, setRosterCheck] = useState<"pending" | "success" | "failed">();
  const heading = useRef<HTMLHeadingElement>(null);
  const form = useRef<HTMLFormElement>(null);
  const submitting = useRef(false);
  const unknownOutcome = mutation.error?.failure.kind === "unknown-outcome";
  const needsRosterCheck = unknownOutcome && rosterCheck !== "success";
  const busy = mutation.isPending || mutation.isSuccess;

  useEffect(() => {
    heading.current?.focus();
  }, []);

  function selectPerson(id: string) {
    setPersonId(id);
    setSelectionError(false);
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    // React may not render the pending state between consecutive submit events.
    if (submitting.current || needsRosterCheck) return;
    if (mode === "existing" && !personId) {
      setSelectionError(true);
      const candidate = form.current?.querySelector<HTMLInputElement>(
        'input[name="personCandidate"]',
      );
      (candidate ?? form.current?.querySelector<HTMLInputElement>('input[type="search"]'))?.focus();
      return;
    }

    const fields = new FormData(event.currentTarget);
    const roster = {
      jerseyNumber: String(fields.get("jerseyNumber")).trim() || null,
      rosterStatus: String(fields.get("rosterStatus")),
    };
    submitting.current = true;
    setRosterCheck(undefined);
    try {
      const player = await mutation.mutateAsync(
        mode === "new"
          ? {
              ...roster,
              newPerson: {
                firstName: String(fields.get("firstName")).trim(),
                lastName: String(fields.get("lastName")).trim(),
              },
            }
          : { ...roster, personId },
      );
      onAdded(player);
    } catch {
      submitting.current = false;
    }
  }

  async function refreshRoster() {
    setRosterCheck("pending");
    setRosterCheck((await onRefreshRoster()) ? "success" : "failed");
  }

  return (
    <form
      id="add-player-form"
      ref={form}
      aria-labelledby="add-player-heading"
      className="my-5 max-w-xl rounded-lg border border-black/15 p-4 sm:p-6"
      onSubmit={(event) => void submit(event)}
    >
      <h3 id="add-player-heading" ref={heading} tabIndex={-1} className="text-xl font-semibold">
        Add Player
      </h3>
      <fieldset disabled={busy} className="mt-4 grid min-w-0 gap-4">
        <fieldset className="min-w-0">
          <legend className="font-semibold">Person</legend>
          <div className="flex flex-wrap gap-x-5">
            <label className="flex min-h-11 cursor-pointer items-center gap-2">
              <input
                type="radio"
                name="personMode"
                checked={mode === "new"}
                onChange={() => {
                  setMode("new");
                  selectPerson("");
                }}
              />
              Create New Person
            </label>
            <label className="flex min-h-11 cursor-pointer items-center gap-2">
              <input
                type="radio"
                name="personMode"
                checked={mode === "existing"}
                onChange={() => setMode("existing")}
              />
              Select Existing Person
            </label>
          </div>
        </fieldset>
        {mode === "new" ? (
          <div className="grid gap-4 sm:grid-cols-2">
            <label className="grid gap-1 font-semibold">
              First name
              <input name="firstName" className={inputStyle} required pattern={".*\\S.*"} />
            </label>
            <label className="grid gap-1 font-semibold">
              Last name
              <input name="lastName" className={inputStyle} required pattern={".*\\S.*"} />
            </label>
          </div>
        ) : (
          <PersonCandidatePicker selectedId={personId} onSelect={selectPerson} />
        )}
        {selectionError && <p role="alert">Select a person from the search results.</p>}
        <div className="grid gap-4 sm:grid-cols-2">
          <div className="grid content-start gap-1">
            <label htmlFor="add-player-jersey" className="font-semibold">
              Jersey number (optional)
            </label>
            <input
              id="add-player-jersey"
              name="jerseyNumber"
              className={inputStyle}
              inputMode="numeric"
              pattern="[0-9]{1,3}"
              aria-describedby="add-player-jersey-hint"
            />
            <p id="add-player-jersey-hint" className="text-sm">
              Enter 1 to 3 digits.
            </p>
          </div>
          <label className="grid content-start gap-1 font-semibold">
            Roster status
            <select name="rosterStatus" className={inputStyle} defaultValue="Active">
              {rosterStatuses.map((status) => (
                <option key={status}>{status}</option>
              ))}
            </select>
          </label>
        </div>
      </fieldset>
      {mutation.isError && (
        <p className="mt-4 text-red-800" role="alert">
          {failureMessage(mutation.error)}
        </p>
      )}
      {unknownOutcome && (
        <div>
          <button
            type="button"
            className="team-back"
            disabled={rosterCheck === "pending"}
            onClick={() => void refreshRoster()}
          >
            Refresh roster
          </button>
          <p className="empty:sr-only" role="status">
            {rosterCheck === "pending" && "Refreshing roster…"}
            {rosterCheck === "success" &&
              "Roster refreshed below. Check whether the player was added before submitting again."}
            {rosterCheck === "failed" && "The roster could not be refreshed. Try refreshing again."}
          </p>
        </div>
      )}
      <p className="mt-4 empty:sr-only" role="status">
        {busy ? "Adding player…" : ""}
      </p>
      <div className="team-form-actions mt-4">
        <button type="submit" className="team-action" disabled={busy || needsRosterCheck}>
          Add to roster
        </button>
        <button type="button" className="team-back" disabled={busy} onClick={onCancel}>
          Cancel
        </button>
      </div>
    </form>
  );
}
