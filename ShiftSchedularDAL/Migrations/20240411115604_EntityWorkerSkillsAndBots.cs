using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class EntityWorkerSkillsAndBots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityWorkers",
                table: "EntityWorkers");

            migrationBuilder.AddColumn<bool>(
                name: "IsBot",
                table: "Workers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                table: "EntityWorkers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityWorkers",
                table: "EntityWorkers",
                columns: new[] { "EntityId", "WorkerId", "SkillId" });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillId);
                });

            migrationBuilder.CreateTable(
                name: "SkillLocalizations",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    SkillDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillLocalizations", x => new { x.SkillId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_SkillLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillLocalizations_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkers_SkillId",
                table: "EntityWorkers",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillLocalizations_LocalizationId",
                table: "SkillLocalizations",
                column: "LocalizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityWorkers_Skills_SkillId",
                table: "EntityWorkers",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityWorkers_Skills_SkillId",
                table: "EntityWorkers");

            migrationBuilder.DropTable(
                name: "SkillLocalizations");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityWorkers",
                table: "EntityWorkers");

            migrationBuilder.DropIndex(
                name: "IX_EntityWorkers_SkillId",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "IsBot",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "EntityWorkers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityWorkers",
                table: "EntityWorkers",
                columns: new[] { "EntityId", "WorkerId" });
        }
    }
}
