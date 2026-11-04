import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { act, renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { afterEach, describe, expect, test, vi } from "vitest";
import type { paths } from "@/lib/api/schema";
import { teamMembershipsQueryKey, useTeamMemberships } from "./use-team-memberships";
import { teamRosterQueryKey, useTeamRoster } from "./use-team-roster";

afterEach(() => vi.unstubAllGlobals());

const teamA = "ad828d12-92c5-4578-985b-e83bb37cc861";
const teamB = "b07d93ad-5644-420b-888c-540ed9d972b8";
const person = {
  id: "2cfeea39-8f80-4266-bdd4-8ac5b6ba0abd",
  firstName: "Alex",
  lastName: "Rivera",
};
const membershipId = "5d2f687c-78fd-43e9-a0e2-ab7d73ed83ad";
const memberships = [
  { id: membershipId, teamId: teamA, person, teamRole: "Player" },
] satisfies paths["/api/teams/{teamId}/memberships"]["get"]["responses"][200]["content"]["application/json"];
const roster = [
  {
    id: "63f1dd2c-5d81-4242-b6c4-94e95368d6ea",
    teamMembershipId: membershipId,
    person,
    jerseyNumber: "07",
    rosterStatus: "Active",
  },
  {
    id: "6512c06f-a6c4-42b8-ae87-07e29aa157fd",
    teamMembershipId: "a4d5495d-a76e-40ba-8ddf-d2c96be74e53",
    person: { ...person, id: "65b50e3b-a267-411c-a749-f1aa10361c0e", firstName: "Sam" },
    jerseyNumber: null,
    rosterStatus: "Inactive",
  },
] satisfies paths["/api/teams/{teamId}/roster"]["get"]["responses"][200]["content"]["application/json"];

function setup() {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  const wrapper = ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={client}>{children}</QueryClientProvider>
  );
  return { client, wrapper };
}

describe.each([
  {
    resource: "memberships",
    useCollection: useTeamMemberships,
    queryKey: teamMembershipsQueryKey,
    body: memberships,
  },
  { resource: "roster", useCollection: useTeamRoster, queryKey: teamRosterQueryKey, body: roster },
])("Team $resource query", ({ resource, useCollection, queryKey, body }) => {
  test.each([false, true])(
    "shares successful data, including empty collections (empty: %s)",
    async (empty) => {
      const responseBody = empty ? [] : body;
      const fetcher = vi.fn<typeof fetch>().mockResolvedValue(Response.json(responseBody));
      vi.stubGlobal("fetch", fetcher);
      const { client, wrapper } = setup();
      const first = renderHook(() => useCollection(teamA), { wrapper });
      await waitFor(() => expect(first.result.current.isSuccess).toBe(true));
      expect(first.result.current.data).toEqual(responseBody);
      expect(client.getQueryData(queryKey(teamA))).toEqual(responseBody);
      const second = renderHook(() => useCollection(teamA), { wrapper });
      expect(second.result.current.data).toEqual(responseBody);
      expect(fetcher).toHaveBeenCalledTimes(1);
      const request = fetcher.mock.calls[0][0] as Request;
      expect(new URL(request.url).pathname).toBe(`/api/teams/${teamA}/${resource}`);
      expect(request.method).toBe("GET");
      first.unmount();
      second.unmount();
      client.clear();
    },
  );

  test.each([
    {
      label: "HTTP failure",
      response: () => Promise.resolve(new Response(null, { status: 500 })),
      message: "HTTP 500",
      status: 500,
    },
    {
      label: "network failure",
      response: () => Promise.reject(new TypeError("Failed to fetch")),
      message: "could not reach the local API",
      status: undefined,
    },
    {
      label: "missing response body",
      response: () => Promise.resolve(new Response(null, { status: 204 })),
      message: "returned no",
      status: undefined,
    },
  ])(
    "exposes $label without substituting an empty collection",
    async ({ response, message, status }) => {
      vi.stubGlobal("fetch", vi.fn<typeof fetch>().mockImplementation(response));
      const { client, wrapper } = setup();
      const view = renderHook(() => useCollection(teamA), { wrapper });
      await waitFor(() => expect(view.result.current.isError).toBe(true));
      expect(view.result.current.error).toMatchObject({ name: "ApiError", status });
      expect(view.result.current.error?.message).toContain(message);
      expect(view.result.current.error?.message).toContain(`/api/teams/${teamA}/${resource}`);
      expect(view.result.current.data).toBeUndefined();
      expect(client.getQueryData(queryKey(teamA))).toBeUndefined();
      view.unmount();
      client.clear();
    },
  );

  test("isolates Team caches while switching IDs and reuses fresh data on return", async () => {
    let resolveTeamB!: (response: Response) => void;
    const fetcher = vi
      .fn<typeof fetch>()
      .mockResolvedValueOnce(Response.json(body))
      .mockImplementationOnce(
        () =>
          new Promise<Response>((resolve) => {
            resolveTeamB = resolve;
          }),
      );
    vi.stubGlobal("fetch", fetcher);
    const { client, wrapper } = setup();
    const view = renderHook(({ teamId }) => useCollection(teamId), {
      initialProps: { teamId: teamA },
      wrapper,
    });
    await waitFor(() => expect(view.result.current.isSuccess).toBe(true));
    view.rerender({ teamId: teamB });
    expect(view.result.current.isPending).toBe(true);
    expect(view.result.current.data).toBeUndefined();
    await waitFor(() => expect(fetcher).toHaveBeenCalledTimes(2));
    expect(new URL((fetcher.mock.calls[1][0] as Request).url).pathname).toBe(
      `/api/teams/${teamB}/${resource}`,
    );
    await act(async () => resolveTeamB(Response.json([])));
    await waitFor(() => expect(view.result.current.isSuccess).toBe(true));
    expect(view.result.current.data).toEqual([]);
    expect(client.getQueryData(queryKey(teamA))).toEqual(body);
    expect(client.getQueryData(queryKey(teamB))).toEqual([]);
    view.rerender({ teamId: teamA });
    expect(view.result.current.data).toEqual(body);
    expect(fetcher).toHaveBeenCalledTimes(2);
    view.unmount();
    client.clear();
  });

  test("aborts an in-flight request when its last observer unmounts", async () => {
    const fetcher = vi.fn<typeof fetch>().mockImplementation(() => new Promise<Response>(() => {}));
    vi.stubGlobal("fetch", fetcher);
    const { client, wrapper } = setup();
    const view = renderHook(() => useCollection(teamA), { wrapper });
    await waitFor(() => expect(fetcher).toHaveBeenCalledTimes(1));
    const request = fetcher.mock.calls[0][0] as Request;
    expect(request.signal.aborted).toBe(false);
    view.unmount();
    expect(request.signal.aborted).toBe(true);
    expect(client.getQueryData(queryKey(teamA))).toBeUndefined();
    client.clear();
  });
});
