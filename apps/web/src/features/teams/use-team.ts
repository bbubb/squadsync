"use client";

import { useQuery } from "@tanstack/react-query";
import { apiClient, ApiError } from "@/lib/api/client";
import type { paths } from "@/lib/api/schema";

type Team = paths["/api/teams/{id}"]["get"]["responses"][200]["content"]["application/json"];

export const teamQueryKey = (teamId: string) => ["teams", "detail", teamId] as const;

export function useTeam(teamId: string) {
  return useQuery({
    queryKey: teamQueryKey(teamId),
    staleTime: 30_000,
    queryFn: async ({ signal }): Promise<Team> => {
      const { data } = await apiClient.GET("/api/teams/{id}", {
        params: { path: { id: teamId } },
        signal,
      });
      if (data === undefined) {
        throw new ApiError(`GET /api/teams/${teamId} returned no Team.`);
      }
      return data;
    },
  });
}
