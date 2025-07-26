using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TestTemplateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_test_template_testTemplateId",
                table: "test");

            migrationBuilder.DropForeignKey(
                name: "FK_test_template_user_userId",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "courseLevel",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "createdAt",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "description",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "fourFirstParts",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "listening",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "reading",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "testType",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "threeFirstParts",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "total",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "GrammarScore",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "KanjiScore",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "ListeningScore",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "ReadingScore",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "VocabularyScore",
                table: "test_score_summary");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "test_template",
                newName: "TestTemplateTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_test_template_userId",
                table: "test_template",
                newName: "IX_test_template_TestTemplateTypeId");

            migrationBuilder.RenameColumn(
                name: "testTemplateId",
                table: "test",
                newName: "TestTemplateTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_test_testTemplateId",
                table: "test",
                newName: "IX_test_TestTemplateTypeId");

            migrationBuilder.DropColumn(
                name: "durationMinutes",
                table: "test_template");

            migrationBuilder.AddColumn<int>(
                name: "durationMinutes",
                table: "test_template",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "toPassPercentage",
                table: "test_template",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "totalScore",
                table: "test_template",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "grammar_max_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "grammar_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "kanji_max_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "kanji_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "listening_max_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "listening_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "passing_percentage",
                table: "test_score_summary",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "percentage_score",
                table: "test_score_summary",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "reading_max_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "reading_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_max_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "vocab_max_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "vocab_score",
                table: "test_score_summary",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "test_template_type",
                columns: table => new
                {
                    TestTemplateTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    typeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    courseLevel = table.Column<string>(type: "text", nullable: false),
                    testType = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_template_type", x => x.TestTemplateTypeId);
                    table.ForeignKey(
                        name: "FK_test_template_type_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_test_template_type_userId",
                table: "test_template_type",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_test_template_type_TestTemplateTypeId",
                table: "test",
                column: "TestTemplateTypeId",
                principalTable: "test_template_type",
                principalColumn: "TestTemplateTypeId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_test_template_test_template_type_TestTemplateTypeId",
                table: "test_template",
                column: "TestTemplateTypeId",
                principalTable: "test_template_type",
                principalColumn: "TestTemplateTypeId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_test_template_type_TestTemplateTypeId",
                table: "test");

            migrationBuilder.DropForeignKey(
                name: "FK_test_template_test_template_type_TestTemplateTypeId",
                table: "test_template");

            migrationBuilder.DropTable(
                name: "test_template_type");

            migrationBuilder.DropColumn(
                name: "toPassPercentage",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "totalScore",
                table: "test_template");

            migrationBuilder.DropColumn(
                name: "grammar_max_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "grammar_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "kanji_max_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "kanji_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "listening_max_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "listening_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "passing_percentage",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "percentage_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "reading_max_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "reading_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "total_max_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "total_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "vocab_max_score",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "vocab_score",
                table: "test_score_summary");

            migrationBuilder.RenameColumn(
                name: "TestTemplateTypeId",
                table: "test_template",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_test_template_TestTemplateTypeId",
                table: "test_template",
                newName: "IX_test_template_userId");

            migrationBuilder.RenameColumn(
                name: "TestTemplateTypeId",
                table: "test",
                newName: "testTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_test_TestTemplateTypeId",
                table: "test",
                newName: "IX_test_testTemplateId");

            migrationBuilder.AlterColumn<string>(
                name: "durationMinutes",
                table: "test_template",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "courseLevel",
                table: "test_template",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdAt",
                table: "test_template",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "test_template",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "fourFirstParts",
                table: "test_template",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "test_template",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "listening",
                table: "test_template",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reading",
                table: "test_template",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "testType",
                table: "test_template",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "threeFirstParts",
                table: "test_template",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "total",
                table: "test_template",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GrammarScore",
                table: "test_score_summary",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KanjiScore",
                table: "test_score_summary",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ListeningScore",
                table: "test_score_summary",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReadingScore",
                table: "test_score_summary",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalScore",
                table: "test_score_summary",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VocabularyScore",
                table: "test_score_summary",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_test_test_template_testTemplateId",
                table: "test",
                column: "testTemplateId",
                principalTable: "test_template",
                principalColumn: "templateId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_test_template_user_userId",
                table: "test_template",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");
        }
    }
}
