using SquadSync.Application.Memberships;
using SquadSync.Application.Teams;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class GetTeamMembershipsTests
{
    [Fact]
    public async Task ExecuteAsync_ExistingTeamWithNoMemberships_ReturnsEmptyList()
    {
        var fake = new Persistence();
        var memberships = new GetTeamMemberships(fake, fake);
        var result = await memberships.ExecuteAsync(fake.Team.Id);

        Assert.Empty(result);
        Assert.Equal(1, fake.TeamQueryCount);
        Assert.Equal(1, fake.MembershipQueryCount);
    }

    [Fact]
    public async Task ExecuteAsync_MissingTeam_ThrowsMembershipReferenceNotFoundException()
    {
        var fake = new Persistence();
        var memberships = new GetTeamMemberships(fake, fake);
        await Assert.ThrowsAsync<MembershipReferenceNotFoundException>(() =>
            memberships.ExecuteAsync(Guid.NewGuid()));

        Assert.Equal(1, fake.TeamQueryCount);
        Assert.Equal(0, fake.MembershipQueryCount);

    }

    [Fact]
    public async Task ExecuteAsync_EmptyTeamId_ThrowsMembershipValidationException()
    {
        var fake = new Persistence();
        var memberships = new GetTeamMemberships(fake, fake);

        var exception = await Assert.ThrowsAsync<MembershipValidationException>(() =>
            memberships.ExecuteAsync(Guid.Empty));

        Assert.Equal("teamId", exception.Field);
        Assert.Equal(0, fake.TeamQueryCount);
        Assert.Equal(0, fake.MembershipQueryCount);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingTeamWithMemberships_ReturnsMembershipDetails()
    {
        var fake = new Persistence();
        fake.MembershipResults = [
            new MembershipDetails(Guid.NewGuid(), fake.Team.Id,
                new MembershipPerson(Guid.NewGuid(), "Alex", "Player"), TeamRole.Player),
            new MembershipDetails(Guid.NewGuid(), fake.Team.Id,
                new MembershipPerson(Guid.NewGuid(), "Bob", "Coach"), TeamRole.Coach)
        ];
        var memberships = new GetTeamMemberships(fake, fake);
        var result = await memberships.ExecuteAsync(fake.Team.Id);

        Assert.Equal(2, result.Count);
        Assert.Equal(fake.MembershipResults, result);
        Assert.Equal(1, fake.TeamQueryCount);
        Assert.Equal(1, fake.MembershipQueryCount);
        Assert.Equal(fake.Team.Id, fake.LastQueriedTeamId);
    }

    private sealed class Persistence : IMembershipPersistence, ITeamPersistence
    {
        public int MembershipQueryCount { get; private set; }
        public int TeamQueryCount { get; private set; }
        public Team Team { get; } = new(Guid.NewGuid(), "Test FC");


        public Task AddAsync(TeamMembership membership, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task AddAsync(Team team, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<MembershipDetails?> GetByIdAsync(Guid teamId, Guid membershipId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            TeamQueryCount++;
            return Task.FromResult<Team?>(id == Team.Id ? Team : null);
        }

        public Task<IReadOnlyList<MembershipDetails>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            MembershipQueryCount++;
            LastQueriedTeamId = teamId;
            return Task.FromResult(MembershipResults);
        }

        public IReadOnlyList<MembershipDetails> MembershipResults { get; set; } = [];
        public Guid? LastQueriedTeamId { get; private set; }
    }
}
