using SquadSync.Application.Teams;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class TeamUseCaseTests
{
    [Fact]
    public async Task Create_GeneratesDistinctIdsNormalizesNameAndPersistsWithCancellation()
    {
        var persistence = new FakePersistence();
        var useCase = new CreateTeam(persistence);
        using var cancellation = new CancellationTokenSource();

        var first = await useCase.ExecuteAsync("  SquadSync FC\t", cancellation.Token);
        Assert.Same(first, persistence.Saved);
        Assert.Equal(cancellation.Token, persistence.Token);
        var second = await useCase.ExecuteAsync("SquadSync FC");

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
        Assert.Equal("SquadSync FC", first.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public async Task Create_InvalidNamesTranslateDomainFailureBeforePersistence(string? name)
    {
        var persistence = new FakePersistence();

        var exception = await Assert.ThrowsAsync<TeamValidationException>(() =>
            new CreateTeam(persistence).ExecuteAsync(name!));

        Assert.Equal("name", exception.Field);
        Assert.IsAssignableFrom<ArgumentException>(exception.InnerException);
        Assert.Null(persistence.Saved);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Get_ReturnsPersistenceResultAndForwardsIdAndCancellation(bool exists)
    {
        var id = Guid.NewGuid();
        var persistence = new FakePersistence { Saved = exists ? new Team(id, "SquadSync FC") : null };
        using var cancellation = new CancellationTokenSource();

        var result = await new GetTeam(persistence).ExecuteAsync(id, cancellation.Token);

        Assert.Same(persistence.Saved, result);
        Assert.Equal(id, persistence.RequestedId);
        Assert.Equal(cancellation.Token, persistence.Token);
    }

    [Fact]
    public async Task PersistenceFailure_IsNotMisclassifiedAsInputValidation()
    {
        var failure = new ArgumentException("Persistence failure", "name");
        var persistence = new FakePersistence { Failure = failure };

        Assert.Same(failure, await Assert.ThrowsAsync<ArgumentException>(() =>
            new CreateTeam(persistence).ExecuteAsync("SquadSync FC")));
    }

    private sealed class FakePersistence : ITeamPersistence
    {
        public Team? Saved { get; set; }
        public Guid RequestedId { get; private set; }
        public CancellationToken Token { get; private set; }
        public Exception? Failure { get; init; }

        public Task AddAsync(Team team, CancellationToken cancellationToken = default)
        {
            if (Failure is not null) throw Failure;
            Saved = team;
            Token = cancellationToken;
            return Task.CompletedTask;
        }

        public Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            RequestedId = id;
            Token = cancellationToken;
            return Task.FromResult(Saved);
        }
    }
}
