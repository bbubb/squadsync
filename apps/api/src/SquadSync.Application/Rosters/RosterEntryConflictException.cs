namespace SquadSync.Application.Rosters;

public sealed class RosterEntryConflictException(Guid rosterEntryId, Guid membershipId, Exception? innerException = null)
    : InvalidOperationException($"Roster entry '{rosterEntryId}' or a roster entry for team membership '{membershipId}' already exists.", innerException)
{
    public Guid RosterEntryId { get; } = rosterEntryId;

    public Guid TeamMembershipId { get; } = membershipId;
}
