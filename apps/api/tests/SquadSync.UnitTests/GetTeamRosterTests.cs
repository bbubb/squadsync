using SquadSync.Application.Rosters;
using SquadSync.Application.Teams;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class GetTeamRosterTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ExistingTeam_ReturnsQueryResultsAndPassesScopeAndToken(bool populated)
    {
        var fake = new Persistence();
        if (populated)
            fake.Results = [new(Guid.NewGuid(), Guid.NewGuid(), new(Guid.NewGuid(), "Alex", "Player"), "007", RosterStatus.Reserved)];
        using var source = new CancellationTokenSource();
        var result = await new GetTeamRoster(fake, fake).ExecuteAsync(fake.Team.Id, source.Token);
        Assert.Same(fake.Results, result);
        Assert.Equal(new[] { fake.Team.Id, fake.Team.Id }, fake.Ids);
        Assert.Equal(2, fake.Tokens.Count);
        Assert.All(fake.Tokens, token => Assert.Equal(source.Token, token));
    }

    [Fact]
    public async Task MissingTeam_StopsBeforeRosterQuery()
    {
        var fake = new Persistence();
        var id = Guid.NewGuid();
        await Assert.ThrowsAsync<RosterTeamNotFoundException>(() => new GetTeamRoster(fake, fake).ExecuteAsync(id));
        Assert.Equal(new[] { id }, fake.Ids);
    }

    [Fact]
    public async Task EmptyTeamId_IsRejectedBeforePersistence()
    {
        var fake = new Persistence();
        var error = await Assert.ThrowsAsync<RosterValidationException>(() => new GetTeamRoster(fake, fake).ExecuteAsync(Guid.Empty));
        Assert.Equal("teamId", error.Field);
        Assert.Empty(fake.Ids);
    }

    [Fact]
    public async Task AlreadyCancelled_DoesNotAccessPersistence()
    {
        var fake = new Persistence();
        var token = new CancellationToken(canceled: true);
        var error = await Assert.ThrowsAsync<OperationCanceledException>(() => new GetTeamRoster(fake, fake).ExecuteAsync(fake.Team.Id, token));
        Assert.Equal(token, error.CancellationToken);
        Assert.Empty(fake.Ids);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PersistenceFailuresAndCancellation_PropagateUnchanged(bool teamQuery)
    {
        foreach (var failure in new Exception[] { new InvalidOperationException("provider detail"), new OperationCanceledException() })
        {
            var fake = new Persistence { Failure = failure, FailTeamQuery = teamQuery };
            var actual = await Record.ExceptionAsync(() => new GetTeamRoster(fake, fake).ExecuteAsync(fake.Team.Id));
            Assert.Same(failure, actual);
            Assert.Equal(teamQuery ? 1 : 2, fake.Ids.Count);
        }
    }

    private sealed class Persistence : ITeamPersistence, IRosterQueries
    {
        public Team Team { get; } = new(Guid.NewGuid(), "Test FC");
        public IReadOnlyList<RosterDetails> Results { get; set; } = [];
        public List<Guid> Ids { get; } = [];
        public List<CancellationToken> Tokens { get; } = [];
        public Exception? Failure { get; init; }
        public bool FailTeamQuery { get; init; }

        public Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Ids.Add(id);
            Tokens.Add(cancellationToken);
            if (FailTeamQuery && Failure is not null) throw Failure;
            return Task.FromResult(id == Team.Id ? Team : null);
        }

        public Task<IReadOnlyList<RosterDetails>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            Ids.Add(teamId);
            Tokens.Add(cancellationToken);
            if (Failure is not null) throw Failure;
            return Task.FromResult(Results);
        }

        public Task AddAsync(Team team, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<RosterDetails?> GetByIdAsync(Guid teamId, Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
