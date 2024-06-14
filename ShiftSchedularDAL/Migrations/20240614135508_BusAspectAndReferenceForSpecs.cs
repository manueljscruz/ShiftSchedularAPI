using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class BusAspectAndReferenceForSpecs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the foreign key constraint exists before trying to drop it
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 
                           FROM sys.foreign_keys 
                           WHERE parent_object_id = OBJECT_ID('EntityRuleSpecifications') 
                           AND name = 'FK_EntityRuleSpecifications_BusinessAspects_BusinessAspectId')
                BEGIN
                    ALTER TABLE [EntityRuleSpecifications] DROP CONSTRAINT [FK_EntityRuleSpecifications_BusinessAspects_BusinessAspectId]
                END
            ");

            // Check if the index exists before trying to drop it
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 
                           FROM sys.indexes 
                           WHERE object_id = OBJECT_ID('EntityRuleSpecifications') 
                           AND name = 'IX_EntityRuleSpecifications_BusinessAspectId')
                BEGIN
                    DROP INDEX [IX_EntityRuleSpecifications_BusinessAspectId] ON [EntityRuleSpecifications]
                END
            ");

            migrationBuilder.AddColumn<bool>(
                name: "MultipleSpecification",
                table: "RuleTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "BusinessAspectId",
                table: "EntityRuleSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspectReferenceId2",
                table: "EntityRuleSpecifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BusinessAspectId2",
                table: "EntityRuleSpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultipleSpecification",
                table: "RuleTypes");

            migrationBuilder.DropColumn(
                name: "AspectReferenceId2",
                table: "EntityRuleSpecifications");

            migrationBuilder.DropColumn(
                name: "BusinessAspectId2",
                table: "EntityRuleSpecifications");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessAspectId",
                table: "EntityRuleSpecifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 
                               FROM sys.indexes 
                               WHERE object_id = OBJECT_ID('EntityRuleSpecifications') 
                               AND name = 'IX_EntityRuleSpecifications_BusinessAspectId')
                BEGIN
                    CREATE INDEX [IX_EntityRuleSpecifications_BusinessAspectId] 
                    ON [EntityRuleSpecifications]([BusinessAspectId])
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 
                               FROM sys.foreign_keys 
                               WHERE parent_object_id = OBJECT_ID('EntityRuleSpecifications') 
                               AND name = 'FK_EntityRuleSpecifications_BusinessAspects_BusinessAspectId')
                BEGIN
                    ALTER TABLE [EntityRuleSpecifications]
                    ADD CONSTRAINT [FK_EntityRuleSpecifications_BusinessAspects_BusinessAspectId] 
                    FOREIGN KEY ([BusinessAspectId]) 
                    REFERENCES [BusinessAspects]([BusinessAspectId])
                END
            ");
        }
    }
}
