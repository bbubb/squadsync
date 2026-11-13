using SquadSync.Domain;

namespace SquadSync.Application.Rosters;

public interface IAddPlayerPersistence
{
    // Dispose without commit rolls back the operation, including newly persisted Person/membership rows.
    Task<IAddPlayerTransaction> BeginAsync(CancellationToken cancellationToken = default);
}

// A single Add Player operation, not a general-purpose unit of work.
public interface IAddPlayerTransaction : IAsyncDisposable
{
    Task<bool> TeamExistsAsync(Guid teamId, CancellationToken cancellationToken);
    Task<Person?> GetPersonAsync(Guid personId, CancellationToken cancellationToken);
    Task<TeamMembership?> GetMembershipAsync(Guid teamId, Guid personId, CancellationToken cancellationToken);
    // Known uniqueness races throw AddPlayerConflictException; absent Person/Team references
    // throw their specific not-found exceptions. Unrelated failures and cancellation propagate.
    Task SaveAsync(Person? newPerson, TeamMembership? newMembership, RosterEntry entry, CancellationToken cancellationToken);
    Task CommitAsync(CancellationToken cancellationToken);
}
