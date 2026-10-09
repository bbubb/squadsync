using SquadSync.Domain;

namespace SquadSync.Application.Teams;

public sealed class GetTeam(ITeamPersistence persistence)
{
    public Task<Team?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        => persistence.GetByIdAsync(id, cancellationToken);
}
