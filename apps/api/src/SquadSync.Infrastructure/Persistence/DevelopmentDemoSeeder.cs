using Microsoft.EntityFrameworkCore;
using SquadSync.Application.Rosters;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

// Explicit bootstrap operation; never registered as a hosted service or invoked by ordinary startup.
public sealed class DevelopmentDemoSeeder(SquadSyncDbContext dbContext, IRosterPersistence rosterPersistence)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Tests may supply an outer transaction and own its rollback. The command owns its transaction.
        await using var transaction = dbContext.Database.CurrentTransaction is null
            ? await dbContext.Database.BeginTransactionAsync(cancellationToken)
            : null;

        var team = new Team(Id(1), "SquadSync Demo FC");
        await AddIfMissingAsync(team, team.Id, cancellationToken);
        var users = new[]
        {
            new User(Id(11), "Casey Coach", "Demo"),
            new User(Id(12), "Morgan Manager", "Demo"),
            new User(Id(13), "Alex Player", "Demo"),
            new User(Id(14), "Sam Player", "Demo")
        };
        var roles = new[] { TeamRole.Coach, TeamRole.Manager, TeamRole.Player, TeamRole.Player };
        for (var index = 0; index < users.Length; index++)
        {
            await AddIfMissingAsync(users[index], users[index].Id, cancellationToken);
            var membership = new TeamMembership(Id(21 + index), users[index].Id, team.Id, roles[index]);
            await AddIfMissingAsync(membership, membership.Id, cancellationToken);
        }

        var profiles = new[]
        {
            new PlayerProfile(Id(31), users[2].Id, DominantFoot.Right, 70, 165m),
            new PlayerProfile(Id(32), users[3].Id, DominantFoot.Left, 68, 150m)
        };
        foreach (var profile in profiles)
        {
            await AddIfMissingAsync(profile, profile.Id, cancellationToken);
        }

        // Principals must exist in the database before Application loads the memberships.
        await dbContext.SaveChangesAsync(cancellationToken);
        var addPlayer = new AddPlayerToRoster(rosterPersistence);
        var entries = new[]
        {
            new RosterEntry(Id(41), Id(23), "007", RosterStatus.Active),
            new RosterEntry(Id(42), Id(24), "12", RosterStatus.Reserved)
        };
        foreach (var entry in entries)
        {
            var existing = await dbContext.RosterEntries.FindAsync([entry.Id], cancellationToken);
            if (existing is null)
            {
                await addPlayer.ExecuteAsync(entry.Id, entry.TeamMembershipId,
                    entry.JerseyNumber, entry.RosterStatus, cancellationToken);
            }
            else
            {
                RequireMatchingData(existing, entry);
            }
        }

        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }
    }

    private async Task AddIfMissingAsync<TEntity>(TEntity expected, Guid id, CancellationToken cancellationToken)
        where TEntity : class
    {
        var existing = await dbContext.Set<TEntity>().FindAsync([id], cancellationToken);
        if (existing is null)
        {
            dbContext.Set<TEntity>().Add(expected);
        }
        else
        {
            RequireMatchingData(existing, expected);
        }
    }

    private void RequireMatchingData<TEntity>(TEntity existing, TEntity expected) where TEntity : class
    {
        var actualValues = dbContext.Entry(existing).CurrentValues;
        var expectedValues = dbContext.Entry(expected).CurrentValues;
        if (actualValues.Properties.Any(property => !Equals(actualValues[property], expectedValues[property])))
        {
            throw new InvalidOperationException(
                $"Demo seed identifier conflicts with existing {typeof(TEntity).Name} data. No existing rows will be changed.");
        }
    }

    private static Guid Id(int value) => Guid.Parse($"11100000-0000-0000-0000-{value:000000000000}");
}
