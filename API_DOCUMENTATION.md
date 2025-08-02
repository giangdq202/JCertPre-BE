# JCertPre-BE API Documentation

This document provides a comprehensive list of all available APIs in the JCertPre Backend application.

---

## 📋 Table of Contents

1. [🔐 Authentication APIs](#-authentication-apis)
2. [👤 User Management APIs](#-user-management-apis)  
3. [📚 Course Management APIs](#-course-management-apis)
4. [📖 Lesson Management APIs](#-lesson-management-apis)
5. [📊 Progress Tracking APIs](#-progress-tracking-apis)
6. [📋 Study Plan Management APIs](#-study-plan-management-apis)
7. [🎯 Study Plan Item APIs](#-study-plan-item-apis)
8. [📄 Sub Content Management APIs](#-sub-content-management-apis)
9. [📁 Document Management APIs](#-document-management-apis)
10. [📂 File Management APIs](#-file-management-apis)
11. [🎓 Enrollment Management APIs](#-enrollment-management-apis)
12. [👨‍🏫 Instructor Profile APIs](#-instructor-profile-apis)
13. [👨‍🎓 Student Profile APIs](#-student-profile-apis)
14. [💬 Conversation Management APIs](#-conversation-management-apis)
15. [📺 Livestream Management APIs](#-livestream-management-apis)
16. [🎥 LiveKit Integration APIs](#-livekit-integration-apis)
17. [📋 Test Management APIs](#-test-management-apis)
18. [❓ Question Management APIs](#-question-management-apis)
19. [⭐ Choice Management APIs](#-choice-management-apis)
20. [🔧 Cache Management APIs](#-cache-management-apis)
21. [📋 Test Template Configuration APIs](#-test-template-configuration-apis)
22. [📝 Test Template Management APIs](#-test-template-management-apis)
23. [🏷️ Test Template Type APIs](#️-test-template-type-apis)
24. [📊 Test Attempt APIs](#-test-attempt-apis)
25. [✅ Attempt Answer APIs](#-attempt-answer-apis)
26. [🔍 Test Question Management APIs](#-test-question-management-apis)
27. [💳 Payment Management APIs](#-payment-management-apis)

---

## 🔐 Authentication APIs

**Base Route:** `/api/auth`

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `POST` | `/register` | Registers a new user | `RegisterModel` (multipart/form-data) |
| `POST` | `/login` | Authenticates user via email/password | `LoginModel` |
| `POST` | `/firebase-login` | Authenticates user via Firebase token | `FirebaseLoginModel` |
| `POST` | `/refresh` | Issues new token pair using refresh token | `RefreshTokenModel` |
| `POST` | `/logout` | Revokes user tokens | `LogoutModel` |
| `POST` | `/validate-access-token` | Validates access token | `ValidateAccessTokenModel` |
| `POST` | `/validate-refresh-token` | Validates refresh token | `ValidateRefreshTokenModel` |
| `POST` | `/forgot-password` | Initiates password reset process | `ForgotPasswordRequest` |
| `POST` | `/reset-password` | Resets password using reset token | `ResetPasswordRequest` |
| `GET` | `/validate-reset-token/{token}` | Validates password reset token | Path: `token` (string) |

---

## 👤 User Management APIs

**Base Route:** `/api/users`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all users with pagination and filtering | Query: `UserQueryParameters` |
| `GET` | `/{userId:guid}` | Gets specific user by ID | Path: `userId` (Guid) |
| `PUT` | `/{userId:guid}` | Updates user profile information | Path: `userId`, Body: `UpdateUserDto` (multipart/form-data) |
| `DELETE` | `/{userId:guid}` | Deactivates user account | Path: `userId` (Guid) |
| `PUT` | `/{userId:guid}/avatar` | Updates user avatar | Path: `userId`, Body: `IFormFile` (multipart/form-data) |
| `HEAD` | `/{userId:guid}` | Checks if user exists | Path: `userId` (Guid) |

---

## 📚 Course Management APIs

**Base Route:** `/api/course`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets courses with filtering and pagination | Query: `CourseQueryParameters` |
| `GET` | `/{id}` | Gets course by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates a new course | Body: `CreateCourseDto` (multipart/form-data) |
| `PUT` | `/{id}` | Updates an existing course | Path: `id`, Body: `UpdateCourseDto` (multipart/form-data) |
| `DELETE` | `/{id}` | Deletes a course | Path: `id` (Guid) |
| `PATCH` | `/{id}/status` | Updates course status | Path: `id`, Body: `CourseStatus` |
| `POST` | `/{courseId}/instructors/{instructorId}` | Assigns instructor to course | Path: `courseId`, `instructorId` |
| `DELETE` | `/{courseId}/instructors/{instructorId}` | Removes instructor from course | Path: `courseId`, `instructorId` |
| `GET` | `/{courseId}/instructors` | Gets all instructors for a course | Path: `courseId` |
| `GET` | `/{courseId}/instructors/history` | Gets instructor assignment history | Path: `courseId` |
| `GET` | `/instructor/{instructorId}` | Gets courses taught by instructor | Path: `instructorId` |
| `GET` | `/student/{studentId}` | Gets courses enrolled by student | Path: `studentId` |

---

## 📖 Lesson Management APIs

**Base Route:** `/api/lessons`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-course/{courseId}` | Gets lessons by course with pagination | Path: `courseId`, Query: `searchTerm`, `pageIndex`, `pageSize` |
| `PUT` | `/{lessonId}` | Updates a lesson | Path: `lessonId`, Body: `UpdateLessonDto` |
| `POST` | `/{courseId}` | Creates a new lesson | Path: `courseId`, Body: `CreateLessonDto` |
| `DELETE` | `/by-course/{courseId}` | Deletes all lessons in a course | Path: `courseId` |
| `DELETE` | `/{lessonId}` | Deletes a specific lesson | Path: `lessonId` |

---

## 📊 Progress Tracking APIs

**Base Route:** `/api/lesson-progress`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-user-course` | Gets lesson progress for user in course | Query: `userId`, `courseId` |
| `GET` | `/by-user-lesson` | Gets lesson progress by user and lesson | Query: `userId`, `lessonId` |
| `POST` | `/` | Creates or updates lesson progress | Body: `CreateLessonProgressDto` |
| `PUT` | `/{progressId:guid}` | Updates lesson progress | Path: `progressId`, Body: `UpdateLessonProgressDto` |
| `DELETE` | `/{progressId:guid}` | Deletes lesson progress record | Path: `progressId` |
| `GET` | `/completion-rate` | Gets user's course completion rate | Query: `userId`, `courseId` |

---

## 📋 Study Plan Management APIs

**Base Route:** `/api/study-plan`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates a new study plan | Body: `CreateStudyPlanDto` |
| `GET` | `/{planId}` | Gets study plan by ID | Path: `planId` (Guid) |
| `GET` | `/get-all` | Gets all study plans | - |
| `GET` | `/get-by-studentid/{studentId}` | Gets study plans by student ID | Path: `studentId` (Guid) |
| `PUT` | `/update/{planId}` | Updates study plan | Path: `planId`, Body: `UpdateStudyPlanDto` |

---

## 🎯 Study Plan Item Management APIs

**Base Route:** `/api/study-plan-item`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates study plan item | Body: `CreateStudyPlanItemDto` |
| `GET` | `/get-by-id/{itemId}` | Gets study plan item by ID | Path: `itemId` (Guid) |
| `GET` | `/get-by-plan/{planId}` | Gets items by study plan ID | Path: `planId` (Guid) |
| `PUT` | `/update/{itemId}` | Updates study plan item | Path: `itemId`, Body: `UpdateStudyPlanItemDto` |
| `DELETE` | `/delete/{itemId}` | Deletes study plan item | Path: `itemId` (Guid) |

---

## 📄 Sub Content Management APIs

**Base Route:** `/api/subcontents`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets sub-contents with search and filtering | Query: `search`, `level`, `contentName`, `subContentName`, `pageIndex`, `pageSize` |
| `POST` | `/` | Creates new sub-content | Body: `CreateSubContentDto` |
| `PUT` | `/{subContentId}` | Updates sub-content | Path: `subContentId`, Body: `UpdateSubContentDto` |
| `DELETE` | `/{subContentId}` | Deletes sub-content | Path: `subContentId` (Guid) |
| `GET` | `/enum-values/subcontent-name` | Gets SubContentName enum values | - |
| `GET` | `/enum-values/level` | Gets CourseLevel enum values | - |
| `GET` | `/enum-values/content-name` | Gets ContentName enum values | - |

---

## 📁 Document Management APIs

**Base Route:** `/api/documents`

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `POST` | `/upload/image` | Uploads an image document | `CreateDocumentDto` (multipart/form-data) |
| `POST` | `/upload/video` | Uploads a video document | `CreateDocumentDto` (multipart/form-data) |
| `POST` | `/upload/document` | Uploads a raw document | `CreateDocumentDto` (multipart/form-data) |
| `GET` | `/{id}` | Gets document by ID | Path: `id` (Guid) |
| `GET` | `/lesson/{lessonId}` | Gets documents by lesson ID | Path: `lessonId` (Guid) |
| `DELETE` | `/{id}` | Deletes a document | Path: `id` (Guid) |

---

## 📂 File Management APIs

**Base Route:** `/api/files`

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `POST` | `/upload/image` | Uploads an image file | `IFormFile` (multipart/form-data - JPEG, PNG, GIF, BMP, WebP, SVG) |
| `POST` | `/upload/video` | Uploads a video file using chunked upload | `IFormFile` (multipart/form-data - MP4, AVI, MOV, WMV, FLV, WebM, MKV, 3GP) |
| `POST` | `/upload/file` | Uploads a raw file (documents, archives, etc.) | `IFormFile` (multipart/form-data) |
| `DELETE` | `/delete/image` | Deletes an image by public ID | `DeleteResourceDto` |
| `DELETE` | `/delete/video` | Deletes a video by public ID | `DeleteResourceDto` |
| `DELETE` | `/delete/file` | Deletes a raw file by public ID | `DeleteResourceDto` |
| `GET` | `/resources` | Gets paginated list of resources | Query: `maxResults`, `nextCursor`, `resourceType` |
| `GET` | `/health` | Health check endpoint | - |

---

## 🎓 Enrollment Management APIs

**Base Route:** `/api/enrollments`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/enroll` | Enrolls user in a course | Body: `EnrollmentRequestDto` |
| `POST` | `/enroll-self` | Self-enrollment in course (authenticated user) | Body: `SelfEnrollmentRequestDto` |
| `GET` | `/check/{courseId}` | Checks enrollment status for current user | Path: `courseId` |
| `GET` | `/my-enrollments` | Gets all enrollments for current user | - |
| `DELETE` | `/unenroll/{courseId}` | Unenrolls from course | Path: `courseId` |

---

## 👨‍🏫 Instructor Profile APIs

**Base Route:** `/api/instructor-profile`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates instructor profile | Query: `userId`, `qualifications`, `experience` |
| `GET` | `/{userId}` | Gets instructor's profile | Path: `userId` |
| `PUT` | `/update/{userId}` | Updates instructor's profile | Path: `userId`, Query: `qualifications`, `experience` |
| `DELETE` | `/delete/{userId}` | Deletes instructor's profile | Path: `userId` |

---

## 👨‍🎓 Student Profile APIs

**Base Route:** `/api/student-profile`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates a new student profile | Query: `userId`, `currentLevel`, `learningGoals` |
| `GET` | `/{userId}` | Gets student's profile | Path: `userId` |
| `PUT` | `/update/{userId}` | Updates student's profile | Path: `userId`, Query: `currentLevel`, `learningGoals` |
| `DELETE` | `/delete/{userId}` | Deletes student's profile | Path: `userId` |

---

## 💬 Conversation Management APIs

**Base Route:** `/api/conversation`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates a new conversation | Query: `studentId` |
| `POST` | `/send-messages/{conversationId}` | Sends message in conversation | Path: `conversationId`, Body: `MessageRequest` |
| `POST` | `/assign-instructor/{conversationId}` | Assigns instructor to conversation | Path: `conversationId`, Query: `instructorId` |
| `GET` | `/{id}` | Gets conversation details with messages | Path: `id` (Guid) |
| `GET` | `/my-conversations` | Gets all conversations for user | - |

---

## 📺 Livestream Management APIs

**Base Route:** `/api/livestreams`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/` | Creates a new livestream | Body: `CreateLivestreamDto` |
| `GET` | `/{id}` | Gets livestream by ID | Path: `id` (Guid) |
| `PUT` | `/{id}` | Updates a livestream | Path: `id`, Body: `UpdateLivestreamDto` |
| `DELETE` | `/{id}` | Deletes a livestream | Path: `id` (Guid) |
| `GET` | `/` | Gets livestreams with comprehensive filtering | Query: `courseId`, `userId`, `startDate`, `endDate`, `timetableFormat`, `pageIndex`, `pageSize` |
| `GET` | `/{id}/join-token` | Generates join token for livestream | Path: `id`, Query: `userId` |
| `GET` | `/{id}/can-join` | Checks if user can join livestream | Path: `id`, Query: `userId` |

**Note:** The GET `/` endpoint supports multiple modes:
- If `userId` and `timetableFormat=true`: Returns user's timetable format
- If `userId` only: Returns user's livestreams  
- If `courseId` only: Returns course livestreams
- Default: Paginated list with date filtering support

---

## 🎥 LiveKit Integration APIs

**Base Route:** `/api/livekit`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/token` | Generates access token for room | Query: `roomName`, `participantIdentity`, `participantName`, `role` |
| `GET` | `/admin-token` | Generates admin token | Query: `roomName`, `participantIdentity`, `participantName` |
| `POST` | `/rooms` | Creates a new room | Body: `CreateRoomRequest` |
| `GET` | `/rooms` | Gets all active rooms | - |
| `GET` | `/rooms/{roomName}` | Gets room information | Path: `roomName` |
| `DELETE` | `/rooms/{roomName}` | Deletes a room | Path: `roomName` |
| `GET` | `/rooms/{roomName}/participants` | Gets room participants | Path: `roomName` |
| `DELETE` | `/rooms/{roomName}/participants/{identity}` | Removes participant from room | Path: `roomName`, `identity` |
| `POST` | `/rooms/{roomName}/participants/{identity}/promote` | Promotes participant to instructor | Path: `roomName`, `identity` |
| `POST` | `/rooms/{roomName}/participants/{identity}/demote` | Demotes participant to student | Path: `roomName`, `identity` |
| `POST` | `/rooms/{roomName}/participants/{identity}/mute` | Mutes participant's audio | Path: `roomName`, `identity` |
| `POST` | `/rooms/{roomName}/broadcast` | Sends message to all participants | Path: `roomName`, Body: `BroadcastMessageRequest` |
| `GET` | `/rooms/{roomName}/statistics` | Gets room statistics | Path: `roomName` |
| `POST` | `/webhook` | Processes LiveKit webhooks | Body: webhook payload |

---

## 📋 Test Management APIs

**Base Route:** `/api/tests`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-user/{userId}` | Gets all tests for a user with pagination | Path: `userId`, Query: `searchTerm`, `pageIndex`, `pageSize` |
| `GET` | `/by-lesson/{lessonId}` | Gets test by lesson ID | Path: `lessonId` |
| `POST` | `/by-lesson/{lessonId}` | Creates test for a lesson | Path: `lessonId`, Body: `CreateTestDto` |
| `PUT` | `/{testId}` | Updates a test | Path: `testId`, Body: `UpdateTestDto` |
| `DELETE` | `/{testId}` | Deletes a test | Path: `testId` (Guid) |
| `PATCH` | `/{testId}/status` | Updates test status | Path: `testId`, Body: `TestStatus` |
| `GET` | `/{testId}` | Gets test by ID | Path: `testId` (Guid) |

---

## ❓ Question Management APIs

**Base Route:** `/api/questions`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/{id:guid}` | Gets question by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates a new question | Body: `CreateQuestionDto` |
| `PUT` | `/{id:guid}` | Updates a question | Path: `id`, Body: `UpdateQuestionDto` |
| `DELETE` | `/{id:guid}` | Deletes a question | Path: `id` (Guid) |
| `GET` | `/paging-details` | Gets paginated questions with details | Query: `pageIndex`, `pageSize`, `search`, `contentName`, `level`, `subContentName` |
| `GET` | `/test/{id:guid}` | Gets question by ID for test | Path: `id` (Guid) |
| `GET` | `/paging-details/active` | Gets paginated active questions with details | Query: `pageIndex`, `pageSize`, `search`, `contentName`, `level`, `subContentName`, `difficulty` |

---

## ⭐ Choice Management APIs

**Base Route:** `/api/choices`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/question/{questionId}` | Gets all choices for a question | Path: `questionId` |
| `POST` | `/question/{questionId}` | Creates a new choice for a question | Path: `questionId`, Body: `ChoiceCreateDto` |
| `PUT` | `/choice/{choiceId}` | Updates a specific choice | Path: `choiceId`, Body: `ChoiceUpdateDto` |
| `DELETE` | `/choice/{choiceId}` | Deletes a specific choice | Path: `choiceId` |

---

## 🔧 Cache Management APIs

**Base Route:** `/api/cache`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/{id}` | Retrieves cached data by ID | Path: `id` (string - cache key) |

---

## 📋 Test Template Configuration APIs

**Base Route:** `/api/test-template-configs`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-template/{templateId:guid}` | Gets all configs for a template | Path: `templateId` |
| `GET` | `/{configId:guid}` | Gets a specific config by ID | Path: `configId` |
| `POST` | `/{templateId:guid}` | Creates a new config for template | Path: `templateId`, Body: `CreateTestTemplateConfigDto` |
| `PUT` | `/{configId:guid}` | Updates a specific config | Path: `configId`, Body: `UpdateTestTemplateConfigDto` |
| `DELETE` | `/{configId:guid}` | Deletes a specific config | Path: `configId` |

---

## 📝 Test Template Management APIs

**Base Route:** `/api/TestTemplate`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-type/{testTemplateTypeId:guid}` | Gets all templates by type | Path: `testTemplateTypeId` |
| `POST` | `/` | Creates a new test template | Body: `CreateTestTemplateDto` |
| `PUT` | `/{templateId:guid}` | Updates a test template | Path: `templateId`, Body: `UpdateTestTemplateDto` |
| `DELETE` | `/{templateId:guid}` | Deletes a test template | Path: `templateId` |

---

## 🏷️ Test Template Type APIs

**Base Route:** `/api/TestTemplateType`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all template types with filtering | Query: `search`, `level`, `type`, `isActive`, `pageIndex`, `pageSize` |
| `POST` | `/` | Creates a new template type | Body: `CreateTestTemplateTypeDto` |
| `PUT` | `/{testTemplateTypeId:guid}` | Updates a template type | Path: `testTemplateTypeId`, Body: `UpdateTestTemplateTypeDto` |
| `DELETE` | `/{testTemplateTypeId:guid}` | Deletes a template type | Path: `testTemplateTypeId` |
| `PATCH` | `/{testTemplateTypeId:guid}/is-active` | Updates template type active status | Path: `testTemplateTypeId`, Query: `isActive` |

---

## 📊 Test Attempt APIs

**Base Route:** `/api/test-attempts`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/start` | Starts a new test attempt | Body: `StartTestAttemptDto` |
| `POST` | `/submit` | Submits a test attempt | Body: `SubmitTestAttemptDto` |
| `GET` | `/by-user/{userId}` | Gets all attempts for a user | Path: `userId` |
| `PUT` | `/update-status/{attemptId}` | Updates attempt status | Path: `attemptId`, Body: `TestAttemptStatus` |
| `GET` | `/{attemptId}/with-score-summary` | Gets attempt with score summary | Path: `attemptId` |

---

## ✅ Attempt Answer APIs

**Base Route:** `/api/attempt-answers`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-attempt/{attemptId}` | Gets all answers for an attempt | Path: `attemptId` |
| `POST` | `/add-or-update` | Adds or updates multiple answers | Body: `List<CreateAttemptAnswerDto>` |

---

## 🔍 Test Question Management APIs

**Base Route:** `/api/test-questions`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/custom-manual/add` | Adds a question to a test manually | Body: `CreateTestQuestionDto` |
| `GET` | `/{testId}/questions` | Gets paginated questions from test | Path: `testId`, Query: `pageIndex`, `pageSize` |
| `DELETE` | `/{testQuestionId}` | Removes a test question | Path: `testQuestionId` (Guid) |
| `POST` | `/{testId}/calculate-max-score` | Calculates maximum score for test | Path: `testId` (Guid) |

---

## 💳 Payment Management APIs

**Base Route:** `/api/payment`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/history/{userId:guid}` | Gets payment history for a user | Path: `userId` (Guid) |
| `GET` | `/credit-history/{userId:guid}` | Gets credit transaction history for a user | Path: `userId` (Guid) |
| `GET` | `/check-credit/{userId:guid}/{amount:decimal}` | Checks if user has sufficient credit | Path: `userId`, `amount` |
| `POST` | `/create-credit-purchase` | Creates PayOS payment link for credit purchase | Body: `CreateCreditPurchaseRequestDto` |
| `POST` | `/payos-webhook` | Webhook endpoint for PayOS payment results | Body: `WebhookTypeDto` |
| `POST` | `/confirm-webhook` | Registers webhook URL with PayOS | Body: `ConfirmWebhookRequestDto` |
| `GET` | `/return` | Handles successful payment return from PayOS | Query: `PaymentCallbackRequestDto` |
| `GET` | `/cancel` | Handles payment cancellation from PayOS | Query: `PaymentCallbackRequestDto` |

---

## 📋 Response Formats

### Success Response
```json
{
  "data": {},
  "message": "Success message",
  "success": true
}
```

### Error Response
```json
{
  "errorCode": "ERROR_CODE",
  "message": "Human-readable error message",
  "details": "Additional error details",
  "statusCode": 400,
  "timestamp": "2024-01-15T08:30:00Z",
  "path": "/api/endpoint"
}
```

### Pagination Response
```json
{
  "items": [],
  "totalItemsCount": 100,
  "pageIndex": 1,
  "pageSize": 10,
  "totalPagesCount": 10
}
```

---

## 🔒 Authentication

Most endpoints require authentication via JWT token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

Public endpoints (no authentication required):
- Authentication endpoints (`/api/auth/*`)
- Some GET endpoints for public content
- Payment return/cancel endpoints

---

## 📝 Notes

- All `Guid` parameters should be valid UUID format
- Pagination typically uses `pageIndex` (1-based) and `pageSize` parameters
- File uploads use `multipart/form-data` content type
- Date/time values are in UTC format
- API responses are in JSON format unless specified otherwise
- **Payment Flow**: New PayOS integration for credit purchases with 1:1 VND to credit ratio
- **File Management**: Supports multiple file types with optimized upload for videos using chunked upload
- **Test System**: Complete test creation, attempt, and automatic grading system with score summaries

---

**Last Updated:** August 2, 2025  
**API Version:** 1.0  
**Base URL:** `https://your-api-domain.com`

---

## 📊 API Summary

This documentation covers **27 controllers** with a total of **123+ API endpoints** for the JCertPre Japanese Certification Learning Platform:

### Controllers Covered:
1. **AuthController** - Authentication & authorization (10 endpoints)
2. **UsersController** - User management (6 endpoints)
3. **CoursesController** - Course management (12 endpoints)
4. **LessonsController** - Lesson management (5 endpoints)
5. **LessonProgressController** - Progress tracking (6 endpoints)
6. **StudyPlansController** - Study plan management (5 endpoints)
7. **StudyPlanItemsController** - Study plan items (5 endpoints)
8. **SubContentsController** - Sub content management (7 endpoints)
9. **DocumentsController** - Document management (6 endpoints)
10. **FileController** - File upload services (8 endpoints)
11. **EnrollmentController** - Course enrollment (5 endpoints)
12. **InstructorProfileController** - Instructor profiles (4 endpoints)
13. **StudentProfileController** - Student profiles (4 endpoints)
14. **ConversationController** - Chat/messaging (5 endpoints)
15. **LivestreamController** - Live streaming (7 endpoints)
16. **LiveKitController** - Video conferencing (14 endpoints)
17. **TestsController** - Test management (7 endpoints)
18. **QuestionController** - Question management (7 endpoints)
19. **ChoiceController** - Choice management (4 endpoints)
20. **CacheController** - Cache management (1 endpoint)
21. **TestTemplateConfigController** - Test template configuration (5 endpoints)
22. **TestTemplateController** - Test template management (4 endpoints)
23. **TestTemplateTypeController** - Test template types (5 endpoints)
24. **TestAttemptController** - Test attempts (5 endpoints)
25. **AttemptAnswerController** - Test answers (2 endpoints)
26. **TestQuestionController** - Test question relationships (4 endpoints)
27. **PaymentController** - Payment & credit management (8 endpoints)

### Key Features:
- **RESTful Design**: All APIs follow REST conventions
- **Consistent Structure**: Uniform response formats and error handling
- **Comprehensive Coverage**: Complete CRUD operations for all entities
- **Authentication Ready**: JWT-based authentication system
- **Pagination Support**: Efficient handling of large data sets
- **File Management**: Comprehensive file upload/management with Cloudinary integration
- **Real-time Features**: LiveKit integration for live streaming and video conferencing
- **Payment Integration**: PayOS payment gateway for credit purchases
- **Testing Platform**: Complete test creation, attempt, grading, and analytics system
- **Progress Tracking**: Detailed lesson and course progress monitoring
- **Content Management**: Hierarchical content structure with sub-contents

### New Features in This Version:
- **Password Reset System**: Complete forgot/reset password flow with email notifications and secure token validation
- **Payment System**: Full PayOS integration with webhook handling
- **Enhanced File Management**: Separate file controller with health checks
- **Improved Test System**: Score summaries and enhanced attempt tracking
- **Credit System**: User credit management with transaction history

---

*Documentation generated for JCertPre Backend API - All 27 controllers accurately documented*
