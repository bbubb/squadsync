using Microsoft.EntityFrameworkCore;
using Npgsql;
using SquadSync.Application.PlayerProfiles;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class EfPlayerProfilePersistence(SquadSyncDbContext dbContext) : IPlayerProfilePersistence
{
    public Task<PlayerProfileDetails?> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
        => dbContext.PlayerProfiles.AsNoTracking()
            .Where(profile => profile.PersonId == personId)
            .Select(profile => new PlayerProfileDetails(profile.Id, profile.PersonId,
                profile.DominantFoot, profile.HeightInches, profile.WeightPounds))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task AddAsync(PlayerProfile profile, CancellationToken cancellationToken = default)
    {
        dbContext.PlayerProfiles.Add(profile);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            dbContext.Entry(profile).State = EntityState.Detached;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "IX_PlayerProfiles_PersonId"
        })
        {
            dbContext.Entry(profile).State = EntityState.Detached;
            throw new PlayerProfileConflictException(exception);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.ForeignKeyViolation, ConstraintName: "FK_PlayerProfiles_People_PersonId"
        })
        {
            dbContext.Entry(profile).State = EntityState.Detached;
            throw new PlayerProfilePersonNotFoundException(exception);
        }
    }

    public async Task UpdateAsync(PlayerProfile profile,CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new PlayerProfileNotFoundException(exception);
        }
    }

    public Task<PlayerProfile?> GetForUpdateAsync(Guid personId, CancellationToken cancellationToken = default)
        => dbContext.PlayerProfiles
        .SingleOrDefaultAsync(
            profile => profile.PersonId == personId,
             cancellationToken);
}
