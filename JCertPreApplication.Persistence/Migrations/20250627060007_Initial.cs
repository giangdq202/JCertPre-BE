using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    conversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    conversationName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.conversationId);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionText = table.Column<string>(type: "text", nullable: false),
                    questionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    explanation = table.Column<string>(type: "text", nullable: false),
                    tagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.questionId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    roleId = table.Column<Guid>(type: "uuid", nullable: false),
                    roleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.roleId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    tagId = table.Column<Guid>(type: "uuid", nullable: false),
                    tagLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    contentSection = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contentDetail = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    tagScore = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.tagId);
                });

            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    choiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    choiceText = table.Column<string>(type: "text", nullable: false),
                    isCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => x.choiceId);
                    table.ForeignKey(
                        name: "FK_Choices_Questions_questionId",
                        column: x => x.questionId,
                        principalTable: "Questions",
                        principalColumn: "questionId");
                });

            migrationBuilder.CreateTable(
                name: "QuestionAttachments",
                columns: table => new
                {
                    attachmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    mediaUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mediaType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAttachments", x => x.attachmentId);
                    table.ForeignKey(
                        name: "FK_QuestionAttachments_Questions_questionId",
                        column: x => x.questionId,
                        principalTable: "Questions",
                        principalColumn: "questionId");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    fullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    passwordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    avatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    credit = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    roleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_roleId",
                        column: x => x.roleId,
                        principalTable: "Roles",
                        principalColumn: "roleId");
                });

            migrationBuilder.CreateTable(
                name: "QuestionTags",
                columns: table => new
                {
                    QuestionsquestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    tagId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    ConversationsconversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantsuserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationParticipant", x => new { x.ConversationsconversationId, x.ParticipantsuserId });
                    table.ForeignKey(
                        name: "FK_ConversationParticipant_Conversations_Conversationsconversa~",
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
                    courseId = table.Column<Guid>(type: "uuid", nullable: false),
                    staffCreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    courseType = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    thumbnailUrl = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.courseId);
                    table.ForeignKey(
                        name: "FK_Courses_Users_staffCreateUserId",
                        column: x => x.staffCreateUserId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "InstructorProfiles",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    introduction = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    experience = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    teachingStyle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorProfiles", x => x.userId);
                    table.ForeignKey(
                        name: "FK_InstructorProfiles_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    messageId = table.Column<Guid>(type: "uuid", nullable: false),
                    senderId = table.Column<Guid>(type: "uuid", nullable: false),
                    conversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    sentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.messageId);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_conversationId",
                        column: x => x.conversationId,
                        principalTable: "Conversations",
                        principalColumn: "conversationId");
                    table.ForeignKey(
                        name: "FK_Messages_Users_senderId",
                        column: x => x.senderId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    paymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    paymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.paymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    reportId = table.Column<Guid>(type: "uuid", nullable: false),
                    reporterStudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    reportedInstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    reportContent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.reportId);
                    table.ForeignKey(
                        name: "FK_Reports_Users_reportedInstructorId",
                        column: x => x.reportedInstructorId,
                        principalTable: "Users",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_Reports_Users_reporterStudentId",
                        column: x => x.reporterStudentId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    currentLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    learningGoals = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.userId);
                    table.ForeignKey(
                        name: "FK_StudentProfiles_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "StudyPlans",
                columns: table => new
                {
                    planId = table.Column<Guid>(type: "uuid", nullable: false),
                    studentId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdByStaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    planName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    startDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlans", x => x.planId);
                    table.ForeignKey(
                        name: "FK_StudyPlans_Users_createdByStaffId",
                        column: x => x.createdByStaffId,
                        principalTable: "Users",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_StudyPlans_Users_studentId",
                        column: x => x.studentId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    enrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    courseId = table.Column<Guid>(type: "uuid", nullable: false),
                    enrollDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.enrollmentId);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "courseId");
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    feedbackId = table.Column<Guid>(type: "uuid", nullable: false),
                    courseId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false),
                    reply = table.Column<string>(type: "text", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.feedbackId);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "courseId");
                    table.ForeignKey(
                        name: "FK_Feedbacks_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    lessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    courseId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    lessonOrder = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.lessonId);
                    table.ForeignKey(
                        name: "FK_Lessons_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "courseId");
                });

            migrationBuilder.CreateTable(
                name: "Livestreams",
                columns: table => new
                {
                    livestreamId = table.Column<Guid>(type: "uuid", nullable: false),
                    courseId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    startTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    meetingUrl = table.Column<string>(type: "text", nullable: false),
                    recordingUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livestreams", x => x.livestreamId);
                    table.ForeignKey(
                        name: "FK_Livestreams_Courses_courseId",
                        column: x => x.courseId,
                        principalTable: "Courses",
                        principalColumn: "courseId");
                });

            migrationBuilder.CreateTable(
                name: "StudyPlanItems",
                columns: table => new
                {
                    itemId = table.Column<Guid>(type: "uuid", nullable: false),
                    planId = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    itemType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    itemIdRef = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPlanItems", x => x.itemId);
                    table.ForeignKey(
                        name: "FK_StudyPlanItems_StudyPlans_planId",
                        column: x => x.planId,
                        principalTable: "StudyPlans",
                        principalColumn: "planId");
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    documentId = table.Column<Guid>(type: "uuid", nullable: false),
                    lessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    documentName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fileUrl = table.Column<string>(type: "text", nullable: false),
                    uploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.documentId);
                    table.ForeignKey(
                        name: "FK_Documents_Lessons_lessonId",
                        column: x => x.lessonId,
                        principalTable: "Lessons",
                        principalColumn: "lessonId");
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    testId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    testType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    durationMinutes = table.Column<int>(type: "integer", nullable: false),
                    lessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.testId);
                    table.ForeignKey(
                        name: "FK_Tests_Lessons_lessonId",
                        column: x => x.lessonId,
                        principalTable: "Lessons",
                        principalColumn: "lessonId");
                    table.ForeignKey(
                        name: "FK_Tests_Users_createdByUserId",
                        column: x => x.createdByUserId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "QuestionTests",
                columns: table => new
                {
                    QuestionsquestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeststestId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    attemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    testId = table.Column<Guid>(type: "uuid", nullable: false),
                    startTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    totalScore = table.Column<int>(type: "integer", nullable: false),
                    languageKnowledgeScore = table.Column<int>(type: "integer", nullable: false),
                    readingScore = table.Column<int>(type: "integer", nullable: false),
                    listeningScore = table.Column<int>(type: "integer", nullable: false),
                    isPass = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttempts", x => x.attemptId);
                    table.ForeignKey(
                        name: "FK_TestAttempts_Tests_testId",
                        column: x => x.testId,
                        principalTable: "Tests",
                        principalColumn: "testId");
                    table.ForeignKey(
                        name: "FK_TestAttempts_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "AttemptAnswers",
                columns: table => new
                {
                    answerId = table.Column<Guid>(type: "uuid", nullable: false),
                    attemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    choiceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptAnswers", x => x.answerId);
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_Choices_choiceId",
                        column: x => x.choiceId,
                        principalTable: "Choices",
                        principalColumn: "choiceId");
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_Questions_questionId",
                        column: x => x.questionId,
                        principalTable: "Questions",
                        principalColumn: "questionId");
                    table.ForeignKey(
                        name: "FK_AttemptAnswers_TestAttempts_attemptId",
                        column: x => x.attemptId,
                        principalTable: "TestAttempts",
                        principalColumn: "attemptId");
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
                name: "IX_Users_roleId",
                table: "Users",
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
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
