using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleEntry",
                columns: table => new
                {
                    ScheduleEntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShiftId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ScheduleStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduleEndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntry", x => x.ScheduleEntryId);
                    table.ForeignKey(
                        name: "FK_ScheduleEntry_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleEntryWorkers",
                columns: table => new
                {
                    ScheduleEntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntryWorkers", x => new { x.ScheduleEntryId, x.WorkerId });
                    table.ForeignKey(
                        name: "FK_ScheduleEntryWorkers_ScheduleEntry_ScheduleEntryId",
                        column: x => x.ScheduleEntryId,
                        principalTable: "ScheduleEntry",
                        principalColumn: "ScheduleEntryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleEntryWorkers_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Workers",
                        principalColumn: "WorkerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntry_ShiftId",
                table: "ScheduleEntry",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkers_WorkerId",
                table: "ScheduleEntryWorkers",
                column: "WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleEntryWorkers");

            migrationBuilder.DropTable(
                name: "ScheduleEntry");
        }
    }
}
