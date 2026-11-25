"use client";

import { useQuery } from "@tanstack/react-query";
import { apiClient, ApiError } from "@/lib/api/client";
import type { paths } from "@/lib/api/schema";

type CandidateQuery = NonNullable<paths["/api/people/candidates"]["get"]["parameters"]["query"]>;
export type PersonCandidatePage =
  paths["/api/people/candidates"]["get"]["responses"][200]["content"]["application/json"];

export const personCandidatesQueryKey = (q: string, limit: number, cursor?: string) =>
  ["people", "candidates", q, limit, cursor ?? null] as const;

export function usePersonCandidates({ q, limit = 20, cursor }: CandidateQuery) {
  const query = q.trim();
  return useQuery({
    queryKey: personCandidatesQueryKey(query, limit, cursor),
    enabled:
      query.length >= 2 &&
      query.length <= 100 &&
      Number.isInteger(limit) &&
      limit >= 1 &&
      limit <= 20,
    staleTime: 30_000,
    queryFn: async ({ signal }): Promise<PersonCandidatePage> => {
      const { data } = await apiClient.GET("/api/people/candidates", {
        params: { query: { q: query, limit, cursor } },
        signal,
      });
      if (data === undefined) {
        throw new ApiError("GET /api/people/candidates returned no candidate page.");
      }
      return data;
    },
  });
}
