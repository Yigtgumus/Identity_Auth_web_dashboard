using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity_Auth.Migrations
{
    /// <inheritdoc />
    public partial class InitialAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserDatabaseConnections",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserDatabaseConnections_UserId",
                table: "UserDatabaseConnections",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDatabaseConnections_AspNetUsers_UserId",
                table: "UserDatabaseConnections",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDatabaseConnections_AspNetUsers_UserId",
                table: "UserDatabaseConnections");

            migrationBuilder.DropIndex(
                name: "IX_UserDatabaseConnections_UserId",
                table: "UserDatabaseConnections");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserDatabaseConnections");
        }
    }
}
