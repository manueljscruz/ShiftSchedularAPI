using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class UserNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationTypes",
                columns: table => new
                {
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationTypeCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypes", x => x.NotificationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTypeLocalizations",
                columns: table => new
                {
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    LocalizationId = table.Column<int>(type: "int", nullable: false),
                    NotificationTypeDisplayValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageTemplate = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypeLocalizations", x => new { x.LocalizationId, x.NotificationTypeId });
                    table.ForeignKey(
                        name: "FK_NotificationTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "LocalizationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationTypeLocalizations_NotificationTypes_NotificationTypeId",
                        column: x => x.NotificationTypeId,
                        principalTable: "NotificationTypes",
                        principalColumn: "NotificationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    UserNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    ContextData = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RelatedEntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.UserNotificationId);
                    table.ForeignKey(
                        name: "FK_UserNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNotifications_NotificationTypes_NotificationTypeId",
                        column: x => x.NotificationTypeId,
                        principalTable: "NotificationTypes",
                        principalColumn: "NotificationTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "NotificationTypes",
                columns: new[] { "NotificationTypeId", "NotificationTypeCode" },
                values: new object[,]
                {
                    { 1, "InvitationReceived" },
                    { 2, "AbsenceApproved" },
                    { 3, "AbsenceDeclined" },
                    { 4, "ScheduleAssigned" },
                    { 5, "ExitDateSet" }
                });

            migrationBuilder.InsertData(
                table: "NotificationTypeLocalizations",
                columns: new[] { "LocalizationId", "NotificationTypeId", "MessageTemplate", "NotificationTypeDisplayValue" },
                values: new object[,]
                {
                    { 1, 1, "You have been invited to join {0}.", "Invitation Received" },
                    { 1, 2, "Your absence request has been approved.", "Absence Approved" },
                    { 1, 3, "Your absence request has been declined.", "Absence Declined" },
                    { 1, 4, "You have been assigned to a shift at {0}.", "Shift Assigned" },
                    { 1, 5, "Your exit date at {0} has been scheduled.", "Exit Date Set" },
                    { 2, 1, "Vous avez été invité à rejoindre {0}.", "Invitation reçue" },
                    { 2, 2, "Votre demande d'absence a été approuvée.", "Absence approuvée" },
                    { 2, 3, "Votre demande d'absence a été refusée.", "Absence refusée" },
                    { 2, 4, "Vous avez été affecté à un quart de travail chez {0}.", "Quart attribué" },
                    { 2, 5, "Votre date de sortie de {0} a été planifiée.", "Date de sortie fixée" },
                    { 3, 1, "Has sido invitado a unirte a {0}.", "Invitación recibida" },
                    { 3, 2, "Tu solicitud de ausencia ha sido aprobada.", "Ausencia aprobada" },
                    { 3, 3, "Tu solicitud de ausencia ha sido rechazada.", "Ausencia rechazada" },
                    { 3, 4, "Has sido asignado a un turno en {0}.", "Turno asignado" },
                    { 3, 5, "Tu fecha de salida en {0} ha sido programada.", "Fecha de salida establecida" },
                    { 4, 1, "Sie wurden eingeladen, {0} beizutreten.", "Einladung erhalten" },
                    { 4, 2, "Ihr Abwesenheitsantrag wurde genehmigt.", "Abwesenheit genehmigt" },
                    { 4, 3, "Ihr Abwesenheitsantrag wurde abgelehnt.", "Abwesenheit abgelehnt" },
                    { 4, 4, "Sie wurden einer Schicht bei {0} zugeteilt.", "Schicht zugewiesen" },
                    { 4, 5, "Ihr Austrittsdatum bei {0} wurde festgelegt.", "Austrittsdatum festgelegt" },
                    { 5, 1, "Sei stato invitato a unirti a {0}.", "Invito ricevuto" },
                    { 5, 2, "La tua richiesta di assenza è stata approvata.", "Assenza approvata" },
                    { 5, 3, "La tua richiesta di assenza è stata rifiutata.", "Assenza rifiutata" },
                    { 5, 4, "Sei stato assegnato a un turno presso {0}.", "Turno assegnato" },
                    { 5, 5, "La tua data di uscita da {0} è stata pianificata.", "Data di uscita stabilita" },
                    { 6, 1, "Você foi convidado para se juntar a {0}.", "Convite recebido" },
                    { 6, 2, "A sua solicitação de ausência foi aprovada.", "Ausência aprovada" },
                    { 6, 3, "A sua solicitação de ausência foi recusada.", "Ausência recusada" },
                    { 6, 4, "Você foi atribuído a um turno em {0}.", "Turno atribuído" },
                    { 6, 5, "A sua data de saída em {0} foi agendada.", "Data de saída definida" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTypeLocalizations_NotificationTypeId",
                table: "NotificationTypeLocalizations",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_NotificationTypeId",
                table: "UserNotifications",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId",
                table: "UserNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId_IsRead",
                table: "UserNotifications",
                columns: new[] { "UserId", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationTypeLocalizations");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "NotificationTypes");
        }
    }
}
