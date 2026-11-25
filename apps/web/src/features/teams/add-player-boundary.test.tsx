import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { act, renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { afterEach, expect, test, vi } from "vitest";
import { useAddPlayer, type AddedPlayer, type AddPlayerInput } from "./use-add-player";
import { personCandidatesQueryKey, usePersonCandidates } from "./use-person-candidates";
import { teamMembershipsQueryKey, useTeamMemberships } from "./use-team-memberships";
import { teamRosterQueryKey, useTeamRoster } from "./use-team-roster";

afterEach(() => vi.unstubAllGlobals());
const teamA = "ad828d12-92c5-4578-985b-e83bb37cc861";
const teamB = "b07d93ad-5644-420b-888c-540ed9d972b8";
const person = {
  id: "2cfeea39-8f80-4266-bdd4-8ac5b6ba0abd",
  firstName: "Alex",
  lastName: "Player",
};
const created = {
  id: "roster-id",
  teamMembershipId: "membership-id",
  person,
  jerseyNumber: "007",
  rosterStatus: "Active",
} satisfies AddedPlayer;
const existing = {
  personId: person.id,
  jerseyNumber: "007",
  rosterStatus: "Active",
} satisfies AddPlayerInput;

function setup() {
  // Deliberately hostile defaults prove that the mutation disables automatic retries.
  const client = new QueryClient({
    defaultOptions: { queries: { retry: false }, mutations: { retry: 3, retryDelay: 0 } },
  });
  const wrapper = ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={client}>{children}</QueryClientProvider>
  );
  return { client, wrapper };
}

test("candidate lookup sends bounded server query and opaque cursor, isolates pages and reuses cache", async () => {
  const cursor = "opaque+/= token";
  const page = {
    items: [{ personId: person.id, firstName: person.firstName, lastName: person.lastName }],
    nextCursor: cursor,
  };
  const fetcher = vi
    .fn<typeof fetch>()
    .mockResolvedValueOnce(Response.json(page))
    .mockResolvedValueOnce(Response.json({ items: [], nextCursor: null }));
  vi.stubGlobal("fetch", fetcher);
  const { client, wrapper } = setup();
  const view = renderHook(
    ({ cursor }: { cursor?: string }) => usePersonCandidates({ q: " Alex ", limit: 5, cursor }),
    { initialProps: { cursor: undefined as string | undefined }, wrapper },
  );
  await waitFor(() => expect(view.result.current.data).toEqual(page));
  view.rerender({ cursor });
  expect(view.result.current.data).toBeUndefined();
  await waitFor(() => expect(view.result.current.data).toEqual({ items: [], nextCursor: null }));
  const requests = fetcher.mock.calls.map(([request]) => new URL((request as Request).url));
  expect(requests[0].pathname).toBe("/api/people/candidates");
  expect(Object.fromEntries(requests[0].searchParams)).toEqual({ q: "Alex", limit: "5" });
  expect(Object.fromEntries(requests[1].searchParams)).toEqual({ q: "Alex", limit: "5", cursor });
  view.rerender({ cursor: undefined });
  expect(view.result.current.data).toEqual(page);
  expect(fetcher).toHaveBeenCalledTimes(2);
  view.unmount();
  client.clear();
});

test.each(["", "  a  ", "x".repeat(101)])("candidate lookup disables invalid q: %s", async (q) => {
  const fetcher = vi.fn<typeof fetch>();
  vi.stubGlobal("fetch", fetcher);
  const { client, wrapper } = setup();
  const view = renderHook(() => usePersonCandidates({ q }), { wrapper });
  expect(view.result.current.fetchStatus).toBe("idle");
  expect(fetcher).not.toHaveBeenCalled();
  view.unmount();
  client.clear();
});

test("candidate default limit, cancellation, and HTTP errors remain query states", async () => {
  const fetcher = vi
    .fn<typeof fetch>()
    .mockResolvedValueOnce(new Response(null, { status: 400 }))
    .mockImplementationOnce(() => new Promise<Response>(() => {}));
  vi.stubGlobal("fetch", fetcher);
  const { client, wrapper } = setup();
  const view = renderHook(({ q }) => usePersonCandidates({ q }), {
    initialProps: { q: "Alex" },
    wrapper,
  });
  await waitFor(() => expect(view.result.current.isError).toBe(true));
  expect(view.result.current.error).toMatchObject({ status: 400 });
  expect(view.result.current.data).toBeUndefined();
  view.rerender({ q: "Sam" });
  await waitFor(() => expect(fetcher).toHaveBeenCalledTimes(2));
  const request = fetcher.mock.calls[1][0] as Request;
  expect(new URL(request.url).searchParams.get("limit")).toBe("20");
  view.unmount();
  expect(request.signal.aborted).toBe(true);
  client.clear();
});

test.each([
  existing,
  {
    newPerson: { firstName: "Alex", lastName: "Player" },
    rosterStatus: "Reserved",
    jerseyNumber: null,
  } satisfies AddPlayerInput,
])(
  "Add Player makes one typed atomic POST and invalidates only selected Team collections",
  async (input) => {
    const fetcher = vi
      .fn<typeof fetch>()
      .mockResolvedValue(Response.json(created, { status: 201 }));
    vi.stubGlobal("fetch", fetcher);
    const { client, wrapper } = setup();
    const keys = [
      teamRosterQueryKey(teamA),
      teamMembershipsQueryKey(teamA),
      teamRosterQueryKey(teamB),
      teamMembershipsQueryKey(teamB),
      ["teams"],
      ["teams", "detail", teamA],
      personCandidatesQueryKey("Alex", 20),
    ];
    keys.forEach((key) => client.setQueryData(key, []));
    const view = renderHook(() => useAddPlayer(teamA), { wrapper });
    await act(async () => expect(await view.result.current.mutateAsync(input)).toEqual(created));
    expect(fetcher).toHaveBeenCalledTimes(1);
    const request = fetcher.mock.calls[0][0] as Request;
    expect(request.method).toBe("POST");
    expect(request.url).toBe(`http://127.0.0.1:5050/api/teams/${teamA}/roster/players`);
    expect(request.credentials).toBe("omit");
    expect(request.redirect).toBe("error");
    expect(await request.json()).toEqual(input);
    keys.forEach((key, index) => expect(client.getQueryState(key)?.isInvalidated).toBe(index < 2));
    view.unmount();
    client.clear();
  },
);

test("success refetches mounted roster/member totals even after selected Team changes", async () => {
  let finish!: (response: Response) => void;
  const fetcher = vi.fn<typeof fetch>().mockImplementation(async (input) => {
    const request = input as Request;
    if (request.method === "POST")
      return new Promise<Response>((resolve) => {
        finish = resolve;
      });
    return Response.json(
      request.url.endsWith("/roster")
        ? [created]
        : [{ id: "membership-id", person, teamId: teamA, teamRole: "Player" }],
    );
  });
  vi.stubGlobal("fetch", fetcher);
  const { client, wrapper } = setup();
  client.setQueryData(teamRosterQueryKey(teamA), []);
  client.setQueryData(teamMembershipsQueryKey(teamA), []);
  const reads = renderHook(
    () => ({ roster: useTeamRoster(teamA), members: useTeamMemberships(teamA) }),
    { wrapper },
  );
  expect(reads.result.current.roster.data).toHaveLength(0);
  fetcher.mockClear();
  const view = renderHook(({ teamId }) => useAddPlayer(teamId), {
    initialProps: { teamId: teamA },
    wrapper,
  });
  let mutation!: Promise<AddedPlayer>;
  act(() => {
    mutation = view.result.current.mutateAsync(existing);
  });
  await waitFor(() => expect(fetcher).toHaveBeenCalledTimes(1));
  view.rerender({ teamId: teamB });
  await act(async () => {
    finish(Response.json(created, { status: 201 }));
    await mutation;
  });
  expect(
    fetcher.mock.calls.filter(([request]) => (request as Request).method === "GET"),
  ).toHaveLength(2);
  expect(fetcher.mock.calls.every(([request]) => (request as Request).url.includes(teamA))).toBe(
    true,
  );
  await waitFor(() => expect(reads.result.current.members.data).toHaveLength(1));
  expect(reads.result.current.roster.data).toHaveLength(1);
  view.unmount();
  reads.unmount();
  client.clear();
});

test.each([
  { status: 400, kind: "validation" },
  { status: 404, kind: "not-found" },
  { status: 409, kind: "conflict" },
  { status: 500, kind: "unknown-outcome", reason: "response" },
  { status: 204, kind: "unknown-outcome", reason: "response" },
  { status: 201, kind: "unknown-outcome", reason: "response" },
  { status: undefined, kind: "unknown-outcome", reason: "transport" },
])(
  "preserves $status / $kind failure with no retry or success invalidation",
  async ({ status, kind, reason }) => {
    const problem = { title: "Request rejected", errors: { jerseyNumber: ["Use digits."] } };
    const fetcher = vi.fn<typeof fetch>().mockImplementation(async () => {
      if (status === undefined) throw new TypeError("Failed to fetch");
      if (status === 204) return new Response(null, { status });
      if (status === 201) return new Response("invalid JSON", { status });
      return Response.json(problem, { status });
    });
    vi.stubGlobal("fetch", fetcher);
    const { client, wrapper } = setup();
    client.setQueryData(teamRosterQueryKey(teamA), []);
    const view = renderHook(() => useAddPlayer(teamA), { wrapper });
    await act(async () => {
      await expect(view.result.current.mutateAsync(existing)).rejects.toMatchObject({
        name: "AddPlayerError",
        failure: { kind, ...(reason ? { reason } : {}) },
      });
    });
    await waitFor(() => expect(view.result.current.isError).toBe(true));
    if (status && status >= 400) {
      expect(view.result.current.error?.failure.status).toBe(status);
      expect(view.result.current.error?.problem).toEqual(problem);
    }
    expect(fetcher).toHaveBeenCalledTimes(1);
    expect(client.getQueryState(teamRosterQueryKey(teamA))?.isInvalidated).toBe(false);
    view.unmount();
    client.clear();
  },
);
