using SquadSync.Application.Rosters;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class AddPlayerToRosterTests
{
    [Theory]
    [InlineData(null, RosterStatus.Active)]
    [InlineData("007", RosterStatus.Reserved)]
    public async Task PlayerMembership_PersistsRequestedEntry(string? jerseyNumber, RosterStatus status)
    {
        var persistence = new FakeRosterPersistence(CreateMembership(TeamRole.Player));
        var id = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();

        var entry = await new AddPlayerToRoster(persistence).ExecuteAsync(
            id, persistence.Membership!.Id, jerseyNumber, status, cancellation.Token);

        Assert.Same(entry, persistence.SavedEntry);
        Assert.Equal(id, entry.Id);
        Assert.Equal(persistence.Membership.Id, entry.TeamMembershipId);
        Assert.Equal(jerseyNumber, entry.JerseyNumber);
        Assert.Equal(status, entry.RosterStatus);
        Assert.Equal(cancellation.Token, persistence.LoadToken);
        Assert.Equal(cancellation.Token, persistence.SaveToken);
        Assert.Equal(persistence.Membership.Id, persistence.RequestedMembershipId);
        Assert.Equal(1, persistence.SaveCalls);
    }

    [Theory]
    [InlineData(TeamRole.AssistantCoach)]
    [InlineData(TeamRole.Coach)]
    [InlineData(TeamRole.Manager)]
    [InlineData(TeamRole.Owner)]
    [InlineData(TeamRole.Viewer)]
    public async Task NonPlayerMembership_IsRejectedBeforePersistence(TeamRole role)
    {
        var persistence = new FakeRosterPersistence(CreateMembership(role));
        var membershipId = persistence.Membership!.Id;

        var exception = await Assert.ThrowsAsync<RosterMembershipNotPlayerException>(() =>
            new AddPlayerToRoster(persistence).ExecuteAsync(Guid.NewGuid(), membershipId, null, RosterStatus.Active));

        Assert.Equal(membershipId, exception.TeamMembershipId);
        Assert.Equal(role, exception.TeamRole);
        Assert.Equal(0, persistence.SaveCalls);
    }

    [Fact]
    public async Task MissingMembership_IsRejectedBeforePersistence()
    {
        var persistence = new FakeRosterPersistence(null);
        var membershipId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<RosterMembershipNotFoundException>(() =>
            new AddPlayerToRoster(persistence).ExecuteAsync(Guid.NewGuid(), membershipId, null, RosterStatus.Active));

        Assert.Equal(membershipId, exception.TeamMembershipId);
        Assert.Equal(membershipId, persistence.RequestedMembershipId);
        Assert.Equal(0, persistence.SaveCalls);
    }

    [Theory]
    [InlineData(true, null, RosterStatus.Active, "id")]
    [InlineData(false, "", RosterStatus.Active, "jerseyNumber")]
    [InlineData(false, "1234", RosterStatus.Active, "jerseyNumber")]
    [InlineData(false, "A", RosterStatus.Active, "jerseyNumber")]
    [InlineData(false, null, (RosterStatus)999, "rosterStatus")]
    public async Task InvalidEntry_PropagatesDomainValidationWithoutPersistence(
        bool emptyId, string? jerseyNumber, RosterStatus status, string parameterName)
    {
        var persistence = new FakeRosterPersistence(CreateMembership(TeamRole.Player));
        var exception = await Assert.ThrowsAnyAsync<ArgumentException>(() =>
            new AddPlayerToRoster(persistence).ExecuteAsync(
                emptyId ? Guid.Empty : Guid.NewGuid(), persistence.Membership!.Id, jerseyNumber, status));

        Assert.Equal(parameterName, exception.ParamName);
        Assert.Contains("SquadSync.Domain.RosterEntry..ctor", exception.StackTrace);
        Assert.Equal(0, persistence.SaveCalls);
    }

    [Fact]
    public async Task DuplicateConflict_PropagatesApplicationException()
    {
        var persistence = new FakeRosterPersistence(CreateMembership(TeamRole.Player));
        var id = Guid.NewGuid();
        var conflict = new RosterEntryConflictException(id, persistence.Membership!.Id);
        persistence.SaveException = conflict;

        var exception = await Assert.ThrowsAsync<RosterEntryConflictException>(() =>
            new AddPlayerToRoster(persistence).ExecuteAsync(id, persistence.Membership.Id, null, RosterStatus.Active));

        Assert.Same(conflict, exception);
        Assert.Equal(1, persistence.SaveCalls);
    }

    [Theory]
    [InlineData(TeamRole.Player)]
    [InlineData(TeamRole.Coach)]
    public async Task TeamScope_WrongTeamFailsBeforeRoleCheckAndWrite(TeamRole role)
    {
        var fake = new FakeRosterPersistence(CreateMembership(role));
        await Assert.ThrowsAsync<RosterMembershipNotFoundException>(() => new AddPlayerToRoster(fake)
            .ExecuteForTeamAsync(Guid.NewGuid(), fake.Membership!.Id, "007", RosterStatus.Active));
        Assert.Equal(0, fake.SaveCalls);
    }

    [Fact]
    public async Task TeamScope_GeneratesIdAndReusesPlayerWorkflow()
    {
        var fake = new FakeRosterPersistence(CreateMembership(TeamRole.Player));
        var entry = await new AddPlayerToRoster(fake).ExecuteForTeamAsync(
            fake.Membership!.TeamId, fake.Membership.Id, "007", RosterStatus.Suspended);
        Assert.NotEqual(Guid.Empty, entry.Id);
        Assert.Equal("007", entry.JerseyNumber);
        Assert.Equal(RosterStatus.Suspended, entry.RosterStatus);
        Assert.Same(entry, fake.SavedEntry);
        Assert.Equal(1, fake.SaveCalls);
    }

    [Fact]
    public async Task TeamScope_EmptyTeamRejectedBeforeLookup()
    {
        var fake = new FakeRosterPersistence(CreateMembership(TeamRole.Player));
        var error = await Assert.ThrowsAsync<RosterValidationException>(() => new AddPlayerToRoster(fake)
            .ExecuteForTeamAsync(Guid.Empty, fake.Membership!.Id, null, RosterStatus.Active));
        Assert.Equal("teamId", error.Field);
        Assert.Equal(Guid.Empty, fake.RequestedMembershipId);
        Assert.Equal(0, fake.SaveCalls);
    }

    [Fact]
    public async Task TeamScope_CancelledRequestDoesNotAccessPersistence()
    {
        var fake = new FakeRosterPersistence(CreateMembership(TeamRole.Player));
        var token = new CancellationToken(canceled: true);
        var error = await Assert.ThrowsAsync<OperationCanceledException>(() => new AddPlayerToRoster(fake)
            .ExecuteForTeamAsync(fake.Membership!.TeamId, fake.Membership.Id, null, RosterStatus.Active, token));
        Assert.Equal(token, error.CancellationToken);
        Assert.Equal(Guid.Empty, fake.RequestedMembershipId);
        Assert.Equal(0, fake.SaveCalls);
    }

    private static TeamMembership CreateMembership(TeamRole role)
        => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), role);

    private sealed class FakeRosterPersistence(TeamMembership? membership) : IRosterPersistence
    {
        public TeamMembership? Membership { get; } = membership;
        public Guid RequestedMembershipId { get; private set; }
        public RosterEntry? SavedEntry { get; private set; }
        public int SaveCalls { get; private set; }
        public Exception? SaveException { get; set; }
        public CancellationToken LoadToken { get; private set; }
        public CancellationToken SaveToken { get; private set; }

        public Task<TeamMembership?> GetMembershipAsync(Guid membershipId, CancellationToken cancellationToken = default)
        {
            RequestedMembershipId = membershipId;
            LoadToken = cancellationToken;
            return Task.FromResult(Membership);
        }

        public Task AddEntryAsync(RosterEntry entry, CancellationToken cancellationToken = default)
        {
            SaveCalls++;
            SaveToken = cancellationToken;
            if (SaveException is not null)
            {
                throw SaveException;
            }

            SavedEntry = entry;
            return Task.CompletedTask;
        }
    }
}
