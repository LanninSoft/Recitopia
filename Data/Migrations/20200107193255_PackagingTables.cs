using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class PackagingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Packaging",
                columns: table => new
                {
                    Package_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Package_Name = table.Column<string>(nullable: false),
                    Vendor_Id = table.Column<int>(nullable: false),
                    Vendor_Name = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Customer_Guid = table.Column<string>(nullable: true),
                    Vendor_Id1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packaging", x => x.Package_Id);
                    table.ForeignKey(
                        name: "FK_Packaging_Vendor_Vendor_Id1",
                        column: x => x.Vendor_Id1,
                        principalTable: "Vendor",
                        principalColumn: "Vendor_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recipe_Packaging",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Recipe_Id = table.Column<int>(nullable: false),
                    Package_Id = table.Column<int>(nullable: false),
                    Weight_g = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Customer_Guid = table.Column<string>(nullable: true),
                    PackagingPackage_Id = table.Column<int>(nullable: true),
                    Recipe_Id1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe_Packaging", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipe_Packaging_Packaging_PackagingPackage_Id",
                        column: x => x.PackagingPackage_Id,
                        principalTable: "Packaging",
                        principalColumn: "Package_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recipe_Packaging_Recipe_Recipe_Id1",
                        column: x => x.Recipe_Id1,
                        principalTable: "Recipe",
                        principalColumn: "Recipe_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Packaging_Vendor_Id1",
                table: "Packaging",
                column: "Vendor_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_Packaging_PackagingPackage_Id",
                table: "Recipe_Packaging",
                column: "PackagingPackage_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_Packaging_Recipe_Id1",
                table: "Recipe_Packaging",
                column: "Recipe_Id1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recipe_Packaging");

            migrationBuilder.DropTable(
                name: "Packaging");
        }
    }
}
