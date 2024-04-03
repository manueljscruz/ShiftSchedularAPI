using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class EntityWorkerNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanCreateSchedules",
                table: "EntityWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOwner",
                table: "EntityWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanCreateSchedules",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "IsOwner",
                table: "EntityWorkers");
        }
    }
}
