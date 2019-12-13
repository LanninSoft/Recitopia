using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Migrations
{
    public partial class updateASPNetUserAddCustName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Customer_Name",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer_Name",
                table: "AspNetUsers");
        }
    }
}
