using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "itemIdRef",
                table: "study_plan_item");

            migrationBuilder.AlterColumn<Guid>(
                name: "lessonId",
                table: "test",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "courseId",
                table: "study_plan_item",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "testId",
                table: "study_plan_item",
                type: "uuid",
                nullable: true);

            // Drop and recreate course_instructor table
            migrationBuilder.DropTable(
                name: "course_instructor");

            migrationBuilder.CreateTable(
                name: "course_instructor",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeftOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_instructor", x => new { x.CourseId, x.InstructorId, x.AssignedOn });
                    table.ForeignKey(
                        name: "FK_course_instructor_course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "course",
                        principalColumn: "courseId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_course_instructor_user_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "user",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_instructor_InstructorId",
                table: "course_instructor",
                column: "InstructorId");

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "roleId", "description", "roleName" },
                values: new object[,]
                {
                    { new Guid("0d1c9d64-3be8-4d5c-9ad0-062f83a3a7f8"), "Academic Manager role", "ACADEMIC_MANAGER" },
                    { new Guid("8174528c-7f5b-4277-aa1a-1150e7b8b275"), "Instructor role", "INSTRUCTOR" },
                    { new Guid("8dd36044-84d4-4e4b-8162-34b7a421657c"), "Student role", "STUDENT" },
                    { new Guid("d500140c-99c5-452f-b44c-a3b4e650d0e6"), "Administrator role", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_study_plan_item_courseId",
                table: "study_plan_item",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_study_plan_item_testId",
                table: "study_plan_item",
                column: "testId");

            migrationBuilder.AddForeignKey(
                name: "FK_study_plan_item_course_courseId",
                table: "study_plan_item",
                column: "courseId",
                principalTable: "course",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_study_plan_item_test_testId",
                table: "study_plan_item",
                column: "testId",
                principalTable: "test",
                principalColumn: "testId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_study_plan_item_course_courseId",
                table: "study_plan_item");

            migrationBuilder.DropForeignKey(
                name: "FK_study_plan_item_test_testId",
                table: "study_plan_item");

            migrationBuilder.DropIndex(
                name: "IX_study_plan_item_courseId",
                table: "study_plan_item");

            migrationBuilder.DropIndex(
                name: "IX_study_plan_item_testId",
                table: "study_plan_item");

            migrationBuilder.DropTable(
                name: "course_instructor");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "roleId",
                keyValue: new Guid("0d1c9d64-3be8-4d5c-9ad0-062f83a3a7f8"));

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "roleId",
                keyValue: new Guid("8174528c-7f5b-4277-aa1a-1150e7b8b275"));

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "roleId",
                keyValue: new Guid("8dd36044-84d4-4e4b-8162-34b7a421657c"));

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "roleId",
                keyValue: new Guid("d500140c-99c5-452f-b44c-a3b4e650d0e6"));

            migrationBuilder.DropColumn(
                name: "courseId",
                table: "study_plan_item");

            migrationBuilder.DropColumn(
                name: "testId",
                table: "study_plan_item");

            migrationBuilder.AddColumn<string>(
                name: "itemIdRef",
                table: "study_plan_item",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "lessonId",
                table: "test",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "course_instructor",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_instructor", x => new { x.CourseId, x.UserId });
                    table.ForeignKey(
                        name: "FK_course_instructor_course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "course",
                        principalColumn: "courseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_course_instructor_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_instructor_UserId",
                table: "course_instructor",
                column: "UserId");
        }
    }
}
