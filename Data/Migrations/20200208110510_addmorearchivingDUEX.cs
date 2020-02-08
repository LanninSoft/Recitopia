using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class addmorearchivingDUEX : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchiveDate",
                table: "Packaging",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "WhoArchived",
                table: "Packaging",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isArchived",
                table: "Packaging",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchiveDate",
                table: "Packaging");

            migrationBuilder.DropColumn(
                name: "WhoArchived",
                table: "Packaging");

            migrationBuilder.DropColumn(
                name: "isArchived",
                table: "Packaging");
        }
    }
}
