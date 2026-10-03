using Microsoft.EntityFrameworkCore;
using Npgsql;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class PlayerRosterPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task PlayerProfile_RoundTripsAndRejectsDuplicate_WithoutLeavingRows()
    {
        await using var dbContext = CreateDbContext();
        var user = new User(Guid.NewGuid(), "Profile", "Validation");
        var profile = new PlayerProfile(Guid.NewGuid(), user.Id, DominantFoot.Both, 71, 165.123456789m);
        var duplicate = new PlayerProfile(Guid.NewGuid(), user.Id, null, null, null);
        var otherUser = new User(Guid.NewGuid(), "Optional", "Measurements");
        var emptyProfile = new PlayerProfile(Guid.NewGuid(), otherUser.Id, null, null, null);

        await using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            dbContext.Users.AddRange(user, otherUser);
            Assert.Equal(2, await dbContext.SaveChangesAsync());
            dbContext.PlayerProfiles.AddRange(profile, emptyProfile);
            Assert.Equal(2, await dbContext.SaveChangesAsync());
            dbContext.ChangeTracker.Clear();

            var persisted = await dbContext.PlayerProfiles.SingleAsync(candidate => candidate.Id == profile.Id);
            Assert.NotSame(profile, persisted);
            Assert.Equal(profile.Id, persisted.Id);
            Assert.Equal(user.Id, persisted.UserId);
            Assert.Equal(DominantFoot.Both, persisted.DominantFoot);
            Assert.Equal(profile.HeightInches, persisted.HeightInches);
            Assert.Equal(profile.WeightPounds, persisted.WeightPounds);
            var persistedEmpty = await dbContext.PlayerProfiles.SingleAsync(candidate => candidate.Id == emptyProfile.Id);
            Assert.NotSame(emptyProfile, persistedEmpty);
            Assert.Equal(otherUser.Id, persistedEmpty.UserId);
            Assert.Null(persistedEmpty.DominantFoot);
            Assert.Null(persistedEmpty.HeightInches);
            Assert.Null(persistedEmpty.WeightPounds);
            var storedFoot = await dbContext.Database.SqlQuery<string>(
                $"""SELECT "DominantFoot" AS "Value" FROM "PlayerProfiles" WHERE "Id" = {profile.Id}""").SingleAsync();
            Assert.Equal("Both", storedFoot);

            // Avoid tracked one-to-one fixup so PostgreSQL enforces the duplicate rule.
            dbContext.ChangeTracker.Clear();
            dbContext.PlayerProfiles.Add(duplicate);
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
            var postgresException = Assert.IsType<PostgresException>(exception.InnerException);
            Assert.Equal(PostgresErrorCodes.UniqueViolation, postgresException.SqlState);
            Assert.Equal("IX_PlayerProfiles_UserId", postgresException.ConstraintName);
            await transaction.RollbackAsync();
        }

        dbContext.ChangeTracker.Clear();
        Assert.False(await dbContext.Users.AnyAsync(candidate => candidate.Id == user.Id || candidate.Id == otherUser.Id));
        Assert.False(await dbContext.PlayerProfiles.AnyAsync(candidate =>
            candidate.UserId == user.Id || candidate.UserId == otherUser.Id ||
            candidate.Id == profile.Id || candidate.Id == duplicate.Id || candidate.Id == emptyProfile.Id));
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task RosterEntry_RoundTripsAndRejectsDuplicate_WithoutLeavingRows()
    {
        await using var dbContext = CreateDbContext();
        var user = new User(Guid.NewGuid(), "Roster", "Validation");
        var team = new Team(Guid.NewGuid(), "Roster Persistence Validation");
        // Role eligibility belongs to Application; persistence only enforces structural relationships.
        var membership = new TeamMembership(Guid.NewGuid(), user.Id, team.Id, TeamRole.AssistantCoach);
        var entry = new RosterEntry(Guid.NewGuid(), membership.Id, "007", RosterStatus.Reserved);
        var duplicate = new RosterEntry(Guid.NewGuid(), membership.Id, null, RosterStatus.Active);

        await using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            dbContext.Users.Add(user);
            dbContext.Teams.Add(team);
            dbContext.TeamMemberships.Add(membership);
            Assert.Equal(3, await dbContext.SaveChangesAsync());
            dbContext.RosterEntries.Add(entry);
            Assert.Equal(1, await dbContext.SaveChangesAsync());
            dbContext.ChangeTracker.Clear();

            var persisted = await dbContext.RosterEntries.SingleAsync(candidate => candidate.Id == entry.Id);
            Assert.NotSame(entry, persisted);
            Assert.Equal(entry.Id, persisted.Id);
            Assert.Equal(membership.Id, persisted.TeamMembershipId);
            Assert.Equal("007", persisted.JerseyNumber);
            Assert.Equal(RosterStatus.Reserved, persisted.RosterStatus);
            var storedStatus = await dbContext.Database.SqlQuery<string>(
                $"""SELECT "RosterStatus" AS "Value" FROM "RosterEntries" WHERE "Id" = {entry.Id}""").SingleAsync();
            Assert.Equal("Reserved", storedStatus);

            dbContext.ChangeTracker.Clear();
            dbContext.RosterEntries.Add(duplicate);
            var exception = await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
            var postgresException = Assert.IsType<PostgresException>(exception.InnerException);
            Assert.Equal(PostgresErrorCodes.UniqueViolation, postgresException.SqlState);
            Assert.Equal("IX_RosterEntries_TeamMembershipId", postgresException.ConstraintName);
            await transaction.RollbackAsync();
        }

        dbContext.ChangeTracker.Clear();
        Assert.False(await dbContext.Users.AnyAsync(candidate => candidate.Id == user.Id));
        Assert.False(await dbContext.Teams.AnyAsync(candidate => candidate.Id == team.Id));
        Assert.False(await dbContext.TeamMemberships.AnyAsync(candidate => candidate.Id == membership.Id));
        Assert.False(await dbContext.RosterEntries.AnyAsync(candidate =>
            candidate.TeamMembershipId == membership.Id || candidate.Id == entry.Id || candidate.Id == duplicate.Id));
    }

    private static SquadSyncDbContext CreateDbContext()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connectionString),
            "Database tests require ConnectionStrings__SquadSync when SQUADSYNC_RUN_DATABASE_TESTS=1.");
        return new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql(connectionString).Options);
    }
}
