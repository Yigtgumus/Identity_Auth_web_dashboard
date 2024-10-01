using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity_Auth.Migrations
{
    /// <inheritdoc />
    public partial class addedConnectionIDtoGraphTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Row",
                table: "Graphics",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ConnectionId",
                table: "Graphics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Graphics_ConnectionId",
                table: "Graphics",
                column: "ConnectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Graphics_UserDatabaseConnections_ConnectionId",
                table: "Graphics",
                column: "ConnectionId",
                principalTable: "UserDatabaseConnections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Graphics_UserDatabaseConnections_ConnectionId",
                table: "Graphics");

            migrationBuilder.DropIndex(
                name: "IX_Graphics_ConnectionId",
                table: "Graphics");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Graphics");

            migrationBuilder.AlterColumn<string>(
                name: "Row",
                table: "Graphics",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
