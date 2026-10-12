namespace SquadSync.Application.Memberships;

public sealed class GetMembership(IMembershipPersistence persistence)
{
    public Task<MembershipDetails?> ExecuteAsync(
        Guid teamId, Guid membershipId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (teamId == Guid.Empty || membershipId == Guid.Empty)
        {
            var field = teamId == Guid.Empty ? "teamId" : "membershipId";
            throw new MembershipValidationException(field, new ArgumentException("A non-empty ID is required.", field));
        }
        return persistence.GetByIdAsync(teamId, membershipId, cancellationToken);
    }
}
