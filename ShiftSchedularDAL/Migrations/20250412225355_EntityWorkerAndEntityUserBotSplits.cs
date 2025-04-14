using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class EntityWorkerAndEntityUserBotSplits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBots_Skills_SkillId",
                table: "EntityUserBots");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkers_Skills_SkillId",
                table: "EntityWorkers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityWorkers",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_SkillId",
                table: "EntityWorkers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityUserBots",
                table: "EntityUserBots");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBots_SkillId",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "EntityUserBots");

            migrationBuilder.AddColumn<bool>(
                name: "WorksWeekDays",
                table: "EntityWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorksWeekends",
                table: "EntityWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorksWeekDays",
                table: "EntityUserBots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WorksWeekends",
                table: "EntityUserBots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityWorkers",
                table: "EntityWorkers",
                columns: new[] { "EntityId", "ApplicationUserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityUserBots",
                table: "EntityUserBots",
                columns: new[] { "EntityId", "UserBotId" });

            migrationBuilder.CreateTable(
                name: "EntityUserBotSkills",
                columns: table => new
                {
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    UserBotId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityUserBotSkills", x => new { x.EntityId, x.UserBotId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_EntityUserBotSkills_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityUserBotSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityUserBotSkills_UserBots_UserBotId",
                        column: x => x.UserBotId,
                        principalTable: "UserBots",
                        principalColumn: "UserBotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityWorkerSkills",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityWorkerSkills", x => new { x.ApplicationUserId, x.EntityId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_EntityWorkerSkills_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityWorkerSkills_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityWorkerSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotSkills_SkillId",
                table: "EntityUserBotSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotSkills_UserBotId",
                table: "EntityUserBotSkills",
                column: "UserBotId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerSkills_EntityId",
                table: "EntityWorkerSkills",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerSkills_SkillId",
                table: "EntityWorkerSkills",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityUserBotSkills");

            migrationBuilder.DropTable(
                name: "EntityWorkerSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityWorkers",
                table: "EntityWorkers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityUserBots",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "WorksWeekDays",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "WorksWeekends",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "WorksWeekDays",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "WorksWeekends",
                table: "EntityUserBots");

            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                table: "EntityWorkers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                table: "EntityUserBots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityWorkers",
                table: "EntityWorkers",
                columns: new[] { "EntityId", "ApplicationUserId", "SkillId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityUserBots",
                table: "EntityUserBots",
                columns: new[] { "EntityId", "UserBotId", "SkillId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_SkillId",
                table: "EntityWorkers",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBots_SkillId",
                table: "EntityUserBots",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBots_Skills_SkillId",
                table: "EntityUserBots",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "SkillId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkers_Skills_SkillId",
                table: "EntityWorkers",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "SkillId");
        }
    }
}
