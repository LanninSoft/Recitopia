using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Migrations
{
    public partial class AddCustomerTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "Groups",
               columns: table => new
               {
                   Group_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                   Group_Name = table.Column<string>(maxLength: 50, nullable: false),
                   Phone = table.Column<string>(maxLength: 15, nullable: true),
                   Email = table.Column<string>(maxLength: 50, nullable: true),
                   Address1 = table.Column<string>(maxLength: 50, nullable: true),
                   Address2 = table.Column<string>(maxLength: 50, nullable: true),
                   City = table.Column<string>(maxLength: 25, nullable: true),
                   State = table.Column<string>(maxLength: 10, nullable: true),
                   Zip = table.Column<int>(nullable: true),
                   Web_URL = table.Column<string>(maxLength: 50, nullable: true),
                   Notes = table.Column<string>(type: "text", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("FK_Group_ID_ToTable", x => x.Group_Id);
               });

            migrationBuilder.CreateTable(
                name: "Group_Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Group_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
                    User_Id = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Group_Users_ToTable",
                        column: x => x.Group_Id,
                        principalTable: "Groups",
                        principalColumn: "Group_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Group_Users_Group_Id",
                table: "Group_Users",
                column: "Group_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Group_Users");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
