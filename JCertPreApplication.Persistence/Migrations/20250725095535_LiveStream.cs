using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LiveStream : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_document_lesson_lessonId",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "FK_lesson_course_courseId",
                table: "lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_livestream_course_courseId",
                table: "livestream");

            migrationBuilder.DropIndex(
                name: "IX_livestream_courseId",
                table: "livestream");

            migrationBuilder.DropColumn(
                name: "meetingUrl",
                table: "livestream");

            migrationBuilder.DropColumn(
                name: "recordingUrl",
                table: "livestream");

            migrationBuilder.DropColumn(
                name: "title",
                table: "livestream");

            migrationBuilder.RenameColumn(
                name: "courseId",
                table: "livestream",
                newName: "lessonId");

            migrationBuilder.AddColumn<int>(
                name: "pointPerQuestion",
                table: "test_template_config",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "totalPoints",
                table: "test_template_config",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "lesson",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_livestream_lessonId",
                table: "livestream",
                column: "lessonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_document_lesson_lessonId",
                table: "document",
                column: "lessonId",
                principalTable: "lesson",
                principalColumn: "lessonId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lesson_course_courseId",
                table: "lesson",
                column: "courseId",
                principalTable: "course",
                principalColumn: "courseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_livestream_lesson_lessonId",
                table: "livestream",
                column: "lessonId",
                principalTable: "lesson",
                principalColumn: "lessonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_document_lesson_lessonId",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "FK_lesson_course_courseId",
                table: "lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_livestream_lesson_lessonId",
                table: "livestream");

            migrationBuilder.DropIndex(
                name: "IX_livestream_lessonId",
                table: "livestream");

            migrationBuilder.DropColumn(
                name: "pointPerQuestion",
                table: "test_template_config");

            migrationBuilder.DropColumn(
                name: "totalPoints",
                table: "test_template_config");

            migrationBuilder.RenameColumn(
                name: "lessonId",
                table: "livestream",
                newName: "courseId");

            migrationBuilder.AddColumn<string>(
                name: "meetingUrl",
                table: "livestream",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "recordingUrl",
                table: "livestream",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "livestream",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "lesson",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_livestream_courseId",
                table: "livestream",
                column: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_document_lesson_lessonId",
                table: "document",
                column: "lessonId",
                principalTable: "lesson",
                principalColumn: "lessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_lesson_course_courseId",
                table: "lesson",
                column: "courseId",
                principalTable: "course",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_livestream_course_courseId",
                table: "livestream",
                column: "courseId",
                principalTable: "course",
                principalColumn: "courseId");
        }
    }
}
