using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class renameColumnCustomerUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Customer_Users");

            migrationBuilder.AddColumn<string>(
                name: "User_Id",
                table: "Customer_Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User_Id",
                table: "Customer_Users");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Customer_Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
