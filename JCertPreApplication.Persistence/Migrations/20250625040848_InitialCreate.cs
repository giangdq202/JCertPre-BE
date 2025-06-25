using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    conversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    conversationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.conversationId);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    questionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    questionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    questionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    explanation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.questionId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    roleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    roleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.roleId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    tagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tagLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    contentSection = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    contentDetail = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    tagScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.tagId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    fullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    passwordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    avatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    credit = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    choiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    questionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    choiceText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => x.choiceId);
                    table.ForeignKey(
                        name: "FK_Choices_Questions_questionId",
                        column: x => x.questionId,
                        principalTable: "Questions",
                        principalColumn: "questionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAttachments",
                columns: table => new
                {
                    attachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    questionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    mediaUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    mediaType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAttachments", x => x.attachmentId);
                    table.ForeignKey(
                        name: "FK_QuestionAttachments_Questions_questionId",
                        column: x => x.questionId,
                        principalTable: "Questions",
                        principalColumn: "questionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTags",
                columns: table => new
                {
                    QuestionsquestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTags", x => new { x.QuestionsquestionId, x.tagId });
                    table.ForeignKey(
                        name: "FK_QuestionTags_Questions_QuestionsquestionId",
                        column: x => x.QuestionsquestionId,
                        principalTable: "Questions",
                        principalColumn: "questionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionTags_Tags_tagId",
                        column: x => x.tagId,
                        principalTable: "Tags",
                        principalColumn: "tagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationParticipant",
                columns: table => new
                {
                    ConversationsconversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantsuserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationParticipant", x => new { x.ConversationsconversationId, x.ParticipantsuserId });
                    table.ForeignKey(
                        name: "FK_ConversationParticipant_Conversations_ConversationsconversationId",
                        column: x => x.ConversationsconversationId,
                        principalTable: "Conversations",
                        principalColumn: "conversationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationParticipant_Users_ParticipantsuserId",
                        column: x => x.ParticipantsuserId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    courseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    staffCreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    level = table.Column<int>(type: "int", nullable: false),
                    courseType = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    thumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.courseId);
                    table.ForeignKey(
                        name: "FK_Courses_Users_staffCreateUserId",
                        column: x => x.staffCreateUserId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorProfiles",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    introduction = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    experience = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    teachingStyle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorProfiles", x => x.userId);
                    table.ForeignKey(
                        name: "FK_InstructorProfiles_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    messageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    senderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    conversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    sentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.messageId);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_conversationId",
                        column: x => x.conversationId,
                        principalTable: "Conversations",
                        principalColumn: "conversationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Users_senderId",
                        column: x => x.senderId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    paymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    paymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    transactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.paymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    reportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reporterStudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reportedInstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reportContent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.reportId);
                    table.ForeignKey(
                        name: "FK_Reports_Users_reportedInstructorId",
                        column: x => x.reportedInstructorId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Users_reporterStudentId",
                        column: x => x.reporterStudentId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    currentLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    learningGoals = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.userId);
                    table.ForeignKey(
                        name: "FK_StudentProfiles_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlans",
                columns: table => new
                {
                    planId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    studentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createdByStaffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    planName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    startDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlans", x => x.planId);
                    table.ForeignKey(
                        name: "FK_StudyPlans_Users_createdByStaffId",
                        column: x => x.createdByStaffId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyPlans_Users_studentId",
                        column: x => x.studentId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    roleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    assignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.userId, x.roleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_roleId",
                        column: x => x.roleId,
                        principalTable: "Roles",
                        principalColumn: "roleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    enrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    courseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    enrollDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.enrollmentId);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "courseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    feedbackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    courseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reply = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.feedbackId);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "courseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    lessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    courseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    lessonOrder = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.lessonId);
                    table.ForeignKey(
                        name: "FK_Lessons_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "courseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Livestreams",
                columns: table => new
                {
                    livestreamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    courseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    startTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    meetingUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    recordingUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livestreams", x => x.livestreamId);
                    table.ForeignKey(
                        name: "FK_Livestreams_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "courseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyPlanItems",
                columns: table => new
                {
                    itemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    planId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sequence = table.Column<int>(type: "int", nullable: false),
                    itemType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    itemIdRef = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlanItems", x => x.itemId);
                    table.ForeignKey(
                        name: "FK_StudyPlanItems_StudyPlans_planId",
                        column: x => x.planId,
                        principalTable: "StudyPlans",
                        principalColumn: "planId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    documentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    lessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    documentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    fileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.documentId);
                    table.ForeignKey(
                        name: "FK_Documents_Lessons_lessonId",
                        column: x => x.lessonId,
                        principalTable: "Lessons",
                        principalColumn: "lessonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    testId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    testType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    durationMinutes = table.Column<int>(type: "int", nullable: false),
                    lessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    createdByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.testId);
                    table.ForeignKey(
                        name: "FK_Tests_Lessons_lessonId",
                        column: x => x.lessonId,
                        principalTable: "Lessons",
                        principalColumn: "lessonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tests_Users_createdByUserId",
                        column: x => x.createdByUserId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTests",
                columns: table => new
                {
                    QuestionsquestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeststestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTests", x => new { x.QuestionsquestionId, x.TeststestId });
                    table.ForeignKey(
                        name: "FK_QuestionTests_Questions_QuestionsquestionId",
                        column: x => x.QuestionsquestionId,
                        principalTable: "Questions",
                        principalColumn: "questionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionTests_Tests_TeststestId",
                        column: x => x.TeststestId,
                        principalTable: "Tests",
                        principalColumn: "testId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestAttempts",
                columns: table => new
                {
                    attemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    testId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    startTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    endTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    totalScore = table.Column<int>(type: "int", nullable: false),
                    languageKnowledgeScore = table.Column<int>(type: "int", nullable: false),
                    readingScore = table.Column<int>(type: "int", nullable: false),
                    listeningScore = table.Column<int>(type: "int", nullable: false),
                    isPass = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttempts", x => x.attemptId);
                    table.ForeignKey(
                        name: "FK_TestAttempts_Tests_testId",
                        column: x => x.testId,
                        principalTable: "Tests",
                        principalColumn: "testId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestAttempts_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttemptAnswers",
                columns: table => new
                {
                    answerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    attemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    questionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    choiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptAnswers", x => x.answerId);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_Choices_choiceId",
                        column: x => x.choiceId,
                        principalTable: "Choices",
                        principalColumn: "choiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_Questions_questionId",
                        column: x => x.questionId,
                        principalTable: "Questions",
                        principalColumn: "questionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_TestAttempts_attemptId",
                        column: x => x.attemptId,
                        principalTable: "TestAttempts",
                        principalColumn: "attemptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_attemptId",
                table: "AttemptAnswers",
                column: "attemptId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_choiceId",
                table: "AttemptAnswers",
                column: "choiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptAnswers_questionId",
                table: "AttemptAnswers",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_Choices_questionId",
                table: "Choices",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipant_ParticipantsuserId",
                table: "ConversationParticipant",
                column: "ParticipantsuserId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_staffCreateUserId",
                table: "Courses",
                column: "staffCreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_lessonId",
                table: "Documents",
                column: "lessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_courseId",
                table: "Enrollments",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_userId",
                table: "Enrollments",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_courseId",
                table: "Feedbacks",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_userId",
                table: "Feedbacks",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_courseId",
                table: "Lessons",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_Livestreams_courseId",
                table: "Livestreams",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_conversationId",
                table: "Messages",
                column: "conversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_senderId",
                table: "Messages",
                column: "senderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_userId",
                table: "Payments",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAttachments_questionId",
                table: "QuestionAttachments",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTags_tagId",
                table: "QuestionTags",
                column: "tagId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTests_TeststestId",
                table: "QuestionTests",
                column: "TeststestId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_reportedInstructorId",
                table: "Reports",
                column: "reportedInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_reporterStudentId",
                table: "Reports",
                column: "reporterStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlanItems_planId",
                table: "StudyPlanItems",
                column: "planId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_createdByStaffId",
                table: "StudyPlans",
                column: "createdByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPlans_studentId",
                table: "StudyPlans",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_testId",
                table: "TestAttempts",
                column: "testId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_userId",
                table: "TestAttempts",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_createdByUserId",
                table: "Tests",
                column: "createdByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_lessonId",
                table: "Tests",
                column: "lessonId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_roleId",
                table: "UserRoles",
                column: "roleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttemptAnswers");

            migrationBuilder.DropTable(
                name: "ConversationParticipant");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "InstructorProfiles");

            migrationBuilder.DropTable(
                name: "Livestreams");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "QuestionAttachments");

            migrationBuilder.DropTable(
                name: "QuestionTags");

            migrationBuilder.DropTable(
                name: "QuestionTests");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "StudentProfiles");

            migrationBuilder.DropTable(
                name: "StudyPlanItems");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Choices");

            migrationBuilder.DropTable(
                name: "TestAttempts");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "StudyPlans");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
