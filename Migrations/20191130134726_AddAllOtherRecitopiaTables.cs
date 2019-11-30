using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Recitopia_LastChance.Migrations
{
    public partial class AddAllOtherRecitopiaTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Comp_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Component_Name = table.Column<string>(maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Comp_Sort = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Componen__DC0BCC2082D90BE8", x => x.Comp_Id);
                });

            migrationBuilder.CreateTable(
                name: "Meal_Category",
                columns: table => new
                {
                    Category_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category_Name = table.Column<string>(maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Customer_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Meal_Cat__6DB38D6EACDEBF3E", x => x.Category_Id);
                });

            migrationBuilder.CreateTable(
                name: "Nutrition",
                columns: table => new
                {
                    Nutrition_Item_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nutrition_Item = table.Column<string>(maxLength: 50, nullable: false),
                    DV = table.Column<int>(nullable: true),
                    Measurement = table.Column<string>(maxLength: 50, nullable: true),
                    OrderOnNutrientPanel = table.Column<int>(nullable: true, defaultValueSql: "((50))"),
                    ShowOnNutrientPanel = table.Column<bool>(nullable: false),
                    Customer_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Nutritio__326DD8CD1CE0BF74", x => x.Nutrition_Item_Id);
                });

            migrationBuilder.CreateTable(
                name: "Serving_Sizes",
                columns: table => new
                {
                    SS_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Serving_Size = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Customer_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Serving___456F9402CBFF87ED", x => x.SS_Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendor",
                columns: table => new
                {
                    Vendor_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vendor_Name = table.Column<string>(maxLength: 50, nullable: false),
                    Phone = table.Column<string>(maxLength: 15, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    Address1 = table.Column<string>(maxLength: 50, nullable: true),
                    Address2 = table.Column<string>(maxLength: 50, nullable: true),
                    City = table.Column<string>(maxLength: 25, nullable: true),
                    State = table.Column<string>(maxLength: 10, nullable: true),
                    Zip = table.Column<int>(nullable: true),
                    Web_URL = table.Column<string>(maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Customer_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vendor__D9CCC2A879687754", x => x.Vendor_Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipe",
                columns: table => new
                {
                    Recipe_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Recipe_Name = table.Column<string>(maxLength: 50, nullable: false),
                    Category_Id = table.Column<int>(nullable: false),
                    Gluten_Free = table.Column<bool>(nullable: false),
                    SKU = table.Column<string>(maxLength: 50, nullable: true),
                    UPC = table.Column<string>(maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    LaborCost = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    SS_Id = table.Column<int>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: true),
                    Customer_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Recipe__0959CED94CA2B8C7", x => x.Recipe_Id);
                    table.ForeignKey(
                        name: "FK_Recipe_ToTable",
                        column: x => x.Category_Id,
                        principalTable: "Meal_Category",
                        principalColumn: "Category_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Serving_Size_ToTable",
                        column: x => x.SS_Id,
                        principalTable: "Serving_Sizes",
                        principalColumn: "SS_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient",
                columns: table => new
                {
                    Ingredient_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ingred_name = table.Column<string>(maxLength: 50, nullable: false),
                    Ingred_Comp_name = table.Column<string>(type: "text", nullable: true),
                    Cost_per_oz = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Cost_per_gram = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Cost_per_cup = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Cost_per_lb = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Cost_per_tsp = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Cost_per_tbsp = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Per_item = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Weight_Equiv_g = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Weight_Equiv_measure = table.Column<string>(maxLength: 15, nullable: true),
                    Vendor_Id = table.Column<int>(nullable: false),
                    Vendor_name = table.Column<string>(maxLength: 50, nullable: true),
                    Website = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Cost_per_lb2 = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Cost_per_ounce2 = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Cost_per_gram2 = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Packaging = table.Column<string>(maxLength: 50, nullable: true),
                    Brand = table.Column<string>(maxLength: 50, nullable: true),
                    LastModified = table.Column<DateTime>(nullable: true),
                    Package = table.Column<bool>(nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18, 3)", nullable: true),
                    Customer_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => x.Ingredient_Id);
                    table.ForeignKey(
                        name: "FK_Ingredient_ToTable",
                        column: x => x.Vendor_Id,
                        principalTable: "Vendor",
                        principalColumn: "Vendor_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient_Components",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ingred_Id = table.Column<int>(nullable: false),
                    Comp_Id = table.Column<int>(nullable: false),
                    Customer_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient_Components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredient_Comp_ToTable_1",
                        column: x => x.Comp_Id,
                        principalTable: "Components",
                        principalColumn: "Comp_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ingredient_Comp_ToTable",
                        column: x => x.Ingred_Id,
                        principalTable: "Ingredient",
                        principalColumn: "Ingredient_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient_Nutrients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ingred_Id = table.Column<int>(nullable: false),
                    Nutrition_Item_Id = table.Column<int>(nullable: false),
                    Nut_per_100_grams = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    Customer_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient_Nutrients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredient_Nutrients_ToTable",
                        column: x => x.Ingred_Id,
                        principalTable: "Ingredient",
                        principalColumn: "Ingredient_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ingredient_Nutrients_ToTable_1",
                        column: x => x.Nutrition_Item_Id,
                        principalTable: "Nutrition",
                        principalColumn: "Nutrition_Item_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recipe_Ingredients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Recipe_Id = table.Column<int>(nullable: false),
                    Ingredient_Id = table.Column<int>(nullable: false),
                    Amount_g = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    Customer_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe_Ingredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipe_Ingredients_ToTable_1",
                        column: x => x.Ingredient_Id,
                        principalTable: "Ingredient",
                        principalColumn: "Ingredient_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recipe_Ingredients_ToTable",
                        column: x => x.Recipe_Id,
                        principalTable: "Recipe",
                        principalColumn: "Recipe_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_Vendor_Id",
                table: "Ingredient",
                column: "Vendor_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_Components_Comp_Id",
                table: "Ingredient_Components",
                column: "Comp_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_Components_Ingred_Id",
                table: "Ingredient_Components",
                column: "Ingred_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_Nutrients_Ingred_Id",
                table: "Ingredient_Nutrients",
                column: "Ingred_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_Nutrients_Nutrition_Item_Id",
                table: "Ingredient_Nutrients",
                column: "Nutrition_Item_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_Category_Id",
                table: "Recipe",
                column: "Category_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_SS_Id",
                table: "Recipe",
                column: "SS_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_Ingredients_Ingredient_Id",
                table: "Recipe_Ingredients",
                column: "Ingredient_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_Ingredients_Recipe_Id",
                table: "Recipe_Ingredients",
                column: "Recipe_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ingredient_Components");

            migrationBuilder.DropTable(
                name: "Ingredient_Nutrients");

            migrationBuilder.DropTable(
                name: "Recipe_Ingredients");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "Nutrition");

            migrationBuilder.DropTable(
                name: "Ingredient");

            migrationBuilder.DropTable(
                name: "Recipe");

            migrationBuilder.DropTable(
                name: "Vendor");

            migrationBuilder.DropTable(
                name: "Meal_Category");

            migrationBuilder.DropTable(
                name: "Serving_Sizes");
        }
    }
}
