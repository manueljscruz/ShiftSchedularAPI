using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftSchedularDAL.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUniqueEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
