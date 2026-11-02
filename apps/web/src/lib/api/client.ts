import createClient from "openapi-fetch";
import { getApiBaseUrl } from "./base-url";
import type { paths } from "./schema";

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status?: number,
    options?: ErrorOptions,
  ) {
    super(message, options);
    this.name = "ApiError";
  }
}

export function createApiClient(fetcher: typeof fetch = (...args) => fetch(...args)) {
  const client = createClient<paths>({
    baseUrl: getApiBaseUrl(),
    fetch: fetcher,
    credentials: "omit",
    redirect: "error",
    headers: { Accept: "application/json" },
  });
  client.use({
    onResponse({ request, response }) {
      if (!response.ok) {
        throw new ApiError(
          `${request.method} ${new URL(request.url).pathname} failed: HTTP ${response.status}.`,
          response.status,
        );
      }
    },
    onError({ request, error }) {
      return new ApiError(
        `${request.method} ${new URL(request.url).pathname} could not reach the local API.`,
        undefined,
        { cause: error },
      );
    },
  });
  return client;
}

export const apiClient = createApiClient();
