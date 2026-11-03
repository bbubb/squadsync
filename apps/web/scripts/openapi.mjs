import { readFile, writeFile, mkdir } from "node:fs/promises";
import openapiTS, { astToString } from "openapi-typescript";
import { format } from "prettier";
import { getApiBaseUrl } from "../src/lib/api/base-url.ts";

const snapshot = new URL("../openapi/squadsync.json", import.meta.url);
const output = new URL("../src/lib/api/schema.d.ts", import.meta.url);
const command = process.argv[2];

if (command === "refresh") {
  const response = await fetch(`${getApiBaseUrl()}/swagger/v1/swagger.json`, {
    redirect: "error",
    signal: AbortSignal.timeout(10000),
  });
  if (!response.ok) throw new Error(`Swagger refresh failed: HTTP ${response.status}`);
  const schema = await response.json();
  const collection = schema.paths?.["/api/teams"]?.get?.responses?.["200"];
  const body = collection?.content?.["application/json"]?.schema;
  const team = schema.components?.schemas?.TeamResponse;
  if (
    body?.type !== "array" ||
    body.items?.$ref !== "#/components/schemas/TeamResponse" ||
    team?.properties?.id?.format !== "uuid" ||
    team?.properties?.name?.type !== "string"
  ) {
    throw new Error("Swagger Team collection differs from issue #166. Stop for owner review.");
  }
  await mkdir(new URL("../openapi/", import.meta.url), { recursive: true });
  await writeFile(snapshot, await format(JSON.stringify(schema), { parser: "json" }));
  console.log(
    "Refreshed live Development Swagger. Review the snapshot diff before generating types.",
  );
} else if (command === "generate" || command === "check") {
  const schema = JSON.parse(await readFile(snapshot, "utf8"));
  // Prevent generation from fetching external references, including in CI.
  function verifyLocalRefs(value) {
    if (!value || typeof value !== "object") return;
    if (typeof value.$ref === "string" && !value.$ref.startsWith("#/")) {
      throw new Error("Only in-document OpenAPI references are allowed.");
    }
    Object.values(value).forEach(verifyLocalRefs);
  }
  verifyLocalRefs(schema);
  const source = await format(
    "// Generated from openapi/squadsync.json; run pnpm api:generate. Do not edit.\n" +
      astToString(await openapiTS(schema)),
    { parser: "typescript" },
  );
  if (command === "generate") {
    await writeFile(output, source);
  } else if ((await readFile(output, "utf8")).replaceAll("\r\n", "\n") !== source) {
    throw new Error("Generated API types are stale. Run pnpm api:generate and review the diff.");
  } else {
    console.log("Generated API types match the committed snapshot.");
  }
} else {
  throw new Error("Expected refresh, generate, or check.");
}
