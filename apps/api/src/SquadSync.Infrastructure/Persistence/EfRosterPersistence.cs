using Microsoft.EntityFrameworkCore;
using Npgsql;
using SquadSync.Application.Rosters;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class EfRosterPersistence(SquadSyncDbContext dbContext) : IRosterPersistence
{
    public Task<TeamMembership?> GetMembershipAsync(Guid membershipId, CancellationToken cancellationToken = default)
        => dbContext.TeamMemberships.AsNoTracking()
            .SingleOrDefaultAsync(membership => membership.Id == membershipId, cancellationToken);

    public async Task AddEntryAsync(RosterEntry entry, CancellationToken cancellationToken = default)
    {
        dbContext.RosterEntries.Add(entry);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            // Keep later attempts on this scoped adapter subject to database constraints,
            // including another request with the same entry ID.
            dbContext.Entry(entry).State = EntityState.Detached;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "IX_RosterEntries_TeamMembershipId" or "PK_RosterEntries"
            })
        {
            dbContext.Entry(entry).State = EntityState.Detached;
            throw new RosterEntryConflictException(entry.Id, entry.TeamMembershipId, exception);
        }
    }
}
