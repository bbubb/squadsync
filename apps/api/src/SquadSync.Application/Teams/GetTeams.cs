namespace SquadSync.Application.Teams;

public sealed class GetTeams(ITeamQueries queries)
{
    public Task<IReadOnlyList<TeamDetails>> ExecuteAsync(CancellationToken cancellationToken = default)
        => queries.GetAllAsync(cancellationToken);
}
