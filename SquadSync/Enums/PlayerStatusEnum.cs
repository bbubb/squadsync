namespace SquadSync.Enums
{
    public enum PlayerStatusEnum
    {
        Active,
        Inactive,
        Reserved,  // For players who are not currently active but are still on the roster (e.g. injured, personal leave)
        Retired,
        Suspended
    }
}
