using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_test_lessonId",
                table: "test");

            migrationBuilder.CreateIndex(
                name: "IX_test_lessonId",
                table: "test",
                column: "lessonId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_test_lessonId",
                table: "test");

            migrationBuilder.CreateIndex(
                name: "IX_test_lessonId",
                table: "test",
                column: "lessonId");
        }
    }
}
