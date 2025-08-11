using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImproveInstructorCourseManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_course_instructor",
                table: "course_instructor");

            migrationBuilder.DropIndex(
                name: "IX_course_instructor_InstructorId",
                table: "course_instructor");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "course_instructor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_instructor",
                table: "course_instructor",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructor_CourseId_IsActive",
                table: "course_instructor",
                columns: new[] { "CourseId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseInstructor_InstructorId_IsActive",
                table: "course_instructor",
                columns: new[] { "InstructorId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_course_instructor",
                table: "course_instructor");

            migrationBuilder.DropIndex(
                name: "IX_CourseInstructor_CourseId_IsActive",
                table: "course_instructor");

            migrationBuilder.DropIndex(
                name: "IX_CourseInstructor_InstructorId_IsActive",
                table: "course_instructor");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "course_instructor");

            migrationBuilder.AddPrimaryKey(
                name: "PK_course_instructor",
                table: "course_instructor",
                columns: new[] { "CourseId", "InstructorId", "AssignedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_course_instructor_InstructorId",
                table: "course_instructor",
                column: "InstructorId");
        }
    }
}
