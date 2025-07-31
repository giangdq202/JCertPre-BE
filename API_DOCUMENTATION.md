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
10. [☁️ File Upload APIs](#️-file-upload-apis)
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

---

## 👤 User Management APIs

**Base Route:** `/api/users`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all users with pagination and filtering | Query: `UserQueryParameters` |
| `GET` | `/{userId}` | Gets specific user by ID | Path: `userId` (Guid) |
| `PUT` | `/{userId}` | Updates user profile information | Path: `userId`, Body: `UpdateUserDto` (multipart/form-data) |
| `DELETE` | `/{userId}` | Deactivates user account | Path: `userId` (Guid) |
| `PUT` | `/{userId}/avatar` | Updates user avatar | Path: `userId`, Body: `IFormFile` (multipart/form-data) |
| `HEAD` | `/{userId}` | Checks if user exists | Path: `userId` (Guid) |

---

## 📚 Course Management APIs

**Base Route:** `/api/course`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets courses with filtering and pagination | Query: `CourseQueryParameters` |
| `GET` | `/{id}` | Gets course by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates a new course | Body: `CreateCourseDto` |
| `PUT` | `/{id}` | Updates an existing course | Path: `id`, Body: `UpdateCourseDto` |
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
| `DELETE` | `/{progressId}` | Deletes lesson progress record | Path: `progressId` |
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
| `GET` | `/{id}` | Gets sub-content by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates new sub-content | Body: `CreateSubContentDto` |
| `PUT` | `/{id}` | Updates sub-content | Path: `id`, Body: `UpdateSubContentDto` |
| `DELETE` | `/{id}` | Deletes sub-content | Path: `id` (Guid) |
| `GET` | `/enum-values/level` | Gets CourseLevel enum values | - |
| `GET` | `/enum-values/content-name` | Gets ContentName enum values | - |
| `GET` | `/enum-values/subcontent-name` | Gets SubContentName enum values | - |

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

## ☁️ File Upload APIs

**Base Route:** `/api/cloudinary`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/upload/image` | Uploads image to Cloudinary | Body: `IFormFile` (multipart/form-data) |
| `POST` | `/upload/video` | Uploads video to Cloudinary | Body: `IFormFile` (multipart/form-data) |
| `POST` | `/upload/raw` | Uploads raw file to Cloudinary | Body: `IFormFile` (multipart/form-data) |
| `DELETE` | `/delete/{publicId}` | Deletes resource from Cloudinary | Path: `publicId` |
| `GET` | `/resources` | Gets Cloudinary resources | Query: `maxResults`, `nextCursor`, `resourceType` |

---

## 🎓 Enrollment Management APIs

**Base Route:** `/api/enrollment`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/enroll` | Enrolls user in a course | Query: `userId`, `courseId` |
| `POST` | `/enroll-self` | Self-enrollment in course (authenticated user) | Body: `SelfEnrollmentDto` |
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

---

## 👨‍🎓 Student Profile APIs

**Base Route:** `/api/student-profile`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates a new student profile | Query: `userId`, `currentLevel`, `learningGoals` |
| `GET` | `/{userId}` | Gets student's profile | Path: `userId` |
| `PUT` | `/update/{userId}` | Updates student's profile | Path: `userId`, Query: `currentLevel`, `learningGoals` |

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

**Base Route:** `/api/Livestream`

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

**Base Route:** `/api/LiveKit`

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

---

## ❓ Question Management APIs

**Base Route:** `/api/Question`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all questions | - |
| `GET` | `/{id}` | Gets question by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates a new question | Body: `CreateQuestionDto` |
| `PUT` | `/{id}` | Updates a question | Path: `id`, Body: `UpdateQuestionDto` |
| `DELETE` | `/{id}` | Deletes a question | Path: `id` (Guid) |
| `GET` | `/paging-details` | Gets paginated questions with details | Query: `pageIndex`, `pageSize`, filters |

---

## ⭐ Choice Management APIs

**Base Route:** `/api/choice`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/question/{questionId}` | Gets all choices for a question | Path: `questionId` |
| `POST` | `/question/{questionId}` | Creates a new choice for a question | Path: `questionId`, Body: `ChoiceCreateDto` |
| `PUT` | `/question/{questionId}` | Updates multiple choices for a question | Path: `questionId`, Body: `IEnumerable<ChoiceUpdateDto>` |
| `DELETE` | `/question/{questionId}/choice/{choiceId}` | Deletes a specific choice | Path: `questionId`, `choiceId` |

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
| `GET` | `/by-template/{templateId}` | Gets all configs for a template | Path: `templateId` |
| `GET` | `/{configId}` | Gets a specific config by ID | Path: `configId` |
| `POST` | `/{templateId}` | Creates a new config for template | Path: `templateId`, Body: `CreateTestTemplateConfigDto` |
| `PUT` | `/{configId}` | Updates a specific config | Path: `configId`, Body: `UpdateTestTemplateConfigDto` |
| `DELETE` | `/{configId}` | Deletes a specific config | Path: `configId` |

---

## 📝 Test Template Management APIs

**Base Route:** `/api/TestTemplate`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-type/{testTemplateTypeId}` | Gets all templates by type | Path: `testTemplateTypeId` |
| `POST` | `/` | Creates a new test template | Body: `CreateTestTemplateDto` |
| `PUT` | `/{templateId}` | Updates a test template | Path: `templateId`, Body: `UpdateTestTemplateDto` |
| `DELETE` | `/{templateId}` | Deletes a test template | Path: `templateId` |

---

## 🏷️ Test Template Type APIs

**Base Route:** `/api/TestTemplateType`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all template types with filtering | Query: `search`, `level`, `type`, `isActive`, `pageIndex`, `pageSize` |
| `POST` | `/` | Creates a new template type | Body: `CreateTestTemplateTypeDto` |
| `PUT` | `/{testTemplateTypeId}` | Updates a template type | Path: `testTemplateTypeId`, Body: `UpdateTestTemplateTypeDto` |
| `DELETE` | `/{testTemplateTypeId}` | Deletes a template type | Path: `testTemplateTypeId` |
| `PATCH` | `/{testTemplateTypeId}/is-active` | Updates template type active status | Path: `testTemplateTypeId`, Query: `isActive` |

---

## 📊 Test Attempt APIs

**Base Route:** `/api/TestAttempt`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/start` | Starts a new test attempt | Body: `StartTestAttemptDto` |
| `POST` | `/submit` | Submits a test attempt | Body: `SubmitTestAttemptDto` |
| `GET` | `/by-user/{userId}` | Gets all attempts for a user | Path: `userId` |
| `PUT` | `/update-status/{attemptId}` | Updates attempt status | Path: `attemptId`, Body: `TestAttemptStatus` |

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
| `POST` | `/{testId}/add/{questionId}` | Adds a question to a test | Path: `testId`, `questionId` |
| `GET` | `/{testId}/questions` | Gets paginated questions from test | Path: `testId`, Query: `pageIndex`, `pageSize` |
| `GET` | `/{testId}/question/{questionId}` | Gets specific question from test | Path: `testId`, `questionId` |
| `GET` | `/{testId}/question-ids` | Gets all question IDs from test | Path: `testId` |
| `PATCH` | `/{testId}/question/{questionId}/is-active` | Updates question active status | Path: `testId`, `questionId`, Body: `bool` |
| `DELETE` | `/{testId}/question/{questionId}` | Removes question from test | Path: `testId`, `questionId` |

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

---

## 📝 Notes

- All `Guid` parameters should be valid UUID format
- Pagination typically uses `pageIndex` (1-based) and `pageSize` parameters
- File uploads use `multipart/form-data` content type
- Date/time values are in UTC format
- API responses are in JSON format unless specified otherwise
- **Livestream Flow Update**: Livestreams now use automatic status management (SCHEDULED → LIVE → COMPLETED) via background service. Manual start/end endpoints have been removed.
- **Date Filtering**: Livestream API now supports `startDate` and `endDate` parameters for precise date range filtering instead of text search.

---

**Last Updated:** July 30, 2025  
**API Version:** 1.0  
**Base URL:** `https://your-api-domain.com`

---

## 📊 API Summary

This documentation covers **26 controllers** with a total of **95+ API endpoints** for the JCertPre Japanese Certification Learning Platform:

### Controllers Covered:
1. **AuthController** - Authentication & authorization (7 endpoints)
2. **UsersController** - User management (6 endpoints)
3. **CoursesController** - Course management (12 endpoints)
4. **LessonsController** - Lesson management (5 endpoints)
5. **LessonProgressController** - Progress tracking (5 endpoints)
6. **StudyPlansController** - Study plan management (5 endpoints)
7. **StudyPlanItemsController** - Study plan items (5 endpoints)
8. **SubContentsController** - Sub content management (8 endpoints)
9. **DocumentsController** - Document management (6 endpoints)
10. **CloudinaryController** - File upload services (5 endpoints)
11. **EnrollmentController** - Course enrollment (5 endpoints)
12. **InstructorProfileController** - Instructor profiles (3 endpoints)
13. **StudentProfileController** - Student profiles (3 endpoints)
14. **ConversationController** - Chat/messaging (5 endpoints)
15. **LivestreamController** - Live streaming (7 endpoints)
16. **LiveKitController** - Video conferencing (14 endpoints)
17. **TestsController** - Test management (5 endpoints)
18. **QuestionController** - Question management (6 endpoints)
19. **ChoiceController** - Choice management (4 endpoints)
20. **CacheController** - Cache management (1 endpoint)
21. **TestTemplateConfigController** - Test template configuration (5 endpoints)
22. **TestTemplateController** - Test template management (4 endpoints)
23. **TestTemplateTypeController** - Test template types (5 endpoints)
24. **TestAttemptController** - Test attempts (4 endpoints)
25. **AttemptAnswerController** - Test answers (2 endpoints)
26. **TestQuestionController** - Test question relationships (6 endpoints)

### Key Features:
- **RESTful Design**: All APIs follow REST conventions
- **Consistent Structure**: Uniform response formats and error handling
- **Comprehensive Coverage**: Complete CRUD operations for all entities
- **Authentication Ready**: JWT-based authentication system
- **Pagination Support**: Efficient handling of large data sets
- **File Management**: Integrated cloud storage via Cloudinary
- **Real-time Features**: LiveKit integration for live streaming and video conferencing
- **Automatic Livestream Management**: Background service handles livestream lifecycle
- **Testing Platform**: Complete test creation, attempt, and grading system
- **Progress Tracking**: Detailed lesson and course progress monitoring

### Recent Updates:
- **Livestream Refactor**: Implemented automatic status management with background service
- **Date-based Filtering**: Enhanced livestream API with date range filtering
- **API Consolidation**: Unified livestream endpoints for better user experience
- **Join Restrictions**: Enhanced security with status-based access control

---

*Documentation generated for JCertPre Backend API - All 26 controllers accurately documented*

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

---

## 👤 User Management APIs

**Base Route:** `/api/users`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all users with pagination and filtering | Query: `UserQueryParameters` |
| `GET` | `/{userId}` | Gets specific user by ID | Path: `userId` (Guid) |
| `PUT` | `/{userId}` | Updates user profile information | Path: `userId`, Body: `UpdateUserDto` (multipart/form-data) |
| `DELETE` | `/{userId}` | Deactivates user account | Path: `userId` (Guid) |
| `PUT` | `/{userId}/avatar` | Updates user avatar | Path: `userId`, Body: `IFormFile` (multipart/form-data) |
| `HEAD` | `/{userId}` | Checks if user exists | Path: `userId` (Guid) |

---

## 📚 Course Management APIs

**Base Route:** `/api/course`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets courses with filtering and pagination | Query: `CourseQueryParameters` |
| `GET` | `/{id}` | Gets course by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates a new course | Body: `CreateCourseDto` |
| `PUT` | `/{id}` | Updates an existing course | Path: `id`, Body: `UpdateCourseDto` |
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

## 📄 Document Management APIs

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

## � Test Management APIs

**Base Route:** `/api/tests`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-user/{userId}` | Gets all tests for a user with pagination | Path: `userId`, Query: `searchTerm`, `pageIndex`, `pageSize` |
| `GET` | `/by-lesson/{lessonId}` | Gets test by lesson ID | Path: `lessonId` |
| `POST` | `/by-lesson/{lessonId}` | Creates test for a lesson | Path: `lessonId`, Body: `CreateTestDto` |
| `PUT` | `/{testId}` | Updates a test | Path: `testId`, Body: `UpdateTestDto` |
| `DELETE` | `/{testId}` | Deletes a test | Path: `testId` (Guid) |

---

## ❓ Question Management APIs

**Base Route:** `/api/Question`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all questions | - |
| `GET` | `/{id}` | Gets question by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates a new question | Body: `CreateQuestionDto` |
| `PUT` | `/{id}` | Updates a question | Path: `id`, Body: `UpdateQuestionDto` |
| `DELETE` | `/{id}` | Deletes a question | Path: `id` (Guid) |
| `GET` | `/paging-details` | Gets paginated questions with details | Query: `pageIndex`, `pageSize`, filters |

---

## ⭐ Choice Management APIs

**Base Route:** `/api/choice`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/question/{questionId}` | Gets all choices for a question | Path: `questionId` |
| `POST` | `/question/{questionId}` | Creates a new choice for a question | Path: `questionId`, Body: `ChoiceCreateDto` |
| `PUT` | `/question/{questionId}` | Updates multiple choices for a question | Path: `questionId`, Body: `IEnumerable<ChoiceUpdateDto>` |
| `DELETE` | `/question/{questionId}/choice/{choiceId}` | Deletes a specific choice | Path: `questionId`, `choiceId` |

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
| `GET` | `/by-template/{templateId}` | Gets all configs for a template | Path: `templateId` |
| `GET` | `/{configId}` | Gets a specific config by ID | Path: `configId` |
| `POST` | `/{templateId}` | Creates a new config for template | Path: `templateId`, Body: `CreateTestTemplateConfigDto` |
| `PUT` | `/{configId}` | Updates a specific config | Path: `configId`, Body: `UpdateTestTemplateConfigDto` |
| `DELETE` | `/{configId}` | Deletes a specific config | Path: `configId` |

---

## 📝 Test Template Management APIs

**Base Route:** `/api/TestTemplate`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-type/{testTemplateTypeId}` | Gets all templates by type | Path: `testTemplateTypeId` |
| `POST` | `/` | Creates a new test template | Body: `CreateTestTemplateDto` |
| `PUT` | `/{templateId}` | Updates a test template | Path: `templateId`, Body: `UpdateTestTemplateDto` |
| `DELETE` | `/{templateId}` | Deletes a test template | Path: `templateId` |

---

## 🏷️ Test Template Type APIs

**Base Route:** `/api/TestTemplateType`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all template types with filtering | Query: `search`, `level`, `type`, `isActive`, `pageIndex`, `pageSize` |
| `POST` | `/` | Creates a new template type | Body: `CreateTestTemplateTypeDto` |
| `PUT` | `/{testTemplateTypeId}` | Updates a template type | Path: `testTemplateTypeId`, Body: `UpdateTestTemplateTypeDto` |
| `DELETE` | `/{testTemplateTypeId}` | Deletes a template type | Path: `testTemplateTypeId` |
| `PATCH` | `/{testTemplateTypeId}/is-active` | Updates template type active status | Path: `testTemplateTypeId`, Query: `isActive` |

---

## 📊 Test Attempt APIs

**Base Route:** `/api/TestAttempt`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/start` | Starts a new test attempt | Body: `StartTestAttemptDto` |
| `POST` | `/submit` | Submits a test attempt | Body: `SubmitTestAttemptDto` |
| `GET` | `/by-user/{userId}` | Gets all attempts for a user | Path: `userId` |
| `PUT` | `/update-status/{attemptId}` | Updates attempt status | Path: `attemptId`, Body: `TestAttemptStatus` |

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
| `POST` | `/{testId}/add/{questionId}` | Adds a question to a test | Path: `testId`, `questionId` |
| `GET` | `/{testId}/questions` | Gets paginated questions from test | Path: `testId`, Query: `pageIndex`, `pageSize` |
| `GET` | `/{testId}/question/{questionId}` | Gets specific question from test | Path: `testId`, `questionId` |
| `GET` | `/{testId}/question-ids` | Gets all question IDs from test | Path: `testId` |
| `PATCH` | `/{testId}/question/{questionId}/is-active` | Updates question active status | Path: `testId`, `questionId`, Body: `bool` |
| `DELETE` | `/{testId}/question/{questionId}` | Removes question from test | Path: `testId`, `questionId` |

---

## 📺 Livestream Management APIs

**Base Route:** `/api/Livestream`

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

**Base Route:** `/api/LiveKit`

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

## 🎓 Enrollment Management APIs

**Base Route:** `/api/enrollment`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/enroll` | Enrolls user in a course | Query: `userId`, `courseId` |
| `POST` | `/enroll-self` | Self-enrollment in course (authenticated user) | Body: `SelfEnrollmentDto` |
| `GET` | `/check/{courseId}` | Checks enrollment status for current user | Path: `courseId` |
| `GET` | `/my-enrollments` | Gets all enrollments for current user | - |
| `DELETE` | `/unenroll/{courseId}` | Unenrolls from course | Path: `courseId` |

---

## 📊 Progress Tracking APIs

**Base Route:** `/api/lesson-progress`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-user-course` | Gets lesson progress for user in course | Query: `userId`, `courseId` |
| `GET` | `/by-user-lesson` | Gets lesson progress by user and lesson | Query: `userId`, `lessonId` |
| `POST` | `/` | Creates or updates lesson progress | Body: `CreateLessonProgressDto` |
| `DELETE` | `/{progressId}` | Deletes lesson progress record | Path: `progressId` |
| `GET` | `/completion-rate` | Gets user's course completion rate | Query: `userId`, `courseId` |

---

## 👥 Profile Management APIs

### Student Profile APIs
**Base Route:** `/api/student-profile`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates a new student profile | Query: `userId`, `currentLevel`, `learningGoals` |
| `GET` | `/{userId}` | Gets student's profile | Path: `userId` |
| `PUT` | `/update/{userId}` | Updates student's profile | Path: `userId`, Query: `currentLevel`, `learningGoals` |

### Instructor Profile APIs
**Base Route:** `/api/instructor-profile`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates instructor profile | Query: `userId`, `qualifications`, `experience` |
| `GET` | `/{userId}` | Gets instructor's profile | Path: `userId` |
| `PUT` | `/update/{userId}` | Updates instructor's profile | Path: `userId`, Query: `qualifications`, `experience` |

---

## 📅 Study Plan Management APIs

**Base Route:** `/api/study-plan`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates a new study plan | Body: `CreateStudyPlanDto` |
| `GET` | `/{planId}` | Gets study plan by ID | Path: `planId` (Guid) |
| `GET` | `/get-all` | Gets all study plans | - |
| `GET` | `/get-by-studentid/{studentId}` | Gets study plans by student ID | Path: `studentId` (Guid) |
| `PUT` | `/update/{planId}` | Updates study plan | Path: `planId`, Body: `UpdateStudyPlanDto` |

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

## 💬 Conversation APIs

**Base Route:** `/api/conversation`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/create` | Creates a new conversation | Query: `studentId` |
| `POST` | `/send-messages/{conversationId}` | Sends message in conversation | Path: `conversationId`, Body: `MessageRequest` |
| `POST` | `/assign-instructor/{conversationId}` | Assigns instructor to conversation | Path: `conversationId`, Query: `instructorId` |
| `GET` | `/{id}` | Gets conversation details with messages | Path: `id` (Guid) |
| `GET` | `/my-conversations` | Gets all conversations for user | - |

---

## 🗂️ Content Management APIs

### Sub-Contents APIs
**Base Route:** `/api/subcontents`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets sub-contents with search and filtering | Query: `search`, `level`, `contentName`, `subContentName`, `pageIndex`, `pageSize` |
| `GET` | `/{id}` | Gets sub-content by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates new sub-content | Body: `CreateSubContentDto` |
| `PUT` | `/{id}` | Updates sub-content | Path: `id`, Body: `UpdateSubContentDto` |
| `DELETE` | `/{id}` | Deletes sub-content | Path: `id` (Guid) |
| `GET` | `/enum-values/level` | Gets CourseLevel enum values | - |
| `GET` | `/enum-values/content-name` | Gets ContentName enum values | - |
| `GET` | `/enum-values/subcontent-name` | Gets SubContentName enum values | - |

### Choice APIs
**Base Route:** `/api/choices`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-question/{questionId}` | Gets choices by question ID | Path: `questionId` |
| `POST` | `/` | Creates new choice | Body: `CreateChoiceDto` |
| `PUT` | `/{id}` | Updates choice | Path: `id`, Body: `UpdateChoiceDto` |
| `DELETE` | `/{id}` | Deletes choice | Path: `id` (Guid) |

---

## ☁️ Cloudinary APIs

**Base Route:** `/api/cloudinary`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/upload/image` | Uploads image to Cloudinary | Body: `IFormFile` (multipart/form-data) |
| `POST` | `/upload/video` | Uploads video to Cloudinary | Body: `IFormFile` (multipart/form-data) |
| `POST` | `/upload/raw` | Uploads raw file to Cloudinary | Body: `IFormFile` (multipart/form-data) |
| `DELETE` | `/delete/{publicId}` | Deletes resource from Cloudinary | Path: `publicId` |
| `GET` | `/resources` | Gets Cloudinary resources | Query: `maxResults`, `nextCursor`, `resourceType` |

---

## 🗄️ Cache Management APIs

**Base Route:** `/api/cache`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `DELETE` | `/clear` | Clears all cache | - |
| `DELETE` | `/clear/{key}` | Clears specific cache key | Path: `key` |
| `GET` | `/keys` | Gets all cache keys | - |

---

## 📋 Test Management APIs

### Test Template APIs
**Base Route:** `/api/TestTemplate`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all test templates with filtering | Query: `search`, `level`, `type`, `isActive`, `pageIndex`, `pageSize` |
| `GET` | `/{id}` | Gets test template by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates new test template | Body: `CreateTestTemplateDto` |
| `PUT` | `/{id}` | Updates test template | Path: `id`, Body: `UpdateTestTemplateDto` |
| `DELETE` | `/{id}` | Deletes test template | Path: `id` (Guid) |

### Test Template Type APIs
**Base Route:** `/api/TestTemplateType`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets test template types with filtering | Query: `search`, `level`, `type`, `isActive`, `pageIndex`, `pageSize` |
| `GET` | `/{id}` | Gets test template type by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates new test template type | Body: `CreateTestTemplateTypeDto` |
| `PUT` | `/{id}` | Updates test template type | Path: `id`, Body: `UpdateTestTemplateTypeDto` |
| `DELETE` | `/{id}` | Deletes test template type | Path: `id` (Guid) |

### Test Template Config APIs
**Base Route:** `/api/test-template-config`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/` | Gets all test template configs | - |
| `GET` | `/{id}` | Gets test template config by ID | Path: `id` (Guid) |
| `POST` | `/` | Creates test template config | Body: `CreateTestTemplateConfigDto` |
| `PUT` | `/{id}` | Updates test template config | Path: `id`, Body: `UpdateTestTemplateConfigDto` |
| `DELETE` | `/{id}` | Deletes test template config | Path: `id` (Guid) |

### Test Question APIs
**Base Route:** `/api/test-questions`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-test/{testId}` | Gets questions by test ID | Path: `testId` |
| `POST` | `/` | Creates test question | Body: `CreateTestQuestionDto` |
| `PUT` | `/{id}` | Updates test question | Path: `id`, Body: `UpdateTestQuestionDto` |
| `DELETE` | `/{id}` | Deletes test question | Path: `id` (Guid) |

### Test Attempt APIs
**Base Route:** `/api/TestAttempt`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/by-user/{userId}` | Gets test attempts by user | Path: `userId` |
| `GET` | `/by-test/{testId}` | Gets test attempts by test | Path: `testId` |
| `POST` | `/` | Creates test attempt | Body: `CreateTestAttemptDto` |
| `PUT` | `/{id}/submit` | Submits test attempt | Path: `id` (Guid) |

### Attempt Answer APIs
**Base Route:** `/api/attempt-answers`

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `POST` | `/add-or-update` | Adds or updates attempt answers | Body: `List<CreateAttemptAnswerDto>` |

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

---

## 📝 Notes

- All `Guid` parameters should be valid UUID format
- Pagination typically uses `pageIndex` (1-based) and `pageSize` parameters
- File uploads use `multipart/form-data` content type
- Date/time values are in UTC format
- API responses are in JSON format unless specified otherwise

---

**Last Updated:** July 30, 2025  
**API Version:** 1.0  
**Base URL:** `https://your-api-domain.com`

---

## 📊 API Summary

This documentation covers **26 controllers** with a total of **110+ API endpoints** for the JCertPre Japanese Certification Learning Platform:

### Controllers Covered:
1. **AuthController** - Authentication & authorization
2. **UsersController** - User management
3. **CoursesController** - Course management  
4. **LessonsController** - Lesson management
5. **LessonProgressController** - Progress tracking
6. **StudyPlansController** - Study plan management
7. **StudyPlanItemsController** - Study plan items
8. **SubContentsController** - Sub content management
9. **DocumentsController** - Document management
10. **CloudinaryController** - File upload services
11. **EnrollmentController** - Course enrollment
12. **InstructorProfileController** - Instructor profiles
13. **StudentProfileController** - Student profiles
14. **ConversationController** - Chat/messaging
15. **LivestreamController** - Live streaming
16. **LiveKitController** - Video conferencing
17. **TestsController** - Test management
18. **QuestionController** - Question management
19. **ChoiceController** - Choice management
20. **CacheController** - Cache management
21. **TestTemplateConfigController** - Test template configuration
22. **TestTemplateController** - Test template management
23. **TestTemplateTypeController** - Test template types
24. **TestAttemptController** - Test attempts
25. **AttemptAnswerController** - Test answers
26. **TestQuestionController** - Test question relationships

### Key Features:
- **RESTful Design**: All APIs follow REST conventions
- **Consistent Structure**: Uniform response formats and error handling
- **Comprehensive Coverage**: Complete CRUD operations for all entities
- **Authentication Ready**: All endpoints prepared for authentication integration
- **Pagination Support**: Large data sets handled with pagination
- **File Management**: Integrated cloud storage for documents and media
- **Real-time Features**: Live streaming and video conferencing capabilities
- **Testing Platform**: Complete test creation, attempt, and grading system

---

*Documentation generated for JCertPre Backend API - All 26 controllers documented*
