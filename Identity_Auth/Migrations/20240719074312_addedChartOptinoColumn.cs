using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity_Auth.Migrations
{
    /// <inheritdoc />
    public partial class addedChartOptinoColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChartOptions",
                table: "Charts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChartOptions",
                table: "Charts");
        }
    }
}
