using SquadSync.Domain;

namespace SquadSync.Application.Rosters;

public sealed class RosterMembershipNotPlayerException(Guid membershipId, TeamRole teamRole)
    : InvalidOperationException($"Team membership '{membershipId}' has role '{teamRole}'; only Player memberships may join the roster.")
{
    public Guid TeamMembershipId { get; } = membershipId;

    public TeamRole TeamRole { get; } = teamRole;
}
