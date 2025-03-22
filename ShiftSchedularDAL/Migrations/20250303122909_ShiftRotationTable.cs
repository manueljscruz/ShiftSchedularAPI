using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class ShiftRotationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityShiftRotations",
                columns: table => new
                {
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    IsLeave = table.Column<bool>(type: "bit", nullable: false),
                    ShiftId = table.Column<byte[]>(type: "BINARY(16)", nullable: true),
                    LeaveDuration = table.Column<TimeSpan>(type: "TIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityShiftRotations", x => new { x.EntityId, x.OrderNo, x.IsLeave });
                    table.ForeignKey(
                        name: "FK_EntityShiftRotations_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityShiftRotations_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityShiftRotations_ShiftId",
                table: "EntityShiftRotations",
                column: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityShiftRotations");
        }
    }
}
