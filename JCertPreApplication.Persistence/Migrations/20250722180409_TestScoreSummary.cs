using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TestScoreSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "languageKnowledgeScore",
                table: "test_attempt");

            migrationBuilder.DropColumn(
                name: "listeningScore",
                table: "test_attempt");

            migrationBuilder.DropColumn(
                name: "readingScore",
                table: "test_attempt");

            migrationBuilder.DropColumn(
                name: "totalScore",
                table: "test_attempt");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamPassThresholdId",
                table: "test",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "test_score_summary",
                columns: table => new
                {
                    TestScoreSummaryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestAttemptId = table.Column<Guid>(type: "uuid", nullable: true),
                    KanjiScore = table.Column<string>(type: "text", nullable: true),
                    VocabularyScore = table.Column<string>(type: "text", nullable: true),
                    GrammarScore = table.Column<string>(type: "text", nullable: true),
                    ReadingScore = table.Column<string>(type: "text", nullable: true),
                    ListeningScore = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_score_summary", x => x.TestScoreSummaryId);
                    table.ForeignKey(
                        name: "FK_test_score_summary_test_TestId",
                        column: x => x.TestId,
                        principalTable: "test",
                        principalColumn: "testId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_score_summary_test_attempt_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalTable: "test_attempt",
                        principalColumn: "attemptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_test_ExamPassThresholdId",
                table: "test",
                column: "ExamPassThresholdId");

            migrationBuilder.CreateIndex(
                name: "IX_test_score_summary_TestAttemptId",
                table: "test_score_summary",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_test_score_summary_TestId",
                table: "test_score_summary",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_exam_pass_threshold_ExamPassThresholdId",
                table: "test",
                column: "ExamPassThresholdId",
                principalTable: "exam_pass_threshold",
                principalColumn: "ExamPassThresholdId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_exam_pass_threshold_ExamPassThresholdId",
                table: "test");

            migrationBuilder.DropTable(
                name: "test_score_summary");

            migrationBuilder.DropIndex(
                name: "IX_test_ExamPassThresholdId",
                table: "test");

            migrationBuilder.DropColumn(
                name: "ExamPassThresholdId",
                table: "test");

            migrationBuilder.AddColumn<string>(
                name: "languageKnowledgeScore",
                table: "test_attempt",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "listeningScore",
                table: "test_attempt",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "readingScore",
                table: "test_attempt",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "totalScore",
                table: "test_attempt",
                type: "text",
                nullable: true);
        }
    }
}
