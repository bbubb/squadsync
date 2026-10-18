using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SquadSync.Application.Memberships;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class MembershipPersistenceTests
{
    [Fact]
    public void MappingAndRegistration_PreserveExistingMembershipSchema()
    {
        var services = new ServiceCollection().AddSquadSyncPersistence("Host=localhost;Database=unused");
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.IsType<EfMembershipPersistence>(scope.ServiceProvider.GetRequiredService<IMembershipPersistence>());
        var context = scope.ServiceProvider.GetRequiredService<SquadSyncDbContext>();
        var entity = context.Model.FindEntityType(typeof(TeamMembership))!;
        var index = Assert.Single(entity.GetIndexes(), candidate => candidate.IsUnique);
        Assert.Equal(new[] { "PersonId", "TeamId" }, index.Properties.Select(property => property.Name));
        Assert.Equal("IX_TeamMemberships_PersonId_TeamId", index.GetDatabaseName());
        Assert.Equal(typeof(string), entity.FindProperty("TeamRole")!.GetTypeMapping().Converter!.ProviderClrType);
        Assert.All(entity.GetForeignKeys(), key => Assert.Equal(DeleteBehavior.Restrict, key.DeleteBehavior));
    }

    [Theory]
    [InlineData(PostgresErrorCodes.UniqueViolation, "IX_TeamMemberships_PersonId_TeamId", true)]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation, "FK_TeamMemberships_People_PersonId", false)]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation, "FK_TeamMemberships_Teams_TeamId", false)]
    public async Task KnownConstraintRaces_AreTranslatedAndFailedInsertDetached(string code, string constraint, bool conflict)
    {
        var failure = DatabaseFailure(code, constraint);
        var interceptor = new FailingSave(failure);
        await using var context = CreateContext(interceptor);
        var membership = new TeamMembership(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), TeamRole.Player);
        var actual = await Assert.ThrowsAnyAsync<Exception>(() => new EfMembershipPersistence(context).AddAsync(membership));
        if (conflict) Assert.IsType<MembershipConflictException>(actual);
        else Assert.IsType<MembershipReferenceNotFoundException>(actual);
        Assert.Same(failure, actual.InnerException);
        Assert.Equal(EntityState.Detached, context.Entry(membership).State);
        Assert.DoesNotContain("private-provider-detail", actual.Message);
    }

    [Theory]
    [InlineData(PostgresErrorCodes.UniqueViolation, "PK_TeamMemberships")]
    [InlineData(PostgresErrorCodes.UniqueViolation, "unrelated_index")]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation, "unrelated_fk")]
    [InlineData(PostgresErrorCodes.CheckViolation, "IX_TeamMemberships_PersonId_TeamId")]
    public async Task UnrelatedDatabaseFailures_PropagateUnchanged(string code, string constraint)
    {
        var failure = DatabaseFailure(code, constraint);
        await using var context = CreateContext(new FailingSave(failure));
        Assert.Same(failure, await Assert.ThrowsAsync<DbUpdateException>(() => new EfMembershipPersistence(context)
            .AddAsync(new TeamMembership(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), TeamRole.Player))));
    }

    [Fact]
    public async Task SaveCancellation_PropagatesWithOriginalToken()
    {
        using var source = new CancellationTokenSource();
        var failure = new OperationCanceledException(source.Token);
        var interceptor = new FailingSave(failure);
        await using var context = CreateContext(interceptor);
        Assert.Same(failure, await Assert.ThrowsAsync<OperationCanceledException>(() => new EfMembershipPersistence(context)
            .AddAsync(new TeamMembership(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), TeamRole.Player), source.Token)));
        Assert.Equal(source.Token, interceptor.Token);
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task RealAdapter_RoundTripsTeamScopedProjectionAndTranslatesDuplicate_WithoutLeavingRows()
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connection), "Opt-in database tests require ConnectionStrings__SquadSync.");
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>().UseNpgsql(connection).Options);
        var person = new Person(Guid.NewGuid(), "Alex", "Membership");
        var team = new Team(Guid.NewGuid(), "Membership Adapter Test");
        var membership = new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.AssistantCoach);
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            context.People.Add(person);
            context.Teams.Add(team);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var adapter = new EfMembershipPersistence(context);
            await adapter.AddAsync(membership);
            Assert.Equal(new MembershipDetails(membership.Id, team.Id,
                new MembershipPerson(person.Id, "Alex", "Membership"), TeamRole.AssistantCoach),
                await adapter.GetByIdAsync(team.Id, membership.Id));
            Assert.Null(await adapter.GetByIdAsync(Guid.NewGuid(), membership.Id));
            Assert.Null(await adapter.GetByIdAsync(team.Id, Guid.NewGuid()));
            Assert.Empty(context.ChangeTracker.Entries());
            await Assert.ThrowsAsync<MembershipConflictException>(() => adapter.AddAsync(
                new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.Player)));
            await transaction.RollbackAsync();
        }
        context.ChangeTracker.Clear();
        Assert.False(await context.People.AnyAsync(entry => entry.Id == person.Id));
        Assert.False(await context.Teams.AnyAsync(entry => entry.Id == team.Id));
        Assert.False(await context.TeamMemberships.AnyAsync(entry => entry.PersonId == person.Id || entry.Id == membership.Id));
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task GetByTeamId_ReturnsOnlyTeamMembershipsInExpectedOrder()
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connection), "Opt-in database tests require ConnectionStrings__SquadSync.");
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>().UseNpgsql(connection).Options);
        var teamA = new Team(Guid.NewGuid(), "Team A");
        var teamB = new Team(Guid.NewGuid(), "Team B");
        var person1 = new Person(Guid.NewGuid(), "Chris", "Delta");
        var person2 = new Person(Guid.NewGuid(), "Dana", "Echo");
        var person3 = new Person(Guid.NewGuid(), "Alice", "Bravo");
        var person4 = new Person(Guid.NewGuid(), "Chris", "Delta");
        var membership1 = new TeamMembership(Guid.Parse("a0000000-0000-0000-0000-000000000002"), person1.Id, teamA.Id, TeamRole.Player);
        var membership2 = new TeamMembership(Guid.NewGuid(), person2.Id, teamA.Id, TeamRole.AssistantCoach);
        var membership3 = new TeamMembership(Guid.NewGuid(), person3.Id, teamB.Id, TeamRole.Player);
        var membership4 = new TeamMembership(Guid.Parse("a0000000-0000-0000-0000-000000000001"), person4.Id, teamA.Id, TeamRole.Player);
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            context.People.AddRange(person1, person2, person3, person4);
            context.Teams.AddRange(teamA, teamB);
            context.TeamMemberships.AddRange(membership1, membership2, membership3, membership4);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var adapter = new EfMembershipPersistence(context);
            var teamAMemberships = await adapter.GetByTeamIdAsync(teamA.Id);
            var expected = new MembershipDetails(membership2.Id, teamA.Id, new MembershipPerson(person2.Id, "Dana", "Echo"), TeamRole.AssistantCoach);
            var actual = teamAMemberships.Single(m => m.Id == membership2.Id);
            Assert.Equal(expected, actual);
            Assert.Equal(3, teamAMemberships.Count);
            Assert.Equal(new[] { membership4.Id, membership1.Id, membership2.Id }, teamAMemberships.Select(m => m.Id));
            Assert.All(teamAMemberships, m => Assert.Equal(teamA.Id, m.TeamId));
            Assert.Equal(new[] { "Delta", "Delta", "Echo" }, teamAMemberships.Select(m => m.Person.LastName));
            Assert.DoesNotContain(teamAMemberships, m => m.Id == membership3.Id);
            await transaction.RollbackAsync();
        }
        context.ChangeTracker.Clear();
        Assert.False(await context.People.AnyAsync(entry => entry.Id == person1.Id || entry.Id == person2.Id || entry.Id == person3.Id || entry.Id == person4.Id));
        Assert.False(await context.Teams.AnyAsync(entry => entry.Id == teamA.Id || entry.Id == teamB.Id));
        Assert.False(await context.TeamMemberships.AnyAsync(entry => entry.Id == membership1.Id || entry.Id == membership2.Id || entry.Id == membership3.Id || entry.Id == membership4.Id));
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
