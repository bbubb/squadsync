import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { afterEach, expect, test, vi } from "vitest";
import type { paths } from "@/lib/api/schema";
import { teamQueryKey, useTeam } from "./use-team";
import { teamsQueryKey } from "./use-teams";

afterEach(() => vi.unstubAllGlobals());

type Team = paths["/api/teams/{id}"]["get"]["responses"][200]["content"]["application/json"];
const firstTeam = { id: "ad828d12-92c5-4578-985b-e83bb37cc861", name: "First FC" } satisfies Team;
const secondTeam = { id: "bd828d12-92c5-4578-985b-e83bb37cc861", name: "Second FC" } satisfies Team;

function setup() {
  const client = new QueryClient({ defaultOptions: { queries: { retryDelay: 0 } } });
  const wrapper = ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={client}>{children}</QueryClientProvider>
  );
  return { client, wrapper };
}

test("a 404 makes one request with production-like QueryClient defaults", async () => {
  const fetcher = vi
    .fn<typeof fetch>()
    .mockImplementation(() => Promise.resolve(new Response(null, { status: 404 })));
  vi.stubGlobal("fetch", fetcher);
  const client = new QueryClient();
  const wrapper = ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={client}>{children}</QueryClientProvider>
  );
  const view = renderHook(() => useTeam(firstTeam.id), { wrapper });
  await waitFor(() => expect(view.result.current.isError).toBe(true));
  expect(view.result.current.error).toMatchObject({ name: "ApiError", status: 404 });
  expect(fetcher).toHaveBeenCalledTimes(1);
  view.unmount();
  client.clear();
});

test.each([
  { label: "HTTP 500", response: () => Promise.resolve(new Response(null, { status: 500 })) },
  { label: "transport failure", response: () => Promise.reject(new TypeError("Failed to fetch")) },
])("retries a transient $label and recovers", async ({ response }) => {
  const fetcher = vi
    .fn<typeof fetch>()
    .mockImplementationOnce(response)
    .mockImplementation(() => Promise.resolve(Response.json(firstTeam)));
  vi.stubGlobal("fetch", fetcher);
  const { client, wrapper } = setup();
  const view = renderHook(() => useTeam(firstTeam.id), { wrapper });
  await waitFor(() => expect(view.result.current.isSuccess).toBe(true));
  expect(view.result.current.data).toEqual(firstTeam);
  expect(fetcher).toHaveBeenCalledTimes(2);
  view.unmount();
  client.clear();
});

test("loads detail independently of the collection, isolates IDs and reuses each Team cache", async () => {
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    const request = input as Request;
    expect(request.method).toBe("GET");
    const path = new URL(request.url).pathname;
    expect([`/api/teams/${firstTeam.id}`, `/api/teams/${secondTeam.id}`]).toContain(path);
    return Promise.resolve(Response.json(path.endsWith(firstTeam.id) ? firstTeam : secondTeam));
  });
  vi.stubGlobal("fetch", fetcher);
  const { client, wrapper } = setup();
  // A collection cache cannot supply or hide the resource detail.
  client.setQueryData(teamsQueryKey, []);
  const view = renderHook(({ id }) => useTeam(id), { wrapper, initialProps: { id: firstTeam.id } });
  await waitFor(() => expect(view.result.current.data).toEqual(firstTeam));
  view.rerender({ id: secondTeam.id });
  expect(view.result.current.isPending).toBe(true);
  expect(view.result.current.data).toBeUndefined();
  await waitFor(() => expect(view.result.current.data).toEqual(secondTeam));
  expect(client.getQueryData(teamQueryKey(firstTeam.id))).toEqual(firstTeam);
  expect(client.getQueryData(teamQueryKey(secondTeam.id))).toEqual(secondTeam);
  view.rerender({ id: firstTeam.id });
  expect(view.result.current.data).toEqual(firstTeam);
  expect(fetcher).toHaveBeenCalledTimes(2);
  view.unmount();
  client.clear();
});

test.each([
  {
    label: "not found",
    status: 404,
    response: () => Promise.resolve(new Response(null, { status: 404 })),
  },
  {
    label: "server error",
    status: 500,
    response: () => Promise.resolve(new Response(null, { status: 500 })),
  },
  {
    label: "transport failure",
    status: undefined,
    response: () => Promise.reject(new TypeError("Failed to fetch")),
  },
  {
    label: "missing body",
    status: undefined,
    response: () => Promise.resolve(new Response(null, { status: 204 })),
  },
])("propagates $label without caching a Team", async ({ status, response }) => {
  const fetcher = vi.fn<typeof fetch>().mockImplementation(response);
  vi.stubGlobal("fetch", fetcher);
  const { client, wrapper } = setup();
  const view = renderHook(() => useTeam(firstTeam.id), { wrapper });
  await waitFor(() => expect(view.result.current.isError).toBe(true));
  expect(view.result.current.error).toMatchObject({ name: "ApiError" });
  expect((view.result.current.error as { status?: number }).status).toBe(status);
  expect(client.getQueryData(teamQueryKey(firstTeam.id))).toBeUndefined();
  expect(fetcher).toHaveBeenCalledTimes(status === 404 ? 1 : 4);
  view.unmount();
  client.clear();
});
