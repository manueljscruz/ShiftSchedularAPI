using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class IneligibilityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "scheduleEntryBotIneligibilities",
                columns: table => new
                {
                    ScheduleEntryId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    UserBotId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    IneligibilityObservations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfAssessement = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scheduleEntryBotIneligibilities", x => new { x.ScheduleEntryId, x.UserBotId });
                    table.ForeignKey(
                        name: "FK_scheduleEntryBotIneligibilities_ScheduleEntry_ScheduleEntryId",
                        column: x => x.ScheduleEntryId,
                        principalTable: "ScheduleEntry",
                        principalColumn: "ScheduleEntryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_scheduleEntryBotIneligibilities_UserBots_UserBotId",
                        column: x => x.UserBotId,
                        principalTable: "UserBots",
                        principalColumn: "UserBotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleEntryWorkerIneligibilities",
                columns: table => new
                {
                    ScheduleEntryId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IneligibilityObservations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfAssessement = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntryWorkerIneligibilities", x => new { x.ScheduleEntryId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_ScheduleEntryWorkerIneligibilities_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleEntryWorkerIneligibilities_ScheduleEntry_ScheduleEntryId",
                        column: x => x.ScheduleEntryId,
                        principalTable: "ScheduleEntry",
                        principalColumn: "ScheduleEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_scheduleEntryBotIneligibilities_UserBotId",
                table: "scheduleEntryBotIneligibilities",
                column: "UserBotId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkerIneligibilities_ApplicationUserId",
                table: "ScheduleEntryWorkerIneligibilities",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropTable(
                name: "ScheduleEntryWorkerIneligibilities");
        }
    }
}
