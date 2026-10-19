namespace SquadSync.Application.Rosters;

public interface IRosterQueries
{
    Task<IReadOnlyList<RosterDetails>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);

    Task<RosterDetails?> GetByIdAsync(Guid teamId, Guid rosterEntryId, CancellationToken cancellationToken = default);
}
