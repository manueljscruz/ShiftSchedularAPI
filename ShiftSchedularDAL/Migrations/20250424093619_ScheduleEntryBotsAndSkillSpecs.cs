using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleEntryBotsAndSkillSpecs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpecificSkillAssignments",
                table: "ScheduleEntryWorkers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "WorksWeekDays",
                table: "EntityWorkerInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorksWeekends",
                table: "EntityWorkerInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ScheduleEntryBots",
                columns: table => new
                {
                    ScheduleEntryId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    UserBotId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    SpecificSkillAssignments = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntryBots", x => new { x.ScheduleEntryId, x.UserBotId });
                    table.ForeignKey(
                        name: "FK_ScheduleEntryBots_ScheduleEntry_ScheduleEntryId",
                        column: x => x.ScheduleEntryId,
                        principalTable: "ScheduleEntry",
                        principalColumn: "ScheduleEntryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleEntryBots_UserBots_UserBotId",
                        column: x => x.UserBotId,
                        principalTable: "UserBots",
                        principalColumn: "UserBotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryBots_UserBotId",
                table: "ScheduleEntryBots",
                column: "UserBotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleEntryBots");

            migrationBuilder.DropColumn(
                name: "SpecificSkillAssignments",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropColumn(
                name: "WorksWeekDays",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropColumn(
                name: "WorksWeekends",
                table: "EntityWorkerInvitations");
        }
    }
}
