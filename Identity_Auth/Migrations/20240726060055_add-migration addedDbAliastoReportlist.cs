using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity_Auth.Migrations
{
    /// <inheritdoc />
    public partial class addmigrationaddedDbAliastoReportlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionAlias",
                table: "UserReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserReports_ConnectionId",
                table: "UserReports",
                column: "ConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReports_UserDatabaseConnections_ConnectionId",
                table: "UserReports",
                column: "ConnectionId",
                principalTable: "UserDatabaseConnections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserReports_UserDatabaseConnections_ConnectionId",
                table: "UserReports");

            migrationBuilder.DropIndex(
                name: "IX_UserReports_ConnectionId",
                table: "UserReports");

            migrationBuilder.DropColumn(
                name: "ConnectionAlias",
                table: "UserReports");
        }
    }
}
