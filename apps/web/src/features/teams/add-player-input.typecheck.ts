import type { paths } from "@/lib/api/schema";
import type { AddPlayerInput } from "./use-add-player";

// Compiled by pnpm typecheck. Unused @ts-expect-error directives fail if the boundary widens.
function checkInput(input: AddPlayerInput) {
  const request: NonNullable<
    paths["/api/teams/{teamId}/roster/players"]["post"]["requestBody"]
  >["content"]["application/json"] = input;
  return request;
}

export function verifyAddPlayerTypes() {
  checkInput({ personId: "person-id", rosterStatus: "Active", jerseyNumber: "007" });
  checkInput({ newPerson: { firstName: "Alex", lastName: "Player" }, rosterStatus: "Active" });
  // @ts-expect-error One Person variant is required.
  checkInput({ rosterStatus: "Active" });
  // @ts-expect-error Both Person variants are forbidden.
  checkInput({
    personId: "person-id",
    newPerson: { firstName: "Alex", lastName: "Player" },
    rosterStatus: "Active",
  });
  // @ts-expect-error Null is not an existing Person variant.
  checkInput({ personId: null, rosterStatus: "Active" });
  // @ts-expect-error New Person names remain generated and required.
  checkInput({ newPerson: { firstName: "Alex" }, rosterStatus: "Active" });
  // @ts-expect-error Roster status remains required.
  checkInput({ personId: "person-id" });
}
