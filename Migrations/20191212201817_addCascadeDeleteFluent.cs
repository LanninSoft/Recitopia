using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Migrations
{
    public partial class addCascadeDeleteFluent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_ToTable",
                table: "Ingredient");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Comp_ToTable_1",
                table: "Ingredient_Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Comp_ToTable",
                table: "Ingredient_Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Nutrients_ToTable",
                table: "Ingredient_Nutrients");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Nutrients_ToTable_1",
                table: "Ingredient_Nutrients");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_ToTable",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_Serving_Size_ToTable",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Ingredients_ToTable_1",
                table: "Recipe_Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Ingredients_ToTable",
                table: "Recipe_Ingredients");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_ToTable",
                table: "Ingredient",
                column: "Vendor_Id",
                principalTable: "Vendor",
                principalColumn: "Vendor_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Comp_ToTable_1",
                table: "Ingredient_Components",
                column: "Comp_Id",
                principalTable: "Components",
                principalColumn: "Comp_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Comp_ToTable",
                table: "Ingredient_Components",
                column: "Ingred_Id",
                principalTable: "Ingredient",
                principalColumn: "Ingredient_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Nutrients_ToTable",
                table: "Ingredient_Nutrients",
                column: "Ingred_Id",
                principalTable: "Ingredient",
                principalColumn: "Ingredient_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Nutrients_ToTable_1",
                table: "Ingredient_Nutrients",
                column: "Nutrition_Item_Id",
                principalTable: "Nutrition",
                principalColumn: "Nutrition_Item_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_ToTable",
                table: "Recipe",
                column: "Category_Id",
                principalTable: "Meal_Category",
                principalColumn: "Category_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Serving_Size_ToTable",
                table: "Recipe",
                column: "SS_Id",
                principalTable: "Serving_Sizes",
                principalColumn: "SS_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Ingredients_ToTable_1",
                table: "Recipe_Ingredients",
                column: "Ingredient_Id",
                principalTable: "Ingredient",
                principalColumn: "Ingredient_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Ingredients_ToTable",
                table: "Recipe_Ingredients",
                column: "Recipe_Id",
                principalTable: "Recipe",
                principalColumn: "Recipe_Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_ToTable",
                table: "Ingredient");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Comp_ToTable_1",
                table: "Ingredient_Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Comp_ToTable",
                table: "Ingredient_Components");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Nutrients_ToTable",
                table: "Ingredient_Nutrients");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Nutrients_ToTable_1",
                table: "Ingredient_Nutrients");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_ToTable",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_Serving_Size_ToTable",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Ingredients_ToTable_1",
                table: "Recipe_Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Ingredients_ToTable",
                table: "Recipe_Ingredients");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_ToTable",
                table: "Ingredient",
                column: "Vendor_Id",
                principalTable: "Vendor",
                principalColumn: "Vendor_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Comp_ToTable_1",
                table: "Ingredient_Components",
                column: "Comp_Id",
                principalTable: "Components",
                principalColumn: "Comp_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Comp_ToTable",
                table: "Ingredient_Components",
                column: "Ingred_Id",
                principalTable: "Ingredient",
                principalColumn: "Ingredient_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Nutrients_ToTable",
                table: "Ingredient_Nutrients",
                column: "Ingred_Id",
                principalTable: "Ingredient",
                principalColumn: "Ingredient_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Nutrients_ToTable_1",
                table: "Ingredient_Nutrients",
                column: "Nutrition_Item_Id",
                principalTable: "Nutrition",
                principalColumn: "Nutrition_Item_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_ToTable",
                table: "Recipe",
                column: "Category_Id",
                principalTable: "Meal_Category",
                principalColumn: "Category_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Serving_Size_ToTable",
                table: "Recipe",
                column: "SS_Id",
                principalTable: "Serving_Sizes",
                principalColumn: "SS_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Ingredients_ToTable_1",
                table: "Recipe_Ingredients",
                column: "Ingredient_Id",
                principalTable: "Ingredient",
                principalColumn: "Ingredient_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Ingredients_ToTable",
                table: "Recipe_Ingredients",
                column: "Recipe_Id",
                principalTable: "Recipe",
                principalColumn: "Recipe_Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
