using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Migrations
{
    public partial class addCustomerIDtoASPNetUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Customer_Id",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "AspNetUsers");
        }
    }
}
