"use client";

import { useRef, useState, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import { ApiError } from "@/lib/api/client";
import { useCreateTeam } from "./use-create-team";

export function CreateTeamForm() {
  const router = useRouter();
  const mutation = useCreateTeam();
  const [name, setName] = useState("");
  const [nameError, setNameError] = useState(false);
  const input = useRef<HTMLInputElement>(null);
  // Lock synchronously: multiple submit events can arrive before React renders pending state.
  const submitting = useRef(false);
  const busy = mutation.isPending || mutation.isSuccess;

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (submitting.current) return;
    const trimmedName = name.trim();
    if (!trimmedName) {
      setNameError(true);
      input.current?.focus();
      return;
    }
    setNameError(false);
    submitting.current = true;
    try {
      const team = await mutation.mutateAsync({ name: trimmedName });
      router.push(`/teams/${team.id}`);
    } catch {
      // The mutation exposes the error below and retains the entered name for correction.
      submitting.current = false;
    }
  }

  return (
    <form className="team-form" onSubmit={(event) => void submit(event)} noValidate>
      <label htmlFor="team-name">Team name</label>
      <p id="team-name-hint">Enter the name you want to see in the workspace.</p>
      <input
        ref={input}
        id="team-name"
        name="name"
        type="text"
        required
        value={name}
        disabled={busy}
        aria-invalid={nameError || undefined}
        aria-describedby={nameError ? "team-name-hint team-name-error" : "team-name-hint"}
        onChange={(event) => {
          setName(event.target.value);
          setNameError(false);
          mutation.reset();
        }}
      />
      {nameError && (
        <p id="team-name-error" role="alert">
          Enter a Team name. Spaces alone cannot be used.
        </p>
      )}
      {mutation.isError && (
        <p role="alert">
          {mutation.error instanceof ApiError && mutation.error.status === 400
            ? "The API did not accept this Team name. Check the name and try again."
            : "Team could not be created. Check that the local Development API is running and try again."}
        </p>
      )}
      <p role="status">
        {mutation.isPending ? "Creating Team…" : mutation.isSuccess ? "Opening Team…" : ""}
      </p>
      <div className="team-form-actions">
        <button className="team-action" type="submit" disabled={busy}>
          {busy ? "Creating Team…" : "Create Team"}
        </button>
        <button
          className="team-back"
          type="button"
          disabled={busy}
          onClick={() => router.push("/")}
        >
          Cancel
        </button>
      </div>
    </form>
  );
}
