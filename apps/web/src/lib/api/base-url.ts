export function getApiBaseUrl(value = process.env.NEXT_PUBLIC_API_BASE_URL) {
  const url = new URL(value ?? "http://127.0.0.1:5050");
  if (
    url.protocol !== "http:" ||
    !["127.0.0.1", "localhost", "[::1]"].includes(url.hostname) ||
    url.username ||
    url.password ||
    url.pathname !== "/" ||
    url.search ||
    url.hash
  ) {
    throw new Error("API base URL must be an HTTP loopback origin without credentials or a path.");
  }
  return url.origin;
}
