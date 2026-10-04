namespace SquadSync.Application.Rosters;

public sealed class RosterMembershipNotFoundException(Guid membershipId)
    : InvalidOperationException($"Team membership '{membershipId}' was not found.")
{
    public Guid TeamMembershipId { get; } = membershipId;
}
