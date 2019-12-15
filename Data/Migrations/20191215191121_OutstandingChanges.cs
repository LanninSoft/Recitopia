using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class OutstandingChanges : Migration
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

            migrationBuilder.AddColumn<int>(
                name: "Customer_Id",
                table: "Components",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Customer_Name",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site_Role_Id",
                table: "AspNetUsers",
                nullable: true);

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
                    Notes = table.Column<string>(nullable: true),
                    AppUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Customer_Id);
                    table.ForeignKey(
                        name: "FK_Customers_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customer_Users",
                columns: table => new
                {
                    CU_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer_Id = table.Column<int>(nullable: false),
                    Customer_Name = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: true),
                    User_Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    CustomersCustomer_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer_Users", x => x.CU_Id);
                    table.ForeignKey(
                        name: "FK_Customer_Users_Customers_CustomersCustomer_Id",
                        column: x => x.CustomersCustomer_Id,
                        principalTable: "Customers",
                        principalColumn: "Customer_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Users_CustomersCustomer_Id",
                table: "Customer_Users",
                column: "CustomersCustomer_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AppUserId",
                table: "Customers",
                column: "AppUserId");

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

            migrationBuilder.DropTable(
                name: "Customer_Users");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropColumn(
                name: "Customer_Id",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Customer_Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Site_Role_Id",
                table: "AspNetUsers");

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
