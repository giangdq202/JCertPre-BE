using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameJoinTablesToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename ConversationParticipant to conversation_participant
            migrationBuilder.RenameTable(
                name: "ConversationParticipant",
                newName: "conversation_participant");

            // Rename CourseInstructor to course_instructor  
            migrationBuilder.RenameTable(
                name: "CourseInstructor", 
                newName: "course_instructor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Rename back to PascalCase
            migrationBuilder.RenameTable(
                name: "conversation_participant",
                newName: "ConversationParticipant");

            migrationBuilder.RenameTable(
                name: "course_instructor",
                newName: "CourseInstructor");
        }
    }
}
