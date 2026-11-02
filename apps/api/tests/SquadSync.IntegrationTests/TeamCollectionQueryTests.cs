using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public sealed class TeamCollectionQueryTests
{
    [Fact]
    public async Task NpgsqlQuery_ProjectsOnlyIdAndNameInNameThenIdOrder()
    {
        var capture = new CaptureCommand();
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql("Host=localhost;Database=unused")
            .AddInterceptors(new SuppressConnection(), capture).Options);
        using var source = new CancellationTokenSource();

        Assert.Empty(await new EfTeamPersistence(context).GetAllAsync(source.Token));

        Assert.Equal("SELECT t.\"Id\", t.\"Name\"\nFROM \"Teams\" AS t\nORDER BY t.\"Name\", t.\"Id\"",
            capture.Sql.Replace("\r\n", "\n"));
        Assert.Equal(source.Token, capture.Token);
        Assert.Empty(context.ChangeTracker.Entries());
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
        public CancellationToken Token { get; private set; }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Sql = command.CommandText;
            Token = cancellationToken;
            return ValueTask.FromResult(InterceptionResult<DbDataReader>.SuppressWithResult(new DataTable().CreateDataReader()));
        }
    }
}
