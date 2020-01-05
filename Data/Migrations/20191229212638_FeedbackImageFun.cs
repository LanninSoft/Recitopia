using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class FeedbackImageFun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeedbackSubject",
                table: "FeedbackFiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileContentType",
                table: "FeedbackFiles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "FeedbackFiles",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeedbackSubject",
                table: "FeedbackFiles");

            migrationBuilder.DropColumn(
                name: "FileContentType",
                table: "FeedbackFiles");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "FeedbackFiles");
        }
    }
}
