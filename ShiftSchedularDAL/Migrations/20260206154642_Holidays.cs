using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class Holidays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HolidayBehaviours",
                columns: table => new
                {
                    HolidayBehaviourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayBehaviourName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayBehaviours", x => x.HolidayBehaviourId);
                });

            migrationBuilder.CreateTable(
                name: "HolidayTypes",
                columns: table => new
                {
                    HolidayTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayTypes", x => x.HolidayTypeId);
                });

            migrationBuilder.CreateTable(
                name: "HolidayBehaviourLocalizations",
                columns: table => new
                {
                    HolidayBehaviourId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    HolidayBehaviourDisplayValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayBehaviourLocalizations", x => new { x.HolidayBehaviourId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_HolidayBehaviourLocalizations_HolidayBehaviours_HolidayBehaviourId",
                        column: x => x.HolidayBehaviourId,
                        principalTable: "HolidayBehaviours",
                        principalColumn: "HolidayBehaviourId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HolidayBehaviourLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HolidayCatalogs",
                columns: table => new
                {
                    HolidayCatalogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HolidayTypeId = table.Column<int>(type: "int", nullable: false),
                    HolidayBehaviourId = table.Column<int>(type: "int", nullable: false),
                    HolidayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HolidayDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RecurrenceDay = table.Column<int>(type: "int", nullable: false),
                    RecurrenceMonth = table.Column<int>(type: "int", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayCatalogs", x => x.HolidayCatalogId);
                    table.ForeignKey(
                        name: "FK_HolidayCatalogs_HolidayBehaviours_HolidayBehaviourId",
                        column: x => x.HolidayBehaviourId,
                        principalTable: "HolidayBehaviours",
                        principalColumn: "HolidayBehaviourId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HolidayCatalogs_HolidayTypes_HolidayTypeId",
                        column: x => x.HolidayTypeId,
                        principalTable: "HolidayTypes",
                        principalColumn: "HolidayTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HolidayTypeLocalizations",
                columns: table => new
                {
                    HolidayTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    HolidayTypeDisplayValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayTypeLocalizations", x => new { x.HolidayTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_HolidayTypeLocalizations_HolidayTypes_HolidayTypeId",
                        column: x => x.HolidayTypeId,
                        principalTable: "HolidayTypes",
                        principalColumn: "HolidayTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HolidayTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityHolidays",
                columns: table => new
                {
                    EntityHolidayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    HolidayCatalogId = table.Column<int>(type: "int", nullable: true),
                    HolidayBehaviourId = table.Column<int>(type: "int", nullable: false),
                    CustomHolidayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomDay = table.Column<int>(type: "int", nullable: false),
                    CustomMonth = table.Column<int>(type: "int", nullable: false),
                    OperatingStartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    OperatingEndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityHolidays", x => x.EntityHolidayId);
                    table.ForeignKey(
                        name: "FK_EntityHolidays_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityHolidays_HolidayBehaviours_HolidayBehaviourId",
                        column: x => x.HolidayBehaviourId,
                        principalTable: "HolidayBehaviours",
                        principalColumn: "HolidayBehaviourId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityHolidays_HolidayCatalogs_HolidayCatalogId",
                        column: x => x.HolidayCatalogId,
                        principalTable: "HolidayCatalogs",
                        principalColumn: "HolidayCatalogId");
                });

            migrationBuilder.CreateTable(
                name: "HolidayCatalogLocalizations",
                columns: table => new
                {
                    HolidayCatalogId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    LocalizedName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LocalizedDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurrenceMonth = table.Column<int>(type: "int", nullable: false),
                    RecurrenceDay = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayCatalogLocalizations", x => new { x.HolidayCatalogId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_HolidayCatalogLocalizations_HolidayCatalogs_HolidayCatalogId",
                        column: x => x.HolidayCatalogId,
                        principalTable: "HolidayCatalogs",
                        principalColumn: "HolidayCatalogId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HolidayCatalogLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityHolidays_EntityId",
                table: "EntityHolidays",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityHolidays_EntityId_CustomDate",
                table: "EntityHolidays",
                columns: new[] { "EntityId", "CustomMonth", "CustomDay" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityHolidays_HolidayBehaviourId",
                table: "EntityHolidays",
                column: "HolidayBehaviourId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityHolidays_HolidayCatalogId",
                table: "EntityHolidays",
                column: "HolidayCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayBehaviourLocalizations_LocalizationId",
                table: "HolidayBehaviourLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCatalogLocalizations_LocalizationId",
                table: "HolidayCatalogLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCatalogs_HolidayBehaviourId",
                table: "HolidayCatalogs",
                column: "HolidayBehaviourId");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCatalogs_HolidayTypeId",
                table: "HolidayCatalogs",
                column: "HolidayTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HolidayCatalogs_RecurrenceDate",
                table: "HolidayCatalogs",
                columns: new[] { "RecurrenceMonth", "RecurrenceDay" });

            migrationBuilder.CreateIndex(
                name: "IX_HolidayTypeLocalizations_LocalizationId",
                table: "HolidayTypeLocalizations",
                column: "LocalizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityHolidays");

            migrationBuilder.DropTable(
                name: "HolidayBehaviourLocalizations");

            migrationBuilder.DropTable(
                name: "HolidayCatalogLocalizations");

            migrationBuilder.DropTable(
                name: "HolidayTypeLocalizations");

            migrationBuilder.DropTable(
                name: "HolidayCatalogs");

            migrationBuilder.DropTable(
                name: "HolidayBehaviours");

            migrationBuilder.DropTable(
                name: "HolidayTypes");
        }
    }
}
