using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class NewEntityUserBotTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBots_Entities_EntityId",
                table: "UserBots");

            migrationBuilder.DropIndex(
                name: "IX_UserBots_EntityId",
                table: "UserBots");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "UserBots");

            migrationBuilder.CreateTable(
                name: "EntityUserBots",
                columns: table => new
                {
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    UserBotId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    ActiveWorkerStatus = table.Column<bool>(type: "bit", nullable: false),
                    DateOfJoin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfExit = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityUserBots", x => new { x.EntityId, x.UserBotId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_EntityUserBots_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityUserBots_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityUserBots_UserBots_UserBotId",
                        column: x => x.UserBotId,
                        principalTable: "UserBots",
                        principalColumn: "UserBotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBots_SkillId",
                table: "EntityUserBots",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBots_UserBotId",
                table: "EntityUserBots",
                column: "UserBotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityUserBots");

            migrationBuilder.AddColumn<byte[]>(
                name: "EntityId",
                table: "UserBots",
                type: "BINARY(16)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_UserBots_EntityId",
                table: "UserBots",
                column: "EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBots_Entities_EntityId",
                table: "UserBots",
                column: "EntityId",
                principalTable: "Entities",
                principalColumn: "EntityId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
