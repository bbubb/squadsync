export function isTeamId(value: string) {
  return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value);
}

export function isRosterEntryId(value: string) {
  return isTeamId(value) && value !== "00000000-0000-0000-0000-000000000000";
}
