using SquadSync.Application.Memberships;
using SquadSync.Application.People;
using SquadSync.Application.Teams;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class MembershipUseCaseTests
{
    [Theory]
    [InlineData(TeamRole.Owner)]
    [InlineData(TeamRole.Coach)]
    [InlineData(TeamRole.AssistantCoach)]
    [InlineData(TeamRole.Manager)]
    [InlineData(TeamRole.Player)]
    [InlineData(TeamRole.Viewer)]
    public async Task Create_GeneratesIdsAssociatesReferencesAndReturnsPersonSummary(TeamRole role)
    {
        var fake = new Persistence();
        using var source = new CancellationTokenSource();
        var create = new CreateMembership(fake, fake, fake);
        var first = await create.ExecuteAsync(fake.Team.Id, fake.Person.Id, role, source.Token);
        var otherReferences = new Persistence();
        var second = await new CreateMembership(otherReferences, otherReferences, otherReferences)
            .ExecuteAsync(otherReferences.Team.Id, otherReferences.Person.Id, role, source.Token);

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
        Assert.Equal(fake.Team.Id, first.TeamId);
        Assert.Equal(new MembershipPerson(fake.Person.Id, "Alex", "Player"), first.Person);
        Assert.Equal(role, first.TeamRole);
        Assert.Equal(new[] { source.Token, source.Token, source.Token }, fake.Tokens);
        Assert.Single(fake.Saved);
        Assert.All(fake.Saved, entry =>
        {
            Assert.Equal(fake.Person.Id, entry.PersonId);
            Assert.Equal(fake.Team.Id, entry.TeamId);
            Assert.Equal(role, entry.TeamRole);
        });
    }

    [Theory]
    [InlineData("teamId")]
    [InlineData("personId")]
    [InlineData("teamRole")]
    public async Task InvalidDomainInput_IsTranslatedBeforePersistence(string field)
    {
        var fake = new Persistence();
        var exception = await Assert.ThrowsAsync<MembershipValidationException>(() =>
            new CreateMembership(fake, fake, fake).ExecuteAsync(
                field == "teamId" ? Guid.Empty : fake.Team.Id,
                field == "personId" ? Guid.Empty : fake.Person.Id,
                field == "teamRole" ? (TeamRole)999 : TeamRole.Player));
        Assert.Equal(field, exception.Field);
        Assert.Empty(fake.Tokens);
        Assert.Empty(fake.Saved);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task MissingReference_DoesNotSave(bool missingTeam)
    {
        var fake = new Persistence();
        await Assert.ThrowsAsync<MembershipReferenceNotFoundException>(() =>
            new CreateMembership(fake, fake, fake).ExecuteAsync(
                missingTeam ? Guid.NewGuid() : fake.Team.Id,
                missingTeam ? fake.Person.Id : Guid.NewGuid(), TeamRole.Player));
        Assert.Empty(fake.Saved);
    }

    [Fact]
    public async Task Get_PassesTeamScopeAndCancellationAndReturnsMissing()
    {
        var fake = new Persistence();
        using var source = new CancellationTokenSource();
        var created = await new CreateMembership(fake, fake, fake)
            .ExecuteAsync(fake.Team.Id, fake.Person.Id, TeamRole.Player);
        var get = new GetMembership(fake);
        Assert.Equal(created, await get.ExecuteAsync(fake.Team.Id, created.Id, source.Token));
        Assert.Equal(source.Token, fake.Tokens.Last());
        Assert.Null(await get.ExecuteAsync(Guid.NewGuid(), created.Id));
        Assert.Null(await get.ExecuteAsync(fake.Team.Id, Guid.NewGuid()));
        await Assert.ThrowsAsync<MembershipValidationException>(() => get.ExecuteAsync(Guid.Empty, created.Id));
        await Assert.ThrowsAsync<MembershipValidationException>(() => get.ExecuteAsync(fake.Team.Id, Guid.Empty));
    }

    [Theory]
    [InlineData("team")]
    [InlineData("person")]
    [InlineData("add")]
    [InlineData("get")]
    public async Task PersistenceFailuresAndCancellation_PropagateUnchanged(string stage)
    {
        var fake = new Persistence { FailureStage = stage };
        var failures = new Exception[]
        {
            new InvalidOperationException("ordinary failure"),
            new ArgumentException("persistence validation is not input validation"),
            new MembershipConflictException(),
            new OperationCanceledException()
        };
        foreach (var failure in failures)
        {
            fake.Failure = failure;
            var actual = await Assert.ThrowsAnyAsync<Exception>(() => stage == "get"
                ? (Task)new GetMembership(fake).ExecuteAsync(fake.Team.Id, Guid.NewGuid())
                : new CreateMembership(fake, fake, fake).ExecuteAsync(fake.Team.Id, fake.Person.Id, TeamRole.Player));
            Assert.Same(failure, actual);
        }
        Assert.Empty(fake.Saved);
    }

    [Fact]
    public async Task AlreadyCancelled_CreateAndGetDoNotAccessPersistence()
    {
        var fake = new Persistence();
        using var source = new CancellationTokenSource();
        source.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new CreateMembership(fake, fake, fake)
            .ExecuteAsync(fake.Team.Id, fake.Person.Id, TeamRole.Player, source.Token));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => new GetMembership(fake)
            .ExecuteAsync(fake.Team.Id, Guid.NewGuid(), source.Token));
        Assert.Empty(fake.Tokens);
    }

    private sealed class Persistence : IMembershipPersistence, ITeamPersistence, IPersonPersistence
    {
        public Person Person { get; } = new(Guid.NewGuid(), "Alex", "Player");
        public Team Team { get; } = new(Guid.NewGuid(), "Test FC");
        public List<TeamMembership> Saved { get; } = [];
        public List<CancellationToken> Tokens { get; } = [];
        public string? FailureStage { get; init; }
        public Exception? Failure { get; set; }

        private void Visit(string stage, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Tokens.Add(token);
            if (stage == FailureStage && Failure is not null) throw Failure;
        }

        public Task AddAsync(TeamMembership membership, CancellationToken cancellationToken = default)
        {
            Visit("add", cancellationToken);
            Saved.Add(membership);
            return Task.CompletedTask;
        }

        public Task<MembershipDetails?> GetByIdAsync(Guid teamId, Guid membershipId, CancellationToken cancellationToken = default)
        {
            Visit("get", cancellationToken);
            var entry = Saved.SingleOrDefault(entry => entry.TeamId == teamId && entry.Id == membershipId);
            return Task.FromResult(entry is null ? null : new MembershipDetails(entry.Id, entry.TeamId,
                new MembershipPerson(Person.Id, Person.FirstName, Person.LastName), entry.TeamRole));
        }

        Task<Team?> ITeamPersistence.GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            Visit("team", cancellationToken);
            return Task.FromResult(id == Team.Id ? Team : null);
        }

        Task<Person?> IPersonPersistence.GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            Visit("person", cancellationToken);
            return Task.FromResult(id == Person.Id ? Person : null);
        }

        public Task AddAsync(Team team, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task AddAsync(Person person, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
