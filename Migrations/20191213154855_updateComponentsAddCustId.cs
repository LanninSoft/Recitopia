using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Migrations
{
    public partial class updateComponentsAddCustId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Components",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Components");
        }
    }
}
