using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class InitialCreateWithIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true),
                    WebUrl = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Site_Role_Id = table.Column<string>(nullable: true),
                    Customer_Id = table.Column<int>(nullable: false),
                    Customer_Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Comp_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Component_Name = table.Column<string>(maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Comp_Sort = table.Column<string>(maxLength: 50, nullable: true),
                    Customer_Id = table.Column<int>(nullable: false)
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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Serving_Size_ToTable",
                        column: x => x.SS_Id,
                        principalTable: "Serving_Sizes",
                        principalColumn: "SS_Id",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingredient_Comp_ToTable",
                        column: x => x.Ingred_Id,
                        principalTable: "Ingredient",
                        principalColumn: "Ingredient_Id",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingredient_Nutrients_ToTable_1",
                        column: x => x.Nutrition_Item_Id,
                        principalTable: "Nutrition",
                        principalColumn: "Nutrition_Item_Id",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recipe_Ingredients_ToTable",
                        column: x => x.Recipe_Id,
                        principalTable: "Recipe",
                        principalColumn: "Recipe_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Users_CustomersCustomer_Id",
                table: "Customer_Users",
                column: "CustomersCustomer_Id");

           

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Customer_Users");

            migrationBuilder.DropTable(
                name: "Ingredient_Components");

            migrationBuilder.DropTable(
                name: "Ingredient_Nutrients");

            migrationBuilder.DropTable(
                name: "Recipe_Ingredients");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "Nutrition");

            migrationBuilder.DropTable(
                name: "Ingredient");

            migrationBuilder.DropTable(
                name: "Recipe");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Vendor");

            migrationBuilder.DropTable(
                name: "Meal_Category");

            migrationBuilder.DropTable(
                name: "Serving_Sizes");
        }
    }
}
