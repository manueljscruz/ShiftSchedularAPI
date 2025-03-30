using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class ShiftRotationSpecAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PartOfRotation",
                table: "EntityWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PartOfRotation",
                table: "EntityWorkerInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PartOfRotation",
                table: "EntityUserBots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EntityUserBotShiftAssigneds",
                columns: table => new
                {
                    UserBotId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ShiftId = table.Column<byte[]>(type: "BINARY(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityUserBotShiftAssigneds", x => new { x.UserBotId, x.EntityId, x.ShiftId });
                    table.ForeignKey(
                        name: "FK_EntityUserBotShiftAssigneds_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityUserBotShiftAssigneds_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityUserBotShiftAssigneds_UserBots_UserBotId",
                        column: x => x.UserBotId,
                        principalTable: "UserBots",
                        principalColumn: "UserBotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityWorkerShiftAssigneds",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ShiftId = table.Column<byte[]>(type: "BINARY(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityWorkerShiftAssigneds", x => new { x.ApplicationUserId, x.EntityId, x.ShiftId });
                    table.ForeignKey(
                        name: "FK_EntityWorkerShiftAssigneds_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityWorkerShiftAssigneds_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityWorkerShiftAssigneds_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "ShiftId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotShiftAssigneds_EntityId",
                table: "EntityUserBotShiftAssigneds",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityUserBotShiftAssigneds_ShiftId",
                table: "EntityUserBotShiftAssigneds",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerShiftAssigneds_EntityId",
                table: "EntityWorkerShiftAssigneds",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityWorkerShiftAssigneds_ShiftId",
                table: "EntityWorkerShiftAssigneds",
                column: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityUserBotShiftAssigneds");

            migrationBuilder.DropTable(
                name: "EntityWorkerShiftAssigneds");

            migrationBuilder.DropColumn(
                name: "PartOfRotation",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "PartOfRotation",
                table: "EntityWorkerInvitations");

            migrationBuilder.DropColumn(
                name: "PartOfRotation",
                table: "EntityUserBots");
        }
    }
}
