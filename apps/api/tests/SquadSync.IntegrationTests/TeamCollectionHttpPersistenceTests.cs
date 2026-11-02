using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using SquadSync.Api.Teams;
using SquadSync.Application.Teams;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

[Collection("Transactional HTTP database")]
public sealed class TeamCollectionHttpPersistenceTests
{
    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task Collection_UsesPostgresProjectionAndOrdering_WithoutChangingStoredData()
    {
        await new TransactionalHttpDatabase().RunAsync(async (client, db) =>
        {
            var teams = new[]
            {
                new Team(Guid.Parse("c0000000-0000-0000-0000-000000000002"), "Collection Alpha FC"),
                new Team(Guid.NewGuid(), "Collection Zulu FC"),
                new Team(Guid.Parse("c0000000-0000-0000-0000-000000000001"), "Collection Alpha FC")
            };
            db.Teams.AddRange(teams);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
            var before = await TransactionalHttpDatabase.SnapshotAsync(db);
            var adapter = new EfTeamPersistence(db);
            var expected = await db.Teams.AsNoTracking().OrderBy(team => team.Name).ThenBy(team => team.Id)
                .Select(team => new TeamDetails(team.Id, team.Name)).ToListAsync();

            Assert.Equal(expected, await adapter.GetAllAsync());
            using var response = await client.GetAsync("/api/teams");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = (await response.Content.ReadFromJsonAsync<TeamResponse[]>())!;
            Assert.Equal(expected.Select(team => new TeamResponse(team.Id, team.Name)), result);
            var insertedIds = teams.Select(team => team.Id).ToHashSet();
            Assert.Equal(new[] { teams[2].Id, teams[0].Id, teams[1].Id },
                result.Where(team => insertedIds.Contains(team.Id)).Select(team => team.Id));
            Assert.Empty(db.ChangeTracker.Entries());
            Assert.Equal(before, await TransactionalHttpDatabase.SnapshotAsync(db));
        });
    }
}
