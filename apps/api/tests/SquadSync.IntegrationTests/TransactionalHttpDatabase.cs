using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using SquadSync.Application.People;
using SquadSync.Application.Teams;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

// xUnit runs this collection separately from parallel collections, including older DB tests.
[CollectionDefinition("Transactional HTTP database", DisableParallelization = true)]
public sealed class TransactionalHttpDatabaseCollection;

internal sealed class TransactionalHttpDatabase
{
    private static readonly string[] Tables =
        ["People", "Teams", "TeamMemberships", "PlayerProfiles", "RosterEntries", "__EFMigrationsHistory"];

    internal ConcurrentQueue<Guid> ContextIds { get; } = new();

    internal async Task RunAsync(Func<HttpClient, SquadSyncDbContext, Task> action)
    {
        var configured = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(configured),
            "Database tests require ConnectionStrings__SquadSync when SQUADSYNC_RUN_DATABASE_TESTS=1.");
        // Pin the schema instead of depending on a role-specific search_path.
        var connectionString = new NpgsqlConnectionStringBuilder(configured) { SearchPath = "public" }.ConnectionString;
        await using var connection = new NpgsqlConnection(connectionString);
        await using var observer = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await observer.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var options = new DbContextOptionsBuilder<SquadSyncDbContext>().UseNpgsql(connection).Options;
        await using var db = new SquadSyncDbContext(options);
        await db.Database.UseTransactionAsync(transaction);
        string[]? before = null;
        try
        {
            await PreflightAsync(db, connection, transaction);
            // Fail promptly rather than waiting on an active developer write. Never change rows.
            await ExecuteAsync(connection, transaction, "SET LOCAL lock_timeout = '5s'");
            await ExecuteAsync(connection, transaction,
                """LOCK TABLE public."People", public."Teams", public."TeamMemberships", public."PlayerProfiles", public."RosterEntries", public."__EFMigrationsHistory" IN SHARE ROW EXCLUSIVE MODE""");
            before = await SnapshotAsync(connection, transaction);
            Assert.Equal(before, await SnapshotAsync(observer));

            await using var factory = new ApiTestFactory(connectionString: connectionString)
                .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
                {
                    // Leave the real scoped persistence ports intact. Every request gets a fresh
                    // context but shares this externally owned connection and transaction.
                    services.RemoveAll<SquadSyncDbContext>();
                    services.AddScoped(_ =>
                    {
                        var context = new SquadSyncDbContext(options);
                        context.Database.UseTransaction(transaction);
                        ContextIds.Enqueue(context.ContextId.InstanceId);
                        return context;
                    });
                }));
            using var client = factory.CreateClient();
            await using (var scope = factory.Services.CreateAsyncScope())
            {
                Assert.True(string.Equals(connectionString, scope.ServiceProvider.GetRequiredService<IConfiguration>()
                    .GetConnectionString("SquadSync"), StringComparison.Ordinal),
                    "The HTTP host must receive the opted-in database configuration.");
                Assert.IsType<EfPersonPersistence>(scope.ServiceProvider.GetRequiredService<IPersonPersistence>());
                Assert.IsType<EfTeamPersistence>(scope.ServiceProvider.GetRequiredService<ITeamPersistence>());
                var hostDb = scope.ServiceProvider.GetRequiredService<SquadSyncDbContext>();
                Assert.Same(connection, hostDb.Database.GetDbConnection());
                Assert.Same(transaction, hostDb.Database.CurrentTransaction!.GetDbTransaction());
            }
            await action(client, db);
            // Server writes must still be invisible to a different PostgreSQL connection.
            Assert.Equal(before, await SnapshotAsync(observer));
        }
        finally
        {
            // The host is disposed before reaching here, including when action/assertions fail.
            await transaction.RollbackAsync();
            if (before is not null)
            {
                // Complete-row fingerprints include demo data, relationships, and migration history.
                Assert.Equal(before, await SnapshotAsync(observer));
            }
        }
    }

    internal static Task<string[]> SnapshotAsync(SquadSyncDbContext db) => SnapshotAsync(
        (NpgsqlConnection)db.Database.GetDbConnection(),
        (NpgsqlTransaction?)db.Database.CurrentTransaction?.GetDbTransaction());

    private static async Task PreflightAsync(
        SquadSyncDbContext db, NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        string[] expectedMigrations =
        [
            "20261002142123_InitialUserTeam",
            "20261002205354_AddTeamMembership",
            "20261003212713_AddPlayerProfileAndRosterEntry",
            "20261009150000_RenameUserToPerson"
        ];
        Assert.Equal(expectedMigrations, db.Database.GetMigrations().ToArray());
        Assert.Equal(expectedMigrations, (await db.Database.GetAppliedMigrationsAsync()).ToArray());

        foreach (var entity in db.Model.GetEntityTypes())
        {
            var table = entity.GetTableName()!;
            var store = StoreObjectIdentifier.Table(table, entity.GetSchema());
            var expected = entity.GetProperties().Select(property =>
                $"{property.GetColumnName(store)}|{property.GetRelationalTypeMapping().StoreType}|{!property.IsNullable}")
                .Order().ToArray();
            await using var command = new NpgsqlCommand("""
                SELECT a.attname, format_type(a.atttypid, a.atttypmod), a.attnotnull
                FROM pg_attribute a
                JOIN pg_class c ON c.oid = a.attrelid
                JOIN pg_namespace n ON n.oid = c.relnamespace
                WHERE n.nspname = 'public' AND c.relname = @table AND c.relkind = 'r'
                    AND a.attnum > 0 AND NOT a.attisdropped
                ORDER BY a.attname
                """, connection, transaction);
            command.Parameters.AddWithValue("table", table);
            var actual = new List<string>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                actual.Add($"{reader.GetString(0)}|{reader.GetString(1)}|{reader.GetBoolean(2)}");
            }
            Assert.Equal(expected, actual.Order().ToArray());
        }
    }

    private static async Task<string[]> SnapshotAsync(NpgsqlConnection connection, NpgsqlTransaction? transaction = null)
    {
        var fingerprints = new List<string>();
        foreach (var table in Tables)
        {
            // Only fixed table identifiers are interpolated. Hash complete ordered rows to avoid
            // printing developer data if an integrity assertion fails.
            var order = table == "__EFMigrationsHistory" ? "MigrationId" : "Id";
            await using var command = new NpgsqlCommand(
                $"SELECT COALESCE(jsonb_agg(to_jsonb(t) ORDER BY t.\"{order}\"), '[]'::jsonb)::text FROM public.\"{table}\" t",
                connection, transaction);
            var rows = (string)(await command.ExecuteScalarAsync())!;
            fingerprints.Add(table + ":" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rows))));
        }
        return fingerprints.ToArray();
    }

    private static async Task ExecuteAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        await command.ExecuteNonQueryAsync();
    }
}
