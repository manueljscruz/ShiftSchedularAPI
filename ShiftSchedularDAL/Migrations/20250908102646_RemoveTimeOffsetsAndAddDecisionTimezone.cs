using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTimeOffsetsAndAddDecisionTimezone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsenceDateDecisionOffset",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropColumn(
                name: "DateOffset",
                table: "WorkerEntityAbsences");

            migrationBuilder.AddColumn<string>(
                name: "DecisionTimezoneId",
                table: "WorkerEntityAbsences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecisionTimezoneId",
                table: "WorkerEntityAbsences");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AbsenceDateDecisionOffset",
                table: "WorkerEntityAbsences",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DateOffset",
                table: "WorkerEntityAbsences",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
