using SquadSync.Application.Teams;
using Xunit;

namespace SquadSync.UnitTests;

public sealed class GetTeamsTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetAll_ReturnsQueryResultsAndForwardsCancellation(bool populated)
    {
        IReadOnlyList<TeamDetails> teams = populated ? [new(Guid.NewGuid(), "SquadSync FC")] : [];
        var queries = new Queries(teams);
        using var source = new CancellationTokenSource();

        var result = await new GetTeams(queries).ExecuteAsync(source.Token);

        Assert.Same(teams, result);
        Assert.Equal(source.Token, queries.Token);
    }

    private sealed class Queries(IReadOnlyList<TeamDetails> teams) : ITeamQueries
    {
        public CancellationToken Token { get; private set; }

        public Task<IReadOnlyList<TeamDetails>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            Token = cancellationToken;
            return Task.FromResult(teams);
        }
    }
}
