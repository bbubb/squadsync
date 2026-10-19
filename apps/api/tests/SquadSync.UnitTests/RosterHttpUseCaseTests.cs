using SquadSync.Application.Memberships;
using SquadSync.Application.Rosters;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class RosterHttpUseCaseTests
{
    [Fact]
    public async Task Create_ReturnsGeneratedEntryWithPersonAndPassesCancellation()
    {
        var fake = new Persistence();
        using var source = new CancellationTokenSource();
        var result = await Create(fake).ExecuteAsync(fake.Membership.TeamId, fake.Membership.Id, "007", RosterStatus.Reserved, source.Token);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(fake.Saved!.Id, result.Id);
        Assert.Equal(fake.Membership.Id, result.TeamMembershipId);
        Assert.Equal(new RosterPerson(fake.Membership.PersonId, "Alex", "Player"), result.Person);
        Assert.Equal("007", result.JerseyNumber);
        Assert.Equal(RosterStatus.Reserved, result.RosterStatus);
        Assert.Equal(3, fake.Tokens.Count);
        Assert.All(fake.Tokens, token => Assert.Equal(source.Token, token));
    }

    [Theory]
    [InlineData("teamId")]
    [InlineData("teamMembershipId")]
    [InlineData("jerseyNumber")]
    [InlineData("rosterStatus")]
    public async Task InvalidCreation_IsRejectedBeforePersistence(string field)
    {
        var fake = new Persistence();
        var error = await Assert.ThrowsAsync<RosterValidationException>(() => Create(fake).ExecuteAsync(
            field == "teamId" ? Guid.Empty : fake.Membership.TeamId,
            field == "teamMembershipId" ? Guid.Empty : fake.Membership.Id,
            field == "jerseyNumber" ? "\uFF11\uFF12" : null,
            field == "rosterStatus" ? (RosterStatus)99 : RosterStatus.Active));
        Assert.Equal(field, error.Field);
        Assert.Empty(fake.Tokens);
        Assert.Null(fake.Saved);
    }

    [Fact]
    public async Task Create_WrongTeamIsNotFoundWithoutWrite()
    {
        var fake = new Persistence();
        await Assert.ThrowsAsync<RosterMembershipNotFoundException>(() => Create(fake).ExecuteAsync(
            Guid.NewGuid(), fake.Membership.Id, null, RosterStatus.Active));
        Assert.Null(fake.Saved);
    }

    [Fact]
    public async Task Get_PassesScopeAndTokenAndReturnsProjectionOrNull()
    {
        var fake = new Persistence();
        var created = await Create(fake).ExecuteAsync(fake.Membership.TeamId, fake.Membership.Id, null, RosterStatus.Active);
        using var source = new CancellationTokenSource();
        var query = new GetRosterEntry(fake);
        Assert.Equal(created, await query.ExecuteAsync(fake.Membership.TeamId, created.Id, source.Token));
        Assert.Equal((fake.Membership.TeamId, created.Id), fake.Lookup);
        Assert.Equal(source.Token, fake.Tokens.Last());
        Assert.Null(await query.ExecuteAsync(Guid.NewGuid(), created.Id));
        Assert.Null(await query.ExecuteAsync(fake.Membership.TeamId, Guid.NewGuid()));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Get_EmptyIdRejectedBeforePersistence(bool emptyTeam)
    {
        var fake = new Persistence();
        var error = await Assert.ThrowsAsync<RosterValidationException>(() => new GetRosterEntry(fake).ExecuteAsync(
            emptyTeam ? Guid.Empty : Guid.NewGuid(), emptyTeam ? Guid.NewGuid() : Guid.Empty));
        Assert.Equal(emptyTeam ? "teamId" : "rosterEntryId", error.Field);
        Assert.Empty(fake.Tokens);
    }

    [Fact]
    public async Task CancelledCreateAndGet_DoNotAccessPersistence()
    {
        var fake = new Persistence();
        var token = new CancellationToken(canceled: true);
        var createError = await Assert.ThrowsAsync<OperationCanceledException>(() => Create(fake).ExecuteAsync(
            fake.Membership.TeamId, fake.Membership.Id, null, RosterStatus.Active, token));
        var readError = await Assert.ThrowsAsync<OperationCanceledException>(() => new GetRosterEntry(fake).ExecuteAsync(
            fake.Membership.TeamId, Guid.NewGuid(), token));
        Assert.Equal(token, createError.CancellationToken);
        Assert.Equal(token, readError.CancellationToken);
        Assert.Empty(fake.Tokens);
    }

    private static CreateRosterEntry Create(Persistence fake) => new(new AddPlayerToRoster(fake), fake);

    private sealed class Persistence : IRosterPersistence, IRosterQueries, IMembershipPersistence
    {
        public TeamMembership Membership { get; } = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), TeamRole.Player);
        public RosterEntry? Saved { get; private set; }
        public List<CancellationToken> Tokens { get; } = new();
        public (Guid TeamId, Guid EntryId) Lookup { get; private set; }

        public Task<TeamMembership?> GetMembershipAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Tokens.Add(cancellationToken);
            return Task.FromResult(id == Membership.Id ? Membership : null);
        }

        public Task AddEntryAsync(RosterEntry entry, CancellationToken cancellationToken = default)
        {
            Tokens.Add(cancellationToken);
            Saved = entry;
            return Task.CompletedTask;
        }

        Task<MembershipDetails?> IMembershipPersistence.GetByIdAsync(Guid teamId, Guid id, CancellationToken cancellationToken)
        {
            Tokens.Add(cancellationToken);
            return Task.FromResult(teamId != Membership.TeamId || id != Membership.Id ? null
                : new MembershipDetails(id, teamId, new MembershipPerson(Membership.PersonId, "Alex", "Player"), TeamRole.Player));
        }

        public Task<RosterDetails?> GetByIdAsync(Guid teamId, Guid id, CancellationToken cancellationToken = default)
        {
            Tokens.Add(cancellationToken);
            Lookup = (teamId, id);
            return Task.FromResult(Saved is null || teamId != Membership.TeamId || id != Saved.Id ? null
                : new RosterDetails(id, Membership.Id, new RosterPerson(Membership.PersonId, "Alex", "Player"), Saved.JerseyNumber, Saved.RosterStatus));
        }

        Task<IReadOnlyList<RosterDetails>> IRosterQueries.GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task AddAsync(TeamMembership membership, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<MembershipDetails>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
