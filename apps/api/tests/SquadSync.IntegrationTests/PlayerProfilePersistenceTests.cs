using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SquadSync.Application.PlayerProfiles;
using SquadSync.Domain;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class PlayerProfilePersistenceTests
{
    [Fact]
    public void MappingAndRegistration_PreserveExistingProfileSchema()
    {
        var services = new ServiceCollection().AddSquadSyncPersistence("Host=localhost;Database=unused");
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.IsType<EfPlayerProfilePersistence>(scope.ServiceProvider.GetRequiredService<IPlayerProfilePersistence>());
        var context = scope.ServiceProvider.GetRequiredService<SquadSyncDbContext>();
        var entity = context.Model.FindEntityType(typeof(PlayerProfile))!;
        var index = Assert.Single(entity.GetIndexes(), candidate => candidate.IsUnique);
        Assert.Equal(new[] { "PersonId" }, index.Properties.Select(property => property.Name));
        Assert.Equal("IX_PlayerProfiles_PersonId", index.GetDatabaseName());
        Assert.Equal(typeof(string), entity.FindProperty("DominantFoot")!.GetTypeMapping().Converter!.ProviderClrType);
        Assert.All(entity.GetForeignKeys(), key => Assert.Equal(DeleteBehavior.Restrict, key.DeleteBehavior));
    }

    [Theory]
    [InlineData(PostgresErrorCodes.UniqueViolation, "IX_PlayerProfiles_PersonId", true)]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation, "FK_PlayerProfiles_People_PersonId", false)]
    public async Task KnownConstraintRaces_AreTranslatedAndFailedInsertDetached(string code, string constraint, bool conflict)
    {
        var failure = DatabaseFailure(code, constraint);
        var interceptor = new FailingSave(failure);
        await using var context = CreateContext(interceptor);
        var profile = new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), null, null, null);
        var actual = await Assert.ThrowsAnyAsync<Exception>(() => new EfPlayerProfilePersistence(context).AddAsync(profile));
        if (conflict) Assert.IsType<PlayerProfileConflictException>(actual);
        else Assert.IsType<PlayerProfilePersonNotFoundException>(actual);
        Assert.Same(failure, actual.InnerException);
        Assert.Equal(EntityState.Detached, context.Entry(profile).State);
        Assert.DoesNotContain("private-provider-detail", actual.Message);
    }

    [Theory]
    [InlineData(PostgresErrorCodes.UniqueViolation, "PK_PlayerProfiles")]
    [InlineData(PostgresErrorCodes.UniqueViolation, "unrelated_index")]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation, "unrelated_fk")]
    [InlineData(PostgresErrorCodes.CheckViolation, "IX_PlayerProfiles_PersonId")]
    public async Task UnrelatedDatabaseFailures_PropagateUnchanged(string code, string constraint)
    {
        var failure = DatabaseFailure(code, constraint);
        await using var context = CreateContext(new FailingSave(failure));
        Assert.Same(failure, await Assert.ThrowsAsync<DbUpdateException>(() => new EfPlayerProfilePersistence(context)
            .AddAsync(new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), null, null, null))));
    }

    [Fact]
    public async Task SaveCancellation_PropagatesWithOriginalToken()
    {
        using var source = new CancellationTokenSource();
        var failure = new OperationCanceledException(source.Token);
        var interceptor = new FailingSave(failure);
        await using var context = CreateContext(interceptor);
        Assert.Same(failure, await Assert.ThrowsAsync<OperationCanceledException>(() => new EfPlayerProfilePersistence(context)
            .AddAsync(new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), null, null, null), source.Token)));
        Assert.Equal(source.Token, interceptor.Token);
    }

    [Fact]
    public async Task NonPostgresFailure_PropagatesUnchanged()
    {
        var failure = new DbUpdateException("private-provider-detail", new InvalidOperationException());
        await using var context = CreateContext(new FailingSave(failure));
        Assert.Same(failure, await Assert.ThrowsAsync<DbUpdateException>(() => new EfPlayerProfilePersistence(context)
            .AddAsync(new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), null, null, null))));
    }

    [Theory]
    [InlineData("Left")]
    [InlineData("Right")]
    [InlineData("Both")]
    [InlineData(null)]
    public async Task NpgsqlProjection_FiltersByPersonAndMaterializesNullableValuesWithoutTracking(string? foot)
    {
        var personId = Guid.NewGuid();
        var id = Guid.NewGuid();
        var capture = new ProfileReader(id, personId, foot);
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql("Host=localhost;Database=unused")
            .AddInterceptors(new SuppressConnection(), capture).Options);
        using var source = new CancellationTokenSource();
        var result = await new EfPlayerProfilePersistence(context).GetByPersonIdAsync(personId, source.Token);
        Assert.Equal(new PlayerProfileDetails(id, personId,
            foot is null ? null : Enum.Parse<DominantFoot>(foot), foot is null ? null : 70, foot is null ? null : 152.5123m), result);
        Assert.Contains("FROM \"PlayerProfiles\" AS p", capture.Sql);
        Assert.Contains("WHERE p.\"PersonId\" = @", capture.Sql);
        Assert.Equal(personId, Assert.Single(capture.Parameters));
        Assert.Equal(source.Token, capture.Token);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task MissingProfileProjection_ReturnsNull()
    {
        var capture = new ProfileReader(Guid.NewGuid(), Guid.NewGuid(), null, empty: true);
        await using var context = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql("Host=localhost;Database=unused")
            .AddInterceptors(new SuppressConnection(), capture).Options);
        Assert.Null(await new EfPlayerProfilePersistence(context).GetByPersonIdAsync(Guid.NewGuid()));
    }

    private sealed class SuppressConnection : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection,
            ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(InterceptionResult.Suppress());
    }

    // Return provider-shaped rows through the real EF/Npgsql query pipeline without a socket.
    private sealed class ProfileReader(Guid id, Guid personId, string? foot, bool empty = false) : DbCommandInterceptor
    {
        public string Sql { get; private set; } = "";
        public object?[] Parameters { get; private set; } = [];
        public CancellationToken Token { get; private set; }
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Sql = command.CommandText;
            Parameters = command.Parameters.Cast<DbParameter>().Select(parameter => parameter.Value).ToArray();
            Token = cancellationToken;
            var table = new DataTable();
            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("PersonId", typeof(Guid));
            table.Columns.Add("DominantFoot", typeof(string));
            table.Columns.Add("HeightInches", typeof(int));
            table.Columns.Add("WeightPounds", typeof(decimal));
            if (!empty) table.Rows.Add(id, personId, (object?)foot ?? DBNull.Value,
                foot is null ? DBNull.Value : 70, foot is null ? DBNull.Value : 152.5123m);
            return ValueTask.FromResult(InterceptionResult<DbDataReader>.SuppressWithResult(table.CreateDataReader()));
        }
    }

    private static DbUpdateException DatabaseFailure(string code, string constraint)
        => new("private-provider-detail", new PostgresException("private-provider-detail", "ERROR", "ERROR", code, constraintName: constraint));

    private static SquadSyncDbContext CreateContext(FailingSave interceptor)
        => new(new DbContextOptionsBuilder<SquadSyncDbContext>().UseNpgsql("Host=localhost;Database=unused")
            .AddInterceptors(interceptor).Options);

    // Exercise the real adapter's SaveChanges exception filters without opening a database connection.
    private sealed class FailingSave(Exception failure) : SaveChangesInterceptor
    {
        public CancellationToken Token { get; private set; }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Token = cancellationToken;
            throw failure;
        }
    }
}
