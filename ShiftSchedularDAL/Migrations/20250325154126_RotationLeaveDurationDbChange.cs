using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class RotationLeaveDurationDbChange : Migration
    {
        /// <inheritdoc />
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing column
            migrationBuilder.DropColumn(
                name: "LeaveDuration",
                table: "EntityShiftRotations");

            // Add a new column with the desired type
            migrationBuilder.AddColumn<long>(
                name: "LeaveDuration",
                table: "EntityShiftRotations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L); // Default to zero to avoid null issues
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the new column
            migrationBuilder.DropColumn(
                name: "LeaveDuration",
                table: "EntityShiftRotations");

            // Add the old column back with the original type
            migrationBuilder.AddColumn<TimeSpan>(
                name: "LeaveDuration",
                table: "EntityShiftRotations",
                type: "TIME",
                nullable: false,
                defaultValue: TimeSpan.Zero); // Default to zero time
        }
    }
}
