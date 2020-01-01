using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class AuditLogtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audit_Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Event = table.Column<string>(nullable: false),
                    EventDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    WhatChanged = table.Column<string>(nullable: true),
                    CustomerGuid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_Log", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audit_Log");
        }
    }
}
