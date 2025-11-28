using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstaHyreSDETest.Migrations
{
    /// <inheritdoc />
    public partial class addNumberTypeflag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "flag",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "flag",
                table: "AspNetUsers");
        }
    }
}
