# JCertPre Backend - Clean Architecture Solution

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-13+-blue.svg)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-6+-red.svg)](https://redis.io/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## QUY TẮC PHÁT TRIỂN PHẢI FOLLOW

> **Đây là quy tắc bắt buộc mọi dev phải tuân thủ khi code. Đọc kỹ và áp dụng ngay!**

### 1. Cấu trúc Clean Architecture (Linh hoạt, không cứng nhắc)

**Nguyên tắc đơn giản:** Chia code thành 4 phần rõ ràng, mỗi phần làm đúng nhiệm vụ của nó.

```
Domain/     -> Định nghĩa business (Entity, Enum, Config)
Application/ -> Xử lý logic nghiệp vụ (Service, Interface, DTO)  
Persistence/ -> Truy cập database (Repository, EF Context)
API/        -> Nhận/trả HTTP request (Controller, Middleware)
```

**Làm đúng:**
```csharp
// Controller chỉ gọi Service
public class UserController 
{
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _userService.GetUserAsync(id);  // Gọi service
        return Ok(user);
    }
}

// Service xử lý logic, gọi Repository
public class UserService 
{
    public async Task<UserDto> GetUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);  // Gọi repository
        if (user == null) throw ApiException.NotFound("User", id);
        return _mapper.Map<UserDto>(user);
    }
}
```

**Làm sai:**
```csharp
// Controller trực tiếp truy cập database
public class UserController 
{
    private readonly JCertPreDatabaseContext _context;  // KHÔNG!
    
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);  // Logic trong Controller (không nên)
        if (user == null) return NotFound();
        return Ok(user);
    }
}
```

### 2. API Exception theo chuẩn DUY NHẤT

**Nguyên tắc đơn giản:** Mọi lỗi phải dùng `ApiException`, không được trả về lỗi theo cách khác.

**Làm đúng:**
```csharp
// Trong Service layer
if (user == null)
    throw ApiException.NotFound("User", userId);  // Dùng ApiException

if (request.Email.IsEmpty())
    throw ApiException.BadRequest("INVALID_EMAIL", "Email không được để trống");

// Validation errors
var errors = new Dictionary<string, string[]>();
if (dto.Age < 18) errors.Add("age", new[] { "Phải từ 18 tuổi trở lên" });
if (errors.Any()) throw new ApiException("VALIDATION_FAILED", "Dữ liệu không hợp lệ", errors);
```

**Làm sai:**
```csharp
// Trả về object lỗi
if (user == null) return new { Error = "User not found" };  // KHÔNG!

// Dùng generic Exception  
throw new Exception("Something wrong");  // KHÔNG!

// Try-catch trong Controller
try { ... } catch { return BadRequest(); }  // KHÔNG! Middleware sẽ handle
```

**Format lỗi trả về (tự động):**
```json
{
  "status": 404,
  "errorCode": "USER_NOT_FOUND", 
  "message": "User with key '123' was not found.",
  "traceId": null,
  "errors": null
}
```

### 3. Tuân theo IoC (Inversion of Control)

**Nguyên tắc đơn giản:** Không được `new` trực tiếp, phải inject dependency qua constructor.

**Làm đúng:**
```csharp
// Service inject Repository qua constructor
public class UserService 
{
    private readonly IUserRepository _userRepository;  // Interface
    
    public UserService(IUserRepository userRepository)  // Constructor injection
    {
        _userRepository = userRepository;
    }
}

// Đăng ký trong DI Container
services.AddScoped<IUserService, UserService>();
services.AddScoped<IUserRepository, UserRepository>();
```

**Làm sai:**
```csharp
// New trực tiếp
public class UserService 
{
    public async Task GetUser()
    {
        var repository = new UserRepository();  // KHÔNG new trực tiếp!
        var context = new JCertPreDatabaseContext();  // KHÔNG!
    }
}

// Domain phụ thuộc vào Infrastructure
// Trong Domain layer:
using Microsoft.EntityFrameworkCore;  // KHÔNG import EF trong Domain!
```

---

## Mục lục

- [Cập nhật gần nhất](#cập-nhật-gần-nhất)
- [Tổng quan](#tổng-quan)
- [Cấu trúc Project](#cấu-trúc-project)
- [Cài đặt và chạy](#cài-đặt-và-chạy)
- [Tài liệu](#tài-liệu)
- [Kiểm thử](#kiểm-thử)
- [Thêm Feature mới](#thêm-feature-mới)
- [Troubleshooting](#troubleshooting)

---

## Tổng quan

JCertPre là một nền tảng học tập và chứng chỉ được xây dựng theo **Clean Architecture** với .NET 8. 

**Tại sao dùng Clean Architecture?**
- Code dễ hiểu: Ai cũng biết tìm code ở đâu
- Dễ sửa bug: Thay đổi ở một chỗ không ảnh hưởng chỗ khác
- Thêm tính năng nhanh: Cấu trúc rõ ràng, không sợ làm hỏng code cũ
- Test dễ dàng: Mock được mọi thứ
- Thay đổi linh hoạt: Đổi database, framework không ảnh hưởng business logic

## Cấu trúc Project

```
JCertPre-BE/
├── Domain/             # Business rules
│   ├── Entities/        # Các đối tượng nghiệp vụ (User, Course, Test...)
│   ├── Enums/           # Các giá trị cố định (UserStatus, CourseLevel...)
│   └── Configuration/   # Cấu hình nghiệp vụ
│
├── Application/        # Xử lý logic nghiệp vụ
│   ├── Contracts/       # Interfaces (IUserRepository, IAuthService...)
│   ├── Features/        # Services/Handlers theo use case
│   └── Dtos/            # DTOs
│
├── Persistence/        # Database & External services
│   ├── Repositories/    # Truy cập database (EF Core)
│   ├── DatabaseContext/ # DbContext
│   └── Services/        # Tích hợp ngoài (Firebase, Redis, LiveKit, PayOS...)
│
└── API/                # HTTP endpoints
    ├── Controllers/     # REST APIs
    └── Middleware/      # Xử lý request/response
```

**Luồng xử lý thông thường:**
```
Client Request -> Controller -> Service -> Repository -> Database
              (API)      (Application) (Persistence)
```

## Cài đặt và chạy

### Yêu cầu hệ thống
- .NET 8 SDK
- PostgreSQL 13+
- Redis 6+ (tùy chọn, dùng cho cache)
- Visual Studio 2022 hoặc VS Code

### Chạy nhanh trong 5 phút

1. **Clone code về:**
```bash
git clone <repository-url>
cd JCertPre-BE
```

2. **Tạo file cấu hình:**
```bash
# Copy file mẫu
cp env.example .env

# Sửa thông tin database trong file .env
# JCERTPRE_DB_CONNECTION_STRING=Host=localhost;Port=5432;Username=...
```

3. **Cài package:**
```bash
dotnet restore
```

4. **Tạo database:**
```bash
dotnet ef database update --project JCertPreApplication.Persistence --startup-project JCertPreApplication.API
```

5. **Chạy ứng dụng:**
```bash
dotnet run --project JCertPreApplication.API
```

6. **Mở trình duyệt:**
```
https://localhost:5001/swagger
```

**Xong! Giờ có thể test API trên Swagger.**

---

## Tài liệu

- API tổng hợp: `Documents/API_DOCUMENTATION.md`
- Admin Dashboard API: `Documents/ADMIN_DASHBOARD_API.md`
- Livestream + LiveKit Guide: `Documents/LIVESTREAM_LIVEKIT_GUIDE.md`
- AI Integration (Generate/Explain Question): `Documents/AI_INTEGRATION.md`
- Hướng dẫn Unit Test: `Documents/UNIT_TESTING_GUIDE.md`

---

## Kiểm thử

- Dự án kiểm thử hiện tại: `JCertPreApplication.UnitTests`
- ĐÃ LOẠI BỎ project Integration Tests khỏi solution để đơn giản hoá pipeline.
- Nguyên tắc: test unit không sửa đổi business code; điều chỉnh test cho phù hợp service.

## Thêm Feature mới

### Quy trình chuẩn (làm theo thứ tự)

**1. Tạo Entity (nếu cần table mới):**
```csharp
// JCertPreApplication.Domain/Entities/Course.cs
public class Course
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}
```

**2. Tạo Interface Repository:**
```csharp
// JCertPreApplication.Application/Contracts/ICourseRepository.cs
public interface ICourseRepository : IGenericRepository<Course>
{
    Task<Course> GetByTitleAsync(string title);
    Task<List<Course>> GetByInstructorAsync(Guid instructorId);
}
```

**3. Implement Repository:**
```csharp
// JCertPreApplication.Persistence/Repositories/CourseRepository.cs
public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(JCertPreDatabaseContext context) : base(context) { }
    
    public async Task<Course> GetByTitleAsync(string title)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Title == title);
    }
}
```

**4. Tạo Service:**
```csharp
// JCertPreApplication.Application/Features/Courses/ICourseService.cs
public interface ICourseService
{
    Task<CourseDto> CreateAsync(CreateCourseDto request);
    Task<CourseDto> GetAsync(Guid courseId);
}

// JCertPreApplication.Application/Features/Courses/CourseService.cs
public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    
    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    
    public async Task<CourseDto> GetAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null) throw ApiException.NotFound("Course", courseId);
        
        return new CourseDto { /* map properties */ };
    }
}
```

**5. Đăng ký DI:**
```csharp
// JCertPreApplication.Application/DependencyInjection.cs
services.AddScoped<ICourseService, CourseService>();

// JCertPreApplication.Persistence/DependencyInjection.cs  
services.AddScoped<ICourseRepository, CourseRepository>();
```

**6. Tạo Controller:**
```csharp
// JCertPreApplication.API/Controllers/CoursesController.cs
[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    
    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var course = await _courseService.GetAsync(id);  // Lỗi sẽ tự động được middleware handle
        return Ok(course);
    }
}
```

**7. Tạo Migration (nếu có DB changes):**
```bash
dotnet ef migrations add "AddCourseEntity" \
  --project JCertPreApplication.Persistence \
  --startup-project JCertPreApplication.API

dotnet ef database update \
  --project JCertPreApplication.Persistence \
  --startup-project JCertPreApplication.API
```

**Xong! Feature mới đã sẵn sàng.**

## Troubleshooting - Xử lý lỗi thường gặp

### Lỗi phổ biến và cách fix

#### 1. **Lỗi Circular Dependency (A phụ thuộc B, B phụ thuộc A)**
```
Error: A circular dependency was detected...
```
**Cách fix:** Tách interface hoặc dùng event để phá vòng lặp phụ thuộc.

#### 2. **Migration không chạy được**
```bash
# Lỗi: Migration fails to apply
# Fix: Kiểm tra connection string và quyền truy cập
dotnet ef database update --verbose

# Nếu vẫn lỗi, drop database và tạo lại
dotnet ef database drop
dotnet ef database update
```

#### 3. **DI Registration sai thứ tự**
```
Error: Unable to resolve service...
```
**Cách fix:** Đảm bảo thứ tự đăng ký trong `Program.cs`:
```csharp
builder.Services.AddApiServices(builder.Configuration);     // 1. API trước
builder.Services.AddApplication();                          // 2. Application
builder.Services.AddInfrastructure(builder.Configuration);  // 3. Infrastructure cuối
```

#### 4. **Import sai namespace**
```
Error: The type or namespace could not be found...
```
**Cách fix:** Kiểm tra namespace:
- Interface: `JCertPreApplication.Application.Contracts.IUserRepository`
- Implementation: `JCertPreApplication.Persistence.Repositories.UserRepository`

#### 5. **API Exception không hoạt động**
```
Error: Exception is not handled properly...
```
**Cách fix:** Đảm bảo middleware được đăng ký:
```csharp
// Trong Program.cs
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
```

---

## Hỗ trợ

- **Có thắc mắc gì?** Hỏi lead hoặc senior dev
- **Bug gì lạ?** Tạo issue trên Git với logs chi tiết
- **Cần review code?** Tạo Pull Request theo template

---

## Lưu ý quan trọng

> **Nhớ kỹ:** Clean Architecture không phải về thư mục, mà về cách quản lý dependency và phân tách trách nhiệm!

**3 điều quan trọng nhất:**
1. **Phân tách rõ ràng**: Mỗi layer làm đúng việc của nó
2. **Exception chuẩn**: Luôn dùng `ApiException`, không trả lỗi kiểu khác  
3. **IoC đúng cách**: Inject dependency, không `new` trực tiếp

**Làm đúng 3 điều này = code clean, dễ maintain, team happy!** 