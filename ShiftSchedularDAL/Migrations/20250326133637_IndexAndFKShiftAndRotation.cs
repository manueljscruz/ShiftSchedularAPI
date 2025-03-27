using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class IndexAndFKShiftAndRotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityShiftRotations_Shifts_ShiftId",
                table: "EntityShiftRotations");

            migrationBuilder.DropIndex(
                name: "IX_EntityShiftRotations_ShiftId",
                table: "EntityShiftRotations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EntityShiftRotations_ShiftId",
                table: "EntityShiftRotations",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityShiftRotations_Shifts_ShiftId",
                table: "EntityShiftRotations",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "ShiftId");
        }
    }
}
