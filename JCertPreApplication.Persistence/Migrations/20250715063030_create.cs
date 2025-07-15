using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JCertPreApplication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conversation",
                columns: table => new
                {
                    conversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    conversationName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversation", x => x.conversationId);
                });

            migrationBuilder.CreateTable(
                name: "course",
                columns: table => new
                {
                    courseId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    level = table.Column<string>(type: "text", nullable: false),
                    courseType = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    thumbnailUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course", x => x.courseId);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    roleId = table.Column<Guid>(type: "uuid", nullable: false),
                    roleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.roleId);
                });

            migrationBuilder.CreateTable(
                name: "sub_contents",
                columns: table => new
                {
                    SubContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubContentName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Level = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ContentName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sub_contents", x => x.SubContentId);
                });

            migrationBuilder.CreateTable(
                name: "lesson",
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
                    table.PrimaryKey("PK_lesson", x => x.lessonId);
                    table.ForeignKey(
                        name: "FK_lesson_course_courseId",
                        column: x => x.courseId,
                        principalTable: "course",
                        principalColumn: "courseId");
                });

            migrationBuilder.CreateTable(
                name: "livestream",
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
                    table.PrimaryKey("PK_livestream", x => x.livestreamId);
                    table.ForeignKey(
                        name: "FK_livestream_course_courseId",
                        column: x => x.courseId,
                        principalTable: "course",
                        principalColumn: "courseId");
                });

            migrationBuilder.CreateTable(
                name: "user",
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
                    status = table.Column<string>(type: "text", nullable: false),
                    roleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.userId);
                    table.ForeignKey(
                        name: "FK_user_role_roleId",
                        column: x => x.roleId,
                        principalTable: "role",
                        principalColumn: "roleId");
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionText = table.Column<string>(type: "text", nullable: false),
                    questionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    explanation = table.Column<string>(type: "text", nullable: false),
                    difficulty = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.questionId);
                    table.ForeignKey(
                        name: "FK_questions_sub_contents_SubContentId",
                        column: x => x.SubContentId,
                        principalTable: "sub_contents",
                        principalColumn: "SubContentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "document",
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
                    table.PrimaryKey("PK_document", x => x.documentId);
                    table.ForeignKey(
                        name: "FK_document_lesson_lessonId",
                        column: x => x.lessonId,
                        principalTable: "lesson",
                        principalColumn: "lessonId");
                });

            migrationBuilder.CreateTable(
                name: "conversation_participant",
                columns: table => new
                {
                    ConversationsconversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantsuserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conversation_participant", x => new { x.ConversationsconversationId, x.ParticipantsuserId });
                    table.ForeignKey(
                        name: "FK_conversation_participant_conversation_Conversationsconversa~",
                        column: x => x.ConversationsconversationId,
                        principalTable: "conversation",
                        principalColumn: "conversationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_conversation_participant_user_ParticipantsuserId",
                        column: x => x.ParticipantsuserId,
                        principalTable: "user",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_instructor",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeftOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_instructor", x => new { x.CourseId, x.InstructorId, x.AssignedOn });
                    table.ForeignKey(
                        name: "FK_course_instructor_course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "course",
                        principalColumn: "courseId");
                    table.ForeignKey(
                        name: "FK_course_instructor_user_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "enrollment",
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
                    table.PrimaryKey("PK_enrollment", x => x.enrollmentId);
                    table.ForeignKey(
                        name: "FK_enrollment_course_courseId",
                        column: x => x.courseId,
                        principalTable: "course",
                        principalColumn: "courseId");
                    table.ForeignKey(
                        name: "FK_enrollment_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "exam_pass_threshold",
                columns: table => new
                {
                    ExamPassThresholdId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TotalMaxScore = table.Column<int>(type: "integer", nullable: false),
                    TotalPassingScore = table.Column<int>(type: "integer", nullable: false),
                    LanguageKnowledgeMin = table.Column<int>(type: "integer", nullable: false),
                    LanguageKnowledgeMax = table.Column<int>(type: "integer", nullable: false),
                    ReadingMax = table.Column<int>(type: "integer", nullable: false),
                    ReadingMin = table.Column<int>(type: "integer", nullable: false),
                    ListeningMax = table.Column<int>(type: "integer", nullable: false),
                    ListeningMin = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exam_pass_threshold", x => x.ExamPassThresholdId);
                    table.ForeignKey(
                        name: "FK_exam_pass_threshold_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "feedback",
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
                    table.PrimaryKey("PK_feedback", x => x.feedbackId);
                    table.ForeignKey(
                        name: "FK_feedback_course_courseId",
                        column: x => x.courseId,
                        principalTable: "course",
                        principalColumn: "courseId");
                    table.ForeignKey(
                        name: "FK_feedback_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "instructor_profile",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    introduction = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    experience = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    teachingStyle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instructor_profile", x => x.userId);
                    table.ForeignKey(
                        name: "FK_instructor_profile_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "lesson_progress",
                columns: table => new
                {
                    progressId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    lessonId = table.Column<Guid>(type: "uuid", nullable: false),
                    isCompleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lesson_progress", x => x.progressId);
                    table.ForeignKey(
                        name: "FK_lesson_progress_lesson_lessonId",
                        column: x => x.lessonId,
                        principalTable: "lesson",
                        principalColumn: "lessonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lesson_progress_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message",
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
                    table.PrimaryKey("PK_message", x => x.messageId);
                    table.ForeignKey(
                        name: "FK_message_conversation_conversationId",
                        column: x => x.conversationId,
                        principalTable: "conversation",
                        principalColumn: "conversationId");
                    table.ForeignKey(
                        name: "FK_message_user_senderId",
                        column: x => x.senderId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    paymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentType = table.Column<string>(type: "text", nullable: false),
                    paymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.paymentId);
                    table.ForeignKey(
                        name: "FK_payment_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "report",
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

            migrationBuilder.CreateTable(
                name: "student_profile",
                columns: table => new
                {
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    currentLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    learningGoals = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_profile", x => x.userId);
                    table.ForeignKey(
                        name: "FK_student_profile_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "study_plan",
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
                    table.PrimaryKey("PK_study_plan", x => x.planId);
                    table.ForeignKey(
                        name: "FK_study_plan_user_createdByStaffId",
                        column: x => x.createdByStaffId,
                        principalTable: "user",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_study_plan_user_studentId",
                        column: x => x.studentId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "test",
                columns: table => new
                {
                    testId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    testType = table.Column<string>(type: "text", nullable: false),
                    durationMinutes = table.Column<int>(type: "integer", nullable: false),
                    lessonId = table.Column<Guid>(type: "uuid", nullable: true),
                    createdByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    availableFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    availableTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    maxAttempts = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test", x => x.testId);
                    table.ForeignKey(
                        name: "FK_test_lesson_lessonId",
                        column: x => x.lessonId,
                        principalTable: "lesson",
                        principalColumn: "lessonId");
                    table.ForeignKey(
                        name: "FK_test_user_createdByUserId",
                        column: x => x.createdByUserId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "choice",
                columns: table => new
                {
                    choiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    choiceText = table.Column<string>(type: "text", nullable: false),
                    isCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_choice", x => x.choiceId);
                    table.ForeignKey(
                        name: "FK_choice_questions_questionId",
                        column: x => x.questionId,
                        principalTable: "questions",
                        principalColumn: "questionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_attachment",
                columns: table => new
                {
                    attachmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    mediaUrl = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    mediaType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_attachment", x => x.attachmentId);
                    table.ForeignKey(
                        name: "FK_question_attachment_questions_questionId",
                        column: x => x.questionId,
                        principalTable: "questions",
                        principalColumn: "questionId");
                });

            migrationBuilder.CreateTable(
                name: "study_plan_item",
                columns: table => new
                {
                    itemId = table.Column<Guid>(type: "uuid", nullable: false),
                    planId = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    itemType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    courseId = table.Column<Guid>(type: "uuid", nullable: true),
                    testId = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_study_plan_item", x => x.itemId);
                    table.ForeignKey(
                        name: "FK_study_plan_item_course_courseId",
                        column: x => x.courseId,
                        principalTable: "course",
                        principalColumn: "courseId");
                    table.ForeignKey(
                        name: "FK_study_plan_item_study_plan_planId",
                        column: x => x.planId,
                        principalTable: "study_plan",
                        principalColumn: "planId");
                    table.ForeignKey(
                        name: "FK_study_plan_item_test_testId",
                        column: x => x.testId,
                        principalTable: "test",
                        principalColumn: "testId");
                });

            migrationBuilder.CreateTable(
                name: "test_attempt",
                columns: table => new
                {
                    attemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false),
                    testId = table.Column<Guid>(type: "uuid", nullable: false),
                    startTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    attemptNumber = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    totalScore = table.Column<int>(type: "integer", nullable: true),
                    languageKnowledgeScore = table.Column<int>(type: "integer", nullable: true),
                    readingScore = table.Column<int>(type: "integer", nullable: true),
                    listeningScore = table.Column<int>(type: "integer", nullable: true),
                    isPass = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_attempt", x => x.attemptId);
                    table.ForeignKey(
                        name: "FK_test_attempt_test_testId",
                        column: x => x.testId,
                        principalTable: "test",
                        principalColumn: "testId");
                    table.ForeignKey(
                        name: "FK_test_attempt_user_userId",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "test_question",
                columns: table => new
                {
                    testQuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    testId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_test_question", x => x.testQuestionId);
                    table.ForeignKey(
                        name: "FK_test_question_questions_questionId",
                        column: x => x.questionId,
                        principalTable: "questions",
                        principalColumn: "questionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_test_question_test_testId",
                        column: x => x.testId,
                        principalTable: "test",
                        principalColumn: "testId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attempt_answer",
                columns: table => new
                {
                    answerId = table.Column<Guid>(type: "uuid", nullable: false),
                    attemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    questionId = table.Column<Guid>(type: "uuid", nullable: false),
                    choiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    isCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attempt_answer", x => x.answerId);
                    table.ForeignKey(
                        name: "FK_attempt_answer_choice_choiceId",
                        column: x => x.choiceId,
                        principalTable: "choice",
                        principalColumn: "choiceId");
                    table.ForeignKey(
                        name: "FK_attempt_answer_questions_questionId",
                        column: x => x.questionId,
                        principalTable: "questions",
                        principalColumn: "questionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attempt_answer_test_attempt_attemptId",
                        column: x => x.attemptId,
                        principalTable: "test_attempt",
                        principalColumn: "attemptId");
                });

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
                name: "IX_attempt_answer_attemptId",
                table: "attempt_answer",
                column: "attemptId");

            migrationBuilder.CreateIndex(
                name: "IX_attempt_answer_choiceId",
                table: "attempt_answer",
                column: "choiceId");

            migrationBuilder.CreateIndex(
                name: "IX_attempt_answer_questionId",
                table: "attempt_answer",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_choice_questionId",
                table: "choice",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_conversation_participant_ParticipantsuserId",
                table: "conversation_participant",
                column: "ParticipantsuserId");

            migrationBuilder.CreateIndex(
                name: "IX_course_instructor_InstructorId",
                table: "course_instructor",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_document_lessonId",
                table: "document",
                column: "lessonId");

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_courseId",
                table: "enrollment",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_enrollment_userId",
                table: "enrollment",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_exam_pass_threshold_UserId",
                table: "exam_pass_threshold",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_courseId",
                table: "feedback",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_userId",
                table: "feedback",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_courseId",
                table: "lesson",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_progress_lessonId",
                table: "lesson_progress",
                column: "lessonId");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_progress_userId",
                table: "lesson_progress",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_livestream_courseId",
                table: "livestream",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_message_conversationId",
                table: "message",
                column: "conversationId");

            migrationBuilder.CreateIndex(
                name: "IX_message_senderId",
                table: "message",
                column: "senderId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_userId",
                table: "payment",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_question_attachment_questionId",
                table: "question_attachment",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_SubContentId",
                table: "questions",
                column: "SubContentId");

            migrationBuilder.CreateIndex(
                name: "IX_report_reportedInstructorId",
                table: "report",
                column: "reportedInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_report_reporterStudentId",
                table: "report",
                column: "reporterStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_study_plan_createdByStaffId",
                table: "study_plan",
                column: "createdByStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_study_plan_studentId",
                table: "study_plan",
                column: "studentId");

            migrationBuilder.CreateIndex(
                name: "IX_study_plan_item_courseId",
                table: "study_plan_item",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_study_plan_item_planId",
                table: "study_plan_item",
                column: "planId");

            migrationBuilder.CreateIndex(
                name: "IX_study_plan_item_testId",
                table: "study_plan_item",
                column: "testId");

            migrationBuilder.CreateIndex(
                name: "IX_test_createdByUserId",
                table: "test",
                column: "createdByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_test_lessonId",
                table: "test",
                column: "lessonId");

            migrationBuilder.CreateIndex(
                name: "IX_test_attempt_testId",
                table: "test_attempt",
                column: "testId");

            migrationBuilder.CreateIndex(
                name: "IX_test_attempt_userId",
                table: "test_attempt",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_test_question_questionId",
                table: "test_question",
                column: "questionId");

            migrationBuilder.CreateIndex(
                name: "IX_test_question_testId",
                table: "test_question",
                column: "testId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roleId",
                table: "user",
                column: "roleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attempt_answer");

            migrationBuilder.DropTable(
                name: "conversation_participant");

            migrationBuilder.DropTable(
                name: "course_instructor");

            migrationBuilder.DropTable(
                name: "document");

            migrationBuilder.DropTable(
                name: "enrollment");

            migrationBuilder.DropTable(
                name: "exam_pass_threshold");

            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "instructor_profile");

            migrationBuilder.DropTable(
                name: "lesson_progress");

            migrationBuilder.DropTable(
                name: "livestream");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "question_attachment");

            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropTable(
                name: "student_profile");

            migrationBuilder.DropTable(
                name: "study_plan_item");

            migrationBuilder.DropTable(
                name: "test_question");

            migrationBuilder.DropTable(
                name: "choice");

            migrationBuilder.DropTable(
                name: "test_attempt");

            migrationBuilder.DropTable(
                name: "conversation");

            migrationBuilder.DropTable(
                name: "study_plan");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "test");

            migrationBuilder.DropTable(
                name: "sub_contents");

            migrationBuilder.DropTable(
                name: "lesson");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "course");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
