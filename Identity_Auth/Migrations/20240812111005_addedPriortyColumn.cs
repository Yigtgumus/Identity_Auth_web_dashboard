using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity_Auth.Migrations
{
    /// <inheritdoc />
    public partial class addedPriortyColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Priorty",
                table: "Graphics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priorty",
                table: "Graphics");
        }
    }
}
