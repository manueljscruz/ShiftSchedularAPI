using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class NewRuleTypeBusinessAspectRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityRuleSpecifications_BusinessAspects_BusinessAspectId",
                table: "EntityRuleSpecifications");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessAspectId",
                table: "EntityRuleSpecifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "RuleTypeBusinessAspects",
                columns: table => new
                {
                    RuleTypeId = table.Column<int>(type: "int", nullable: false),
                    BusinessAspectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleTypeBusinessAspects", x => new { x.RuleTypeId, x.BusinessAspectId });
                    table.ForeignKey(
                        name: "FK_RuleTypeBusinessAspects_BusinessAspects_BusinessAspectId",
                        column: x => x.BusinessAspectId,
                        principalTable: "BusinessAspects",
                        principalColumn: "BusinessAspectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleTypeBusinessAspects_RuleTypes_RuleTypeId",
                        column: x => x.RuleTypeId,
                        principalTable: "RuleTypes",
                        principalColumn: "RuleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RuleTypeBusinessAspects_BusinessAspectId",
                table: "RuleTypeBusinessAspects",
                column: "BusinessAspectId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityRuleSpecifications_BusinessAspects_BusinessAspectId",
                table: "EntityRuleSpecifications",
                column: "BusinessAspectId",
                principalTable: "BusinessAspects",
                principalColumn: "BusinessAspectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityRuleSpecifications_BusinessAspects_BusinessAspectId",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropTable(
                name: "RuleTypeBusinessAspects");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessAspectId",
                table: "EntityRuleSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityRuleSpecifications_BusinessAspects_BusinessAspectId",
                table: "EntityRuleSpecifications",
                column: "BusinessAspectId",
                principalTable: "BusinessAspects",
                principalColumn: "BusinessAspectId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
