using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class ModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Customer_Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Users_AppUserId",
                table: "Customer_Users",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Users_AspNetUsers_AppUserId",
                table: "Customer_Users",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Users_AspNetUsers_AppUserId",
                table: "Customer_Users");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Users_AppUserId",
                table: "Customer_Users");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Customer_Users");
        }
    }
}
