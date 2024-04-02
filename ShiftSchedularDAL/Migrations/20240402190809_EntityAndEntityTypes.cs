using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class EntityAndEntityTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityTypes",
                columns: table => new
                {
                    EntityTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityTypeValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.EntityTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.EntityId);
                    table.ForeignKey(
                        name: "FK_Entities_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "EntityTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityTypeLocalizations",
                columns: table => new
                {
                    EntityTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    EntityTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypeLocalizations", x => new { x.EntityTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_EntityTypeLocalizations_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "EntityTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityWorkers",
                columns: table => new
                {
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WorkerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActiveWorkerStatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityWorkers", x => new { x.EntityId, x.WorkerId });
                    table.ForeignKey(
                        name: "FK_EntityWorkers_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityWorkers_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Workers",
                        principalColumn: "WorkerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entities_EntityTypeId",
                table: "Entities",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityTypeLocalizations_LocalizationId",
                table: "EntityTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_WorkerId",
                table: "EntityWorkers",
                column: "WorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityTypeLocalizations");

            migrationBuilder.DropTable(
                name: "EntityWorkers");

            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.DropTable(
                name: "EntityTypes");
        }
    }
}
