using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntityRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaseEntityRules",
                columns: table => new
                {
                    BaseEntityRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntityRules", x => x.BaseEntityRuleId);
                    table.ForeignKey(
                        name: "FK_BaseEntityRules_RuleTypes_RuleTypeId",
                        column: x => x.RuleTypeId,
                        principalTable: "RuleTypes",
                        principalColumn: "RuleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaseEntityRuleSpecifications",
                columns: table => new
                {
                    BaseEntityRuleId = table.Column<int>(type: "int", nullable: false),
                    SpecificationId = table.Column<int>(type: "int", nullable: false),
                    SpecificationValue = table.Column<int>(type: "int", nullable: false),
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntityRuleSpecifications", x => new { x.BaseEntityRuleId, x.SpecificationId });
                    table.ForeignKey(
                        name: "FK_BaseEntityRuleSpecifications_BaseEntityRules_BaseEntityRuleId",
                        column: x => x.BaseEntityRuleId,
                        principalTable: "BaseEntityRules",
                        principalColumn: "BaseEntityRuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseEntityRules_RuleTypeId",
                table: "BaseEntityRules",
                column: "RuleTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseEntityRuleSpecifications");

            migrationBuilder.DropTable(
                name: "BaseEntityRules");
        }
    }
}
