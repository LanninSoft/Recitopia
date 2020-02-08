using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class addmorearchiving : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchiveDate",
                table: "Vendor",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "WhoArchived",
                table: "Vendor",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isArchived",
                table: "Vendor",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WhoArchived",
                table: "Recipe",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchiveDate",
                table: "Nutrition",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "WhoArchived",
                table: "Nutrition",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isArchived",
                table: "Nutrition",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchiveDate",
                table: "Ingredient",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "WhoArchived",
                table: "Ingredient",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isArchived",
                table: "Ingredient",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchiveDate",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "WhoArchived",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "isArchived",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "WhoArchived",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "ArchiveDate",
                table: "Nutrition");

            migrationBuilder.DropColumn(
                name: "WhoArchived",
                table: "Nutrition");

            migrationBuilder.DropColumn(
                name: "isArchived",
                table: "Nutrition");

            migrationBuilder.DropColumn(
                name: "ArchiveDate",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "WhoArchived",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "isArchived",
                table: "Ingredient");
        }
    }
}
