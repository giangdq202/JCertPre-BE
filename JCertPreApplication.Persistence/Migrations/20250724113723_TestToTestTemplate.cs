using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TestToTestTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "testTemplateId",
                table: "test",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_test_testTemplateId",
                table: "test",
                column: "testTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_test_template_testTemplateId",
                table: "test",
                column: "testTemplateId",
                principalTable: "test_template",
                principalColumn: "templateId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_test_template_testTemplateId",
                table: "test");

            migrationBuilder.DropIndex(
                name: "IX_test_testTemplateId",
                table: "test");

            migrationBuilder.DropColumn(
                name: "testTemplateId",
                table: "test");
        }
    }
}
