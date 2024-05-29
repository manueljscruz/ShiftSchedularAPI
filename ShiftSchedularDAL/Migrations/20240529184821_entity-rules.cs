using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class entityrules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessAspects",
                columns: table => new
                {
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessAspectName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessAspects", x => x.BusinessAspectId);
                });

            migrationBuilder.CreateTable(
                name: "RuleTypes",
                columns: table => new
                {
                    RuleTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleTypes", x => x.RuleTypeId);
                });

            migrationBuilder.CreateTable(
                name: "BusinessAspectLocalizations",
                columns: table => new
                {
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    BusinessAspectDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessAspectLocalizations", x => new { x.BusinessAspectId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_BusinessAspectLocalizations_BusinessAspects_BusinessAspectId",
                        column: x => x.BusinessAspectId,
                        principalTable: "BusinessAspects",
                        principalColumn: "BusinessAspectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessAspectLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityRules",
                columns: table => new
                {
                    EntityRuleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RuleTypeId = table.Column<int>(type: "int", nullable: false),
                    RuleTypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRules", x => x.EntityRuleId);
                    table.ForeignKey(
                        name: "FK_EntityRules_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityRules_RuleTypes_RuleTypeId",
                        column: x => x.RuleTypeId,
                        principalTable: "RuleTypes",
                        principalColumn: "RuleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RuleTypeLocalizations",
                columns: table => new
                {
                    RuleTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    RuleTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleTypeLocalizations", x => new { x.RuleTypeId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_RuleTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleTypeLocalizations_RuleTypes_RuleTypeId",
                        column: x => x.RuleTypeId,
                        principalTable: "RuleTypes",
                        principalColumn: "RuleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityRuleSpecifications",
                columns: table => new
                {
                    EntityRuleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpecificationId = table.Column<int>(type: "int", nullable: false),
                    SpecificationValue = table.Column<int>(type: "int", nullable: false),
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false),
                    AspectReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRuleSpecifications", x => new { x.EntityRuleId, x.SpecificationId });
                    table.ForeignKey(
                        name: "FK_EntityRuleSpecifications_BusinessAspects_BusinessAspectId",
                        column: x => x.BusinessAspectId,
                        principalTable: "BusinessAspects",
                        principalColumn: "BusinessAspectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityRuleSpecifications_EntityRules_EntityRuleId",
                        column: x => x.EntityRuleId,
                        principalTable: "EntityRules",
                        principalColumn: "EntityRuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessAspectLocalizations_LocalizationId",
                table: "BusinessAspectLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRules_EntityId",
                table: "EntityRules",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRules_RuleTypeId",
                table: "EntityRules",
                column: "RuleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRuleSpecifications_BusinessAspectId",
                table: "EntityRuleSpecifications",
                column: "BusinessAspectId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleTypeLocalizations_LocalizationId",
                table: "RuleTypeLocalizations",
                column: "LocalizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessAspectLocalizations");

            migrationBuilder.DropTable(
                name: "EntityRuleSpecifications");

            migrationBuilder.DropTable(
                name: "RuleTypeLocalizations");

            migrationBuilder.DropTable(
                name: "BusinessAspects");

            migrationBuilder.DropTable(
                name: "EntityRules");

            migrationBuilder.DropTable(
                name: "RuleTypes");
        }
    }
}
