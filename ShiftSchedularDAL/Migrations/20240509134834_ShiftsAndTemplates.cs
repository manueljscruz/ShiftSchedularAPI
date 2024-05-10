using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class ShiftsAndTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShiftBreakTypes",
                columns: table => new
                {
                    ShiftBreakTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftBreakTypeValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBreakTypes", x => x.ShiftBreakTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    ShiftId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftAlias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftStartHour = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftDuration = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.ShiftId);
                    table.ForeignKey(
                        name: "FK_Shifts_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTemplates",
                columns: table => new
                {
                    ShiftTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftAlias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftStartHour = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    TemplateClicks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTemplates", x => x.ShiftTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "ShiftBreakTemplates",
                columns: table => new
                {
                    ShiftBreakTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftBreakTypeId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftBreakDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    IncludedInShift = table.Column<bool>(type: "bit", nullable: false),
                    IsTimeFlexible = table.Column<bool>(type: "bit", nullable: false),
                    TemplateClicks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBreakTemplates", x => x.ShiftBreakTemplateId);
                    table.ForeignKey(
                        name: "FK_ShiftBreakTemplates_ShiftBreakTypes_ShiftBreakTypeId",
                        column: x => x.ShiftBreakTypeId,
                        principalTable: "ShiftBreakTypes",
                        principalColumn: "ShiftBreakTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftBreakTypeLocalizations",
                columns: table => new
                {
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakTypeId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBreakTypeLocalizations", x => new { x.ShiftBreakTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_ShiftBreakTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftBreakTypeLocalizations_ShiftBreakTypes_ShiftBreakTypeId",
                        column: x => x.ShiftBreakTypeId,
                        principalTable: "ShiftBreakTypes",
                        principalColumn: "ShiftBreakTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftBreaks",
                columns: table => new
                {
                    ShiftBreakId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShiftId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShiftBreakTypeId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftBreakDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    IncludedInShift = table.Column<bool>(type: "bit", nullable: false),
                    IsTimeFlexible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftBreaks", x => x.ShiftBreakId);
                    table.ForeignKey(
                        name: "FK_ShiftBreaks_ShiftBreakTypes_ShiftBreakTypeId",
                        column: x => x.ShiftBreakTypeId,
                        principalTable: "ShiftBreakTypes",
                        principalColumn: "ShiftBreakTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftBreaks_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTemplateBreaks",
                columns: table => new
                {
                    ShiftTemplateId = table.Column<int>(type: "int", nullable: false),
                    ShiftBreakTemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTemplateBreaks", x => new { x.ShiftTemplateId, x.ShiftBreakTemplateId });
                    table.ForeignKey(
                        name: "FK_ShiftTemplateBreaks_ShiftBreakTemplates_ShiftBreakTemplateId",
                        column: x => x.ShiftBreakTemplateId,
                        principalTable: "ShiftBreakTemplates",
                        principalColumn: "ShiftBreakTemplateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftTemplateBreaks_ShiftTemplates_ShiftTemplateId",
                        column: x => x.ShiftTemplateId,
                        principalTable: "ShiftTemplates",
                        principalColumn: "ShiftTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreaks_ShiftBreakTypeId",
                table: "ShiftBreaks",
                column: "ShiftBreakTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreaks_ShiftId",
                table: "ShiftBreaks",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreakTemplates_ShiftBreakTypeId",
                table: "ShiftBreakTemplates",
                column: "ShiftBreakTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftBreakTypeLocalizations_LocalizationId",
                table: "ShiftBreakTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_EntityId",
                table: "Shifts",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftTemplateBreaks_ShiftBreakTemplateId",
                table: "ShiftTemplateBreaks",
                column: "ShiftBreakTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftBreaks");

            migrationBuilder.DropTable(
                name: "ShiftBreakTypeLocalizations");

            migrationBuilder.DropTable(
                name: "ShiftTemplateBreaks");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "ShiftBreakTemplates");

            migrationBuilder.DropTable(
                name: "ShiftTemplates");

            migrationBuilder.DropTable(
                name: "ShiftBreakTypes");
        }
    }
}
