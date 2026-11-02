using Microsoft.EntityFrameworkCore;
using SquadSync.Application.Teams;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class EfTeamPersistence(SquadSyncDbContext context) : ITeamPersistence, ITeamQueries
{
    public async Task<IReadOnlyList<TeamDetails>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Teams.AsNoTracking()
            .OrderBy(team => team.Name).ThenBy(team => team.Id)
            .Select(team => new TeamDetails(team.Id, team.Name))
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        context.Teams.Add(team);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Teams.AsNoTracking().SingleOrDefaultAsync(team => team.Id == id, cancellationToken);
}
