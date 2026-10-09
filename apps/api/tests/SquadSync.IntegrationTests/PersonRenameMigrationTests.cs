using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using SquadSync.Infrastructure.Persistence;
using Xunit;

namespace SquadSync.IntegrationTests;

public class PersonRenameMigrationTests
{
    private const string Before = "20261003212713_AddPlayerProfileAndRosterEntry";
    private const string After = "20261009150000_RenameUserToPerson";

    [DatabaseFact]
    [Trait("Category", "Database")]
    public async Task Rename_UpgradeRollbackAndReupgrade_PreserveLinkedRowsAndConstraints()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__SquadSync");
        Assert.False(string.IsNullOrWhiteSpace(connectionString),
            "Database tests require ConnectionStrings__SquadSync when SQUADSYNC_RUN_DATABASE_TESTS=1.");
        await using var db = new SquadSyncDbContext(new DbContextOptionsBuilder<SquadSyncDbContext>()
            .UseNpgsql(connectionString).Options);
        await using var transaction = await db.Database.BeginTransactionAsync();
        // All old/new tables and history live in a disposable schema inside this transaction.
        // SET LOCAL and rollback restore the connection and leave public developer records untouched.
        var schema = "person_rename_" + Guid.NewGuid().ToString("N");
        // Identifier is generated solely from a fixed prefix and hexadecimal GUID.
#pragma warning disable EF1002
        await db.Database.ExecuteSqlRawAsync($"CREATE SCHEMA {schema}; SET LOCAL search_path TO {schema};");
#pragma warning restore EF1002
        var migrator = db.GetService<IMigrator>();
        await db.Database.ExecuteSqlRawAsync(migrator.GenerateScript("0", Before, MigrationsSqlGenerationOptions.NoTransactions));
        await db.Database.ExecuteSqlRawAsync("""
            INSERT INTO "Users" VALUES ('22200000-0000-0000-0000-000000000001', 'Existing', 'Player');
            INSERT INTO "Teams" VALUES ('22200000-0000-0000-0000-000000000002', 'Existing Team');
            INSERT INTO "TeamMemberships" ("Id", "UserId", "TeamId", "TeamRole")
            VALUES ('22200000-0000-0000-0000-000000000003', '22200000-0000-0000-0000-000000000001',
                '22200000-0000-0000-0000-000000000002', 'Player');
            INSERT INTO "PlayerProfiles" ("Id", "UserId", "DominantFoot", "HeightInches", "WeightPounds")
            VALUES ('22200000-0000-0000-0000-000000000004', '22200000-0000-0000-0000-000000000001', 'Both', 71, 165.123456789);
            INSERT INTO "RosterEntries" ("Id", "TeamMembershipId", "JerseyNumber", "RosterStatus")
            VALUES ('22200000-0000-0000-0000-000000000005', '22200000-0000-0000-0000-000000000003', '007', 'Active');
            """);
        var original = await ReadRowsAsync(db, renamed: false);
        var originalObjects = await ReadObjectIdentityAsync(db, schema);
        await AssertConstraintsAsync(db, renamed: false);

        await db.Database.ExecuteSqlRawAsync(migrator.GenerateScript(Before, After, MigrationsSqlGenerationOptions.NoTransactions));
        Assert.Equal(original, await ReadRowsAsync(db, renamed: true));
        Assert.Equal(originalObjects, await ReadObjectIdentityAsync(db, schema));
        await AssertConstraintsAsync(db, renamed: true);
        var membership = await db.TeamMemberships.SingleAsync();
        var profile = await db.PlayerProfiles.SingleAsync();
        var person = await db.People.SingleAsync();
        Assert.Equal(person.Id, membership.PersonId);
        Assert.Equal(person.Id, profile.PersonId);
        Assert.Equal(165.123456789m, profile.WeightPounds);
        Assert.Equal("007", (await db.RosterEntries.SingleAsync()).JerseyNumber);
        db.ChangeTracker.Clear();

        await db.Database.ExecuteSqlRawAsync(migrator.GenerateScript(After, Before, MigrationsSqlGenerationOptions.NoTransactions));
        Assert.Equal(original, await ReadRowsAsync(db, renamed: false));
        Assert.Equal(originalObjects, await ReadObjectIdentityAsync(db, schema));
        await AssertConstraintsAsync(db, renamed: false);
        await db.Database.ExecuteSqlRawAsync(migrator.GenerateScript(Before, After, MigrationsSqlGenerationOptions.NoTransactions));
        Assert.Equal(original, await ReadRowsAsync(db, renamed: true));
        await AssertConstraintsAsync(db, renamed: true);
        await transaction.RollbackAsync();
    }

    private static async Task<string> ReadRowsAsync(SquadSyncDbContext db, bool renamed)
    {
        var table = renamed ? "People" : "Users";
        var id = renamed ? "PersonId" : "UserId";
        // Normalize only the renamed JSON property; compare every row, field, GUID, and association.
        var sql = $"""
            SELECT jsonb_build_object(
                'people', (SELECT jsonb_agg(to_jsonb(p) ORDER BY "Id") FROM "{table}" p),
                'teams', (SELECT jsonb_agg(to_jsonb(t) ORDER BY "Id") FROM "Teams" t),
                'memberships', (SELECT jsonb_agg((to_jsonb(m) - '{id}') || jsonb_build_object('PersonId', "{id}") ORDER BY "Id") FROM "TeamMemberships" m),
                'profiles', (SELECT jsonb_agg((to_jsonb(p) - '{id}') || jsonb_build_object('PersonId', "{id}") ORDER BY "Id") FROM "PlayerProfiles" p),
                'roster', (SELECT jsonb_agg(to_jsonb(r) ORDER BY "Id") FROM "RosterEntries" r)
            )::text AS "Value"
            """;
        return await db.Database.SqlQueryRaw<string>(sql).SingleAsync();
    }

    private static async Task<string> ReadObjectIdentityAsync(SquadSyncDbContext db, string schema)
    {
        // Rename keeps table, index, and constraint OIDs, proving no replacements were performed.
        return await db.Database.SqlQuery<string>($"""
            SELECT jsonb_build_object(
                'relations', (SELECT jsonb_agg(c.oid ORDER BY c.oid) FROM pg_class c
                    JOIN pg_namespace n ON n.oid = c.relnamespace WHERE n.nspname = {schema}),
                'constraints', (SELECT jsonb_agg(c.oid ORDER BY c.oid) FROM pg_constraint c
                    JOIN pg_namespace n ON n.oid = c.connamespace WHERE n.nspname = {schema})
            )::text AS "Value"
            """).SingleAsync();
    }

    private static async Task AssertConstraintsAsync(SquadSyncDbContext db, bool renamed)
    {
        var table = renamed ? "People" : "Users";
        var id = renamed ? "PersonId" : "UserId";
        await RejectAsync(db, $"""
            INSERT INTO "TeamMemberships" ("Id", "{id}", "TeamId", "TeamRole")
            SELECT '22200000-0000-0000-0000-000000000006', "{id}", "TeamId", 'Coach' FROM "TeamMemberships"
            """, PostgresErrorCodes.UniqueViolation, $"IX_TeamMemberships_{id}_TeamId");
        await RejectAsync(db, $"""
            INSERT INTO "PlayerProfiles" ("Id", "{id}")
            SELECT '22200000-0000-0000-0000-000000000007', "{id}" FROM "PlayerProfiles"
            """, PostgresErrorCodes.UniqueViolation, $"IX_PlayerProfiles_{id}");
        await RejectAsync(db, $"""
            UPDATE "TeamMemberships" SET "{id}" = '22200000-0000-0000-0000-000000000099'
            """, PostgresErrorCodes.ForeignKeyViolation, $"FK_TeamMemberships_{table}_{id}");
        await RejectAsync(db, $"""
            UPDATE "PlayerProfiles" SET "{id}" = '22200000-0000-0000-0000-000000000099'
            """, PostgresErrorCodes.ForeignKeyViolation, $"FK_PlayerProfiles_{table}_{id}");
        await RejectAsync(db, $"""DELETE FROM "{table}" """, PostgresErrorCodes.RestrictViolation);
        await RejectAsync(db, """DELETE FROM "Teams" """, PostgresErrorCodes.RestrictViolation, "FK_TeamMemberships_Teams_TeamId");
        await RejectAsync(db, """DELETE FROM "TeamMemberships" """, PostgresErrorCodes.RestrictViolation,
            "FK_RosterEntries_TeamMemberships_TeamMembershipId");
    }

    private static async Task RejectAsync(SquadSyncDbContext db, string sql, string state, string? constraint = null)
    {
        var transaction = db.Database.CurrentTransaction!;
        await transaction.CreateSavepointAsync("constraint_check");
        var error = await Assert.ThrowsAsync<PostgresException>(() => db.Database.ExecuteSqlRawAsync(sql));
        // Recover immediately after the expected PostgreSQL failure before any further queries.
        await transaction.RollbackToSavepointAsync("constraint_check");
        await transaction.ReleaseSavepointAsync("constraint_check");
        Assert.Equal(state, error.SqlState);
        if (constraint is not null) Assert.Equal(constraint, error.ConstraintName);
    }
}
