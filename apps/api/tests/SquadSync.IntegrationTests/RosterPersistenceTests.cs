using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SquadSync.Application.Rosters;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class RosterPersistenceTests
{
    [Theory]
    [InlineData(PostgresErrorCodes.UniqueViolation, "IX_RosterEntries_TeamMembershipId", true)]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation, "FK_RosterEntries_TeamMemberships_TeamMembershipId", false)]
    [InlineData(PostgresErrorCodes.UniqueViolation, "PK_RosterEntries", true)]
    public async Task KnownConstraintRaces_AreTranslatedAndFailedInsertDetached(string code, string constraint, bool conflict)
    {
        var failure = DatabaseFailure(code, constraint);
        var interceptor = new FailingSave(failure);
        await using var context = CreateContext(interceptor);
        var membership = new RosterEntry(Guid.NewGuid(), Guid.NewGuid(), "007", RosterStatus.Active);
        var actual = await Assert.ThrowsAnyAsync<Exception>(() => new EfRosterPersistence(context).AddEntryAsync(membership));
        if (conflict) Assert.IsType<RosterEntryConflictException>(actual);
        else Assert.IsType<RosterMembershipNotFoundException>(actual);
        Assert.Same(failure, actual.InnerException);
        Assert.Equal(EntityState.Detached, context.Entry(membership).State);
        Assert.DoesNotContain("private-provider-detail", actual.Message);
    }

    [Theory]
    [InlineData(PostgresErrorCodes.UniqueViolation, "PK_Unrelated")]
    [InlineData(PostgresErrorCodes.UniqueViolation, "unrelated_index")]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation, "unrelated_fk")]
    [InlineData(PostgresErrorCodes.CheckViolation, "IX_RosterEntries_TeamMembershipId")]
    public async Task UnrelatedDatabaseFailures_PropagateUnchanged(string code, string constraint)
    {
        var failure = DatabaseFailure(code, constraint);
        await using var context = CreateContext(new FailingSave(failure));
        Assert.Same(failure, await Assert.ThrowsAsync<DbUpdateException>(() => new EfRosterPersistence(context)
            .AddEntryAsync(new RosterEntry(Guid.NewGuid(), Guid.NewGuid(), "007", RosterStatus.Active))));
    }

    [Fact]
    public async Task SaveCancellation_PropagatesWithOriginalToken()
    {
        using var source = new CancellationTokenSource();
        var failure = new OperationCanceledException(source.Token);
        var interceptor = new FailingSave(failure);
        await using var context = CreateContext(interceptor);
        Assert.Same(failure, await Assert.ThrowsAsync<OperationCanceledException>(() => new EfRosterPersistence(context)
            .AddEntryAsync(new RosterEntry(Guid.NewGuid(), Guid.NewGuid(), "007", RosterStatus.Active), source.Token)));
        Assert.Equal(source.Token, interceptor.Token);
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task IndividualProjection_IsTeamScopedAndPlayerOnly_WithoutLeavingRows()
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connection), "Opt-in database tests require ConnectionStrings__SquadSync.");
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>().UseNpgsql(connection).Options);
        var person = new Person(Guid.NewGuid(), "Alex", "Projection");
        var coach = new Person(Guid.NewGuid(), "Chris", "Coach");
        var team = new Team(Guid.NewGuid(), "Roster Projection Test");
        var playerMembership = new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.Player);
        var coachMembership = new TeamMembership(Guid.NewGuid(), coach.Id, team.Id, TeamRole.Coach);
        var entry = new RosterEntry(Guid.NewGuid(), playerMembership.Id, "007", RosterStatus.Suspended);
        // Structural persistence permits this; reads must still filter non-Players.
        var invalidEntry = new RosterEntry(Guid.NewGuid(), coachMembership.Id, null, RosterStatus.Active);
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            context.People.AddRange(person, coach);
            context.Teams.Add(team);
            context.TeamMemberships.AddRange(playerMembership, coachMembership);
            context.RosterEntries.AddRange(entry, invalidEntry);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var adapter = new EfRosterPersistence(context);
            Assert.Equal(new RosterDetails(entry.Id, playerMembership.Id,
                new RosterPerson(person.Id, "Alex", "Projection"), "007", RosterStatus.Suspended),
                await adapter.GetByIdAsync(team.Id, entry.Id));
            Assert.Null(await adapter.GetByIdAsync(Guid.NewGuid(), entry.Id));
            Assert.Null(await adapter.GetByIdAsync(team.Id, Guid.NewGuid()));
            Assert.Null(await adapter.GetByIdAsync(team.Id, invalidEntry.Id));
            Assert.Empty(context.ChangeTracker.Entries());
            await transaction.RollbackAsync();
        }
        context.ChangeTracker.Clear();
        Assert.False(await context.People.AnyAsync(p => p.Id == person.Id || p.Id == coach.Id));
        Assert.False(await context.Teams.AnyAsync(t => t.Id == team.Id));
        Assert.False(await context.TeamMemberships.AnyAsync(m => m.TeamId == team.Id));
        Assert.False(await context.RosterEntries.AnyAsync(e => e.Id == entry.Id || e.Id == invalidEntry.Id));
    }

    private static DbUpdateException DatabaseFailure(string code, string constraint)
        => new("private-provider-detail", new PostgresException("private-provider-detail", "ERROR", "ERROR", code, constraintName: constraint));

    private static SquadSyncDbContext CreateContext(FailingSave interceptor)
        => new(new DbContextOptionsBuilder<SquadSyncDbContext>().UseNpgsql("Host=localhost;Database=unused")
            .AddInterceptors(interceptor).Options);

    // Exercise the real adapter's SaveChanges exception filters without opening a database connection.
    private sealed class FailingSave(Exception failure) : SaveChangesInterceptor
    {
        public CancellationToken Token { get; private set; }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Token = cancellationToken;
            throw failure;
        }
    }
}
