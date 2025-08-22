using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBehaviorStudyPlanItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attempt_answer_questions_questionId",
                table: "attempt_answer");

            migrationBuilder.DropForeignKey(
                name: "FK_question_attachment_questions_questionId",
                table: "question_attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_study_plan_item_test_testId",
                table: "study_plan_item");

            migrationBuilder.DropForeignKey(
                name: "FK_test_question_questions_questionId",
                table: "test_question");

            migrationBuilder.DropForeignKey(
                name: "FK_test_template_test_template_type_TestTemplateTypeId",
                table: "test_template");

            migrationBuilder.RenameColumn(
                name: "testId",
                table: "study_plan_item",
                newName: "TestTemplateTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_study_plan_item_testId",
                table: "study_plan_item",
                newName: "IX_study_plan_item_TestTemplateTypeId");

            migrationBuilder.AlterColumn<Guid>(
                name: "testId",
                table: "test_attempt",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "study_plan_item",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_attempt_answer_questions_questionId",
                table: "attempt_answer",
                column: "questionId",
                principalTable: "questions",
                principalColumn: "questionId");

            migrationBuilder.AddForeignKey(
                name: "FK_question_attachment_questions_questionId",
                table: "question_attachment",
                column: "questionId",
                principalTable: "questions",
                principalColumn: "questionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_study_plan_item_test_template_type_TestTemplateTypeId",
                table: "study_plan_item",
                column: "TestTemplateTypeId",
                principalTable: "test_template_type",
                principalColumn: "TestTemplateTypeId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_test_question_questions_questionId",
                table: "test_question",
                column: "questionId",
                principalTable: "questions",
                principalColumn: "questionId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_template_test_template_type_TestTemplateTypeId",
                table: "test_template",
                column: "TestTemplateTypeId",
                principalTable: "test_template_type",
                principalColumn: "TestTemplateTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attempt_answer_questions_questionId",
                table: "attempt_answer");

            migrationBuilder.DropForeignKey(
                name: "FK_question_attachment_questions_questionId",
                table: "question_attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_study_plan_item_test_template_type_TestTemplateTypeId",
                table: "study_plan_item");

            migrationBuilder.DropForeignKey(
                name: "FK_test_question_questions_questionId",
                table: "test_question");

            migrationBuilder.DropForeignKey(
                name: "FK_test_template_test_template_type_TestTemplateTypeId",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "description",
                table: "study_plan_item");

            migrationBuilder.RenameColumn(
                name: "TestTemplateTypeId",
                table: "study_plan_item",
                newName: "testId");

            migrationBuilder.RenameIndex(
                name: "IX_study_plan_item_TestTemplateTypeId",
                table: "study_plan_item",
                newName: "IX_study_plan_item_testId");

            migrationBuilder.AlterColumn<Guid>(
                name: "testId",
                table: "test_attempt",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_attempt_answer_questions_questionId",
                table: "attempt_answer",
                column: "questionId",
                principalTable: "questions",
                principalColumn: "questionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_question_attachment_questions_questionId",
                table: "question_attachment",
                column: "questionId",
                principalTable: "questions",
                principalColumn: "questionId");

            migrationBuilder.AddForeignKey(
                name: "FK_study_plan_item_test_testId",
                table: "study_plan_item",
                column: "testId",
                principalTable: "test",
                principalColumn: "testId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_question_questions_questionId",
                table: "test_question",
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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
