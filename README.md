# 🏗️ JCertPre Backend - Clean Architecture Solution

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-13+-blue.svg)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-6+-red.svg)](https://redis.io/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## 📋 Mục lục

- [Tổng quan](#-tổng-quan)
- [Kiến trúc Clean Architecture](#-kiến-trúc-clean-architecture)
- [Cấu trúc Project](#-cấu-trúc-project)
- [Nguyên tắc phát triển](#-nguyên-tắc-phát-triển)
- [Cài đặt và chạy](#-cài-đặt-và-chạy)
- [Hướng dẫn phát triển](#-hướng-dẫn-phát-triển)
- [Best Practices](#-best-practices)
- [Troubleshooting](#-troubleshooting)

---

## 🎯 Tổng quan

JCertPre là một nền tảng học tập và chứng chỉ được xây dựng theo **Clean Architecture** với .NET 8. Project được thiết kế để:

- 📚 **Dễ hiểu**: Cấu trúc rõ ràng, phân tách trách nhiệm
- 🔧 **Dễ maintain**: Loose coupling, high cohesion
- 🚀 **Dễ mở rộng**: Thêm features mà không ảnh hưởng code cũ
- ✅ **Dễ test**: Dependency injection và mocking
- 🔄 **Linh hoạt**: Có thể thay đổi database, framework

---

## 🏛️ Kiến trúc Clean Architecture

### Dependency Flow (Quy tắc phụ thuộc)

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                            │
│                    (Presentation)                           │
│  ┌─────────────────────────────────────────────────────┐   │
│  │                Application Layer                    │   │
│  │                 (Use Cases)                         │   │
│  │  ┌─────────────────────────────────────────────┐   │   │
│  │  │              Domain Layer                   │   │   │
│  │  │               (Entities)                    │   │   │
│  │  └─────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                      │
│                   (Persistence)                             │
└─────────────────────────────────────────────────────────────┘
```

**⚠️ Nguyên tắc vàng:** Dependencies chỉ được hướng vào trong (inward), không bao giờ hướng ra ngoài!

---

## 📁 Cấu trúc Project

```
JCertPre-BE/
├── 🎯 JCertPreApplication.Domain/           # Lớp 1 - Core Business
│   ├── Entities/                           # Business entities
│   ├── Enums/                             # Business enums  
│   └── Configuration/                      # Business configurations
│
├── 🧠 JCertPreApplication.Application/      # Lớp 2 - Use Cases
│   ├── Contracts/                         # Interfaces (abstractions)
│   ├── Features/                          # Business logic services
│   ├── Dtos/                             # Data transfer objects
│   └── DependencyInjection.cs            # Service registration
│
├── 💾 JCertPreApplication.Persistence/      # Lớp 3 - Infrastructure  
│   ├── Repositories/                      # Data access implementations
│   ├── DatabaseContext/                   # EF Core context
│   ├── Configurations/                    # EF configurations
│   ├── Services/                          # External service implementations
│   └── DependencyInjection.cs            # Infrastructure registration
│
└── 🌐 JCertPreApplication.API/              # Lớp 4 - Presentation
    ├── Controllers/                       # API endpoints
    ├── DependencyInjection.cs            # API service registration
    └── Program.cs                        # Application entry point
```

### 🔍 Chi tiết từng layer:

#### 1️⃣ **Domain Layer** - Trái tim ứng dụng
```csharp
// ✅ Chứa gì:
- Business entities (User, Course, Test...)
- Business enums (UserStatus, CourseLevel...)  
- Business rules và validations
- Configuration models

// ❌ KHÔNG chứa:
- Database code
- HTTP requests  
- Framework dependencies
- Infrastructure concerns
```

#### 2️⃣ **Application Layer** - Bộ não điều phối
```csharp
// ✅ Chứa gì:
- Interfaces/Contracts (IUserRepository, IAuthService...)
- Business logic services (AuthService, CourseService...)
- Use cases và workflows
- DTOs cho data transfer

// ❌ KHÔNG chứa:
- Database implementations
- HTTP handling
- External service implementations
```

#### 3️⃣ **Persistence Layer** - Kho dữ liệu
```csharp
// ✅ Chứa gì:
- Repository implementations
- Database context (EF Core)
- External service integrations (Firebase, Redis...)
- Data access logic

// ❌ KHÔNG chứa:
- Business logic
- HTTP handling
- Domain rules
```

#### 4️⃣ **API Layer** - Cổng giao tiếp
```csharp
// ✅ Chứa gì:
- Controllers (API endpoints)
- Request/Response handling
- Authentication middleware
- Swagger configuration

// ❌ KHÔNG chứa:
- Business logic
- Database code
- Domain rules
```

---

## ⚡ Cài đặt và chạy

### Prerequisites

- ✅ .NET 8 SDK
- ✅ PostgreSQL 13+
- ✅ Redis 6+ (optional, for caching)
- ✅ Visual Studio 2022 hoặc VS Code

### 🚀 Quick Start

1. **Clone repository:**
```bash
git clone <repository-url>
cd JCertPre-BE
```

2. **Setup environment:**
```bash
# Copy file env.example thành .env
cp env.example .env

# Chỉnh sửa file .env với thông tin của bạn
# JCERTPRE_DB_CONNECTION_STRING=Host=localhost;Port=5432;Username=...
```

3. **Restore packages:**
```bash
dotnet restore
```

4. **Run database migrations:**
```bash
dotnet ef database update --project JCertPreApplication.Persistence --startup-project JCertPreApplication.API
```

5. **Run application:**
```bash
dotnet run --project JCertPreApplication.API
```

6. **Access Swagger UI:**
```
https://localhost:5001/swagger
```

---

## 👨‍💻 Hướng dẫn phát triển

### 🆕 Thêm Feature mới

#### Bước 1: Tạo Domain Entity (nếu cần)
```csharp
// JCertPreApplication.Domain/Entities/NewEntity.cs
public class Course
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    // ... other properties
}
```

#### Bước 2: Tạo Repository Interface
```csharp
// JCertPreApplication.Application/Contracts/ICourseRepository.cs
public interface ICourseRepository : IGenericRepository<Course>
{
    Task<Course> GetByTitleAsync(string title);
    Task<List<Course>> GetCoursesByInstructorAsync(Guid instructorId);
}
```

#### Bước 3: Implement Repository
```csharp
// JCertPreApplication.Persistence/Repositories/CourseRepository.cs
public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(JCertPreDatabaseContext context) : base(context) { }

    public async Task<Course> GetByTitleAsync(string title)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Title == title);
    }
    
    // ... implement other methods
}
```

#### Bước 4: Tạo Service Interface & Implementation
```csharp
// JCertPreApplication.Application/Features/Courses/ICourseService.cs
public interface ICourseService
{
    Task<CourseDto> CreateCourseAsync(CreateCourseDto request);
    Task<CourseDto> GetCourseAsync(Guid courseId);
}

// JCertPreApplication.Application/Features/Courses/CourseService.cs
public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    
    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }
    
    // ... implement methods
}
```

#### Bước 5: Đăng ký Dependencies
```csharp
// JCertPreApplication.Application/DependencyInjection.cs
services.AddScoped<ICourseService, CourseService>();

// JCertPreApplication.Persistence/DependencyInjection.cs  
services.AddScoped<ICourseRepository, CourseRepository>();
```

#### Bước 6: Tạo Controller
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
        var course = await _courseService.GetCourseAsync(id);
        return Ok(course);
    }
}
```

### 🗃️ Thêm Database Migration

```bash
# Tạo migration mới
dotnet ef migrations add "MigrationName" \
  --project JCertPreApplication.Persistence \
  --startup-project JCertPreApplication.API \
  --output-dir Data/Migrations

# Apply migration
dotnet ef database update \
  --project JCertPreApplication.Persistence \
  --startup-project JCertPreApplication.API
```

---

## 📏 Best Practices

### ✅ DO's

1. **Dependency Direction:**
   ```csharp
   // ✅ Đúng: Application phụ thuộc vào Domain
   using JCertPreApplication.Domain.Entities;
   
   // ✅ Đúng: Infrastructure implement Application interfaces
   public class UserRepository : IUserRepository
   ```

2. **Interface Segregation:**
   ```csharp
   // ✅ Đúng: Interface nhỏ, tập trung
   public interface IUserRepository : IGenericRepository<User>
   {
       Task<User> GetByEmailAsync(string email);
   }
   ```

3. **Constructor Injection:**
   ```csharp
   // ✅ Đúng: Inject dependencies qua constructor
   public class AuthService
   {
       private readonly IUserRepository _userRepository;
       
       public AuthService(IUserRepository userRepository)
       {
           _userRepository = userRepository;
       }
   }
   ```

4. **Service Lifetimes:**
   ```csharp
   // ✅ Đúng: Chọn lifetime phù hợp
   services.AddScoped<IUserRepository, UserRepository>();    // Có state
   services.AddSingleton<IPasswordService, PasswordService>(); // Stateless
   ```

### ❌ DON'Ts

1. **Sai dependency direction:**
   ```csharp
   // ❌ Sai: Domain không được phụ thuộc vào Infrastructure
   // Trong Domain layer:
   using Microsoft.EntityFrameworkCore; // ❌ KHÔNG!
   ```

2. **Business logic trong Controller:**
   ```csharp
   // ❌ Sai: Logic nghiệp vụ trong controller
   [HttpPost]
   public async Task<IActionResult> Register(RegisterModel model)
   {
       var hashedPassword = BCrypt.HashPassword(model.Password); // ❌ KHÔNG!
       // Logic này phải ở Service layer
   }
   ```

3. **Direct database access trong Controller:**
   ```csharp
   // ❌ Sai: Truy cập database trực tiếp
   public class AuthController : ControllerBase
   {
       private readonly JCertPreDatabaseContext _context; // ❌ KHÔNG!
   }
   ```

### 🏗️ Architecture Patterns

1. **Repository Pattern:**
   ```csharp
   // Interface trong Application layer
   public interface IUserRepository : IGenericRepository<User>
   
   // Implementation trong Persistence layer  
   public class UserRepository : GenericRepository<User>, IUserRepository
   ```

2. **Service Pattern:**
   ```csharp
   // Business logic tập trung trong Services
   public class AuthService : IAuthService
   {
       // Orchestrate business operations
   }
   ```

3. **Dependency Injection:**
   ```csharp
   // Composition Root trong Program.cs
   builder.Services.AddApplication();
   builder.Services.AddInfrastructure(builder.Configuration);
   ```

---

## 🔧 Troubleshooting

### Lỗi thường gặp:

#### 1. **Circular Dependency**
```csharp
// ❌ Lỗi: A phụ thuộc B, B phụ thuộc A
// Giải pháp: Tách interface hoặc dùng events
```

#### 2. **Migration Issues**
```bash
# Lỗi: Migration không apply được
# Giải pháp: Check connection string và permissions
dotnet ef database update --verbose
```

#### 3. **DI Registration Order**
```csharp
// ⚠️ Thứ tự quan trọng trong Program.cs:
builder.Services.AddApiServices(builder.Configuration);     // 1. API
builder.Services.AddApplication();                          // 2. Application  
builder.Services.AddInfrastructure(builder.Configuration);  // 3. Infrastructure
```

#### 4. **Namespace Confusion**
```csharp
// ⚠️ Phân biệt rõ namespace:
JCertPreApplication.Application.Contracts.IUserRepository    // Interface
JCertPreApplication.Persistence.Repositories.UserRepository  // Implementation
```

---

## 📚 Tài liệu tham khảo

- [Clean Architecture - Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)

---

## 👥 Contributing

1. **Luôn follow Clean Architecture principles**
2. **Viết unit tests cho business logic**
3. **Update README khi thay đổi architecture**
4. **Code review trước khi merge**
5. **Maintain high code quality standards**

---

## 📄 License

This project is licensed under the MIT License.

---

**🎯 Remember: Clean Architecture is not just about folder structure, it's about dependency management and separation of concerns!** 