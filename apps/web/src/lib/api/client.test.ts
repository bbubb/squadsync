import { afterEach, expect, test, vi } from "vitest";
import { ApiError, createApiClient } from "./client";
import { getApiBaseUrl } from "./base-url";
import type { paths } from "./schema";

afterEach(() => vi.unstubAllEnvs());

test("returns the generated Team collection and uses the configured loopback origin", async () => {
  vi.stubEnv("NEXT_PUBLIC_API_BASE_URL", "http://localhost:5051");
  const teams = [
    { id: "ad828d12-92c5-4578-985b-e83bb37cc861", name: "SquadSync Test FC" },
  ] satisfies paths["/api/teams"]["get"]["responses"][200]["content"]["application/json"];
  const fetcher = vi.fn<typeof fetch>().mockResolvedValue(Response.json(teams));
  const result = await createApiClient(fetcher).GET("/api/teams");
  expect(result.data).toEqual(teams);
  const request = fetcher.mock.calls[0][0] as Request;
  expect(request.url).toBe("http://localhost:5051/api/teams");
  expect(request.credentials).toBe("omit");
  expect(request.redirect).toBe("error");
  expect(request.headers.get("Accept")).toBe("application/json");
});

test("preserves a successful empty collection", async () => {
  const fetcher = vi.fn<typeof fetch>().mockResolvedValue(Response.json([]));
  expect((await createApiClient(fetcher).GET("/api/teams")).data).toEqual([]);
});

test.each([400, 404, 500])(
  "rejects HTTP %s rather than returning an empty collection",
  async (status) => {
    const fetcher = vi
      .fn<typeof fetch>()
      .mockResolvedValue(new Response("unavailable", { status }));
    await expect(createApiClient(fetcher).GET("/api/teams")).rejects.toMatchObject({
      name: "ApiError",
      status,
      message: `GET /api/teams failed: HTTP ${status}.`,
    });
  },
);

test("reports transport failure and retains its cause", async () => {
  const cause = new TypeError("Failed to fetch");
  const fetcher = vi.fn<typeof fetch>().mockRejectedValue(cause);
  const request = createApiClient(fetcher).GET("/api/teams");
  await expect(request).rejects.toBeInstanceOf(ApiError);
  await expect(request).rejects.toMatchObject({
    message: "GET /api/teams could not reach the local API.",
    cause,
  });
});

test.each([
  "https://example.com",
  "http://192.168.1.2:5050",
  "http://user:password@localhost:5050",
  "http://localhost:5050/proxy",
  "http://localhost:5050?token=value",
  "http://localhost:5050#fragment",
])("rejects an origin outside the approved boundary: %s", (value) => {
  expect(() => getApiBaseUrl(value)).toThrow();
});

test("defaults to the documented API origin", () => {
  vi.stubEnv("NEXT_PUBLIC_API_BASE_URL", undefined);
  expect(getApiBaseUrl()).toBe("http://127.0.0.1:5050");
});
