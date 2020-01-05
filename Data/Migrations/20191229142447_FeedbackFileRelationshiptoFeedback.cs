using Microsoft.EntityFrameworkCore.Migrations;

namespace Recitopia.Data.Migrations
{
    public partial class FeedbackFileRelationshiptoFeedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FeedbackFiles_FeedbackId",
                table: "FeedbackFiles",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackFiles_Feedback_FeedbackId",
                table: "FeedbackFiles",
                column: "FeedbackId",
                principalTable: "Feedback",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackFiles_Feedback_FeedbackId",
                table: "FeedbackFiles");

            migrationBuilder.DropIndex(
                name: "IX_FeedbackFiles_FeedbackId",
                table: "FeedbackFiles");
        }
    }
}
