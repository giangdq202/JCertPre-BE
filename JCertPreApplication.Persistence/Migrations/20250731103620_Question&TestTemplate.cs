using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class QuestionTestTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attempt_answer_questions_questionId",
                table: "attempt_answer");

            migrationBuilder.DropForeignKey(
                name: "FK_test_template_test_template_type_TestTemplateTypeId",
                table: "test_template");

            migrationBuilder.AlterColumn<string>(
                name: "templateName",
                table: "test_template",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "sequence",
                table: "test_template",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "questionText",
                table: "questions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "explanation",
                table: "questions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "questions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_attempt_answer_questions_questionId",
                table: "attempt_answer",
                column: "questionId",
                principalTable: "questions",
                principalColumn: "questionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_test_template_test_template_type_TestTemplateTypeId",
                table: "test_template",
                column: "TestTemplateTypeId",
                principalTable: "test_template_type",
                principalColumn: "TestTemplateTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attempt_answer_questions_questionId",
                table: "attempt_answer");

            migrationBuilder.DropForeignKey(
                name: "FK_test_template_test_template_type_TestTemplateTypeId",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "sequence",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "questions");

            migrationBuilder.AlterColumn<string>(
                name: "templateName",
                table: "test_template",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "questionText",
                table: "questions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "explanation",
                table: "questions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_attempt_answer_questions_questionId",
                table: "attempt_answer",
                column: "questionId",
                principalTable: "questions",
                principalColumn: "questionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_template_test_template_type_TestTemplateTypeId",
                table: "test_template",
                column: "TestTemplateTypeId",
                principalTable: "test_template_type",
                principalColumn: "TestTemplateTypeId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
