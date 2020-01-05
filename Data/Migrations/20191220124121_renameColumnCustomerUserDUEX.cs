using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class renameColumnCustomerUserDUEX : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer_Users",
                table: "Customer_Users");

            migrationBuilder.DropColumn(
                name: "CU_Id",
                table: "Customer_Users");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Customer_Users",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer_Users",
                table: "Customer_Users",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer_Users",
                table: "Customer_Users");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Customer_Users");

            migrationBuilder.AddColumn<int>(
                name: "CU_Id",
                table: "Customer_Users",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer_Users",
                table: "Customer_Users",
                column: "CU_Id");
        }
    }
}
