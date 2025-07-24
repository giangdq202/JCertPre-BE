using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TestTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_exam_pass_threshold_ExamPassThresholdId",
                table: "test");

            migrationBuilder.DropTable(
                name: "exam_pass_threshold");

            migrationBuilder.DropIndex(
                name: "IX_test_ExamPassThresholdId",
                table: "test");

            migrationBuilder.DropColumn(
                name: "ExamPassThresholdId",
                table: "test");

            migrationBuilder.AddColumn<string>(
                name: "TotalScore",
                table: "test_score_summary",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "test_template",
                columns: table => new
                {
                    templateId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    templateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    courseLevel = table.Column<string>(type: "text", nullable: false),
                    testType = table.Column<string>(type: "text", nullable: false),
                    durationMinutes = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    threeFirstParts = table.Column<string>(type: "text", nullable: true),
                    fourFirstParts = table.Column<string>(type: "text", nullable: true),
                    reading = table.Column<string>(type: "text", nullable: true),
                    listening = table.Column<string>(type: "text", nullable: false),
                    total = table.Column<string>(type: "text", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_template", x => x.templateId);
                    table.ForeignKey(
                        name: "FK_test_template_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "test_template_config",
                columns: table => new
                {
                    configId = table.Column<Guid>(type: "uuid", nullable: false),
                    templateId = table.Column<Guid>(type: "uuid", nullable: false),
                    subContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionCount = table.Column<int>(type: "integer", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_template_config", x => x.configId);
                    table.ForeignKey(
                        name: "FK_test_template_config_sub_contents_subContentId",
                        column: x => x.subContentId,
                        principalTable: "sub_contents",
                        principalColumn: "SubContentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_template_config_test_template_templateId",
                        column: x => x.templateId,
                        principalTable: "test_template",
                        principalColumn: "templateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_test_template_userId",
                table: "test_template",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_test_template_config_subContentId",
                table: "test_template_config",
                column: "subContentId");

            migrationBuilder.CreateIndex(
                name: "IX_test_template_config_templateId",
                table: "test_template_config",
                column: "templateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "test_template_config");

            migrationBuilder.DropTable(
                name: "test_template");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "test_score_summary");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamPassThresholdId",
                table: "test",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "exam_pass_threshold",
                columns: table => new
                {
                    ExamPassThresholdId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LanguageKnowledgeMax = table.Column<int>(type: "integer", nullable: false),
                    LanguageKnowledgeMin = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ListeningMax = table.Column<int>(type: "integer", nullable: false),
                    ListeningMin = table.Column<int>(type: "integer", nullable: false),
                    ReadingMax = table.Column<int>(type: "integer", nullable: false),
                    ReadingMin = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalMaxScore = table.Column<int>(type: "integer", nullable: false),
                    TotalPassingScore = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exam_pass_threshold", x => x.ExamPassThresholdId);
                    table.ForeignKey(
                        name: "FK_exam_pass_threshold_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_test_ExamPassThresholdId",
                table: "test",
                column: "ExamPassThresholdId");

            migrationBuilder.CreateIndex(
                name: "IX_exam_pass_threshold_UserId",
                table: "exam_pass_threshold",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_exam_pass_threshold_ExamPassThresholdId",
                table: "test",
                column: "ExamPassThresholdId",
                principalTable: "exam_pass_threshold",
                principalColumn: "ExamPassThresholdId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
