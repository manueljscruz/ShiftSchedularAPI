using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftNavigationPropertyToEntityShiftRotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if FK already exists before attempting to drop
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_EntityShiftRotations_Shifts_ShiftId')
                BEGIN
                    ALTER TABLE [EntityShiftRotations] DROP CONSTRAINT [FK_EntityShiftRotations_Shifts_ShiftId]
                END
            ");

            // Fix orphaned ShiftId references - set to NULL if the Shift doesn't exist
            // This converts invalid shift references to leave entries
            migrationBuilder.Sql(@"
                UPDATE esr
                SET esr.ShiftId = NULL, esr.IsLeave = 1
                FROM EntityShiftRotations esr
                WHERE esr.ShiftId IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM Shifts s WHERE s.ShiftId = esr.ShiftId)
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityShiftRotations_Shifts_ShiftId",
                table: "EntityShiftRotations",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityShiftRotations_Shifts_ShiftId",
                table: "EntityShiftRotations");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityShiftRotations_Shifts_ShiftId",
                table: "EntityShiftRotations",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "ShiftId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
