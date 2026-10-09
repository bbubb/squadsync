using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SquadSync.Infrastructure.Persistence.Migrations;

public partial class RenameUserToPerson : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // PostgreSQL renames preserve rows, index identity, and FK/delete semantics.
        migrationBuilder.RenameTable(name: "Users", newName: "People");
        migrationBuilder.RenameColumn(name: "UserId", table: "TeamMemberships", newName: "PersonId");
        migrationBuilder.RenameColumn(name: "UserId", table: "PlayerProfiles", newName: "PersonId");
        migrationBuilder.RenameIndex(name: "IX_TeamMemberships_UserId_TeamId", table: "TeamMemberships",
            newName: "IX_TeamMemberships_PersonId_TeamId");
        migrationBuilder.RenameIndex(name: "IX_PlayerProfiles_UserId", table: "PlayerProfiles",
            newName: "IX_PlayerProfiles_PersonId");
        migrationBuilder.Sql("""
            ALTER TABLE "People" RENAME CONSTRAINT "PK_Users" TO "PK_People";
            ALTER TABLE "TeamMemberships" RENAME CONSTRAINT "FK_TeamMemberships_Users_UserId" TO "FK_TeamMemberships_People_PersonId";
            ALTER TABLE "PlayerProfiles" RENAME CONSTRAINT "FK_PlayerProfiles_Users_UserId" TO "FK_PlayerProfiles_People_PersonId";
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "People" RENAME CONSTRAINT "PK_People" TO "PK_Users";
            ALTER TABLE "TeamMemberships" RENAME CONSTRAINT "FK_TeamMemberships_People_PersonId" TO "FK_TeamMemberships_Users_UserId";
            ALTER TABLE "PlayerProfiles" RENAME CONSTRAINT "FK_PlayerProfiles_People_PersonId" TO "FK_PlayerProfiles_Users_UserId";
            """);
        migrationBuilder.RenameIndex(name: "IX_TeamMemberships_PersonId_TeamId", table: "TeamMemberships",
            newName: "IX_TeamMemberships_UserId_TeamId");
        migrationBuilder.RenameIndex(name: "IX_PlayerProfiles_PersonId", table: "PlayerProfiles",
            newName: "IX_PlayerProfiles_UserId");
        migrationBuilder.RenameColumn(name: "PersonId", table: "TeamMemberships", newName: "UserId");
        migrationBuilder.RenameColumn(name: "PersonId", table: "PlayerProfiles", newName: "UserId");
        migrationBuilder.RenameTable(name: "People", newName: "Users");
    }
}
