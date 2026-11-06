"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiClient, ApiError } from "@/lib/api/client";
import type { paths } from "@/lib/api/schema";
import { teamsQueryKey } from "./use-teams";

type CreateTeamRequest = NonNullable<
  paths["/api/teams"]["post"]["requestBody"]
>["content"]["application/json"];
type CreatedTeam = paths["/api/teams"]["post"]["responses"][201]["content"]["application/json"];

export function useCreateTeam() {
  const queryClient = useQueryClient();
  return useMutation({
    retry: false,
    mutationFn: async (body: CreateTeamRequest): Promise<CreatedTeam> => {
      const { data } = await apiClient.POST("/api/teams", { body });
      if (data === undefined) {
        throw new ApiError("POST /api/teams returned no Team.");
      }
      return data;
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: teamsQueryKey, exact: true });
    },
  });
}
