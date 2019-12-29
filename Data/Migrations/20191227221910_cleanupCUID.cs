using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class cleanupCUID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Serving_Sizes");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Recipe_Ingredients");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Nutrition");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Meal_Category");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Ingredient_Nutrients");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Ingredient_Components");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Vendor",
                type: "int",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Serving_Sizes",
                type: "int",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Recipe_Ingredients",
                type: "int",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Recipe",
                type: "int",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Nutrition",
                type: "int",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Meal_Category",
                type: "int",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Ingredient_Nutrients",
                type: "int",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Ingredient_Components",
                type: "int",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Ingredient",
                type: "int",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Components",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
