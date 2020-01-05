using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class recipemodelupdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category_Name",
                table: "Recipe",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SS_Name",
                table: "Recipe",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category_Name",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "SS_Name",
                table: "Recipe");
        }
    }
}
