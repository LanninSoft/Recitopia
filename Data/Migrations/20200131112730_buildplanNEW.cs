using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class buildplanNEW : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildPlan",
                columns: table => new
                {
                    BuildPlan_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plan_Name = table.Column<string>(nullable: false),
                    PlanDate = table.Column<DateTime>(nullable: false),
                    NeedByDate = table.Column<DateTime>(nullable: false),
                    FullFilled = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    LastModified = table.Column<DateTime>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    Customer_Guid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildPlan", x => x.BuildPlan_Id);
                });

            migrationBuilder.CreateTable(
                name: "BuildPlan_Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Recipe_Id = table.Column<int>(nullable: false),
                    BuildPlan_Id = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    Customer_Guid = table.Column<string>(nullable: true),
                    Recipe_Name = table.Column<string>(nullable: true),
                    BuildPlan_Id1 = table.Column<int>(nullable: true),
                    Recipe_Id1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildPlan_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuildPlan_Recipes_BuildPlan_BuildPlan_Id1",
                        column: x => x.BuildPlan_Id1,
                        principalTable: "BuildPlan",
                        principalColumn: "BuildPlan_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuildPlan_Recipes_Recipe_Recipe_Id1",
                        column: x => x.Recipe_Id1,
                        principalTable: "Recipe",
                        principalColumn: "Recipe_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlan_Recipes_BuildPlan_Id1",
                table: "BuildPlan_Recipes",
                column: "BuildPlan_Id1");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPlan_Recipes_Recipe_Id1",
                table: "BuildPlan_Recipes",
                column: "Recipe_Id1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildPlan_Recipes");

            migrationBuilder.DropTable(
                name: "BuildPlan");
        }
    }
}
