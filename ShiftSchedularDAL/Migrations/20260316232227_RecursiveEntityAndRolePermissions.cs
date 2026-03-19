using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class RecursiveEntityAndRolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveWorkerStatus",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "CanCreateSchedules",
                table: "EntityWorkers");

            migrationBuilder.DropColumn(
                name: "IsOwner",
                table: "EntityWorkers");

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentEntityId",
                table: "Entities",
                type: "BINARY(16)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntityPermissionRoles",
                columns: table => new
                {
                    EntityPermissionRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityPermissionRoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPermissionRoles", x => x.EntityPermissionRoleId);
                });

            migrationBuilder.CreateTable(
                name: "EntityPermissionRoleLocalizations",
                columns: table => new
                {
                    EntityPermissionRoleId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    EntityPermissionRoleDisplayValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPermissionRoleLocalizations", x => new { x.EntityPermissionRoleId, x.LocalizationId });
                    table.ForeignKey(
                        name: "FK_EntityPermissionRoleLocalizations_EntityPermissionRoles_EntityPermissionRoleId",
                        column: x => x.EntityPermissionRoleId,
                        principalTable: "EntityPermissionRoles",
                        principalColumn: "EntityPermissionRoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityPermissionRoleLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityPermissions",
                columns: table => new
                {
                    EntityId = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityPermissionRoleId = table.Column<int>(type: "int", nullable: false),
                    CanManageChildren = table.Column<bool>(type: "bit", nullable: false),
                    PartOfRoster = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPermissions", x => new { x.EntityId, x.ApplicationUserId, x.EntityPermissionRoleId });
                    table.ForeignKey(
                        name: "FK_EntityPermissions_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityPermissions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityPermissions_AspNetUsers_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityPermissions_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityPermissions_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "EntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityPermissions_EntityPermissionRoles_EntityPermissionRoleId",
                        column: x => x.EntityPermissionRoleId,
                        principalTable: "EntityPermissionRoles",
                        principalColumn: "EntityPermissionRoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entities_ParentEntityId",
                table: "Entities",
                column: "ParentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissionRoleLocalizations_LocalizationId",
                table: "EntityPermissionRoleLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissions_ApplicationUserId",
                table: "EntityPermissions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissions_CreatedById",
                table: "EntityPermissions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissions_DeletedById",
                table: "EntityPermissions",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissions_EntityId_RoleId",
                table: "EntityPermissions",
                columns: new[] { "EntityId", "EntityPermissionRoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissions_EntityPermissionRoleId",
                table: "EntityPermissions",
                column: "EntityPermissionRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPermissions_UpdatedById",
                table: "EntityPermissions",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Entities_Entities_ParentEntityId",
                table: "Entities",
                column: "ParentEntityId",
                principalTable: "Entities",
                principalColumn: "EntityId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entities_Entities_ParentEntityId",
                table: "Entities");

            migrationBuilder.DropTable(
                name: "EntityPermissionRoleLocalizations");

            migrationBuilder.DropTable(
                name: "EntityPermissions");

            migrationBuilder.DropTable(
                name: "EntityPermissionRoles");

            migrationBuilder.DropIndex(
                name: "IX_Entities_ParentEntityId",
                table: "Entities");

            migrationBuilder.DropColumn(
                name: "ParentEntityId",
                table: "Entities");

            migrationBuilder.AddColumn<bool>(
                name: "ActiveWorkerStatus",
                table: "EntityWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanCreateSchedules",
                table: "EntityWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOwner",
                table: "EntityWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
