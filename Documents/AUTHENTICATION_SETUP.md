# Authentication Setup cho Backend

## Tổng quan

Đã thêm authentication cho tất cả các controller trong backend, trừ LiveKit và FileController theo yêu cầu.

## Controllers đã thêm [Authorize] attribute

### ✅ Đã thêm authentication:
1. **AdminDashboardController** - `[Authorize(Roles = "ADMIN")]` (đã có sẵn)
2. **CoursesController** - `[Authorize]`
3. **UsersController** - `[Authorize]`
4. **EnrollmentController** - `[Authorize]`
5. **LessonsController** - `[Authorize]`
6. **LessonProgressController** - `[Authorize]`
7. **DocumentsController** - `[Authorize]`
8. **StudentProfileController** - `[Authorize]`
9. **InstructorProfileController** - `[Authorize]`
10. **QuestionController** - `[Authorize]`
11. **ChoiceController** - `[Authorize]`
12. **TestsController** - `[Authorize]`
13. **TestQuestionController** - `[Authorize]`
14. **TestAttemptController** - `[Authorize]`
15. **AttemptAnswerController** - `[Authorize]`
16. **ConversationController** - `[Authorize]`
17. **StudyPlansController** - `[Authorize]`
18. **StudyPlanItemsController** - `[Authorize]`
19. **SubContentsController** - `[Authorize]`
20. **TestTemplateController** - `[Authorize]`
21. **TestTemplateTypeController** - `[Authorize]`
22. **TestTemplateConfigController** - `[Authorize]`
23. **LivestreamController** - `[Authorize]`
24. **FeedbackController** - `[Authorize]`
25. **CacheController** - `[Authorize]`
26. **PaymentController** - `[Authorize]`

### ❌ Không thêm authentication (theo yêu cầu):
1. **AuthController** - Không cần authentication vì đây là controller xử lý đăng nhập/đăng ký
2. **LiveKitController** - Không thêm authentication (theo yêu cầu)
3. **FileController** - Không thêm authentication (theo yêu cầu)

## Cấu hình Authentication hiện tại

### JWT Configuration
- **SecretKey**: Được cấu hình trong `appsettings.json`
- **RefreshSecretKey**: Được cấu hình trong `appsettings.json`
- **Issuer**: Được cấu hình trong `appsettings.json`
- **Audience**: Được cấu hình trong `appsettings.json`
- **ExpiryInMinutes**: 60 phút (mặc định)

### Middleware Pipeline
1. **GlobalExceptionHandlingMiddleware** - Xử lý exception toàn cục
2. **UseAuthentication()** - JWT Bearer authentication
3. **TokenRevocationMiddleware** - Kiểm tra token bị thu hồi
4. **UseAuthorization()** - Authorization middleware
5. **MapControllers()** - Map các controller

### Swagger Configuration
- Đã cấu hình JWT Bearer authentication trong Swagger UI
- Có thể test API với token thông qua Swagger

## Cách sử dụng

### 1. Đăng nhập để lấy token
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password"
}
```

### 2. Sử dụng token trong các API calls
```http
GET /api/course
Authorization: Bearer {your-jwt-token}
```

### 3. Refresh token khi cần
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "accessToken": "old-access-token",
  "refreshToken": "refresh-token"
}
```

## Lưu ý quan trọng

1. **AuthController** không cần authentication vì đây là controller xử lý đăng nhập
2. **LiveKitController** và **FileController** được loại trừ theo yêu cầu
3. Tất cả các API khác đều yêu cầu JWT token hợp lệ
4. Token có thời gian sống 60 phút, cần refresh khi hết hạn
5. Hệ thống hỗ trợ token revocation để logout an toàn

## Testing

### Với Swagger UI:
1. Đăng nhập qua `/api/auth/login`
2. Copy access token từ response
3. Click "Authorize" trong Swagger UI
4. Nhập `Bearer {token}` vào Authorization field
5. Test các API khác

### Với Postman/curl:
1. Thêm header: `Authorization: Bearer {token}`
2. Gọi các API được bảo vệ

## Security Features

- ✅ JWT token validation
- ✅ Token expiration check
- ✅ Token revocation support
- ✅ Role-based authorization (cho AdminDashboard)
- ✅ Secure token refresh mechanism
- ✅ Firebase authentication support
