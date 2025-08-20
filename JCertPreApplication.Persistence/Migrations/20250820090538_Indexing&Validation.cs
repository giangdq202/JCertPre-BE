using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IndexingValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "verifiedUserId",
                table: "test_template_type",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "choiceText",
                table: "choice",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_user_createdAt",
                table: "user",
                column: "createdAt");

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roleId_status",
                table: "user",
                columns: new[] { "roleId", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_user_status",
                table: "user",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_test_template_type_verifiedUserId",
                table: "test_template_type",
                column: "verifiedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_test_attempt_status",
                table: "test_attempt",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_test_attempt_testId_userId",
                table: "test_attempt",
                columns: new[] { "testId", "userId" });

            migrationBuilder.CreateIndex(
                name: "IX_test_availableFrom",
                table: "test",
                column: "availableFrom");

            migrationBuilder.CreateIndex(
                name: "IX_test_courseLevel",
                table: "test",
                column: "courseLevel");

            migrationBuilder.CreateIndex(
                name: "IX_test_status",
                table: "test",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_test_TestTemplateTypeId_status",
                table: "test",
                columns: new[] { "TestTemplateTypeId", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_test_testType",
                table: "test",
                column: "testType");

            migrationBuilder.CreateIndex(
                name: "IX_questions_difficulty",
                table: "questions",
                column: "difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_questions_isActive",
                table: "questions",
                column: "isActive");

            migrationBuilder.CreateIndex(
                name: "IX_questions_isActive_SubContentId_difficulty",
                table: "questions",
                columns: new[] { "isActive", "SubContentId", "difficulty" });

            migrationBuilder.CreateIndex(
                name: "IX_questions_questionText",
                table: "questions",
                column: "questionText");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_courseId_lessonOrder",
                table: "lesson",
                columns: new[] { "courseId", "lessonOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_lesson_courseId_title",
                table: "lesson",
                columns: new[] { "courseId", "title" });

            migrationBuilder.CreateIndex(
                name: "IX_lesson_lessonOrder",
                table: "lesson",
                column: "lessonOrder");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_title",
                table: "lesson",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_userId_courseId",
                table: "enrollment",
                columns: new[] { "userId", "courseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_choice_isCorrect",
                table: "choice",
                column: "isCorrect");

            migrationBuilder.CreateIndex(
                name: "IX_choice_questionId_isCorrect",
                table: "choice",
                columns: new[] { "questionId", "isCorrect" });

            migrationBuilder.AddForeignKey(
                name: "FK_test_template_type_user_verifiedUserId",
                table: "test_template_type",
                column: "verifiedUserId",
                principalTable: "user",
                principalColumn: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_template_type_user_verifiedUserId",
                table: "test_template_type");

            migrationBuilder.DropIndex(
                name: "IX_user_createdAt",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_email",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_roleId_status",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_status",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_test_template_type_verifiedUserId",
                table: "test_template_type");

            migrationBuilder.DropIndex(
                name: "IX_test_attempt_status",
                table: "test_attempt");

            migrationBuilder.DropIndex(
                name: "IX_test_attempt_testId_userId",
                table: "test_attempt");

            migrationBuilder.DropIndex(
                name: "IX_test_availableFrom",
                table: "test");

            migrationBuilder.DropIndex(
                name: "IX_test_courseLevel",
                table: "test");

            migrationBuilder.DropIndex(
                name: "IX_test_status",
                table: "test");

            migrationBuilder.DropIndex(
                name: "IX_test_TestTemplateTypeId_status",
                table: "test");

            migrationBuilder.DropIndex(
                name: "IX_test_testType",
                table: "test");

            migrationBuilder.DropIndex(
                name: "IX_questions_difficulty",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "IX_questions_isActive",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "IX_questions_isActive_SubContentId_difficulty",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "IX_questions_questionText",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "IX_lesson_courseId_lessonOrder",
                table: "lesson");

            migrationBuilder.DropIndex(
                name: "IX_lesson_courseId_title",
                table: "lesson");

            migrationBuilder.DropIndex(
                name: "IX_lesson_lessonOrder",
                table: "lesson");

            migrationBuilder.DropIndex(
                name: "IX_lesson_title",
                table: "lesson");

            migrationBuilder.DropIndex(
                name: "IX_enrollment_userId_courseId",
                table: "enrollment");

            migrationBuilder.DropIndex(
                name: "IX_choice_isCorrect",
                table: "choice");

            migrationBuilder.DropIndex(
                name: "IX_choice_questionId_isCorrect",
                table: "choice");

            migrationBuilder.DropColumn(
                name: "verifiedUserId",
                table: "test_template_type");

            migrationBuilder.AlterColumn<string>(
                name: "choiceText",
                table: "choice",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);
        }
    }
}
