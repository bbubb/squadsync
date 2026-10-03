using SquadSync.Domain;

namespace SquadSync.Application.Rosters;

public interface IRosterPersistence
{
    Task<TeamMembership?> GetMembershipAsync(Guid membershipId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists the entry atomically. Throws RosterEntryConflictException when its ID or membership
    /// already has a roster entry. Other persistence failures propagate unchanged.
    /// </summary>
    Task AddEntryAsync(RosterEntry entry, CancellationToken cancellationToken = default);
}
