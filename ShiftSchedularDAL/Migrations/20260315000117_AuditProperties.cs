using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class AuditProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateOfCreation",
                table: "UserBots",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WorkerEntityAbsences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "WorkerEntityAbsences",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WorkerEntityAbsences",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "WorkerEntityAbsences",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkerEntityAbsences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "WorkerEntityAbsences",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "WorkerEntityAbsences",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "UserBots",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserBots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "UserBots",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserBots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserBots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "UserBots",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Shifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Shifts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Shifts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Shifts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Shifts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Shifts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Shifts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ShiftBreaks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ShiftBreaks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ShiftBreaks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ShiftBreaks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShiftBreaks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ShiftBreaks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "ShiftBreaks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ScheduleEntryWorkers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ScheduleEntryWorkers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ScheduleEntryWorkers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ScheduleEntryWorkers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ScheduleEntryWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ScheduleEntryWorkers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "ScheduleEntryWorkers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ScheduleEntryWorkerIneligibilities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ScheduleEntryWorkerIneligibilities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ScheduleEntryWorkerIneligibilities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ScheduleEntryWorkerIneligibilities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ScheduleEntryWorkerIneligibilities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ScheduleEntryWorkerIneligibilities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "ScheduleEntryWorkerIneligibilities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ScheduleEntryBots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ScheduleEntryBots",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ScheduleEntryBots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ScheduleEntryBots",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ScheduleEntryBots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ScheduleEntryBots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "ScheduleEntryBots",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "scheduleEntryBotIneligibilities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "scheduleEntryBotIneligibilities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "scheduleEntryBotIneligibilities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "scheduleEntryBotIneligibilities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "scheduleEntryBotIneligibilities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "scheduleEntryBotIneligibilities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "scheduleEntryBotIneligibilities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ScheduleEntry",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ScheduleEntry",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ScheduleEntry",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "ScheduleEntry",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ScheduleEntry",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ScheduleEntry",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "ScheduleEntry",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityWorkerSkills",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityWorkerSkills",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityWorkerSkills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityWorkerSkills",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityWorkerSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityWorkerSkills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityWorkerSkills",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityWorkerShiftAssigneds",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityWorkerShiftAssigneds",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityWorkerShiftAssigneds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityWorkerShiftAssigneds",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityWorkerShiftAssigneds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityWorkerShiftAssigneds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityWorkerShiftAssigneds",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConvertedAt",
                table: "EntityWorkers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConvertedBy",
                table: "EntityWorkers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ConvertedFromBotId",
                table: "EntityWorkers",
                type: "BINARY(16)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityWorkers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityWorkers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityWorkers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityWorkers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityWorkers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityWorkers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityWorkerInvitations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityWorkerInvitations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityWorkerInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityWorkerInvitations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityWorkerInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityWorkerInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityWorkerInvitations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityUserBotSkills",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityUserBotSkills",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityUserBotSkills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityUserBotSkills",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityUserBotSkills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityUserBotSkills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityUserBotSkills",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityUserBotShiftAssigneds",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityUserBotShiftAssigneds",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityUserBotShiftAssigneds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityUserBotShiftAssigneds",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityUserBotShiftAssigneds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityUserBotShiftAssigneds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityUserBotShiftAssigneds",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityUserBots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityUserBots",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityUserBots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityUserBots",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityUserBots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityUserBots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityUserBots",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityShiftRotations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityShiftRotations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityShiftRotations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityShiftRotations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityShiftRotations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityShiftRotations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityShiftRotations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityRuleSpecifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityRuleSpecifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityRuleSpecifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityRuleSpecifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityRuleSpecifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityRuleSpecifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityRuleSpecifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityRules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityRules",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityRules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityRules",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityRules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityRules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityRules",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EntityHolidays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "EntityHolidays",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EntityHolidays",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "EntityHolidays",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EntityHolidays",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityHolidays",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "EntityHolidays",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Entities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Entities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Entities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Entities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Entities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Entities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Entities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerAbsences_IsDeleted",
                table: "WorkerEntityAbsences",
                column: "IsDeleted",
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEntityAbsences_CreatedById",
                table: "WorkerEntityAbsences",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEntityAbsences_DeletedById",
                table: "WorkerEntityAbsences",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEntityAbsences_UpdatedById",
                table: "WorkerEntityAbsences",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserBots_CreatedById",
                table: "UserBots",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserBots_DeletedById",
                table: "UserBots",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserBots_IsDeleted",
                table: "UserBots",
                column: "IsDeleted",
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_UserBots_UpdatedById",
                table: "UserBots",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_CreatedById",
                table: "Shifts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_DeletedById",
                table: "Shifts",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_UpdatedById",
                table: "Shifts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreaks_CreatedById",
                table: "ShiftBreaks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreaks_DeletedById",
                table: "ShiftBreaks",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreaks_UpdatedById",
                table: "ShiftBreaks",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkers_CreatedById",
                table: "ScheduleEntryWorkers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkers_DeletedById",
                table: "ScheduleEntryWorkers",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkers_IsDeleted",
                table: "ScheduleEntryWorkers",
                column: "IsDeleted",
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkers_UpdatedById",
                table: "ScheduleEntryWorkers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkerIneligibilities_CreatedById",
                table: "ScheduleEntryWorkerIneligibilities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkerIneligibilities_DeletedById",
                table: "ScheduleEntryWorkerIneligibilities",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryWorkerIneligibilities_UpdatedById",
                table: "ScheduleEntryWorkerIneligibilities",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryBots_CreatedById",
                table: "ScheduleEntryBots",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryBots_DeletedById",
                table: "ScheduleEntryBots",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryBots_IsDeleted",
                table: "ScheduleEntryBots",
                column: "IsDeleted",
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntryBots_UpdatedById",
                table: "ScheduleEntryBots",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_scheduleEntryBotIneligibilities_CreatedById",
                table: "scheduleEntryBotIneligibilities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_scheduleEntryBotIneligibilities_DeletedById",
                table: "scheduleEntryBotIneligibilities",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_scheduleEntryBotIneligibilities_UpdatedById",
                table: "scheduleEntryBotIneligibilities",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntries_IsDeleted",
                table: "ScheduleEntry",
                column: "IsDeleted",
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntry_CreatedById",
                table: "ScheduleEntry",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntry_DeletedById",
                table: "ScheduleEntry",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntry_UpdatedById",
                table: "ScheduleEntry",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerSkills_CreatedById",
                table: "EntityWorkerSkills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerSkills_DeletedById",
                table: "EntityWorkerSkills",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerSkills_UpdatedById",
                table: "EntityWorkerSkills",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerShiftAssigneds_CreatedById",
                table: "EntityWorkerShiftAssigneds",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerShiftAssigneds_DeletedById",
                table: "EntityWorkerShiftAssigneds",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerShiftAssigneds_UpdatedById",
                table: "EntityWorkerShiftAssigneds",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_ConvertedBy",
                table: "EntityWorkers",
                column: "ConvertedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_ConvertedFromBotId",
                table: "EntityWorkers",
                column: "ConvertedFromBotId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_CreatedById",
                table: "EntityWorkers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_DeletedById",
                table: "EntityWorkers",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_IsDeleted",
                table: "EntityWorkers",
                column: "IsDeleted",
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_UpdatedById",
                table: "EntityWorkers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerInvitations_CreatedById",
                table: "EntityWorkerInvitations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerInvitations_DeletedById",
                table: "EntityWorkerInvitations",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerInvitations_UpdatedById",
                table: "EntityWorkerInvitations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotSkills_CreatedById",
                table: "EntityUserBotSkills",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotSkills_DeletedById",
                table: "EntityUserBotSkills",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotSkills_UpdatedById",
                table: "EntityUserBotSkills",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotShiftAssigneds_CreatedById",
                table: "EntityUserBotShiftAssigneds",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotShiftAssigneds_DeletedById",
                table: "EntityUserBotShiftAssigneds",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotShiftAssigneds_UpdatedById",
                table: "EntityUserBotShiftAssigneds",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBots_CreatedById",
                table: "EntityUserBots",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBots_DeletedById",
                table: "EntityUserBots",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBots_IsDeleted",
                table: "EntityUserBots",
                column: "IsDeleted",
                filter: "IsDeleted = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBots_UpdatedById",
                table: "EntityUserBots",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityShiftRotations_CreatedById",
                table: "EntityShiftRotations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityShiftRotations_DeletedById",
                table: "EntityShiftRotations",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityShiftRotations_UpdatedById",
                table: "EntityShiftRotations",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRuleSpecifications_CreatedById",
                table: "EntityRuleSpecifications",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRuleSpecifications_DeletedById",
                table: "EntityRuleSpecifications",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRuleSpecifications_UpdatedById",
                table: "EntityRuleSpecifications",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRules_CreatedById",
                table: "EntityRules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRules_DeletedById",
                table: "EntityRules",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRules_UpdatedById",
                table: "EntityRules",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityHolidays_CreatedById",
                table: "EntityHolidays",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityHolidays_DeletedById",
                table: "EntityHolidays",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityHolidays_UpdatedById",
                table: "EntityHolidays",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_CreatedById",
                table: "Entities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_DeletedById",
                table: "Entities",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_UpdatedById",
                table: "Entities",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_AspNetUsers_CreatedById",
                table: "Entities",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_AspNetUsers_DeletedById",
                table: "Entities",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_AspNetUsers_UpdatedById",
                table: "Entities",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityHolidays_AspNetUsers_CreatedById",
                table: "EntityHolidays",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityHolidays_AspNetUsers_DeletedById",
                table: "EntityHolidays",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityHolidays_AspNetUsers_UpdatedById",
                table: "EntityHolidays",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityRules_AspNetUsers_CreatedById",
                table: "EntityRules",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityRules_AspNetUsers_DeletedById",
                table: "EntityRules",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityRules_AspNetUsers_UpdatedById",
                table: "EntityRules",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityRuleSpecifications_AspNetUsers_CreatedById",
                table: "EntityRuleSpecifications",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityRuleSpecifications_AspNetUsers_DeletedById",
                table: "EntityRuleSpecifications",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityRuleSpecifications_AspNetUsers_UpdatedById",
                table: "EntityRuleSpecifications",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityShiftRotations_AspNetUsers_CreatedById",
                table: "EntityShiftRotations",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityShiftRotations_AspNetUsers_DeletedById",
                table: "EntityShiftRotations",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityShiftRotations_AspNetUsers_UpdatedById",
                table: "EntityShiftRotations",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBots_AspNetUsers_CreatedById",
                table: "EntityUserBots",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBots_AspNetUsers_DeletedById",
                table: "EntityUserBots",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBots_AspNetUsers_UpdatedById",
                table: "EntityUserBots",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBotShiftAssigneds_AspNetUsers_CreatedById",
                table: "EntityUserBotShiftAssigneds",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBotShiftAssigneds_AspNetUsers_DeletedById",
                table: "EntityUserBotShiftAssigneds",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBotShiftAssigneds_AspNetUsers_UpdatedById",
                table: "EntityUserBotShiftAssigneds",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBotSkills_AspNetUsers_CreatedById",
                table: "EntityUserBotSkills",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBotSkills_AspNetUsers_DeletedById",
                table: "EntityUserBotSkills",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityUserBotSkills_AspNetUsers_UpdatedById",
                table: "EntityUserBotSkills",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkerInvitations_AspNetUsers_CreatedById",
                table: "EntityWorkerInvitations",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkerInvitations_AspNetUsers_DeletedById",
                table: "EntityWorkerInvitations",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkerInvitations_AspNetUsers_UpdatedById",
                table: "EntityWorkerInvitations",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkers_AspNetUsers_ConvertedBy",
                table: "EntityWorkers",
                column: "ConvertedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkers_AspNetUsers_CreatedById",
                table: "EntityWorkers",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkers_AspNetUsers_DeletedById",
                table: "EntityWorkers",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkers_AspNetUsers_UpdatedById",
                table: "EntityWorkers",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkers_UserBots_ConvertedFromBotId",
                table: "EntityWorkers",
                column: "ConvertedFromBotId",
                principalTable: "UserBots",
                principalColumn: "UserBotId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkerShiftAssigneds_AspNetUsers_CreatedById",
                table: "EntityWorkerShiftAssigneds",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkerShiftAssigneds_AspNetUsers_DeletedById",
                table: "EntityWorkerShiftAssigneds",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkerShiftAssigneds_AspNetUsers_UpdatedById",
                table: "EntityWorkerShiftAssigneds",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkerSkills_AspNetUsers_CreatedById",
                table: "EntityWorkerSkills",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkerSkills_AspNetUsers_DeletedById",
                table: "EntityWorkerSkills",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkerSkills_AspNetUsers_UpdatedById",
                table: "EntityWorkerSkills",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntry_AspNetUsers_CreatedById",
                table: "ScheduleEntry",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntry_AspNetUsers_DeletedById",
                table: "ScheduleEntry",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntry_AspNetUsers_UpdatedById",
                table: "ScheduleEntry",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_scheduleEntryBotIneligibilities_AspNetUsers_CreatedById",
                table: "scheduleEntryBotIneligibilities",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_scheduleEntryBotIneligibilities_AspNetUsers_DeletedById",
                table: "scheduleEntryBotIneligibilities",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_scheduleEntryBotIneligibilities_AspNetUsers_UpdatedById",
                table: "scheduleEntryBotIneligibilities",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntryBots_AspNetUsers_CreatedById",
                table: "ScheduleEntryBots",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntryBots_AspNetUsers_DeletedById",
                table: "ScheduleEntryBots",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntryBots_AspNetUsers_UpdatedById",
                table: "ScheduleEntryBots",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntryWorkerIneligibilities_AspNetUsers_CreatedById",
                table: "ScheduleEntryWorkerIneligibilities",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntryWorkerIneligibilities_AspNetUsers_DeletedById",
                table: "ScheduleEntryWorkerIneligibilities",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntryWorkerIneligibilities_AspNetUsers_UpdatedById",
                table: "ScheduleEntryWorkerIneligibilities",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntryWorkers_AspNetUsers_CreatedById",
                table: "ScheduleEntryWorkers",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntryWorkers_AspNetUsers_DeletedById",
                table: "ScheduleEntryWorkers",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleEntryWorkers_AspNetUsers_UpdatedById",
                table: "ScheduleEntryWorkers",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftBreaks_AspNetUsers_CreatedById",
                table: "ShiftBreaks",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftBreaks_AspNetUsers_DeletedById",
                table: "ShiftBreaks",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftBreaks_AspNetUsers_UpdatedById",
                table: "ShiftBreaks",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_CreatedById",
                table: "Shifts",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_DeletedById",
                table: "Shifts",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_UpdatedById",
                table: "Shifts",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBots_AspNetUsers_CreatedById",
                table: "UserBots",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBots_AspNetUsers_DeletedById",
                table: "UserBots",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBots_AspNetUsers_UpdatedById",
                table: "UserBots",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerEntityAbsences_AspNetUsers_CreatedById",
                table: "WorkerEntityAbsences",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerEntityAbsences_AspNetUsers_DeletedById",
                table: "WorkerEntityAbsences",
                column: "DeletedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerEntityAbsences_AspNetUsers_UpdatedById",
                table: "WorkerEntityAbsences",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entities_AspNetUsers_CreatedById",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_AspNetUsers_DeletedById",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_Entities_AspNetUsers_UpdatedById",
                table: "Entities");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityHolidays_AspNetUsers_CreatedById",
                table: "EntityHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityHolidays_AspNetUsers_DeletedById",
                table: "EntityHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityHolidays_AspNetUsers_UpdatedById",
                table: "EntityHolidays");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityRules_AspNetUsers_CreatedById",
                table: "EntityRules");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityRules_AspNetUsers_DeletedById",
                table: "EntityRules");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityRules_AspNetUsers_UpdatedById",
                table: "EntityRules");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityRuleSpecifications_AspNetUsers_CreatedById",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityRuleSpecifications_AspNetUsers_DeletedById",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityRuleSpecifications_AspNetUsers_UpdatedById",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityShiftRotations_AspNetUsers_CreatedById",
                table: "EntityShiftRotations");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityShiftRotations_AspNetUsers_DeletedById",
                table: "EntityShiftRotations");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityShiftRotations_AspNetUsers_UpdatedById",
                table: "EntityShiftRotations");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBots_AspNetUsers_CreatedById",
                table: "EntityUserBots");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBots_AspNetUsers_DeletedById",
                table: "EntityUserBots");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBots_AspNetUsers_UpdatedById",
                table: "EntityUserBots");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBotShiftAssigneds_AspNetUsers_CreatedById",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBotShiftAssigneds_AspNetUsers_DeletedById",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBotShiftAssigneds_AspNetUsers_UpdatedById",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBotSkills_AspNetUsers_CreatedById",
                table: "EntityUserBotSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBotSkills_AspNetUsers_DeletedById",
                table: "EntityUserBotSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityUserBotSkills_AspNetUsers_UpdatedById",
                table: "EntityUserBotSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkerInvitations_AspNetUsers_CreatedById",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkerInvitations_AspNetUsers_DeletedById",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkerInvitations_AspNetUsers_UpdatedById",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkers_AspNetUsers_ConvertedBy",
                table: "EntityWorkers");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkers_AspNetUsers_CreatedById",
                table: "EntityWorkers");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkers_AspNetUsers_DeletedById",
                table: "EntityWorkers");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkers_AspNetUsers_UpdatedById",
                table: "EntityWorkers");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkers_UserBots_ConvertedFromBotId",
                table: "EntityWorkers");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkerShiftAssigneds_AspNetUsers_CreatedById",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkerShiftAssigneds_AspNetUsers_DeletedById",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkerShiftAssigneds_AspNetUsers_UpdatedById",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkerSkills_AspNetUsers_CreatedById",
                table: "EntityWorkerSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkerSkills_AspNetUsers_DeletedById",
                table: "EntityWorkerSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkerSkills_AspNetUsers_UpdatedById",
                table: "EntityWorkerSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntry_AspNetUsers_CreatedById",
                table: "ScheduleEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntry_AspNetUsers_DeletedById",
                table: "ScheduleEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntry_AspNetUsers_UpdatedById",
                table: "ScheduleEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_scheduleEntryBotIneligibilities_AspNetUsers_CreatedById",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_scheduleEntryBotIneligibilities_AspNetUsers_DeletedById",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_scheduleEntryBotIneligibilities_AspNetUsers_UpdatedById",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntryBots_AspNetUsers_CreatedById",
                table: "ScheduleEntryBots");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntryBots_AspNetUsers_DeletedById",
                table: "ScheduleEntryBots");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntryBots_AspNetUsers_UpdatedById",
                table: "ScheduleEntryBots");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntryWorkerIneligibilities_AspNetUsers_CreatedById",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntryWorkerIneligibilities_AspNetUsers_DeletedById",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntryWorkerIneligibilities_AspNetUsers_UpdatedById",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntryWorkers_AspNetUsers_CreatedById",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntryWorkers_AspNetUsers_DeletedById",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleEntryWorkers_AspNetUsers_UpdatedById",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftBreaks_AspNetUsers_CreatedById",
                table: "ShiftBreaks");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftBreaks_AspNetUsers_DeletedById",
                table: "ShiftBreaks");

            migrationBuilder.DropForeignKey(
                name: "FK_ShiftBreaks_AspNetUsers_UpdatedById",
                table: "ShiftBreaks");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_CreatedById",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_DeletedById",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_UpdatedById",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBots_AspNetUsers_CreatedById",
                table: "UserBots");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBots_AspNetUsers_DeletedById",
                table: "UserBots");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBots_AspNetUsers_UpdatedById",
                table: "UserBots");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerEntityAbsences_AspNetUsers_CreatedById",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerEntityAbsences_AspNetUsers_DeletedById",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerEntityAbsences_AspNetUsers_UpdatedById",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerAbsences_IsDeleted",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropIndex(
                name: "IX_WorkerEntityAbsences_CreatedById",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropIndex(
                name: "IX_WorkerEntityAbsences_DeletedById",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropIndex(
                name: "IX_WorkerEntityAbsences_UpdatedById",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropIndex(
                name: "IX_UserBots_CreatedById",
                table: "UserBots");

            migrationBuilder.DropIndex(
                name: "IX_UserBots_DeletedById",
                table: "UserBots");

            migrationBuilder.DropIndex(
                name: "IX_UserBots_IsDeleted",
                table: "UserBots");

            migrationBuilder.DropIndex(
                name: "IX_UserBots_UpdatedById",
                table: "UserBots");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_CreatedById",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_DeletedById",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_UpdatedById",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_ShiftBreaks_CreatedById",
                table: "ShiftBreaks");

            migrationBuilder.DropIndex(
                name: "IX_ShiftBreaks_DeletedById",
                table: "ShiftBreaks");

            migrationBuilder.DropIndex(
                name: "IX_ShiftBreaks_UpdatedById",
                table: "ShiftBreaks");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryWorkers_CreatedById",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryWorkers_DeletedById",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryWorkers_IsDeleted",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryWorkers_UpdatedById",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryWorkerIneligibilities_CreatedById",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryWorkerIneligibilities_DeletedById",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryWorkerIneligibilities_UpdatedById",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryBots_CreatedById",
                table: "ScheduleEntryBots");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryBots_DeletedById",
                table: "ScheduleEntryBots");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryBots_IsDeleted",
                table: "ScheduleEntryBots");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntryBots_UpdatedById",
                table: "ScheduleEntryBots");

            migrationBuilder.DropIndex(
                name: "IX_scheduleEntryBotIneligibilities_CreatedById",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropIndex(
                name: "IX_scheduleEntryBotIneligibilities_DeletedById",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropIndex(
                name: "IX_scheduleEntryBotIneligibilities_UpdatedById",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntries_IsDeleted",
                table: "ScheduleEntry");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntry_CreatedById",
                table: "ScheduleEntry");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntry_DeletedById",
                table: "ScheduleEntry");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleEntry_UpdatedById",
                table: "ScheduleEntry");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerSkills_CreatedById",
                table: "EntityWorkerSkills");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerSkills_DeletedById",
                table: "EntityWorkerSkills");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerSkills_UpdatedById",
                table: "EntityWorkerSkills");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerShiftAssigneds_CreatedById",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerShiftAssigneds_DeletedById",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerShiftAssigneds_UpdatedById",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_ConvertedBy",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_ConvertedFromBotId",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_CreatedById",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_DeletedById",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_IsDeleted",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_UpdatedById",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerInvitations_CreatedById",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerInvitations_DeletedById",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkerInvitations_UpdatedById",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBotSkills_CreatedById",
                table: "EntityUserBotSkills");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBotSkills_DeletedById",
                table: "EntityUserBotSkills");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBotSkills_UpdatedById",
                table: "EntityUserBotSkills");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBotShiftAssigneds_CreatedById",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBotShiftAssigneds_DeletedById",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBotShiftAssigneds_UpdatedById",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBots_CreatedById",
                table: "EntityUserBots");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBots_DeletedById",
                table: "EntityUserBots");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBots_IsDeleted",
                table: "EntityUserBots");

            migrationBuilder.DropIndex(
                name: "IX_EntityUserBots_UpdatedById",
                table: "EntityUserBots");

            migrationBuilder.DropIndex(
                name: "IX_EntityShiftRotations_CreatedById",
                table: "EntityShiftRotations");

            migrationBuilder.DropIndex(
                name: "IX_EntityShiftRotations_DeletedById",
                table: "EntityShiftRotations");

            migrationBuilder.DropIndex(
                name: "IX_EntityShiftRotations_UpdatedById",
                table: "EntityShiftRotations");

            migrationBuilder.DropIndex(
                name: "IX_EntityRuleSpecifications_CreatedById",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_EntityRuleSpecifications_DeletedById",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_EntityRuleSpecifications_UpdatedById",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropIndex(
                name: "IX_EntityRules_CreatedById",
                table: "EntityRules");

            migrationBuilder.DropIndex(
                name: "IX_EntityRules_DeletedById",
                table: "EntityRules");

            migrationBuilder.DropIndex(
                name: "IX_EntityRules_UpdatedById",
                table: "EntityRules");

            migrationBuilder.DropIndex(
                name: "IX_EntityHolidays_CreatedById",
                table: "EntityHolidays");

            migrationBuilder.DropIndex(
                name: "IX_EntityHolidays_DeletedById",
                table: "EntityHolidays");

            migrationBuilder.DropIndex(
                name: "IX_EntityHolidays_UpdatedById",
                table: "EntityHolidays");

            migrationBuilder.DropIndex(
                name: "IX_Entities_CreatedById",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_DeletedById",
                table: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Entities_UpdatedById",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "WorkerEntityAbsences");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "UserBots");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserBots");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "UserBots");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserBots");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserBots");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "UserBots");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ShiftBreaks");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ShiftBreaks");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ShiftBreaks");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ShiftBreaks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShiftBreaks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ShiftBreaks");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "ShiftBreaks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "ScheduleEntryWorkers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "ScheduleEntryWorkerIneligibilities");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ScheduleEntryBots");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ScheduleEntryBots");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ScheduleEntryBots");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ScheduleEntryBots");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ScheduleEntryBots");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ScheduleEntryBots");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "ScheduleEntryBots");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "scheduleEntryBotIneligibilities");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ScheduleEntry");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ScheduleEntry");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ScheduleEntry");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "ScheduleEntry");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ScheduleEntry");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ScheduleEntry");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "ScheduleEntry");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityWorkerSkills");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityWorkerSkills");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityWorkerSkills");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityWorkerSkills");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityWorkerSkills");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityWorkerSkills");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityWorkerSkills");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "ConvertedAt",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "ConvertedBy",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "ConvertedFromBotId",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityUserBotSkills");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityUserBotSkills");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityUserBotSkills");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityUserBotSkills");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityUserBotSkills");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityUserBotSkills");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityUserBotSkills");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityUserBots");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityShiftRotations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityShiftRotations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityShiftRotations");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityShiftRotations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityShiftRotations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityShiftRotations");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityShiftRotations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityRules");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityRules");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityRules");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityRules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityRules");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityRules");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityRules");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EntityHolidays");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EntityHolidays");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EntityHolidays");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "EntityHolidays");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EntityHolidays");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityHolidays");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "EntityHolidays");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Entities");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "UserBots",
                newName: "DateOfCreation");
        }
    }
}
