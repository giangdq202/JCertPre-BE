using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TestQuestionLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isActive",
                table: "test_question");

            migrationBuilder.AddColumn<string>(
                name: "comment",
                table: "lesson",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comment",
                table: "lesson");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "test_question",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
