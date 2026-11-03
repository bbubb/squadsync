import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { afterEach, expect, test, vi } from "vitest";
import type { paths } from "@/lib/api/schema";
import { teamsQueryKey, useTeams } from "./use-teams";

afterEach(() => vi.unstubAllGlobals());

const teams = [
  { id: "ad828d12-92c5-4578-985b-e83bb37cc861", name: "SquadSync Test FC" },
] satisfies paths["/api/teams"]["get"]["responses"][200]["content"]["application/json"];

test.each([
  { label: "Team collection", body: teams },
  { label: "empty collection", body: [] },
])("caches a successful $label for shared reads", async ({ body }) => {
  const fetcher = vi.fn<typeof fetch>().mockResolvedValue(Response.json(body));
  vi.stubGlobal("fetch", fetcher);
  const client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  const wrapper = ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={client}>{children}</QueryClientProvider>
  );
  const first = renderHook(() => useTeams(), { wrapper });
  await waitFor(() => expect(first.result.current.isSuccess).toBe(true));
  expect(first.result.current.data).toEqual(body);
  expect(client.getQueryData(teamsQueryKey)).toEqual(body);
  const second = renderHook(() => useTeams(), { wrapper });
  expect(second.result.current.data).toEqual(body);
  expect(fetcher).toHaveBeenCalledTimes(1);
  const request = fetcher.mock.calls[0][0] as Request;
  expect(new URL(request.url).pathname).toBe("/api/teams");
  expect(request.method).toBe("GET");
  first.unmount();
  second.unmount();
  client.clear();
});

test.each([
  { label: "HTTP error", response: () => Promise.resolve(new Response(null, { status: 500 })) },
  { label: "transport failure", response: () => Promise.reject(new TypeError("Failed to fetch")) },
  {
    label: "missing response body",
    response: () => Promise.resolve(new Response(null, { status: 204 })),
  },
])("exposes $label without caching an empty Team collection", async ({ response }) => {
  vi.stubGlobal("fetch", vi.fn<typeof fetch>().mockImplementation(response));
  const client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  const wrapper = ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={client}>{children}</QueryClientProvider>
  );
  const view = renderHook(() => useTeams(), { wrapper });
  await waitFor(() => expect(view.result.current.isError).toBe(true));
  expect(view.result.current.error).toMatchObject({ name: "ApiError" });
  expect(view.result.current.data).toBeUndefined();
  expect(client.getQueryData(teamsQueryKey)).toBeUndefined();
  view.unmount();
  client.clear();
});
