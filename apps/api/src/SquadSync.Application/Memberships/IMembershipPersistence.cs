using SquadSync.Domain;

namespace SquadSync.Application.Memberships;

public interface IMembershipPersistence
{
    Task AddAsync(TeamMembership membership, CancellationToken cancellationToken = default);

    Task<MembershipDetails?> GetByIdAsync(Guid teamId, Guid membershipId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MembershipDetails>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);
}
