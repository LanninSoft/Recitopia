using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class addFeedbackFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedbackFiles",
                columns: table => new
                {
                    ImageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<byte[]>(nullable: false),
                    FeedbackId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackFiles", x => x.ImageId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackFiles");
        }
    }
}
