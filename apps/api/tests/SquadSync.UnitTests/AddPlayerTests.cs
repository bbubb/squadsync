using SquadSync.Application.Rosters;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public sealed class AddPlayerTests
{
    [Theory]
    [InlineData("variants")]
    [InlineData("both")]
    [InlineData("personId")]
    [InlineData("teamId")]
    [InlineData("firstName")]
    [InlineData("lastName")]
    [InlineData("jerseyNumber")]
    [InlineData("rosterStatus")]
    public async Task InvalidInput_NeverBeginsTransaction(string invalid)
    {
        var store = new Store();
        var personId = invalid is "variants" or "firstName" or "lastName" ? (Guid?)null : Guid.NewGuid();
        NewPlayerPerson? person = invalid switch
        {
            "both" => new("Alex", "Player"),
            "firstName" => new(" ", "Player"),
            "lastName" => new("Alex", " "),
            _ => null
        };
        if (invalid == "personId") personId = Guid.Empty;
        await Assert.ThrowsAsync<RosterValidationException>(() => new AddPlayer(store).ExecuteAsync(
            invalid == "teamId" ? Guid.Empty : Guid.NewGuid(), personId, person,
            invalid == "jerseyNumber" ? "??" : "007", invalid == "rosterStatus" ? (RosterStatus)99 : RosterStatus.Active));
        Assert.Equal(0, store.Begins);
    }

    [Fact]
    public async Task NewPerson_IsNormalizedAndCreatesPlayerMembership()
    {
        var store = new Store();
        var team = Guid.NewGuid();
        var result = await new AddPlayer(store).ExecuteAsync(team, null, new(" Alex ", " Player "), "007", RosterStatus.Reserved);
        Assert.Equal("Alex", store.Person!.FirstName);
        Assert.Equal("Player", store.Person.LastName);
        Assert.Equal(store.Person.Id, store.Membership!.PersonId);
        Assert.Equal(team, store.Membership.TeamId);
        Assert.Equal(TeamRole.Player, store.Membership.TeamRole);
        Assert.Equal(store.Membership.Id, result.TeamMembershipId);
        Assert.Equal("007", result.JerseyNumber);
        Assert.True(store.Committed);
        Assert.True(store.Disposed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExistingPerson_CreatesOrReusesPlayerMembership(bool reuse)
    {
        var store = new Store();
        var person = new Person(Guid.NewGuid(), "Existing", "Player");
        var team = Guid.NewGuid();
        var membership = reuse ? new TeamMembership(Guid.NewGuid(), person.Id, team, TeamRole.Player) : null;
        store.ExistingPerson = person;
        store.ExistingMembership = membership;
        var result = await new AddPlayer(store).ExecuteAsync(team, person.Id, null, null, RosterStatus.Active);
        Assert.Null(store.Person);
        Assert.Equal(person.Id, result.Person.Id);
        Assert.Equal(reuse, store.Membership is null);
        if (reuse) Assert.Equal(membership!.Id, result.TeamMembershipId);
        Assert.True(store.Committed);
    }

    [Theory]
    [InlineData(TeamRole.Owner)]
    [InlineData(TeamRole.Coach)]
    [InlineData(TeamRole.AssistantCoach)]
    [InlineData(TeamRole.Manager)]
    [InlineData(TeamRole.Viewer)]
    public async Task StaffMembership_IsNeverChanged(TeamRole role)
    {
        var store = new Store { ExistingPerson = new(Guid.NewGuid(), "Staff", "Person") };
        store.ExistingMembership = new(Guid.NewGuid(), store.ExistingPerson.Id, Guid.NewGuid(), role);
        await Assert.ThrowsAsync<RosterMembershipNotPlayerException>(() => new AddPlayer(store).ExecuteAsync(
            store.ExistingMembership.TeamId, store.ExistingPerson.Id, null, null, RosterStatus.Active));
        Assert.Equal(role, store.ExistingMembership.TeamRole);
        Assert.Null(store.Entry);
        Assert.False(store.Committed);
        Assert.True(store.Disposed);
    }

    [Fact]
    public async Task MissingReferences_DisposeWithoutSaving()
    {
        var store = new Store { TeamExists = false };
        await Assert.ThrowsAsync<RosterTeamNotFoundException>(() => new AddPlayer(store).ExecuteAsync(
            Guid.NewGuid(), null, new("Alex", "Player"), null, RosterStatus.Active));
        store.TeamExists = true;
        await Assert.ThrowsAsync<AddPlayerPersonNotFoundException>(() => new AddPlayer(store).ExecuteAsync(
            Guid.NewGuid(), Guid.NewGuid(), null, null, RosterStatus.Active));
        Assert.Null(store.Entry);
        Assert.False(store.Committed);
    }

    [Fact]
    public async Task PersistenceFailure_PropagatesWithoutCommitOrRetry()
    {
        var failure = new InvalidOperationException("unrelated");
        var store = new Store { Failure = failure };
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => new AddPlayer(store).ExecuteAsync(
            Guid.NewGuid(), null, new("Alex", "Player"), null, RosterStatus.Active)));
        Assert.Equal(1, store.Begins);
        Assert.False(store.Committed);
        Assert.True(store.Disposed);
    }

    [Fact]
    public async Task Cancellation_PropagatesBeforeTransaction()
    {
        var store = new Store();
        await Assert.ThrowsAsync<OperationCanceledException>(() => new AddPlayer(store).ExecuteAsync(
            Guid.NewGuid(), null, new("Alex", "Player"), null, RosterStatus.Active, new CancellationToken(true)));
        Assert.Equal(0, store.Begins);
    }

    private sealed class Store : IAddPlayerPersistence, IAddPlayerTransaction
    {
        public int Begins;
        public bool TeamExists = true, Committed, Disposed;
        public Person? ExistingPerson, Person;
        public TeamMembership? ExistingMembership, Membership;
        public RosterEntry? Entry;
        public Exception? Failure;
        public Task<IAddPlayerTransaction> BeginAsync(CancellationToken cancellationToken) { Begins++; return Task.FromResult<IAddPlayerTransaction>(this); }
        public Task<bool> TeamExistsAsync(Guid id, CancellationToken token) => Task.FromResult(TeamExists);
        public Task<Person?> GetPersonAsync(Guid id, CancellationToken token) => Task.FromResult(ExistingPerson);
        public Task<TeamMembership?> GetMembershipAsync(Guid team, Guid person, CancellationToken token) => Task.FromResult(ExistingMembership);
        public Task SaveAsync(Person? person, TeamMembership? membership, RosterEntry entry, CancellationToken token)
        {
            if (Failure is not null) throw Failure;
            Person = person; Membership = membership; Entry = entry;
            return Task.CompletedTask;
        }
        public Task CommitAsync(CancellationToken token) { Committed = true; return Task.CompletedTask; }
        public ValueTask DisposeAsync() { Disposed = true; return ValueTask.CompletedTask; }
    }
}
