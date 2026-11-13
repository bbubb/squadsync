using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using SquadSync.Application.Rosters;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

[Collection("Transactional HTTP database")]
public sealed class AddPlayerPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task KnownConstraintRaces_MapSafelyAndRollbackAllWrites()
    {
        foreach (var (code, constraint, expected) in new (string, string, Type)[]
        {
            (PostgresErrorCodes.UniqueViolation, "PK_People", typeof(AddPlayerConflictException)),
            (PostgresErrorCodes.UniqueViolation, "PK_TeamMemberships", typeof(AddPlayerConflictException)),
            (PostgresErrorCodes.UniqueViolation, "IX_TeamMemberships_PersonId_TeamId", typeof(AddPlayerConflictException)),
            (PostgresErrorCodes.UniqueViolation, "PK_RosterEntries", typeof(AddPlayerConflictException)),
            (PostgresErrorCodes.UniqueViolation, "IX_RosterEntries_TeamMembershipId", typeof(AddPlayerConflictException)),
            (PostgresErrorCodes.ForeignKeyViolation, "FK_TeamMemberships_People_PersonId", typeof(AddPlayerPersonNotFoundException)),
            (PostgresErrorCodes.ForeignKeyViolation, "FK_TeamMemberships_Teams_TeamId", typeof(RosterTeamNotFoundException)),
            (PostgresErrorCodes.ForeignKeyViolation, "FK_RosterEntries_TeamMemberships_TeamMembershipId", typeof(AddPlayerConflictException))
        })
        {
            var fault = new RosterFault(Failure(code, constraint));
            await new TransactionalHttpDatabase().RunAsync(async (_, db) =>
            {
                var team = await SeedTeam(db);
                var before = await TransactionalHttpDatabase.SnapshotAsync(db);
                fault.Enabled = true;
                var exception = await Record.ExceptionAsync(() => new AddPlayer(new EfAddPlayerPersistence(db))
                    .ExecuteAsync(team.Id, null, new("Constraint", "Race"), "007", RosterStatus.Active));
                Assert.IsType(expected, exception);
                Assert.Equal(1, fault.Attempts);
                Assert.Equal(before, await TransactionalHttpDatabase.SnapshotAsync(db));
                Assert.Empty(db.ChangeTracker.Entries());
            }, fault);
        }
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task UnrelatedFailuresAndCancellation_PropagateAndPreserveExistingPersonAndMembership()
    {
        foreach (var failure in new Exception[]
        {
            new InvalidOperationException("unrelated"),
            Failure(PostgresErrorCodes.UniqueViolation, "unrelated_index"),
            Failure(PostgresErrorCodes.ForeignKeyViolation, "unrelated_fk"),
            new OperationCanceledException(new CancellationToken(true))
        })
        {
            var fault = new RosterFault(failure);
            await new TransactionalHttpDatabase().RunAsync(async (_, db) =>
            {
                var team = await SeedTeam(db);
                var person = new Person(Guid.NewGuid(), "Existing", "Player");
                var membership = new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.Player);
                db.People.Add(person);
                db.TeamMemberships.Add(membership);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();
                var before = await TransactionalHttpDatabase.SnapshotAsync(db);
                fault.Enabled = true;
                var exception = await Record.ExceptionAsync(() => new AddPlayer(new EfAddPlayerPersistence(db))
                    .ExecuteAsync(team.Id, person.Id, null, null, RosterStatus.Suspended));
                Assert.Same(failure, exception);
                Assert.Equal(1, fault.Attempts);
                Assert.Equal(before, await TransactionalHttpDatabase.SnapshotAsync(db));
                Assert.Empty(db.ChangeTracker.Entries());
            }, fault);
        }
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task OwnedTransaction_RollsBackStagedWritesWithoutAnExternalTransaction()
    {
        var fault = new RosterFault(new InvalidOperationException("roster failure"));
        await using var db = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync"))
            .AddInterceptors(fault).Options);
        await db.Database.OpenConnectionAsync();
        var before = await TransactionalHttpDatabase.SnapshotAsync(db);
        var person = new Person(Guid.NewGuid(), "Owned", "Rollback");
        await using (var transaction = await new EfAddPlayerPersistence(db).BeginAsync())
        {
            var team = await SeedTeam(db); // Test setup stays inside the operation's transaction.
            var membership = new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.Player);
            fault.Enabled = true;
            Assert.Same(fault.Failure, await Record.ExceptionAsync(() => transaction.SaveAsync(person, membership,
                new RosterEntry(Guid.NewGuid(), membership.Id, null, RosterStatus.Active), default)));
            Assert.True(await db.People.AnyAsync(p => p.Id == person.Id));
            Assert.True(await db.TeamMemberships.AnyAsync(m => m.Id == membership.Id));
        }
        Assert.Null(db.Database.CurrentTransaction);
        Assert.Equal(before, await TransactionalHttpDatabase.SnapshotAsync(db));
        Assert.Empty(db.ChangeTracker.Entries());
        // Exercise owned commit/disposal without leaving durable test data.
        await using (var transaction = await new EfAddPlayerPersistence(db).BeginAsync())
            await transaction.CommitAsync(default);
        Assert.Null(db.Database.CurrentTransaction);
    }

    private static async Task<Team> SeedTeam(SquadSyncDbContext db)
    {
        var team = new Team(Guid.NewGuid(), "Add Player adapter test");
        db.Teams.Add(team);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        return team;
    }

    private static DbUpdateException Failure(string code, string constraint)
        => new("private-provider-detail", new PostgresException("private-provider-detail", "ERROR", "ERROR", code, constraintName: constraint));

    private sealed class RosterFault(Exception failure) : SaveChangesInterceptor
    {
        public Exception Failure { get; } = failure;
        public bool Enabled;
        public int Attempts;
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (Enabled && eventData.Context!.ChangeTracker.Entries<RosterEntry>().Any(e => e.State == EntityState.Added))
            {
                Attempts++;
                throw Failure;
            }
            return ValueTask.FromResult(result);
        }
    }
}
