using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class EntityWorkerAbsences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbsenceTypes",
                columns: table => new
                {
                    AbsenceTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AbsenceTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsenceTypes", x => x.AbsenceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AbsenceTypeLocalizations",
                columns: table => new
                {
                    AbsenceTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    AbsenceTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsenceTypeLocalizations", x => new { x.AbsenceTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_AbsenceTypeLocalizations_AbsenceTypes_AbsenceTypeId",
                        column: x => x.AbsenceTypeId,
                        principalTable: "AbsenceTypes",
                        principalColumn: "AbsenceTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbsenceTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerEntityAbsences",
                columns: table => new
                {
                    EntityWorkerAbsenceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AbsenceTypeId = table.Column<int>(type: "int", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AbsenceStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsenceEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsenceApproved = table.Column<bool>(type: "bit", nullable: false),
                    AbsenceDateDecision = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerEntityAbsences", x => x.EntityWorkerAbsenceId);
                    table.ForeignKey(
                        name: "FK_WorkerEntityAbsences_AbsenceTypes_AbsenceTypeId",
                        column: x => x.AbsenceTypeId,
                        principalTable: "AbsenceTypes",
                        principalColumn: "AbsenceTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerEntityAbsences_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerEntityAbsences_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Workers",
                        principalColumn: "WorkerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceTypeLocalizations_LocalizationId",
                table: "AbsenceTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEntityAbsences_AbsenceTypeId",
                table: "WorkerEntityAbsences",
                column: "AbsenceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEntityAbsences_EntityId",
                table: "WorkerEntityAbsences",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerEntityAbsences_WorkerId",
                table: "WorkerEntityAbsences",
                column: "WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbsenceTypeLocalizations");

            migrationBuilder.DropTable(
                name: "WorkerEntityAbsences");

            migrationBuilder.DropTable(
                name: "AbsenceTypes");
        }
    }
}
