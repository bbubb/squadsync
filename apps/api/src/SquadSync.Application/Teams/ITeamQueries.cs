namespace SquadSync.Application.Teams;

public interface ITeamQueries
{
    // Returns only Team IDs and names, ordered by name then ID.
    Task<IReadOnlyList<TeamDetails>> GetAllAsync(CancellationToken cancellationToken = default);
}
