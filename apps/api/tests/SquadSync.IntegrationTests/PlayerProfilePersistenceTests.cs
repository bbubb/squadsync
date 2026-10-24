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

    [Fact]
    public async Task GetForUpdateAsync_ReturnsTrackedPlayerProfile()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var reader = new ProfileReader(id, personId, "Both", entityShape: true);

        await using var context = new SquadSyncDbContext(
            new DbContextOptionsBuilder<SquadSyncDbContext>()
                .UseNpgsql("Host=localhost;Database=unused")
                .AddInterceptors(new SuppressConnection(), reader)
                .Options);

        var adapter = new EfPlayerProfilePersistence(context);

        var result = await adapter.GetForUpdateAsync(personId);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(personId, result.PersonId);
        Assert.Equal(DominantFoot.Both, result.DominantFoot);

        Assert.Equal(EntityState.Unchanged, context.Entry(result).State);
    }

    [Fact]
    public async Task GetForUpdateAsync_ReturnsNullWhenProfileIsMissing()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var reader = new ProfileReader(id, personId, null, empty: true);

        await using var context = new SquadSyncDbContext(
            new DbContextOptionsBuilder<SquadSyncDbContext>()
                .UseNpgsql("Host=localhost;Database=unused")
                .AddInterceptors(new SuppressConnection(), reader)
                .Options);

        var adapter = new EfPlayerProfilePersistence(context);

        var result = await adapter.GetForUpdateAsync(Guid.NewGuid());

        Assert.Null(result);
        Assert.Empty(context.ChangeTracker.Entries<PlayerProfile>());
    }

    [DatabaseFact]
    public async Task UpdateAsync_PersistsChangesAndPreservesIdentity()
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connection), "Opt-in database tests require ConnectionStrings_SquadSync");
        await using var context = new SquadSyncDbContext(
            new DbContextOptionsBuilder<SquadSyncDbContext>().UseNpgsql(connection).Options);
        var person = new Person(Guid.NewGuid(), "Alex", "ProfileUpdate");
        var id = Guid.NewGuid();
        var expectedPersonId = person.Id;
        var profile = new PlayerProfile(id, person.Id, DominantFoot.Left, 70, 150.5m);

        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            context.People.Add(person);
            context.PlayerProfiles.Add(profile);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Load Existing Tracked Profile, Update/Modify, Save, Reload, Test
            var adapter = new EfPlayerProfilePersistence(context);
            var trackedProfile = await adapter.GetForUpdateAsync(person.Id);
            Assert.NotNull(trackedProfile);

            trackedProfile.Update(DominantFoot.Right, 75, 165.25m);
            await adapter.UpdateAsync(trackedProfile);
            context.ChangeTracker.Clear();

            var expected = new PlayerProfile(id, expectedPersonId, DominantFoot.Right, 75, 165.25m);
            var actual = await adapter.GetForUpdateAsync(person.Id);
            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.PersonId, actual.PersonId);
            Assert.Equal(expected.DominantFoot, actual.DominantFoot);
            Assert.Equal(expected.HeightInches, actual.HeightInches);
            Assert.Equal(expected.WeightPounds, actual.WeightPounds);
            await transaction.RollbackAsync();
        }
        context.ChangeTracker.Clear();
        Assert.False(await context.People.AnyAsync(entry => entry.Id == person.Id));
        Assert.False(await context.PlayerProfiles.AnyAsync(entry => entry.Id == profile.Id));
    }

    [Fact]
    public async Task UpdateAsync_PropagatesCancellation()
    {
        using var source = new CancellationTokenSource();
        var failure = new OperationCanceledException(source.Token);
        var interceptor = new FailingSave(failure);

        await using var context = CreateContext(interceptor);

        var profile = new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), DominantFoot.Left, 70, 150.5m);

        context.PlayerProfiles.Attach(profile);
        profile.Update(DominantFoot.Right, 72, 175.25m);

        Assert.Same(failure, await Assert.ThrowsAsync<OperationCanceledException>(
            () => new EfPlayerProfilePersistence(context)
            .UpdateAsync(new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), DominantFoot.Both, 70, 150.5m),
                source.Token)));
        Assert.Equal(source.Token, interceptor.Token);
    }

    [Fact]
    public async Task UpdateAsync_PropagatesUnexpectedSaveFailure()
    {
        var failure = new DbUpdateException(
            "private-provider-detail",
            new InvalidOperationException());

        await using var context = CreateContext(new FailingSave(failure));

        var profile = new PlayerProfile(
            Guid.NewGuid(), Guid.NewGuid(), DominantFoot.Left, 70, 150.5m);

        context.PlayerProfiles.Attach(profile);
        profile.Update(DominantFoot.Right, 72, 175.25m);

        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => new EfPlayerProfilePersistence(context).UpdateAsync(profile));

        Assert.Same(failure, exception);
    }

    private sealed class SuppressConnection : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection,
            ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(InterceptionResult.Suppress());
    }

    // Return provider-shaped rows through the real EF/Npgsql query pipeline without a socket.
    private sealed class ProfileReader(
        Guid id,
        Guid personId,
        string? foot,
        bool empty = false,
        bool entityShape = false) : DbCommandInterceptor
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

            if (entityShape)
            {
                // Order used by the tracked-entity query
                table.Columns.Add("DominantFoot", typeof(string));
                table.Columns.Add("HeightInches", typeof(int));
                table.Columns.Add("PersonId", typeof(Guid));
            }
            else
            {
                // Order used by the existing DTO projection
                table.Columns.Add("PersonId", typeof(Guid));
                table.Columns.Add("DominantFoot", typeof(string));
                table.Columns.Add("HeightInches", typeof(int));
            }

            table.Columns.Add("WeightPounds", typeof(decimal));

            if (!empty)
            {
                var storedFoot = (object?)foot ?? DBNull.Value;
                var height = foot is null ? (object)DBNull.Value : 70;
                var weight = foot is null ? (object)DBNull.Value : 152.5123m;

                if (entityShape)
                    table.Rows.Add(id, storedFoot, height, personId, weight);
                else
                    table.Rows.Add(id, personId, storedFoot, height, weight);
            }

            return ValueTask.FromResult(
                InterceptionResult<DbDataReader>.SuppressWithResult(
                    table.CreateDataReader()));
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
