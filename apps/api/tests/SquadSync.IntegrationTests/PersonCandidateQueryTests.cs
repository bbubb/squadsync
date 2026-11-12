using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SquadSync.Application.People;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class PersonCandidateQueryTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task NpgsqlQuery_FiltersOrdersProjectsAndBoundsBeforeMaterialization(int continuation)
    {
        var capture = new CaptureCommand();
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql("Host=localhost;Database=unused").AddInterceptors(new SuppressConnection(), capture).Options);
        using var source = new CancellationTokenSource();
        var position = new PersonCandidatePosition(continuation == 2 ? null : "player", continuation == 2 ? null : "alex", Guid.NewGuid());
        Assert.Empty(await new EfPersonCandidateQueries(context).SearchAsync("Al%_\\", 21, continuation != 0 ? position : null, source.Token));
        Assert.Contains("ILIKE", capture.Sql);
        Assert.Contains("p.\"FirstName\" || ' ' || p.\"LastName\"", capture.Sql);
        Assert.Contains("ORDER BY lower(p.\"LastName\") COLLATE \"C\", lower(p.\"FirstName\") COLLATE \"C\", p.\"Id\"", capture.Sql);
        Assert.Contains("LIMIT @", capture.Sql);
        Assert.DoesNotContain("OFFSET", capture.Sql);
        Assert.DoesNotContain("COUNT(", capture.Sql);
        Assert.Contains(21, capture.Parameters);
        Assert.Contains("%Al\\%\\_\\\\%", capture.Parameters);
        if (continuation != 0)
        {
            Assert.Contains("p.\"Id\" > @", capture.Sql);
            Assert.Contains("lower(p.\"LastName\") COLLATE \"C\" >", capture.Sql);
            Assert.Contains("lower(p.\"FirstName\") COLLATE \"C\" >", capture.Sql);
            Assert.Contains(position.PersonId, capture.Parameters);
        }
        Assert.Equal(source.Token, capture.Token);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task PostgreSql_LongNamesUseBoundedCursorWithSqlPositionLookup()
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connection), "Opt-in database tests require ConnectionStrings__SquadSync.");
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>().UseNpgsql(connection).Options);
        var marker = "Long" + Guid.NewGuid().ToString("N");
        var people = new[]
        {
            new Person(Guid.NewGuid(), marker, new string('a', 5000)),
            new Person(Guid.NewGuid(), marker, new string('b', 5000))
        };
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            context.People.AddRange(people);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var useCase = new GetPersonCandidates(new EfPersonCandidateQueries(context));
            var first = await useCase.ExecuteAsync(marker, 1);
            Assert.Equal(people[0].Id, Assert.Single(first.Items).PersonId);
            Assert.InRange(first.NextCursor!.Length, 1, GetPersonCandidates.MaximumCursorLength);
            var last = await useCase.ExecuteAsync(marker, 1, first.NextCursor);
            Assert.Equal(people[1].Id, Assert.Single(last.Items).PersonId);
            Assert.Null(last.NextCursor);
            Assert.Empty(context.ChangeTracker.Entries());
            await transaction.RollbackAsync();
        }
        Assert.False(await context.People.AnyAsync(person => person.Id == people[0].Id || person.Id == people[1].Id));
    }

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task PostgreSql_SearchesLiteralNamesAndPagesAcrossAllKeysWithoutDuplicates()
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connection), "Opt-in database tests require ConnectionStrings__SquadSync.");
        await using var services = new ServiceCollection().AddSquadSyncPersistence(connection).BuildServiceProvider();
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<SquadSyncDbContext>();
        var query = scope.ServiceProvider.GetRequiredService<IPersonCandidateQueries>();
        var useCase = new GetPersonCandidates(query);
        // Unique marker isolates the search from existing data. Fixed suffix IDs establish tie order.
        var marker = "Candidate" + Guid.NewGuid().ToString("N");
        var people = new[]
        {
            new Person(Guid.NewGuid(), marker + " Zoe", "Beta"),
            new Person(Guid.NewGuid(), marker + " Bob", "alpha"),
            new Person(Guid.NewGuid(), marker + " Amy", "Alpha"),
            new Person(Guid.NewGuid(), "Other", marker + " Zulu"),
            new Person(Guid.Parse("a0000000-0000-0000-0000-000000000002"), marker + " Amy", "ALPHA"),
            new Person(Guid.Parse("a0000000-0000-0000-0000-000000000001"), marker + " Amy", "alpha"),
            new Person(Guid.NewGuid(), marker + "%_\\", "Literal"),
            new Person(Guid.NewGuid(), "Unrelated", "Person")
        };
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            context.People.AddRange(people);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var all = await query.SearchAsync(marker.ToUpperInvariant(), 21, null);
            Assert.Equal(7, all.Count);
            var expectedIds = new[] { people[2].Id, people[4].Id, people[5].Id }.Order()
                .Concat(new[] { people[1].Id, people[0].Id, people[3].Id, people[6].Id });
            Assert.Equal(expectedIds, all.Select(row => row.Candidate.PersonId));
            Assert.Equal(people.Take(7).Select(p => p.Id).Order(), all.Select(row => row.Candidate.PersonId).Order());
            Assert.Equal(people[5].Id, all.Where(row => row.Candidate.PersonId == people[4].Id || row.Candidate.PersonId == people[5].Id).First().Candidate.PersonId);
            Assert.Equal(new PersonCandidate(people[0].Id, people[0].FirstName, people[0].LastName),
                Assert.Single(await query.SearchAsync((marker + " zOE bETA").ToUpperInvariant(), 21, null)).Candidate);
            Assert.Equal(people[3].Id, Assert.Single(await query.SearchAsync(marker + " ZULU", 21, null)).Candidate.PersonId);
            Assert.Equal(people[6].Id, Assert.Single(await query.SearchAsync(marker + "%_\\", 21, null)).Candidate.PersonId);
            Assert.Empty(await query.SearchAsync(marker + " Missing", 21, null));
            foreach (var limit in new[] { 1, 2, 3, 20 })
            {
                var collected = new List<PersonCandidate>();
                string? cursor = null;
                var pages = 0;
                do
                {
                    var page = await useCase.ExecuteAsync(marker, limit, cursor);
                    Assert.InRange(page.Items.Count, 1, limit);
                    collected.AddRange(page.Items);
                    cursor = page.NextCursor;
                    Assert.True(++pages <= 7, "Paging must terminate.");
                } while (cursor is not null);
                Assert.Equal(all.Select(row => row.Candidate), collected);
                Assert.Equal(7, collected.Select(row => row.PersonId).Distinct().Count());
            }
            Assert.Empty(context.ChangeTracker.Entries());
            await transaction.RollbackAsync();
        }
        var ids = people.Select(person => person.Id).ToArray();
        Assert.False(await context.People.AnyAsync(person => ids.Contains(person.Id)));
    }

    private sealed class SuppressConnection : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection,
            ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(InterceptionResult.Suppress());
    }

    private sealed class CaptureCommand : DbCommandInterceptor
    {
        public string Sql { get; private set; } = "";
        public object?[] Parameters { get; private set; } = [];
        public CancellationToken Token { get; private set; }
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Sql = command.CommandText;
            Parameters = command.Parameters.Cast<DbParameter>().Select(p => p.Value).ToArray();
            Token = cancellationToken;
            return ValueTask.FromResult(InterceptionResult<DbDataReader>.SuppressWithResult(new DataTable().CreateDataReader()));
        }
    }
}
