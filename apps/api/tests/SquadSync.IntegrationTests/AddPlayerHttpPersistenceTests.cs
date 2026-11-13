using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SquadSync.Api.Rosters;
using SquadSync.Application.Rosters;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

[Collection("Transactional HTTP database")]
public sealed class AddPlayerHttpPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task BothVariantsAndReuse_PersistAcrossRequestsAndPreservePriorData()
    {
        var fixture = new TransactionalHttpDatabase();
        await fixture.RunAsync(async (client, db) =>
        {
            var team = new Team(Guid.NewGuid(), "Atomic Add Player");
            var existing = new Person(Guid.NewGuid(), "Existing", "Player");
            var unrostered = new Person(Guid.NewGuid(), "Unrostered", "Player");
            var staff = new Person(Guid.NewGuid(), "Existing", "Coach");
            var membership = new TeamMembership(Guid.NewGuid(), unrostered.Id, team.Id, TeamRole.Player);
            var staffMembership = new TeamMembership(Guid.NewGuid(), staff.Id, team.Id, TeamRole.Coach);
            db.Teams.Add(team);
            db.People.AddRange(existing, unrostered, staff);
            db.TeamMemberships.AddRange(membership, staffMembership);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
            var profileCount = await db.PlayerProfiles.CountAsync();
            var initialPeople = await db.People.CountAsync();
            var initialMemberships = await db.TeamMemberships.CountAsync();
            var initialRoster = await db.RosterEntries.CountAsync();
            var route = $"/api/teams/{team.Id}/roster/players";

            var created = await client.PostAsJsonAsync(route, new
            {
                newPerson = new { firstName = " Alex ", lastName = " Player " },
                jerseyNumber = "007", rosterStatus = "Active"
            });
            Assert.Equal(HttpStatusCode.Created, created.StatusCode);
            var entry = (await created.Content.ReadFromJsonAsync<RosterEntryResponse>())!;
            Assert.Equal("Alex", entry.Person.FirstName);
            Assert.Equal("007", entry.JerseyNumber);
            Assert.Equal(entry, await client.GetFromJsonAsync<RosterEntryResponse>(created.Headers.Location));
            Assert.Equal($"/api/teams/{team.Id}/roster/{entry.Id}", created.Headers.Location!.AbsolutePath);

            foreach (var person in new[] { existing, unrostered })
            {
                var response = await client.PostAsJsonAsync(route, new { personId = person.Id, jerseyNumber = (string?)null, rosterStatus = "Reserved" });
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                var result = (await response.Content.ReadFromJsonAsync<RosterEntryResponse>())!;
                Assert.Equal(person.Id, result.Person.Id);
                Assert.Null(result.JerseyNumber);
                if (person == unrostered) Assert.Equal(membership.Id, result.TeamMembershipId);
                Assert.Equal(result, await client.GetFromJsonAsync<RosterEntryResponse>(response.Headers.Location));
            }
            Assert.Equal(initialPeople + 1, await db.People.CountAsync());
            Assert.Equal(initialMemberships + 2, await db.TeamMemberships.CountAsync());
            Assert.Equal(initialRoster + 3, await db.RosterEntries.CountAsync());
            Assert.Equal(profileCount, await db.PlayerProfiles.CountAsync());
            Assert.Equal(3, await db.TeamMemberships.CountAsync(m => m.TeamId == team.Id && m.TeamRole == TeamRole.Player));
            var beforeRejected = await TransactionalHttpDatabase.SnapshotAsync(db);
            foreach (var (target, body, expected) in new (Guid, object, HttpStatusCode)[]
            {
                (team.Id, new { personId = existing.Id, rosterStatus = "Active" }, HttpStatusCode.Conflict),
                (team.Id, new { personId = staff.Id, rosterStatus = "Active" }, HttpStatusCode.Conflict),
                (team.Id, new { personId = Guid.NewGuid(), rosterStatus = "Active" }, HttpStatusCode.NotFound),
                (Guid.NewGuid(), new { newPerson = new { firstName = "Missing", lastName = "Team" }, rosterStatus = "Active" }, HttpStatusCode.NotFound),
                (team.Id, new { personId = existing.Id, jerseyNumber = "bad", rosterStatus = "Active" }, HttpStatusCode.BadRequest)
            })
            {
                var response = await client.PostAsJsonAsync($"/api/teams/{target}/roster/players", body);
                Assert.Equal(expected, response.StatusCode);
                Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
                Assert.Equal(beforeRejected, await TransactionalHttpDatabase.SnapshotAsync(db));
            }
            Assert.Equal(TeamRole.Coach, (await db.TeamMemberships.AsNoTracking().SingleAsync(m => m.Id == staffMembership.Id)).TeamRole);
        });
        Assert.True(fixture.ContextIds.Distinct().Count() >= 10);
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task RosterInsertFailure_RollsBackNewPersonAndMembership_AndAllowsNextRequest()
    {
        var collision = new RosterCollision();
        await new TransactionalHttpDatabase().RunAsync(async (client, db) =>
        {
            var team = new Team(Guid.NewGuid(), "Atomic rollback");
            db.Teams.Add(team);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
            var before = await TransactionalHttpDatabase.SnapshotAsync(db);
            var route = $"/api/teams/{team.Id}/roster/players";
            collision.Enabled = true;
            var response = await client.PostAsJsonAsync(route, new
            {
                newPerson = new { firstName = "Rollback", lastName = "Player" }, rosterStatus = "Active"
            });
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.True(collision.SawPersistedPersonAndMembership);
            Assert.Equal(before, await TransactionalHttpDatabase.SnapshotAsync(db));
            collision.Enabled = false;
            Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync(route, new
            {
                newPerson = new { firstName = "Next", lastName = "Player" }, rosterStatus = "Suspended"
            })).StatusCode);
        }, collision);
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task StaleMembershipRead_RealUniqueConstraintRollsBackOperation()
    {
        await new TransactionalHttpDatabase().RunAsync(async (_, db) =>
        {
            var team = new Team(Guid.NewGuid(), "Membership race");
            var person = new Person(Guid.NewGuid(), "Race", "Player");
            db.Teams.Add(team);
            db.People.Add(person);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
            var before = await TransactionalHttpDatabase.SnapshotAsync(db);
            await using (var operation = await new EfAddPlayerPersistence(db).BeginAsync())
            {
                Assert.Null(await operation.GetMembershipAsync(team.Id, person.Id, default));
                // Deterministically interleave a competing insert after the stale read.
                var competing = new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.Player);
                db.TeamMemberships.Add(competing);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();
                var attempted = new TeamMembership(Guid.NewGuid(), person.Id, team.Id, TeamRole.Player);
                var failure = await Assert.ThrowsAsync<AddPlayerConflictException>(() => operation.SaveAsync(
                    null, attempted, new RosterEntry(Guid.NewGuid(), attempted.Id, null, RosterStatus.Active), default));
                Assert.IsType<DbUpdateException>(failure.InnerException);
            }
            Assert.Equal(before, await TransactionalHttpDatabase.SnapshotAsync(db));
            Assert.Empty(db.ChangeTracker.Entries());
        });
    }

    // Inject a real PK collision after the earlier writes have reached PostgreSQL.
    // The operation's rollback must remove the injected row and both preceding rows.
    private sealed class RosterCollision : SaveChangesInterceptor
    {
        public bool Enabled, SawPersistedPersonAndMembership;
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var db = (SquadSyncDbContext)eventData.Context!;
            var entry = db.ChangeTracker.Entries<RosterEntry>().SingleOrDefault(e => e.State == EntityState.Added)?.Entity;
            if (!Enabled || entry is null) return result;
            var membership = await db.TeamMemberships.AsNoTracking().SingleAsync(m => m.Id == entry.TeamMembershipId, cancellationToken);
            SawPersistedPersonAndMembership = await db.People.AnyAsync(p => p.Id == membership.PersonId, cancellationToken);
            await db.Database.ExecuteSqlInterpolatedAsync(
                $"""INSERT INTO "RosterEntries" ("Id", "TeamMembershipId", "JerseyNumber", "RosterStatus") VALUES ({entry.Id}, {entry.TeamMembershipId}, NULL, 'Active')""",
                cancellationToken);
            return result;
        }
    }
}
