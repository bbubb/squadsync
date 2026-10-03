using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SquadSync.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerProfileAndRosterEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DominantFoot = table.Column<string>(type: "text", nullable: true),
                    HeightInches = table.Column<int>(type: "integer", nullable: true),
                    WeightPounds = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RosterEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamMembershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    JerseyNumber = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    RosterStatus = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RosterEntries_TeamMemberships_TeamMembershipId",
                        column: x => x.TeamMembershipId,
                        principalTable: "TeamMemberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProfiles_UserId",
                table: "PlayerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RosterEntries_TeamMembershipId",
                table: "RosterEntries",
                column: "TeamMembershipId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerProfiles");

            migrationBuilder.DropTable(
                name: "RosterEntries");
        }
    }
}
