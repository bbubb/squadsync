namespace SquadSync.Application.Rosters;

public sealed class GetRosterEntry(IRosterQueries persistence)
{
    public Task<RosterDetails?> ExecuteAsync(Guid teamId, Guid rosterEntryId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (teamId == Guid.Empty || rosterEntryId == Guid.Empty)
        {
            var field = teamId == Guid.Empty ? "teamId" : "rosterEntryId";
            throw new RosterValidationException(field, new ArgumentException("A non-empty ID is required.", field));
        }
        return persistence.GetByIdAsync(teamId, rosterEntryId, cancellationToken);
    }
}
