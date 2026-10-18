using SquadSync.Application.Teams;
using SquadSync.Domain;

namespace SquadSync.Application.Memberships;

public sealed class GetTeamMemberships(ITeamPersistence team, IMembershipPersistence memberships)
{
    public async Task<IReadOnlyList<MembershipDetails>> ExecuteAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (teamId == Guid.Empty)
        {
            throw new MembershipValidationException("teamId", new ArgumentException("A non-empty ID is required.", "teamId"));
        }
        if (await team.GetByIdAsync(teamId, cancellationToken) is null)
        {
            throw new MembershipReferenceNotFoundException();
        }
        return await memberships.GetByTeamIdAsync(teamId, cancellationToken);
    }
}
