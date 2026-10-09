using SquadSync.Domain;

namespace SquadSync.Application.Teams;

public interface ITeamPersistence
{
    Task AddAsync(Team team, CancellationToken cancellationToken = default);

    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
