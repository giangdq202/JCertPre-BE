using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FeedBackRemoveReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropColumn(
                name: "reply",
                table: "feedback");

            migrationBuilder.AlterColumn<decimal>(
                name: "rating",
                table: "feedback",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "feedback",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "rating",
                table: "feedback",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "comment",
                table: "feedback",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reply",
                table: "feedback",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    reportId = table.Column<Guid>(type: "uuid", nullable: false),
                    reportedInstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    reporterStudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reportContent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report", x => x.reportId);
                    table.ForeignKey(
                        name: "FK_report_user_reportedInstructorId",
                        column: x => x.reportedInstructorId,
                        principalTable: "user",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_report_user_reporterStudentId",
                        column: x => x.reporterStudentId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_report_reportedInstructorId",
                table: "report",
                column: "reportedInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_report_reporterStudentId",
                table: "report",
                column: "reporterStudentId");
        }
    }
}
