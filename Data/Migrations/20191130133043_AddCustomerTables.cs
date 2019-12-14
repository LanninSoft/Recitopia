using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class AddCustomerTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Customer_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer_Name = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Zip = table.Column<int>(nullable: true),
                    Web_URL = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Customer_Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer_Users",
                columns: table => new
                {
                    CU_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer_Id = table.Column<int>(nullable: false),
                    Id = table.Column<string>(maxLength: 450, nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    CustomersCustomer_Id = table.Column<int>(nullable: true),
                    AppUsersId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer_Users", x => x.CU_Id);
                    //table.ForeignKey(
                    //    name: "FK_Customer_Users_AspNetUsers_AppUsersId",
                    //    column: x => x.AppUsersId,
                    //    principalTable: "AspNetUsers",
                    //    principalColumn: "Id",
                    //    onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customer_Users_Customers_Id",
                        column: x => x.Customer_Id,
                        principalTable: "Customers",
                        principalColumn: "Customer_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Users_AppUsersId",
                table: "Customer_Users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Users_CustomersCustomer_Id",
                table: "Customer_Users",
                column: "CustomersCustomer_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer_Users");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
