namespace SquadSync.Application.Rosters;

public interface IRosterQueries
{
    Task<RosterDetails?> GetByIdAsync(Guid teamId, Guid rosterEntryId, CancellationToken cancellationToken = default);
}
