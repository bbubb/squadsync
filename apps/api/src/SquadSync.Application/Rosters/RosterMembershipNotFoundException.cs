namespace SquadSync.Application.Rosters;

public sealed class RosterMembershipNotFoundException(Guid membershipId, Exception? innerException = null)
    : InvalidOperationException($"Team membership '{membershipId}' was not found.", innerException)
{
    public Guid TeamMembershipId { get; } = membershipId;
}
