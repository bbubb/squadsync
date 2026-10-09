using Microsoft.EntityFrameworkCore;
using SquadSync.Application.Teams;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class EfTeamPersistence(SquadSyncDbContext context) : ITeamPersistence
{
    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        context.Teams.Add(team);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Teams.AsNoTracking().SingleOrDefaultAsync(team => team.Id == id, cancellationToken);
}
