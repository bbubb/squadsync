import { useRef, useState } from "react";
import { ApiError } from "@/lib/api/client";
import { usePersonCandidates, type PersonCandidatePage } from "./use-person-candidates";

type Candidate = PersonCandidatePage["items"][number];

export function PersonCandidatePicker({
  selectedId,
  onSelect,
}: {
  selectedId: string;
  onSelect: (personId: string) => void;
}) {
  const [search, setSearch] = useState("");
  const [cursor, setCursor] = useState<string>();
  const [previous, setPrevious] = useState<Candidate[]>([]);
  const results = useRef<HTMLFieldSetElement>(null);
  const q = search.trim();
  const validQuery = q.length >= 2 && q.length <= 100;
  const query = usePersonCandidates({ q, cursor });
  const candidates = Array.from(
    new Map(
      [...previous, ...(query.data?.items ?? [])].map((person) => [person.personId, person]),
    ).values(),
  );
  const nextCursor = query.data?.nextCursor;
  const rejected = query.error instanceof ApiError && query.error.status === 400;

  function restartSearch() {
    setCursor(undefined);
    setPrevious([]);
    onSelect("");
  }

  return (
    <div className="grid gap-3">
      <label htmlFor="add-player-search" className="font-semibold">
        Search by name
      </label>
      <p id="add-player-search-hint" className="text-sm">
        Enter 2 to 100 characters.
      </p>
      <input
        id="add-player-search"
        type="search"
        className="min-h-12 w-full rounded-md border bg-white px-3"
        aria-describedby="add-player-search-hint"
        aria-invalid={q.length > 100 || undefined}
        value={search}
        onChange={(event) => {
          setSearch(event.target.value);
          restartSearch();
        }}
      />
      {q.length > 100 && <p role="alert">Shorten the search to 100 characters or fewer.</p>}
      <p className="empty:sr-only" role="status">
        {validQuery && query.isFetching
          ? cursor === undefined
            ? "Searching for people…"
            : "Loading more people…"
          : validQuery && query.isSuccess && candidates.length === 0
            ? "No people match this name. Try another name or choose Create New Person."
            : ""}
      </p>
      {candidates.length > 0 && (
        <fieldset
          ref={results}
          tabIndex={-1}
          className="min-w-0"
          aria-describedby="add-player-candidate-hint"
        >
          <legend className="font-semibold">Matching people</legend>
          <p id="add-player-candidate-hint" className="text-sm">
            People can share the same name. Choose a record only if you know which person to use.
          </p>
          {candidates.map((person) => (
            <label
              key={person.personId}
              className="flex min-h-11 cursor-pointer items-center gap-3 border-b border-black/10 py-2 wrap-anywhere"
            >
              <input
                className="shrink-0"
                type="radio"
                name="personCandidate"
                checked={person.personId === selectedId}
                onChange={() => onSelect(person.personId)}
              />
              <span className="min-w-0">
                {person.firstName} {person.lastName}
              </span>
            </label>
          ))}
        </fieldset>
      )}
      {query.isError && !query.isFetching && (
        <div>
          <p role="alert">
            {rejected
              ? "The search request was rejected. Change the name or search again."
              : "People could not be loaded. Check the local Development API and try again."}
          </p>
          <button
            type="button"
            className="team-back"
            onClick={() => {
              if (rejected && cursor !== undefined) {
                restartSearch();
              } else {
                void query.refetch();
              }
            }}
          >
            {rejected && cursor !== undefined ? "Start search over" : "Retry search"}
          </button>
        </div>
      )}
      {nextCursor != null && !query.isError && (
        <button
          type="button"
          className="team-back justify-self-start"
          disabled={query.isFetching}
          onClick={() => {
            results.current?.focus();
            setPrevious(candidates);
            setCursor(nextCursor);
          }}
        >
          Load more
        </button>
      )}
    </div>
  );
}
