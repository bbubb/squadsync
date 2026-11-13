using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using SquadSync.Application.Rosters;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class EfAddPlayerPersistence(SquadSyncDbContext db) : IAddPlayerPersistence
{
    public async Task<IAddPlayerTransaction> BeginAsync(CancellationToken cancellationToken = default)
    {
        var existing = db.Database.CurrentTransaction;
        if (existing is null)
            return new Transaction(db, await db.Database.BeginTransactionAsync(cancellationToken), null);
        var savepoint = "add_player_" + Guid.NewGuid().ToString("N");
        await existing.CreateSavepointAsync(savepoint, cancellationToken);
        return new Transaction(db, existing, savepoint);
    }

    private sealed class Transaction(SquadSyncDbContext db, IDbContextTransaction transaction, string? savepoint)
        : IAddPlayerTransaction
    {
        private bool committed;
        private readonly List<object> added = [];

        public Task<bool> TeamExistsAsync(Guid teamId, CancellationToken cancellationToken)
            => db.Teams.AsNoTracking().AnyAsync(team => team.Id == teamId, cancellationToken);

        public Task<Person?> GetPersonAsync(Guid personId, CancellationToken cancellationToken)
            => db.People.AsNoTracking().SingleOrDefaultAsync(person => person.Id == personId, cancellationToken);

        public Task<TeamMembership?> GetMembershipAsync(Guid teamId, Guid personId, CancellationToken cancellationToken)
            => db.TeamMemberships.AsNoTracking().SingleOrDefaultAsync(
                membership => membership.TeamId == teamId && membership.PersonId == personId, cancellationToken);

        public async Task SaveAsync(Person? newPerson, TeamMembership? newMembership, RosterEntry entry,
            CancellationToken cancellationToken)
        {
            try
            {
                if (newPerson is not null)
                {
                    db.People.Add(newPerson);
                    added.Add(newPerson);
                }
                if (newMembership is not null)
                {
                    db.TeamMemberships.Add(newMembership);
                    added.Add(newMembership);
                }
                // Staged saves remain in one transaction. Roster failure must undo these rows too.
                await db.SaveChangesAsync(cancellationToken);
                db.RosterEntries.Add(entry);
                added.Add(entry);
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "PK_People" or "PK_TeamMemberships" or "IX_TeamMemberships_PersonId_TeamId"
                    or "PK_RosterEntries" or "IX_RosterEntries_TeamMembershipId"
            })
            {
                throw new AddPlayerConflictException(exception);
            }
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.ForeignKeyViolation,
                ConstraintName: "FK_TeamMemberships_People_PersonId"
            })
            {
                throw new AddPlayerPersonNotFoundException();
            }
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.ForeignKeyViolation,
                ConstraintName: "FK_TeamMemberships_Teams_TeamId"
            })
            {
                throw new RosterTeamNotFoundException();
            }
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.ForeignKeyViolation,
                ConstraintName: "FK_RosterEntries_TeamMemberships_TeamMembershipId"
            })
            {
                throw new AddPlayerConflictException(exception);
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            if (savepoint is null) await transaction.CommitAsync(cancellationToken);
            else await transaction.ReleaseSavepointAsync(savepoint, cancellationToken);
            committed = true;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                // Cleanup must run even when the request was cancelled.
                if (!committed)
                {
                    if (savepoint is null) await transaction.RollbackAsync(CancellationToken.None);
                    else
                    {
                        await transaction.RollbackToSavepointAsync(savepoint, CancellationToken.None);
                        await transaction.ReleaseSavepointAsync(savepoint, CancellationToken.None);
                    }
                }
            }
            finally
            {
                foreach (var entity in added) db.Entry(entity).State = EntityState.Detached;
                if (savepoint is null) await transaction.DisposeAsync();
            }
        }
    }
}
