using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class DevelopmentDemoSeedTests
{
    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    public void SeedCommand_RejectsNonDevelopmentBeforeAccessingDatabase(string environment)
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseEnvironment(environment).ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["seed-demo"] = "true",
                    ["ConnectionStrings:SquadSync"] = string.Empty
                })));

        var exception = Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
        Assert.Contains("Development", exception.Message);
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task Seed_CreatesCoherentScenario_IsIdempotent_AndPreservesUnrelatedRows()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connectionString),
            "Database tests require ConnectionStrings__SquadSync when SQUADSYNC_RUN_DATABASE_TESTS=1.");
        await using var provider = new ServiceCollection().AddSquadSyncPersistence(connectionString)
            .BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<SquadSyncDbContext>();
        var seed = scope.ServiceProvider.GetRequiredService<DevelopmentDemoSeeder>();
        var unrelatedTeam = new Team(Guid.NewGuid(), "Unrelated seed validation team");
        var before = await RowCountsAsync(db);
        var demoExists = await db.Teams.AnyAsync(team => team.Name == "SquadSync Demo FC");
        using (var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseEnvironment("Development").ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["seed-demo"] = "false",
                    ["ConnectionStrings:SquadSync"] = connectionString
                }))))
        {
            using var client = factory.CreateClient();
            (await client.GetAsync("/health")).EnsureSuccessStatusCode();
            Assert.Equal(before, await RowCountsAsync(db));
        }

        await using (var transaction = await db.Database.BeginTransactionAsync())
        {
            db.Teams.Add(unrelatedTeam);
            await db.SaveChangesAsync();
            await seed.SeedAsync();
            db.ChangeTracker.Clear();
            var first = await RowCountsAsync(db);
            var additions = demoExists ? new[] { 1, 0, 0, 0, 0 } : new[] { 2, 4, 4, 2, 2 };
            Assert.Equal(before.Zip(additions, (count, added) => count + added), first);

            var team = await db.Teams.SingleAsync(candidate => candidate.Name == "SquadSync Demo FC");
            var memberships = await db.TeamMemberships.Where(candidate => candidate.TeamId == team.Id).ToListAsync();
            Assert.Equal(4, memberships.Count);
            Assert.Single(memberships, membership => membership.TeamRole == TeamRole.Coach);
            Assert.Single(memberships, membership => membership.TeamRole == TeamRole.Manager);
            var players = memberships.Where(membership => membership.TeamRole == TeamRole.Player).ToArray();
            Assert.Equal(2, players.Length);
            var userIds = memberships.Select(membership => membership.UserId).ToArray();
            Assert.Equal(4, await db.Users.CountAsync(user => userIds.Contains(user.Id) && user.LastName == "Demo"));
            var playerIds = players.Select(player => player.UserId).ToArray();
            Assert.Equal(2, await db.PlayerProfiles.CountAsync(profile => playerIds.Contains(profile.UserId)));
            var membershipIds = memberships.Select(membership => membership.Id).ToArray();
            var entries = await db.RosterEntries.Where(entry => membershipIds.Contains(entry.TeamMembershipId)).ToListAsync();
            Assert.Equal(2, entries.Count);
            Assert.All(entries, entry => Assert.Contains(players, player => player.Id == entry.TeamMembershipId));
            Assert.Contains(entries, entry => entry.JerseyNumber == "007" && entry.RosterStatus == RosterStatus.Active);
            Assert.Contains(entries, entry => entry.JerseyNumber == "12" && entry.RosterStatus == RosterStatus.Reserved);

            await seed.SeedAsync();
            db.ChangeTracker.Clear();
            Assert.Equal(first, await RowCountsAsync(db));
            Assert.Equal(unrelatedTeam.Name, (await db.Teams.SingleAsync(team => team.Id == unrelatedTeam.Id)).Name);

            // A reserved identifier with different data must fail rather than overwrite the row.
            var conflictingTeam = await db.Teams.SingleAsync(candidate => candidate.Id == team.Id);
            db.Entry(conflictingTeam).Property(candidate => candidate.Name).CurrentValue = "Conflicting demo identifier";
            await db.SaveChangesAsync();
            var conflict = await Assert.ThrowsAsync<InvalidOperationException>(() => seed.SeedAsync());
            Assert.Contains("conflicts", conflict.Message);
            Assert.Equal("Conflicting demo identifier", (await db.Teams.AsNoTracking()
                .SingleAsync(candidate => candidate.Id == team.Id)).Name);
            Assert.Equal(first, await RowCountsAsync(db));
            await transaction.RollbackAsync();
        }

        db.ChangeTracker.Clear();
        Assert.Equal(before, await RowCountsAsync(db));
    }

    private static async Task<int[]> RowCountsAsync(SquadSyncDbContext db) =>
        [await db.Teams.CountAsync(), await db.Users.CountAsync(), await db.TeamMemberships.CountAsync(),
            await db.PlayerProfiles.CountAsync(), await db.RosterEntries.CountAsync()];
}
