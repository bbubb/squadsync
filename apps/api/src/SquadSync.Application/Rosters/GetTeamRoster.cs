using SquadSync.Application.Teams;

namespace SquadSync.Application.Rosters;

public sealed class GetTeamRoster(ITeamPersistence teams, IRosterQueries roster)
{
    public async Task<IReadOnlyList<RosterDetails>> ExecuteAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (teamId == Guid.Empty)
        {
            throw new RosterValidationException("teamId", new ArgumentException("A non-empty ID is required.", "teamId"));
        }
        if (await teams.GetByIdAsync(teamId, cancellationToken) is null)
        {
            throw new RosterTeamNotFoundException();
        }
        return await roster.GetByTeamIdAsync(teamId, cancellationToken);
    }
}
