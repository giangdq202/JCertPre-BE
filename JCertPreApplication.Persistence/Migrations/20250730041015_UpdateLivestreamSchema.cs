using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLivestreamSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_livestream_lesson_lessonId",
                table: "livestream");

            migrationBuilder.DropIndex(
                name: "IX_livestream_lessonId",
                table: "livestream");

            migrationBuilder.DropColumn(
                name: "endTime",
                table: "livestream");

            migrationBuilder.RenameColumn(
                name: "startTime",
                table: "livestream",
                newName: "scheduledDateTime");

            migrationBuilder.RenameColumn(
                name: "lessonId",
                table: "livestream",
                newName: "courseId");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "livestream",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "durationMinutes",
                table: "livestream",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "livestream",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_livestream_courseId",
                table: "livestream",
                column: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_livestream_course_courseId",
                table: "livestream",
                column: "courseId",
                principalTable: "course",
                principalColumn: "courseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_livestream_course_courseId",
                table: "livestream");

            migrationBuilder.DropIndex(
                name: "IX_livestream_courseId",
                table: "livestream");

            migrationBuilder.DropColumn(
                name: "description",
                table: "livestream");

            migrationBuilder.DropColumn(
                name: "durationMinutes",
                table: "livestream");

            migrationBuilder.DropColumn(
                name: "status",
                table: "livestream");

            migrationBuilder.RenameColumn(
                name: "scheduledDateTime",
                table: "livestream",
                newName: "startTime");

            migrationBuilder.RenameColumn(
                name: "courseId",
                table: "livestream",
                newName: "lessonId");

            migrationBuilder.AddColumn<DateTime>(
                name: "endTime",
                table: "livestream",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_livestream_lessonId",
                table: "livestream",
                column: "lessonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_livestream_lesson_lessonId",
                table: "livestream",
                column: "lessonId",
                principalTable: "lesson",
                principalColumn: "lessonId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
