using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesFromPascalCaseToLowercase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttemptAnswers_Choices_choiceId",
                table: "AttemptAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_AttemptAnswers_Questions_questionId",
                table: "AttemptAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_AttemptAnswers_TestAttempts_attemptId",
                table: "AttemptAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_Choices_Questions_questionId",
                table: "Choices");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_Conversations_Conversationsconversa~",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_Users_ParticipantsuserId",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseInstructor_Courses_CourseId",
                table: "CourseInstructor");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseInstructor_Users_UserId",
                table: "CourseInstructor");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Lessons_lessonId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_courseId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_userId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Courses_courseId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Users_userId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorProfiles_Users_userId",
                table: "InstructorProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Courses_courseId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Livestreams_Courses_courseId",
                table: "Livestreams");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_conversationId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_senderId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_userId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAttachments_Questions_questionId",
                table: "QuestionAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Questions_QuestionsquestionId",
                table: "QuestionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Tags_tagId",
                table: "QuestionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTests_Questions_QuestionsquestionId",
                table: "QuestionTests");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTests_Tests_TeststestId",
                table: "QuestionTests");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_reportedInstructorId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_reporterStudentId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_Users_userId",
                table: "StudentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlanItems_StudyPlans_planId",
                table: "StudyPlanItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlans_Users_createdByStaffId",
                table: "StudyPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlans_Users_studentId",
                table: "StudyPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAttempts_Tests_testId",
                table: "TestAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAttempts_Users_userId",
                table: "TestAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Lessons_lessonId",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Users_createdByUserId",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_roleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests",
                table: "Tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestAttempts",
                table: "TestAttempts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyPlans",
                table: "StudyPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyPlanItems",
                table: "StudyPlanItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentProfiles",
                table: "StudentProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionTests",
                table: "QuestionTests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionTags",
                table: "QuestionTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionAttachments",
                table: "QuestionAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Livestreams",
                table: "Livestreams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructorProfiles",
                table: "InstructorProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Choices",
                table: "Choices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AttemptAnswers",
                table: "AttemptAnswers");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "user");

            migrationBuilder.RenameTable(
                name: "Tests",
                newName: "test");

            migrationBuilder.RenameTable(
                name: "TestAttempts",
                newName: "test_attempt");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "tag");

            migrationBuilder.RenameTable(
                name: "StudyPlans",
                newName: "study_plan");

            migrationBuilder.RenameTable(
                name: "StudyPlanItems",
                newName: "study_plan_item");

            migrationBuilder.RenameTable(
                name: "StudentProfiles",
                newName: "student_profile");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "role");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "report");

            migrationBuilder.RenameTable(
                name: "QuestionTests",
                newName: "question_test");

            migrationBuilder.RenameTable(
                name: "QuestionTags",
                newName: "question_tag");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "question");

            migrationBuilder.RenameTable(
                name: "QuestionAttachments",
                newName: "question_attachment");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "payment");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "message");

            migrationBuilder.RenameTable(
                name: "Livestreams",
                newName: "livestream");

            migrationBuilder.RenameTable(
                name: "Lessons",
                newName: "lesson");

            migrationBuilder.RenameTable(
                name: "InstructorProfiles",
                newName: "instructor_profile");

            migrationBuilder.RenameTable(
                name: "Feedbacks",
                newName: "feedback");

            migrationBuilder.RenameTable(
                name: "Enrollments",
                newName: "enrollment");

            migrationBuilder.RenameTable(
                name: "Documents",
                newName: "document");

            migrationBuilder.RenameTable(
                name: "Courses",
                newName: "course");

            migrationBuilder.RenameTable(
                name: "Conversations",
                newName: "conversation");

            migrationBuilder.RenameTable(
                name: "Choices",
                newName: "choice");

            migrationBuilder.RenameTable(
                name: "AttemptAnswers",
                newName: "attempt_answer");

            migrationBuilder.RenameIndex(
                name: "IX_Users_roleId",
                table: "user",
                newName: "IX_user_roleId");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_lessonId",
                table: "test",
                newName: "IX_test_lessonId");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_createdByUserId",
                table: "test",
                newName: "IX_test_createdByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TestAttempts_userId",
                table: "test_attempt",
                newName: "IX_test_attempt_userId");

            migrationBuilder.RenameIndex(
                name: "IX_TestAttempts_testId",
                table: "test_attempt",
                newName: "IX_test_attempt_testId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyPlans_studentId",
                table: "study_plan",
                newName: "IX_study_plan_studentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyPlans_createdByStaffId",
                table: "study_plan",
                newName: "IX_study_plan_createdByStaffId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyPlanItems_planId",
                table: "study_plan_item",
                newName: "IX_study_plan_item_planId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_reporterStudentId",
                table: "report",
                newName: "IX_report_reporterStudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_reportedInstructorId",
                table: "report",
                newName: "IX_report_reportedInstructorId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionTests_TeststestId",
                table: "question_test",
                newName: "IX_question_test_TeststestId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionTags_tagId",
                table: "question_tag",
                newName: "IX_question_tag_tagId");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionAttachments_questionId",
                table: "question_attachment",
                newName: "IX_question_attachment_questionId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_userId",
                table: "payment",
                newName: "IX_payment_userId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_senderId",
                table: "message",
                newName: "IX_message_senderId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_conversationId",
                table: "message",
                newName: "IX_message_conversationId");

            migrationBuilder.RenameIndex(
                name: "IX_Livestreams_courseId",
                table: "livestream",
                newName: "IX_livestream_courseId");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_courseId",
                table: "lesson",
                newName: "IX_lesson_courseId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_userId",
                table: "feedback",
                newName: "IX_feedback_userId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_courseId",
                table: "feedback",
                newName: "IX_feedback_courseId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_userId",
                table: "enrollment",
                newName: "IX_enrollment_userId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_courseId",
                table: "enrollment",
                newName: "IX_enrollment_courseId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_lessonId",
                table: "document",
                newName: "IX_document_lessonId");

            migrationBuilder.RenameIndex(
                name: "IX_Choices_questionId",
                table: "choice",
                newName: "IX_choice_questionId");

            migrationBuilder.RenameIndex(
                name: "IX_AttemptAnswers_questionId",
                table: "attempt_answer",
                newName: "IX_attempt_answer_questionId");

            migrationBuilder.RenameIndex(
                name: "IX_AttemptAnswers_choiceId",
                table: "attempt_answer",
                newName: "IX_attempt_answer_choiceId");

            migrationBuilder.RenameIndex(
                name: "IX_AttemptAnswers_attemptId",
                table: "attempt_answer",
                newName: "IX_attempt_answer_attemptId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user",
                table: "user",
                column: "userId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_test",
                table: "test",
                column: "testId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_test_attempt",
                table: "test_attempt",
                column: "attemptId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tag",
                table: "tag",
                column: "tagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_study_plan",
                table: "study_plan",
                column: "planId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_study_plan_item",
                table: "study_plan_item",
                column: "itemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_profile",
                table: "student_profile",
                column: "userId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_role",
                table: "role",
                column: "roleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_report",
                table: "report",
                column: "reportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_question_test",
                table: "question_test",
                columns: new[] { "QuestionsquestionId", "TeststestId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_question_tag",
                table: "question_tag",
                columns: new[] { "QuestionsquestionId", "tagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_question",
                table: "question",
                column: "questionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_question_attachment",
                table: "question_attachment",
                column: "attachmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payment",
                table: "payment",
                column: "paymentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_message",
                table: "message",
                column: "messageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_livestream",
                table: "livestream",
                column: "livestreamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lesson",
                table: "lesson",
                column: "lessonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_instructor_profile",
                table: "instructor_profile",
                column: "userId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_feedback",
                table: "feedback",
                column: "feedbackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_enrollment",
                table: "enrollment",
                column: "enrollmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_document",
                table: "document",
                column: "documentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_course",
                table: "course",
                column: "courseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_conversation",
                table: "conversation",
                column: "conversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_choice",
                table: "choice",
                column: "choiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_attempt_answer",
                table: "attempt_answer",
                column: "answerId");

            migrationBuilder.AddForeignKey(
                name: "FK_attempt_answer_choice_choiceId",
                table: "attempt_answer",
                column: "choiceId",
                principalTable: "choice",
                principalColumn: "choiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_attempt_answer_question_questionId",
                table: "attempt_answer",
                column: "questionId",
                principalTable: "question",
                principalColumn: "questionId");

            migrationBuilder.AddForeignKey(
                name: "FK_attempt_answer_test_attempt_attemptId",
                table: "attempt_answer",
                column: "attemptId",
                principalTable: "test_attempt",
                principalColumn: "attemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_choice_question_questionId",
                table: "choice",
                column: "questionId",
                principalTable: "question",
                principalColumn: "questionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_conversation_Conversationsconversat~",
                table: "ConversationParticipant",
                column: "ConversationsconversationId",
                principalTable: "conversation",
                principalColumn: "conversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_user_ParticipantsuserId",
                table: "ConversationParticipant",
                column: "ParticipantsuserId",
                principalTable: "user",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseInstructor_course_CourseId",
                table: "CourseInstructor",
                column: "CourseId",
                principalTable: "course",
                principalColumn: "courseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseInstructor_user_UserId",
                table: "CourseInstructor",
                column: "UserId",
                principalTable: "user",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_document_lesson_lessonId",
                table: "document",
                column: "lessonId",
                principalTable: "lesson",
                principalColumn: "lessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_enrollment_course_courseId",
                table: "enrollment",
                column: "courseId",
                principalTable: "course",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_enrollment_user_userId",
                table: "enrollment",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_feedback_course_courseId",
                table: "feedback",
                column: "courseId",
                principalTable: "course",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_feedback_user_userId",
                table: "feedback",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_instructor_profile_user_userId",
                table: "instructor_profile",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_lesson_course_courseId",
                table: "lesson",
                column: "courseId",
                principalTable: "course",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_livestream_course_courseId",
                table: "livestream",
                column: "courseId",
                principalTable: "course",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_message_conversation_conversationId",
                table: "message",
                column: "conversationId",
                principalTable: "conversation",
                principalColumn: "conversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_message_user_senderId",
                table: "message",
                column: "senderId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_payment_user_userId",
                table: "payment",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_question_attachment_question_questionId",
                table: "question_attachment",
                column: "questionId",
                principalTable: "question",
                principalColumn: "questionId");

            migrationBuilder.AddForeignKey(
                name: "FK_question_tag_question_QuestionsquestionId",
                table: "question_tag",
                column: "QuestionsquestionId",
                principalTable: "question",
                principalColumn: "questionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_question_tag_tag_tagId",
                table: "question_tag",
                column: "tagId",
                principalTable: "tag",
                principalColumn: "tagId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_question_test_question_QuestionsquestionId",
                table: "question_test",
                column: "QuestionsquestionId",
                principalTable: "question",
                principalColumn: "questionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_question_test_test_TeststestId",
                table: "question_test",
                column: "TeststestId",
                principalTable: "test",
                principalColumn: "testId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_report_user_reportedInstructorId",
                table: "report",
                column: "reportedInstructorId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_report_user_reporterStudentId",
                table: "report",
                column: "reporterStudentId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_student_profile_user_userId",
                table: "student_profile",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_study_plan_user_createdByStaffId",
                table: "study_plan",
                column: "createdByStaffId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_study_plan_user_studentId",
                table: "study_plan",
                column: "studentId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_study_plan_item_study_plan_planId",
                table: "study_plan_item",
                column: "planId",
                principalTable: "study_plan",
                principalColumn: "planId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_lesson_lessonId",
                table: "test",
                column: "lessonId",
                principalTable: "lesson",
                principalColumn: "lessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_user_createdByUserId",
                table: "test",
                column: "createdByUserId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_attempt_test_testId",
                table: "test_attempt",
                column: "testId",
                principalTable: "test",
                principalColumn: "testId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_attempt_user_userId",
                table: "test_attempt",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_role_roleId",
                table: "user",
                column: "roleId",
                principalTable: "role",
                principalColumn: "roleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attempt_answer_choice_choiceId",
                table: "attempt_answer");

            migrationBuilder.DropForeignKey(
                name: "FK_attempt_answer_question_questionId",
                table: "attempt_answer");

            migrationBuilder.DropForeignKey(
                name: "FK_attempt_answer_test_attempt_attemptId",
                table: "attempt_answer");

            migrationBuilder.DropForeignKey(
                name: "FK_choice_question_questionId",
                table: "choice");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_conversation_Conversationsconversat~",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_user_ParticipantsuserId",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseInstructor_course_CourseId",
                table: "CourseInstructor");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseInstructor_user_UserId",
                table: "CourseInstructor");

            migrationBuilder.DropForeignKey(
                name: "FK_document_lesson_lessonId",
                table: "document");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollment_course_courseId",
                table: "enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_enrollment_user_userId",
                table: "enrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_feedback_course_courseId",
                table: "feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_feedback_user_userId",
                table: "feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_instructor_profile_user_userId",
                table: "instructor_profile");

            migrationBuilder.DropForeignKey(
                name: "FK_lesson_course_courseId",
                table: "lesson");

            migrationBuilder.DropForeignKey(
                name: "FK_livestream_course_courseId",
                table: "livestream");

            migrationBuilder.DropForeignKey(
                name: "FK_message_conversation_conversationId",
                table: "message");

            migrationBuilder.DropForeignKey(
                name: "FK_message_user_senderId",
                table: "message");

            migrationBuilder.DropForeignKey(
                name: "FK_payment_user_userId",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "FK_question_attachment_question_questionId",
                table: "question_attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_question_tag_question_QuestionsquestionId",
                table: "question_tag");

            migrationBuilder.DropForeignKey(
                name: "FK_question_tag_tag_tagId",
                table: "question_tag");

            migrationBuilder.DropForeignKey(
                name: "FK_question_test_question_QuestionsquestionId",
                table: "question_test");

            migrationBuilder.DropForeignKey(
                name: "FK_question_test_test_TeststestId",
                table: "question_test");

            migrationBuilder.DropForeignKey(
                name: "FK_report_user_reportedInstructorId",
                table: "report");

            migrationBuilder.DropForeignKey(
                name: "FK_report_user_reporterStudentId",
                table: "report");

            migrationBuilder.DropForeignKey(
                name: "FK_student_profile_user_userId",
                table: "student_profile");

            migrationBuilder.DropForeignKey(
                name: "FK_study_plan_user_createdByStaffId",
                table: "study_plan");

            migrationBuilder.DropForeignKey(
                name: "FK_study_plan_user_studentId",
                table: "study_plan");

            migrationBuilder.DropForeignKey(
                name: "FK_study_plan_item_study_plan_planId",
                table: "study_plan_item");

            migrationBuilder.DropForeignKey(
                name: "FK_test_lesson_lessonId",
                table: "test");

            migrationBuilder.DropForeignKey(
                name: "FK_test_user_createdByUserId",
                table: "test");

            migrationBuilder.DropForeignKey(
                name: "FK_test_attempt_test_testId",
                table: "test_attempt");

            migrationBuilder.DropForeignKey(
                name: "FK_test_attempt_user_userId",
                table: "test_attempt");

            migrationBuilder.DropForeignKey(
                name: "FK_user_role_roleId",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "PK_test_attempt",
                table: "test_attempt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_test",
                table: "test");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tag",
                table: "tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_study_plan_item",
                table: "study_plan_item");

            migrationBuilder.DropPrimaryKey(
                name: "PK_study_plan",
                table: "study_plan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student_profile",
                table: "student_profile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_role",
                table: "role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_report",
                table: "report");

            migrationBuilder.DropPrimaryKey(
                name: "PK_question_test",
                table: "question_test");

            migrationBuilder.DropPrimaryKey(
                name: "PK_question_tag",
                table: "question_tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_question_attachment",
                table: "question_attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_question",
                table: "question");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payment",
                table: "payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_message",
                table: "message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_livestream",
                table: "livestream");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lesson",
                table: "lesson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_instructor_profile",
                table: "instructor_profile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_feedback",
                table: "feedback");

            migrationBuilder.DropPrimaryKey(
                name: "PK_enrollment",
                table: "enrollment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_document",
                table: "document");

            migrationBuilder.DropPrimaryKey(
                name: "PK_course",
                table: "course");

            migrationBuilder.DropPrimaryKey(
                name: "PK_conversation",
                table: "conversation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_choice",
                table: "choice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_attempt_answer",
                table: "attempt_answer");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "test_attempt",
                newName: "TestAttempts");

            migrationBuilder.RenameTable(
                name: "test",
                newName: "Tests");

            migrationBuilder.RenameTable(
                name: "tag",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "study_plan_item",
                newName: "StudyPlanItems");

            migrationBuilder.RenameTable(
                name: "study_plan",
                newName: "StudyPlans");

            migrationBuilder.RenameTable(
                name: "student_profile",
                newName: "StudentProfiles");

            migrationBuilder.RenameTable(
                name: "role",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "report",
                newName: "Reports");

            migrationBuilder.RenameTable(
                name: "question_test",
                newName: "QuestionTests");

            migrationBuilder.RenameTable(
                name: "question_tag",
                newName: "QuestionTags");

            migrationBuilder.RenameTable(
                name: "question_attachment",
                newName: "QuestionAttachments");

            migrationBuilder.RenameTable(
                name: "question",
                newName: "Questions");

            migrationBuilder.RenameTable(
                name: "payment",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "message",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "livestream",
                newName: "Livestreams");

            migrationBuilder.RenameTable(
                name: "lesson",
                newName: "Lessons");

            migrationBuilder.RenameTable(
                name: "instructor_profile",
                newName: "InstructorProfiles");

            migrationBuilder.RenameTable(
                name: "feedback",
                newName: "Feedbacks");

            migrationBuilder.RenameTable(
                name: "enrollment",
                newName: "Enrollments");

            migrationBuilder.RenameTable(
                name: "document",
                newName: "Documents");

            migrationBuilder.RenameTable(
                name: "course",
                newName: "Courses");

            migrationBuilder.RenameTable(
                name: "conversation",
                newName: "Conversations");

            migrationBuilder.RenameTable(
                name: "choice",
                newName: "Choices");

            migrationBuilder.RenameTable(
                name: "attempt_answer",
                newName: "AttemptAnswers");

            migrationBuilder.RenameIndex(
                name: "IX_user_roleId",
                table: "Users",
                newName: "IX_Users_roleId");

            migrationBuilder.RenameIndex(
                name: "IX_test_attempt_userId",
                table: "TestAttempts",
                newName: "IX_TestAttempts_userId");

            migrationBuilder.RenameIndex(
                name: "IX_test_attempt_testId",
                table: "TestAttempts",
                newName: "IX_TestAttempts_testId");

            migrationBuilder.RenameIndex(
                name: "IX_test_lessonId",
                table: "Tests",
                newName: "IX_Tests_lessonId");

            migrationBuilder.RenameIndex(
                name: "IX_test_createdByUserId",
                table: "Tests",
                newName: "IX_Tests_createdByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_study_plan_item_planId",
                table: "StudyPlanItems",
                newName: "IX_StudyPlanItems_planId");

            migrationBuilder.RenameIndex(
                name: "IX_study_plan_studentId",
                table: "StudyPlans",
                newName: "IX_StudyPlans_studentId");

            migrationBuilder.RenameIndex(
                name: "IX_study_plan_createdByStaffId",
                table: "StudyPlans",
                newName: "IX_StudyPlans_createdByStaffId");

            migrationBuilder.RenameIndex(
                name: "IX_report_reporterStudentId",
                table: "Reports",
                newName: "IX_Reports_reporterStudentId");

            migrationBuilder.RenameIndex(
                name: "IX_report_reportedInstructorId",
                table: "Reports",
                newName: "IX_Reports_reportedInstructorId");

            migrationBuilder.RenameIndex(
                name: "IX_question_test_TeststestId",
                table: "QuestionTests",
                newName: "IX_QuestionTests_TeststestId");

            migrationBuilder.RenameIndex(
                name: "IX_question_tag_tagId",
                table: "QuestionTags",
                newName: "IX_QuestionTags_tagId");

            migrationBuilder.RenameIndex(
                name: "IX_question_attachment_questionId",
                table: "QuestionAttachments",
                newName: "IX_QuestionAttachments_questionId");

            migrationBuilder.RenameIndex(
                name: "IX_payment_userId",
                table: "Payments",
                newName: "IX_Payments_userId");

            migrationBuilder.RenameIndex(
                name: "IX_message_senderId",
                table: "Messages",
                newName: "IX_Messages_senderId");

            migrationBuilder.RenameIndex(
                name: "IX_message_conversationId",
                table: "Messages",
                newName: "IX_Messages_conversationId");

            migrationBuilder.RenameIndex(
                name: "IX_livestream_courseId",
                table: "Livestreams",
                newName: "IX_Livestreams_courseId");

            migrationBuilder.RenameIndex(
                name: "IX_lesson_courseId",
                table: "Lessons",
                newName: "IX_Lessons_courseId");

            migrationBuilder.RenameIndex(
                name: "IX_feedback_userId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_userId");

            migrationBuilder.RenameIndex(
                name: "IX_feedback_courseId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_courseId");

            migrationBuilder.RenameIndex(
                name: "IX_enrollment_userId",
                table: "Enrollments",
                newName: "IX_Enrollments_userId");

            migrationBuilder.RenameIndex(
                name: "IX_enrollment_courseId",
                table: "Enrollments",
                newName: "IX_Enrollments_courseId");

            migrationBuilder.RenameIndex(
                name: "IX_document_lessonId",
                table: "Documents",
                newName: "IX_Documents_lessonId");

            migrationBuilder.RenameIndex(
                name: "IX_choice_questionId",
                table: "Choices",
                newName: "IX_Choices_questionId");

            migrationBuilder.RenameIndex(
                name: "IX_attempt_answer_questionId",
                table: "AttemptAnswers",
                newName: "IX_AttemptAnswers_questionId");

            migrationBuilder.RenameIndex(
                name: "IX_attempt_answer_choiceId",
                table: "AttemptAnswers",
                newName: "IX_AttemptAnswers_choiceId");

            migrationBuilder.RenameIndex(
                name: "IX_attempt_answer_attemptId",
                table: "AttemptAnswers",
                newName: "IX_AttemptAnswers_attemptId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "userId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestAttempts",
                table: "TestAttempts",
                column: "attemptId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests",
                table: "Tests",
                column: "testId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "tagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyPlanItems",
                table: "StudyPlanItems",
                column: "itemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyPlans",
                table: "StudyPlans",
                column: "planId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentProfiles",
                table: "StudentProfiles",
                column: "userId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "roleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                column: "reportId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionTests",
                table: "QuestionTests",
                columns: new[] { "QuestionsquestionId", "TeststestId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionTags",
                table: "QuestionTags",
                columns: new[] { "QuestionsquestionId", "tagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionAttachments",
                table: "QuestionAttachments",
                column: "attachmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "questionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "paymentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "messageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Livestreams",
                table: "Livestreams",
                column: "livestreamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lessons",
                table: "Lessons",
                column: "lessonId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructorProfiles",
                table: "InstructorProfiles",
                column: "userId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks",
                column: "feedbackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments",
                column: "enrollmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "documentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "courseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations",
                column: "conversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Choices",
                table: "Choices",
                column: "choiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttemptAnswers",
                table: "AttemptAnswers",
                column: "answerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptAnswers_Choices_choiceId",
                table: "AttemptAnswers",
                column: "choiceId",
                principalTable: "Choices",
                principalColumn: "choiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptAnswers_Questions_questionId",
                table: "AttemptAnswers",
                column: "questionId",
                principalTable: "Questions",
                principalColumn: "questionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttemptAnswers_TestAttempts_attemptId",
                table: "AttemptAnswers",
                column: "attemptId",
                principalTable: "TestAttempts",
                principalColumn: "attemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Choices_Questions_questionId",
                table: "Choices",
                column: "questionId",
                principalTable: "Questions",
                principalColumn: "questionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_Conversations_Conversationsconversa~",
                table: "ConversationParticipant",
                column: "ConversationsconversationId",
                principalTable: "Conversations",
                principalColumn: "conversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_Users_ParticipantsuserId",
                table: "ConversationParticipant",
                column: "ParticipantsuserId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseInstructor_Courses_CourseId",
                table: "CourseInstructor",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "courseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseInstructor_Users_UserId",
                table: "CourseInstructor",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Lessons_lessonId",
                table: "Documents",
                column: "lessonId",
                principalTable: "Lessons",
                principalColumn: "lessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_courseId",
                table: "Enrollments",
                column: "courseId",
                principalTable: "Courses",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_userId",
                table: "Enrollments",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Courses_courseId",
                table: "Feedbacks",
                column: "courseId",
                principalTable: "Courses",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Users_userId",
                table: "Feedbacks",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorProfiles_Users_userId",
                table: "InstructorProfiles",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Courses_courseId",
                table: "Lessons",
                column: "courseId",
                principalTable: "Courses",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Livestreams_Courses_courseId",
                table: "Livestreams",
                column: "courseId",
                principalTable: "Courses",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_conversationId",
                table: "Messages",
                column: "conversationId",
                principalTable: "Conversations",
                principalColumn: "conversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_senderId",
                table: "Messages",
                column: "senderId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_userId",
                table: "Payments",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAttachments_Questions_questionId",
                table: "QuestionAttachments",
                column: "questionId",
                principalTable: "Questions",
                principalColumn: "questionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Questions_QuestionsquestionId",
                table: "QuestionTags",
                column: "QuestionsquestionId",
                principalTable: "Questions",
                principalColumn: "questionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Tags_tagId",
                table: "QuestionTags",
                column: "tagId",
                principalTable: "Tags",
                principalColumn: "tagId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTests_Questions_QuestionsquestionId",
                table: "QuestionTests",
                column: "QuestionsquestionId",
                principalTable: "Questions",
                principalColumn: "questionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTests_Tests_TeststestId",
                table: "QuestionTests",
                column: "TeststestId",
                principalTable: "Tests",
                principalColumn: "testId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_reportedInstructorId",
                table: "Reports",
                column: "reportedInstructorId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_reporterStudentId",
                table: "Reports",
                column: "reporterStudentId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_Users_userId",
                table: "StudentProfiles",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlanItems_StudyPlans_planId",
                table: "StudyPlanItems",
                column: "planId",
                principalTable: "StudyPlans",
                principalColumn: "planId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlans_Users_createdByStaffId",
                table: "StudyPlans",
                column: "createdByStaffId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlans_Users_studentId",
                table: "StudyPlans",
                column: "studentId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAttempts_Tests_testId",
                table: "TestAttempts",
                column: "testId",
                principalTable: "Tests",
                principalColumn: "testId");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAttempts_Users_userId",
                table: "TestAttempts",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Lessons_lessonId",
                table: "Tests",
                column: "lessonId",
                principalTable: "Lessons",
                principalColumn: "lessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Users_createdByUserId",
                table: "Tests",
                column: "createdByUserId",
                principalTable: "Users",
                principalColumn: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_roleId",
                table: "Users",
                column: "roleId",
                principalTable: "Roles",
                principalColumn: "roleId");
        }
    }
}
