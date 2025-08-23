# Hướng Dẫn Unit Testing cho JCertPre-BE

## Tổng Quan

Tài liệu này cung cấp hướng dẫn chi tiết về cách thực hiện unit testing cho dự án JCertPre-BE sử dụng Clean Architecture với .NET 8.

## Mục Lục

1. [Kiến Trúc Testing](#kiến-trúc-testing)
2. [Công Cụ và Thư Viện](#công-cụ-và-thư-viện)
3. [Cấu Trúc Dự Án Testing](#cấu-trúc-dự-án-testing)
4. [Nguyên Tắc Testing](#nguyên-tắc-testing)
5. [Patterns và Best Practices](#patterns-và-best-practices)
6. [Hướng Dẫn Thực Hiện](#hướng-dẫn-thực-hiện)
7. [Ví Dụ Cụ Thể](#ví-dụ-cụ-thể)
8. [CI/CD Integration](#cicd-integration)

## Kiến Trúc Testing

### Phân Tầng Testing theo Clean Architecture

```
Tests/
├── JCertPreApplication.UnitTests/           # Unit tests cho Application layer
│   ├── Features/                            # Tests cho business logic
│   │   ├── Auth/
│   │   ├── Course/
│   │   ├── Payment/
│   │   └── ...
│   ├── Utilities/                           # Tests cho utility classes
│   └── Common/                              # Shared testing utilities
└── JCertPreApplication.IntegrationTests/    # Integration tests
    ├── Controllers/                         # API endpoint tests
    ├── Repositories/                        # Database integration tests
    └── Infrastructure/                      # Test setup and fixtures
```

### Test Strategy

1. **Unit Tests (80%)**: Test các business logic, services, utilities
2. **Integration Tests (20%)**: Test tương tác giữa các components

## Công Cụ và Thư Viện

### Core Testing Framework
- **xUnit**: Framework chính cho .NET testing
- **Microsoft.NET.Test.Sdk**: Test SDK cho .NET

### Mocking và Assertions
- **Moq**: Mocking framework cho dependencies
- **FluentAssertions**: Assertion library với syntax dễ đọc
- **AutoFixture**: Tạo test data tự động

### Database Testing
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database cho testing
- **Testcontainers**: Container-based testing cho PostgreSQL



### Test Coverage
- **coverlet.collector**: Code coverage collection
- **ReportGenerator**: Tạo coverage reports

## Cấu Trúc Dự Án Testing

### 1. Unit Tests Project Structure

```
JCertPreApplication.UnitTests/
├── Features/
│   ├── Auth/
│   │   ├── AuthServiceTests.cs
│   │   └── Fixtures/
│   │       └── AuthServiceFixture.cs
│   ├── Course/
│   │   ├── CourseServiceTests.cs
│   │   └── Fixtures/
│   │       └── CourseServiceFixture.cs
│   └── Payment/
│       ├── PaymentServiceTests.cs
│       └── Fixtures/
│           └── PaymentServiceFixture.cs
├── Utilities/
│   ├── EnumHelperTests.cs
│   └── FileUrlParserTests.cs
├── Common/
│   ├── TestFixtures/
│   │   ├── DatabaseFixture.cs
│   │   └── AutoMapperFixture.cs
│   ├── Builders/
│   │   ├── UserBuilder.cs
│   │   ├── CourseBuilder.cs
│   │   └── PaymentBuilder.cs
│   └── Extensions/
│       └── MockExtensions.cs
└── JCertPreApplication.UnitTests.csproj
```

### 2. Integration Tests Project Structure

```
JCertPreApplication.IntegrationTests/
├── Controllers/
│   ├── AuthControllerTests.cs
│   ├── CourseControllerTests.cs
│   └── PaymentControllerTests.cs
├── Repositories/
│   ├── UserRepositoryTests.cs
│   ├── CourseRepositoryTests.cs
│   └── PaymentRepositoryTests.cs
├── Infrastructure/
│   ├── DatabaseTestFixture.cs
│   ├── WebApplicationFactory.cs
│   └── TestConfiguration.cs
└── JCertPreApplication.IntegrationTests.csproj
```

## Nguyên Tắc Testing

### 1. FIRST Principles
- **Fast**: Tests phải chạy nhanh
- **Independent**: Mỗi test độc lập
- **Repeatable**: Có thể chạy lại nhiều lần
- **Self-Validating**: Kết quả rõ ràng (pass/fail)
- **Timely**: Viết test sớm

### 2. AAA Pattern (Arrange-Act-Assert)
```csharp
[Fact]
public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
{
    // Arrange
    var email = "test@example.com";
    var password = "password123";
    var expectedUser = UserBuilder.Create().WithEmail(email).Build();
    
    _mockUserRepository.Setup(x => x.GetByEmailWithRoleAsync(email))
                      .ReturnsAsync(expectedUser);
    
    // Act
    var result = await _authService.LoginAsync(email, password);
    
    // Assert
    result.Should().NotBeNull();
    result.AccessToken.Should().NotBeNullOrEmpty();
    result.RefreshToken.Should().NotBeNullOrEmpty();
    result.User.Email.Should().Be(email);
}
```

### 3. Test Naming Convention
```
MethodName_StateUnderTest_ExpectedBehavior
```

Ví dụ:
- `LoginAsync_WithValidCredentials_ShouldReturnTokens`
- `CreateCourse_WithDuplicateTitle_ShouldThrowException`
- `ProcessPayment_WithInsufficientCredits_ShouldReturnFailure`

### 4. Test Categories
- **Happy Path**: Trường hợp thành công
- **Edge Cases**: Trường hợp biên
- **Error Cases**: Trường hợp lỗi
- **Business Rules**: Kiểm tra business logic

## Patterns và Best Practices

### 1. Test Fixtures và Builders

#### User Builder Pattern
```csharp
public class UserBuilder
{
    private User _user;
    
    public UserBuilder()
    {
        _user = new User
        {
            userId = Guid.NewGuid(),
            email = "test@example.com",
            passwordHash = "hashedpassword",
            firstName = "Test",
            lastName = "User",
            isActive = true,
            createdAt = DateTime.UtcNow
        };
    }
    
    public static UserBuilder Create() => new UserBuilder();
    
    public UserBuilder WithEmail(string email)
    {
        _user.email = email;
        return this;
    }
    
    public UserBuilder WithRole(Role role)
    {
        _user.Role = role;
        return this;
    }
    
    public UserBuilder AsInactive()
    {
        _user.isActive = false;
        return this;
    }
    
    public User Build() => _user;
}
```

### 2. Mock Setup Extensions
```csharp
public static class MockExtensions
{
    public static void SetupUserRepository(this Mock<IUserRepository> mock, User user)
    {
        mock.Setup(x => x.GetByIdAsync(user.userId))
            .ReturnsAsync(user);
        mock.Setup(x => x.GetByEmailAsync(user.email))
            .ReturnsAsync(user);
    }
    
    public static void SetupCourseRepository(this Mock<ICourseRepository> mock, Course course)
    {
        mock.Setup(x => x.GetByIdAsync(course.courseId))
            .ReturnsAsync(course);
        mock.Setup(x => x.IsTitleUniqueAsync(course.title, null))
            .ReturnsAsync(false);
    }
}
```

### 3. Database Test Fixture
```csharp
public class DatabaseFixture : IDisposable
{
    public JCertPreDatabaseContext Context { get; private set; }
    
    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<JCertPreDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        Context = new JCertPreDatabaseContext(options);
        Context.Database.EnsureCreated();
        SeedData();
    }
    
    private void SeedData()
    {
        // Seed common test data
        var adminRole = new Role { roleId = Guid.NewGuid(), roleName = "ADMIN" };
        var studentRole = new Role { roleId = Guid.NewGuid(), roleName = "STUDENT" };
        
        Context.Roles.AddRange(adminRole, studentRole);
        Context.SaveChanges();
    }
    
    public void Dispose()
    {
        Context.Dispose();
    }
}
```

## Hướng Dẫn Thực Hiện

### Bước 1: Tạo Branch Testing
```bash
git checkout -b feature/unit-testing
```

### Bước 2: Tạo Test Projects
```bash
# Tạo Unit Test project
dotnet new xunit -n JCertPreApplication.UnitTests
dotnet sln add JCertPreApplication.UnitTests/JCertPreApplication.UnitTests.csproj

# Tạo Integration Test project  
dotnet new xunit -n JCertPreApplication.IntegrationTests
dotnet sln add JCertPreApplication.IntegrationTests/JCertPreApplication.IntegrationTests.csproj
```

### Bước 3: Thêm Package References
```xml
<!-- JCertPreApplication.UnitTests.csproj -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.6.1" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="AutoFixture" Version="4.18.0" />
<PackageReference Include="AutoFixture.Xunit2" Version="4.18.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.13" />
<PackageReference Include="coverlet.collector" Version="6.0.0" />
```

### Bước 4: Cấu Hình Test Settings
```json
// coverlet.runsettings
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Format>opencover</Format>
          <Exclude>[*]JCertPreApplication.*.Migrations.*</Exclude>
          <Exclude>[*]JCertPreApplication.API.Program</Exclude>
          <ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute</ExcludeByAttribute>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

## Ví Dụ Cụ Thể

### 1. Unit Test cho AuthService

```csharp
public class AuthServiceTests : IClassFixture<AuthServiceFixture>
{
    private readonly AuthServiceFixture _fixture;
    private readonly AuthService _authService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordService> _mockPasswordService;

    public AuthServiceTests(AuthServiceFixture fixture)
    {
        _fixture = fixture;
        _authService = _fixture.AuthService;
        _mockUserRepository = _fixture.MockUserRepository;
        _mockPasswordService = _fixture.MockPasswordService;
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var user = UserBuilder.Create()
            .WithEmail(email)
            .WithRole(new Role { roleName = "STUDENT" })
            .Build();

        _mockUserRepository.Setup(x => x.GetByEmailWithRoleAsync(email))
                          .ReturnsAsync(user);
        _mockPasswordService.Setup(x => x.VerifyPassword(password, user.passwordHash))
                           .Returns(true);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be(email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";

        _mockUserRepository.Setup(x => x.GetByEmailWithRoleAsync(email))
                          .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.LoginAsync(email, password));
        
        exception.StatusCode.Should().Be(401);
        exception.ErrorCode.Should().Be("INVALID_CREDENTIALS");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task LoginAsync_WithInvalidEmail_ShouldThrowBadRequestException(string email)
    {
        // Arrange
        var password = "password123";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _authService.LoginAsync(email, password));
        
        exception.StatusCode.Should().Be(400);
    }
}
```

### 2. Unit Test cho CourseService

```csharp
public class CourseServiceTests : IClassFixture<CourseServiceFixture>
{
    private readonly CourseServiceFixture _fixture;
    private readonly CourseService _courseService;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IFileService> _mockFileService;

    public CourseServiceTests(CourseServiceFixture fixture)
    {
        _fixture = fixture;
        _courseService = _fixture.CourseService;
        _mockCourseRepository = _fixture.MockCourseRepository;
        _mockFileService = _fixture.MockFileService;
    }

    [Fact]
    public async Task CreateCourseAsync_WithValidData_ShouldReturnCourseDto()
    {
        // Arrange
        var createDto = new CreateCourseDto
        {
            Title = "New Course",
            Description = "Course Description",
            Price = 100.00m,
            Level = CourseLevel.BEGINNER
        };

        _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(createDto.Title))
                            .ReturnsAsync(true);
        _mockCourseRepository.Setup(x => x.AddAsync(It.IsAny<Course>()))
                            .ReturnsAsync((Course course) => course);

        // Act
        var result = await _courseService.CreateCourseAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(createDto.Title);
        result.Description.Should().Be(createDto.Description);
        result.Price.Should().Be(createDto.Price);
        
        _mockCourseRepository.Verify(x => x.IsTitleUniqueAsync(createDto.Title), Times.Once);
        _mockCourseRepository.Verify(x => x.AddAsync(It.IsAny<Course>()), Times.Once);
    }

    [Fact]
    public async Task CreateCourseAsync_WithDuplicateTitle_ShouldThrowBadRequestException()
    {
        // Arrange
        var createDto = new CreateCourseDto
        {
            Title = "Existing Course",
            Description = "Course Description",
            Price = 100.00m
        };

        _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(createDto.Title))
                            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _courseService.CreateCourseAsync(createDto));
        
        exception.StatusCode.Should().Be(400);
        exception.ErrorCode.Should().Be("COURSE_TITLE_EXISTS");
    }
}
```

### 3. Integration Test cho Controller

```csharp
public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithTokens()
    {
        // Arrange
        var loginRequest = new LoginDto
        {
            Email = "admin@example.com",
            Password = "Admin123!"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/login", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponseDto>(responseContent);
        
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }
}
```



## CI/CD Integration

### GitHub Actions Workflow

```yaml
# .github/workflows/tests.yml
name: Tests

on:
  push:
    branches: [ main, dev ]
  pull_request:
    branches: [ main, dev ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:13
        env:
          POSTGRES_PASSWORD: postgres
          POSTGRES_DB: jcertpre_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Run Unit Tests
      run: |
        dotnet test JCertPreApplication.UnitTests \
          --no-build \
          --verbosity normal \
          --collect:"XPlat Code Coverage" \
          --settings coverlet.runsettings
          
    - name: Run Integration Tests
      run: |
        dotnet test JCertPreApplication.IntegrationTests \
          --no-build \
          --verbosity normal
          
    - name: Generate Coverage Report
      run: |
        dotnet tool install -g dotnet-reportgenerator-globaltool
        reportgenerator \
          -reports:**/coverage.opencover.xml \
          -targetdir:coverage \
          -reporttypes:Html;Badges
          
    - name: Upload Coverage to Codecov
      uses: codecov/codecov-action@v3
      with:
        files: coverage/coverage.opencover.xml
        fail_ci_if_error: true
```

## Test Commands

### Chạy Tests
```bash
# Chạy tất cả tests
dotnet test

# Chạy Unit Tests với coverage
dotnet test JCertPreApplication.UnitTests \
  --collect:"XPlat Code Coverage" \
  --settings coverlet.runsettings

# Chạy Integration Tests
dotnet test JCertPreApplication.IntegrationTests

# Chạy tests với filter
dotnet test --filter "FullyQualifiedName~AuthService"

# Chạy tests parallel
dotnet test --parallel
```

### Coverage Reports
```bash
# Tạo HTML coverage report
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator \
  -reports:**/coverage.opencover.xml \
  -targetdir:coverage \
  -reporttypes:Html

# Mở report
open coverage/index.html
```

## Metrics và KPIs

### Code Coverage Targets
- **Unit Tests**: Minimum 80% coverage
- **Critical Business Logic**: Minimum 95% coverage
- **Controllers**: Minimum 70% coverage

### Test Categories Distribution
- **Unit Tests**: 80% of total tests
- **Integration Tests**: 20% of total tests

### Quality Gates
- All tests must pass
- Code coverage >= 80%
- Performance tests within acceptable limits

## Troubleshooting

### Common Issues

1. **In-Memory Database Issues**
   ```csharp
   // Ensure unique database names
   var options = new DbContextOptionsBuilder<JCertPreDatabaseContext>()
       .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
       .Options;
   ```

2. **Mock Setup Issues**
   ```csharp
   // Use It.IsAny<> for flexible matching
   mock.Setup(x => x.Method(It.IsAny<string>()))
       .ReturnsAsync(result);
   ```

3. **Async Testing Issues**
   ```csharp
   // Always await async methods in tests
   var result = await service.MethodAsync();
   ```

## Kết Luận

Việc implement unit testing theo hướng dẫn này sẽ giúp:

1. **Tăng chất lượng code**: Phát hiện bugs sớm
2. **Cải thiện maintainability**: Dễ dàng refactor và thêm features
3. **Tăng confidence**: Deploy với sự tự tin cao hơn
4. **Documentation**: Tests như living documentation
5. **Team collaboration**: Hiểu rõ business requirements

### Next Steps

1. Tạo branch `feature/unit-testing`
2. Setup test projects theo cấu trúc đề xuất
3. Bắt đầu với critical business logic (Auth, Payment)
4. Thêm dần các tests khác
5. Setup CI/CD pipeline
6. Monitor coverage và quality metrics

---

*Tài liệu này sẽ được cập nhật thường xuyên khi có thêm patterns và best practices mới.*
