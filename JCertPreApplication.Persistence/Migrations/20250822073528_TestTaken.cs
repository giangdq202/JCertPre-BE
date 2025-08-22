using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TestTaken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questions_sub_contents_SubContentId",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "FK_test_template_config_sub_contents_subContentId",
                table: "test_template_config");

            migrationBuilder.AddColumn<DateTime>(
                name: "lastResetTestTime",
                table: "student_profile",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "numberOfTestsTaken",
                table: "student_profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_questions_sub_contents_SubContentId",
                table: "questions",
                column: "SubContentId",
                principalTable: "sub_contents",
                principalColumn: "SubContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_template_config_sub_contents_subContentId",
                table: "test_template_config",
                column: "subContentId",
                principalTable: "sub_contents",
                principalColumn: "SubContentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questions_sub_contents_SubContentId",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "FK_test_template_config_sub_contents_subContentId",
                table: "test_template_config");

            migrationBuilder.DropColumn(
                name: "lastResetTestTime",
                table: "student_profile");

            migrationBuilder.DropColumn(
                name: "numberOfTestsTaken",
                table: "student_profile");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_sub_contents_SubContentId",
                table: "questions",
                column: "SubContentId",
                principalTable: "sub_contents",
                principalColumn: "SubContentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_template_config_sub_contents_subContentId",
                table: "test_template_config",
                column: "subContentId",
                principalTable: "sub_contents",
                principalColumn: "SubContentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
