using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class UserBotTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBots",
                columns: table => new
                {
                    UserBotId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    UserDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBots", x => x.UserBotId);
                    table.ForeignKey(
                        name: "FK_UserBots_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBots_EntityId",
                table: "UserBots",
                column: "EntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBots");
        }
    }
}
