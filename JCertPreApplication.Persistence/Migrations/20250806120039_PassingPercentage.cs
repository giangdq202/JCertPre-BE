using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PassingPercentage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passing_percentage",
                table: "test_score_summary");

            migrationBuilder.DropColumn(
                name: "percentage_score",
                table: "test_score_summary");

            migrationBuilder.AddColumn<decimal>(
                name: "passing_percentage",
                table: "test",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passing_percentage",
                table: "test");

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
        }
    }
}
