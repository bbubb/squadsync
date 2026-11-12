using SquadSync.Application.People;
using Xunit;

namespace SquadSync.UnitTests;

public class GetPersonCandidatesTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    [InlineData(" a ")]
    public async Task InvalidQuery_DoesNotQuery(string? query)
    {
        var fake = new Queries();
        var error = await Assert.ThrowsAsync<PersonCandidateValidationException>(() => new GetPersonCandidates(fake).ExecuteAsync(query));
        Assert.Equal("q", error.Field);
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task InvalidBoundsAndCursors_DoNotQuery()
    {
        var fake = new Queries();
        var useCase = new GetPersonCandidates(fake);
        await Assert.ThrowsAsync<PersonCandidateValidationException>(() => useCase.ExecuteAsync(new string('x', 101)));
        foreach (var limit in new[] { 0, -1, 21, int.MaxValue })
            Assert.Equal("limit", (await Assert.ThrowsAsync<PersonCandidateValidationException>(() => useCase.ExecuteAsync("Alex", limit))).Field);
        foreach (var cursor in new[] { "", " ", "!", "a", "bnVsbA", "e30", new string('a', 4097), "eyJWZXJzaW9uIjoyfQ" })
            Assert.Equal("cursor", (await Assert.ThrowsAsync<PersonCandidateValidationException>(() => useCase.ExecuteAsync("Alex", cursor: cursor))).Field);
        Assert.Equal(0, fake.Calls);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(100)]
    public async Task QueryBoundariesTrimAndDefaultLimit_WithEmptyPage(int length)
    {
        var fake = new Queries();
        using var source = new CancellationTokenSource();
        var result = await new GetPersonCandidates(fake).ExecuteAsync("  " + new string('x', length) + " \t", cancellationToken: source.Token);
        Assert.Empty(result.Items);
        Assert.Null(result.NextCursor);
        Assert.Equal(new string('x', length), fake.Query);
        Assert.Equal(21, fake.Take);
        Assert.Null(fake.After);
        Assert.Equal(source.Token, fake.Token);
    }

    [Fact]
    public async Task CursorUsesLastReturnedDatabaseKeysAndBindsTrimmedQuery()
    {
        var first = new PersonCandidateRow(new(Guid.NewGuid(), "Alex", "PLAYER"), new("player", "alex", Guid.NewGuid()));
        var second = new PersonCandidateRow(new(Guid.NewGuid(), "Alex", "Player"), new("player", "alex", Guid.NewGuid()));
        var fake = new Queries { Rows = [first, second] };
        var useCase = new GetPersonCandidates(fake);
        var page = await useCase.ExecuteAsync(" Alex ", 1);
        Assert.Equal(first.Candidate, Assert.Single(page.Items));
        Assert.NotNull(page.NextCursor);
        Assert.Equal(2, fake.Take);
        var before = fake.Calls;
        await Assert.ThrowsAsync<PersonCandidateValidationException>(() => useCase.ExecuteAsync("Other", 1, page.NextCursor));
        Assert.Equal(before, fake.Calls);
        fake.Rows = [second];
        var last = await useCase.ExecuteAsync("Alex", 1, page.NextCursor);
        Assert.Equal(first.Position, fake.After);
        Assert.Equal(second.Candidate, Assert.Single(last.Items));
        Assert.Null(last.NextCursor);
    }

    [Fact]
    public async Task UnboundedPersonNames_StillProduceBoundedRoundTrippableCursor()
    {
        var id = Guid.NewGuid();
        var row = new PersonCandidateRow(new(id, "Alex", new string('x', 5000)), new(new string('x', 5000), "alex", id));
        var fake = new Queries { Rows = [row, row] };
        var useCase = new GetPersonCandidates(fake);
        var page = await useCase.ExecuteAsync("Alex", 1);
        Assert.InRange(page.NextCursor!.Length, 1, GetPersonCandidates.MaximumCursorLength);
        await useCase.ExecuteAsync("Alex", 1, page.NextCursor);
        Assert.Equal(new PersonCandidatePosition(null, null, id), fake.After);
    }

    [Fact]
    public async Task CancellationAndQueryFailures_Propagate()
    {
        var fake = new Queries();
        var useCase = new GetPersonCandidates(fake);
        var token = new CancellationToken(true);
        var cancelled = await Assert.ThrowsAsync<OperationCanceledException>(() => useCase.ExecuteAsync("Alex", cancellationToken: token));
        Assert.Equal(token, cancelled.CancellationToken);
        Assert.Equal(0, fake.Calls);
        foreach (var failure in new Exception[] { new InvalidOperationException("provider"), new OperationCanceledException() })
        {
            fake.Failure = failure;
            Assert.Same(failure, await Record.ExceptionAsync(() => useCase.ExecuteAsync("Alex")));
        }
    }

    private sealed class Queries : IPersonCandidateQueries
    {
        public IReadOnlyList<PersonCandidateRow> Rows { get; set; } = [];
        public int Calls { get; private set; }
        public string? Query { get; private set; }
        public int Take { get; private set; }
        public PersonCandidatePosition? After { get; private set; }
        public CancellationToken Token { get; private set; }
        public Exception? Failure { get; set; }

        public Task<IReadOnlyList<PersonCandidateRow>> SearchAsync(string query, int take,
            PersonCandidatePosition? after, CancellationToken cancellationToken = default)
        {
            Calls++;
            Query = query;
            Take = take;
            After = after;
            Token = cancellationToken;
            if (Failure is not null) throw Failure;
            return Task.FromResult(Rows);
        }
    }
}
