using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TestQuestionLessonpg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCompleted",
                table: "lesson_progress");

            migrationBuilder.AddColumn<int>(
                name: "partDurationMinutes",
                table: "test_question",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "partNumber",
                table: "test_question",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "questionNumber",
                table: "test_question",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "completionRate",
                table: "lesson_progress",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0.0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "partDurationMinutes",
                table: "test_question");

            migrationBuilder.DropColumn(
                name: "partNumber",
                table: "test_question");

            migrationBuilder.DropColumn(
                name: "questionNumber",
                table: "test_question");

            migrationBuilder.DropColumn(
                name: "completionRate",
                table: "lesson_progress");

            migrationBuilder.AddColumn<bool>(
                name: "isCompleted",
                table: "lesson_progress",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
