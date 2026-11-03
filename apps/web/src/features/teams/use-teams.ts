"use client";

import { useQuery } from "@tanstack/react-query";
import { apiClient, ApiError } from "@/lib/api/client";
import type { paths } from "@/lib/api/schema";

type TeamCollection = paths["/api/teams"]["get"]["responses"][200]["content"]["application/json"];

export const teamsQueryKey = ["teams"] as const;

export function useTeams() {
  return useQuery({
    queryKey: teamsQueryKey,
    staleTime: 30_000,
    queryFn: async ({ signal }): Promise<TeamCollection> => {
      const { data } = await apiClient.GET("/api/teams", { signal });
      if (data === undefined) {
        throw new ApiError("GET /api/teams returned no Team collection.");
      }
      return data;
    },
  });
}
