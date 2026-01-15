using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_WorkerEntityAbsences_EntityId",
                table: "WorkerEntityAbsences",
                newName: "IX_EntityWorkerAbsences_EntityId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkerEntityAbsences_ApplicationUserId",
                table: "WorkerEntityAbsences",
                newName: "IX_EntityWorkerAbsences_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleEntry_ShiftId",
                table: "ScheduleEntry",
                newName: "IX_ScheduleEntries_ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerAbsences_DateRange",
                table: "WorkerEntityAbsences",
                columns: new[] { "AbsenceStartDate", "AbsenceEndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkers_ScheduleEntryId",
                table: "ScheduleEntryWorkers",
                column: "ScheduleEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntries_DateRange",
                table: "ScheduleEntry",
                columns: new[] { "ScheduleStartDate", "ScheduleEndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntries_ShiftId_DateRange",
                table: "ScheduleEntry",
                columns: new[] { "ShiftId", "ScheduleStartDate", "ScheduleEndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerSkills_ApplicationUserId",
                table: "EntityWorkerSkills",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_EntityId",
                table: "EntityWorkers",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_EntityId_ApplicationUserId",
                table: "EntityWorkers",
                columns: new[] { "EntityId", "ApplicationUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerInvitations_Email",
                table: "EntityWorkerInvitations",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerInvitations_EntityId",
                table: "EntityWorkerInvitations",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotSkills_EntityId",
                table: "EntityUserBotSkills",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBots_EntityId",
                table: "EntityUserBots",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityShiftRotations_EntityId",
                table: "EntityShiftRotations",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityShiftRotations_ShiftId",
                table: "EntityShiftRotations",
                column: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerAbsences_DateRange",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryWorkers_ScheduleEntryId",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntries_DateRange",
                table: "ScheduleEntry");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntries_ShiftId_DateRange",
                table: "ScheduleEntry");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerSkills_ApplicationUserId",
                table: "EntityWorkerSkills");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_EntityId",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_EntityId_ApplicationUserId",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerInvitations_Email",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerInvitations_EntityId",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBotSkills_EntityId",
                table: "EntityUserBotSkills");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBots_EntityId",
                table: "EntityUserBots");

            migrationBuilder.DropIndex(
                name: "IX_EntityShiftRotations_EntityId",
                table: "EntityShiftRotations");

            migrationBuilder.DropIndex(
                name: "IX_EntityShiftRotations_ShiftId",
                table: "EntityShiftRotations");

            migrationBuilder.RenameIndex(
                name: "IX_EntityWorkerAbsences_EntityId",
                table: "WorkerEntityAbsences",
                newName: "IX_WorkerEntityAbsences_EntityId");

            migrationBuilder.RenameIndex(
                name: "IX_EntityWorkerAbsences_ApplicationUserId",
                table: "WorkerEntityAbsences",
                newName: "IX_WorkerEntityAbsences_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleEntries_ShiftId",
                table: "ScheduleEntry",
                newName: "IX_ScheduleEntry_ShiftId");
        }
    }
}
